#define COLS 3
#define ROWS 4
#define ADDRESS 3
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 2
#include<SoftwareSerial.h>
#include <Lan.h>
SoftwareSerial serial(10, 11);
int inputPinsCount = 1;
rPin inputPins[1] = {{.pinNumber=0,.initializing=0}};
int outputPinsCount = 0;
rPin outputPins[1] = {{.pinNumber=7,.initializing=0}};
int analogPinsCount = 0;
rPin analogPins[1] = {{.pinNumber=0,.initializing=0}};
int analogTriggeredValue[] = {4};
int keyboardColPins[] = {2, 3, 4};
int keyboardRowPins[] = {5, 6, 7, 8};
int insertedPin[4];
int countInsertedPin=0;
int pin[]={0,0,0,0};

void writeLan(int byte)
{
  serial.write(byte);
}
int readLan()
{
  return serial.read();
}
int countLan()
{
  return serial.available();
}
Lan lan(ADDRESS,DEVICE_TYPE,TRIGGERED_PIN, &writeLan, &readLan, &countLan);
void setup()
{
  Serial.begin(9600);
  serial.begin(9600);
  for (int i = 0; i < COLS; ++i)
  {
    pinMode(keyboardColPins[i], OUTPUT);
    digitalWrite(keyboardColPins[i], HIGH);
  }
  for (int i = 0; i < ROWS; ++i)
  {
    pinMode(keyboardRowPins[i], INPUT_PULLUP);
  }
  lan.SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan.Register();
}
int  getLinie(int pin)
{
  return keyboardRowPins[ROWS - 1] - pin + 1;
}
int  getColoana(int pin)
{
  return keyboardColPins[COLS - 1] - pin + 1;
}
int transforLinColToNumber(int lin, int col)
{
  return ((lin - 1) * 3) + col;
}
int getTasta(int pin1, int pin2)
{
  int col, lin;
  col = getColoana(pin1);
  lin = getLinie(pin2);
  int rez = transforLinColToNumber(lin, col);
  if (rez == 11)
    return 0;
  return rez;
}
int getPressedTasta()
{
  for (int i = 0; i < COLS; ++i)
  {
    digitalWrite(keyboardColPins[i], LOW);
    for (int j = 0; j < ROWS; ++j)
    {
      int val = digitalRead(keyboardRowPins[j]);
      if (digitalRead(keyboardRowPins[j]) == 0)
      {
        digitalWrite(keyboardColPins[i], HIGH);
        return getTasta(keyboardColPins[i], keyboardRowPins[j]);
      }
    }
    digitalWrite(keyboardColPins[i], HIGH);
  }
  return -1;
}
bool isPinOk()
{
  bool ok=true;
  for(int i=0;i<4;++i)
  {
    if(pin[i]!=insertedPin[i])
    {
      ok=false;
      break;
    }
  }
  return ok;
}
void keyPressed(int number)
{
  insertedPin[countInsertedPin]=number;
  countInsertedPin++;
  if(countInsertedPin==4)
  {
    if(isPinOk())
    {
      lan.InputPinTriggered(0, 1);
      Serial.println("logged in");
    }
    else
    {
      Serial.println("bad pin");
    }
    countInsertedPin=0;
  }
}
void loop()
{
  int tasta = getPressedTasta();
  if (tasta != -1)
  {
    keyPressed(tasta);
    Serial.println(tasta) ;
    delay(500);
  }
  lan.CheckMessages();
  lan.CheckAnalogPins();
  //lan.CheckInputPins();
}
