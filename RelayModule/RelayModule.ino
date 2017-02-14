//relay module
#define ASCIIVALUES 0
#define ADDRESS 2
#define TRIGGERED_PIN 9
#define MASTER_ADDRESS 0
#include<SoftwareSerial.h>
#include <LanCommunication.h>
SoftwareSerial serial(10, 11);
int deviceType = 1;
int inputPinsCount = 1;
int inputPins[1] = {8};
int outputPinsCount = 1;
int outputPins[1] = {7};
int analogPinsCount = 1;
int analogPins[1] = {0};
int analogTriggeredValue[] = {7};
int isAnalogTriggered[]={1};
void writeLan(int byte)
{
  serial.write(byte);
}
int readLan()
{
  //Serial.println("read funct");
  return serial.read();
}
int countLan()
{
  return serial.available();
}
LanCommunication lan(TRIGGERED_PIN, &writeLan, &readLan, &countLan);
void registerInLan()
{
  int data[6] = {MASTER_ADDRESS, 0, ADDRESS, deviceType, 0};
  lan.SendCommand(data);
  //toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
  for (int i = 0; i < outputPinsCount; ++i)
  {
    data[0] = MASTER_ADDRESS;
    data[1] = 1;
    data[2] = ADDRESS;
    data[3] = outputPins[i];
    data[4] = 1;
    data[5] = 0;
    lan.SendCommand(data);
  }
  for (int i = 0; i < analogPinsCount; ++i)
  {
    data[0] = MASTER_ADDRESS;
    data[1] = 1;
    data[2] = ADDRESS;
    data[3] = analogPins[i];
    data[4] = 2;
    data[5] = 0;
    lan.SendCommand(data);
  }
  //toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
  for (int i = 0; i < inputPinsCount; ++i)
  {
    data[0] = MASTER_ADDRESS;
    data[1] = 1;
    data[2] = ADDRESS;
    data[3] = inputPins[i];
    data[4] = 0;
    data[5] = 0;
    lan.SendCommand(data);
  }
  data[0] = MASTER_ADDRESS;
  data[1] = 4;
  data[2] = ADDRESS;
  data[3] = 0;
  data[4] = 0;
  data[5] = 0;
  lan.SendCommand(data);
}
void setup()
{
  serial.begin(9600);
  Serial.begin(9600);
  registerInLan();
  for (int i = 0; i < inputPinsCount; ++i)
  {
    pinMode(inputPins[i], INPUT_PULLUP);
  }
  for (int i = 0; i < outputPinsCount; ++i)
  {
    pinMode(outputPins[i], OUTPUT);
  }
}
void checkMax()
{
  if (lan.IsCommandAvailable())
  {
    if (lan.ReadCommand())
    {
      int *bytes = lan.GetLastCommand();
      if (bytes[0] == ADDRESS)
        {
          Serial.println("int2");
          Serial.print(bytes[0]);
          Serial.print(bytes[1]);
          Serial.print(bytes[2]);
          Serial.println(bytes[3]);
          switch (bytes[1])
          {
            case 0:

              switch (bytes[3])
              {
                case 0:
                  digitalWrite(bytes[2], LOW);
                  break;
                case 1:
                  digitalWrite(bytes[2], HIGH);
                  break;
                case 2:
                  int status = digitalRead(bytes[2]);
                  if (status == 1)
                    status = 0;
                  else
                    status = 1;
                  digitalWrite(bytes[2], status);
                  break;
              }
              break;
            case 1:
              if (ADDRESS != 0)
                registerInLan();
              break;
            case 2:
              for (int i = 0; i < analogPinsCount; ++i)
              {
                if(analogPins[i]==bytes[2])
                {
                  analogTriggeredValue[i]=bytes[3];
                  Serial.println("limit setted");
                  Serial.println(bytes[3]);
                }
              }
              break;
          }
        }
    }
  }
}
void checkAnalogPins()
{
  for (int i = 0; i < analogPinsCount; ++i)
  {
    int value=map(analogRead(A0-analogPins[i]),0,1024,0,9);
    /*Serial.print(value);
    Serial.print(" ");
    Serial.print(isAnalogTriggered[i]);
    Serial.print(" ");
    Serial.println(analogTriggeredValue[i]);*/
    if(value>analogTriggeredValue[i] && isAnalogTriggered[i]==0)
    {
      int data[6] = {MASTER_ADDRESS, 2, ADDRESS, inputPins[i], value, 0};
      lan.SendCommand(data);
      isAnalogTriggered[i]=1;
      Serial.println("anal pin trig");
      Serial.println(value);
    }
    else if(value<=analogTriggeredValue[i] && isAnalogTriggered[i]==1)
    {
      int data[6] = {MASTER_ADDRESS, 2, ADDRESS, inputPins[i], value, 1};
      lan.SendCommand(data);
      isAnalogTriggered[i]=0;
      Serial.println("agb");
    }
  }
}
void loop() {
  checkMax();
  for (int i = 0; i < inputPinsCount; ++i)
  {
    int value = digitalRead(inputPins[i]);
    if (value == 0)
    {
      //masterAddress,respondType,fromAddress,pinNumber,value
      int data[6] = {MASTER_ADDRESS, 2, ADDRESS, inputPins[i], value, 0};
      lan.SendCommand(data);
      delay(500);
    }
  }
  checkAnalogPins();
}
