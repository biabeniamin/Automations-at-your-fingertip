#include <SoftwareSerial.h>
SoftwareSerial mySerial(10,11);
#define address 0
int y[] = {9, 5, 6, 8};
int x[5];
int triggerPin = 9;
void setup()
{
  mySerial.begin(9600);
  Serial.begin(9600);
  pinMode(triggerPin, OUTPUT);
  digitalWrite(triggerPin, LOW);
}
int readOneByteMax()
{
  return mySerial.read() - 48;
}
void writeByteMax(int value)
{
  mySerial.write(value + 48);
}
void sendCommandViaMax(int bytes[])
{
  digitalWrite(triggerPin, HIGH);
  delay(1);
  writeByteMax(9);
  writeByteMax(5);
  writeByteMax(6);
  writeByteMax(8);
  //mySerial.write(48+bytes[1]);
  for (int i = 0; i < 6; ++i)
  {
    writeByteMax(bytes[i]);
    Serial.print("    ");
    Serial.print(bytes[i]);
    Serial.print("    ");
  }
  Serial.println("    ");
  digitalWrite(triggerPin, LOW);
  delay(1);
}
void sendOneByteViaMax(int toAddress, int byte)
{
  x[0] = toAddress;
  x[1] = byte;
  x[2] = 0;
  x[3] = 0;
  x[4] = 0;
  x[5] = 0;
  sendCommandViaMax(x);
}
void checkMax()
{
  if (mySerial.available() > 8)
  {

    while (mySerial.available())
    {
      for (int i = 0; i < 3; ++i)
      {
        x[i] = x[i + 1];
      }
      x[3] = readOneByteMax();
      bool isOk = true;
      for (int i = 0; i < 4; ++i)
      {
        if (x[i] != y[i])
        {
          isOk = false;
        }
      }
      if (isOk)
      {
        for (int i = 0; i < 6; ++i)
        {
          x[i] = readOneByteMax();
        }
        if (x[0] == address)
        {

          for (int i = 0; i < 6; ++i)
          {
            Serial.print(x[i]);
            //Serial.print(" ");
          }
          Serial.println();
        }
      }
    }
  }
}
void checkSerial()
{
  if(Serial.available()>5)
  {
    for(int i=0;i<6;++i)
    {
      x[i]=Serial.read()-48;
    }
    sendCommandViaMax(x);
  }
}

void loop()
{
  checkMax();
  checkSerial();
  /*int test[]={1,0,8,2,0,0};
  sendCommandViaMax(test);
  delay(2000);*/
}
