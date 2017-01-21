#include <EEPROM.h>

#include <SoftwareSerial.h>
SoftwareSerial mySerial(10, 11);
#define address 0
int y[] = {9, 5, 6, 8};
int x[6];
int triggerPin = 9;
int progCurrentDev = 0;
int progCurrentPort = 0;
int deviceCount = 0;
int portCount = 0;
int actionCount = 0;
int devices[10][2];
int pins[20][3];
int actions[40][4];
void loadSettingsFromEeprom()
{
  deviceCount = EEPROM.read(0);
  actionCount = 0;
  int lastPinCount = 0;
  for (int i = 0; i < deviceCount; ++i)
  {
    devices[i][0] = EEPROM.read(i * 2 + 1);
    devices[i][1] = EEPROM.read(i * 2 + 2) + lastPinCount;
    lastPinCount += devices[i][1];
    portCount = devices[i][1];
  }
  int lastActionsCount = 0;
  for (int i = 0 ; i < portCount; i++)
  {
    pins[i][0] = EEPROM.read(deviceCount * 2 + i * 3 + 1);
    pins[i][1] = EEPROM.read(deviceCount * 2 + i * 3 + 2) + lastActionsCount;
    lastActionsCount = pins[i][1];
    pins[i][2] = EEPROM.read(deviceCount * 2 + i * 3 + 3);
    actionCount = pins[i][1];
  }
  for (int i = 0 ; i < actionCount; i++)
  {
    actions[i][0] = EEPROM.read(deviceCount * 2 + portCount * 3 + 4 * i + 1);
    actions[i][1] = EEPROM.read(deviceCount * 2 + portCount * 3 + 4 * i + 2);
    actions[i][2] = EEPROM.read(deviceCount * 2 + portCount * 3 + 4 * i + 3);
    actions[i][3] = EEPROM.read(deviceCount * 2 + portCount * 3 + 4 * i + 4);
  }
  /*for (int i = 0 ; i < portCount; i++)
    {
    Serial.print(pins[i][0]);
    Serial.print(pins[i][1]);
    Serial.print(pins[i][2]);
    Serial.print(" ");
    }*//*
  for (int i = 0 ; i < actionCount; i++)
    {
    Serial.print(actions[i][0]);
    Serial.print(actions[i][1]);
    Serial.print(actions[i][2]);
    Serial.print(actions[i][3]);
    Serial.print(" --");
    }*/
  //Serial.println(" ");
}
void executeAction(int actionId)
{
  //1 8 2 0
  //1 0 8 2 0 0
  int actionCommand[]={actions[actionId][0],0,actions[actionId][1],actions[actionId][2],0,0};
  if(actionCommand[3]==3)
  {
    delay(actions[actionId][3]*1000);
  }
  else
  {
    sendCommandViaMax(actionCommand);
  }
}
void pinTriggered(int deviceId, int pinNumber)
{
  for (int i = 0; i < deviceCount; ++i)
  {
    if (devices[i][0] == deviceId)
    {
      int pinStart = 0;
      int pinEnd = devices[i][1];
      if (i > 0)
      {
        pinStart = devices[i - 1][1];
        pinEnd = 2 * devices[i][1] - devices[i - 1][1];
      }
      for (int j = pinStart; j < pinEnd; ++j)
      {
        if (pins[j][0] == pinNumber)
        {
          int actionStart = 0;
          int actionEnd = pins[j][1];
          if (j > 0)
          {
            actionStart = pins[j][1] - pins[j - 1][1];
            actionEnd = 2 * pins[j][1] - pins[j - 1][1];
          }
          for (int k = 0; k < pins[j][2]; ++k)
          {
            for (int l = actionStart; l < actionEnd; ++l)
            {
              executeAction(l);
            }
          }
          break;
        }
      }
      break;
    }
  }
}
void setup()
{
  mySerial.begin(9600);
  Serial.begin(9600);
  pinMode(triggerPin, OUTPUT);
  digitalWrite(triggerPin, LOW);
  loadSettingsFromEeprom();
}
int readOneByteMax()
{
  return mySerial.read();
}
void writeByteMax(int value)
{
  mySerial.write(value );
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
    /*Serial.print("    ");
    Serial.print(bytes[i]);
    Serial.print("    ");*/
  }
  //Serial.println("    ");
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
void sendOneBigByteViaMax(int toAddress, int byte)
{
  x[0] = toAddress;
  x[1] = 99;
  x[2] = byte/256;
  x[3] = byte%256;
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
          if (x[1] == 2)
          {
            pinTriggered(x[2], x[3]);
          }
        }
      }
    }
  }
}
int portCountS=0,deviceCountS=0,actionCountS=0;
int pos=0;
int commandReceived[6];
void checkSerial()
{

  if (Serial.available() > 5)
  {
    for (int i = 0; i < 6; ++i)
    {
      commandReceived[i] = Serial.read() - 48;
    }
    if (commandReceived[0] == address)
    {
      switch (commandReceived[1])
      {
        case 2:
          for (int i = 0 ; i < 50 ; i++)
          {
            EEPROM.write(i, 0);
          }
          EEPROM.write(0, commandReceived[2]);
          progCurrentPort = 0;
          portCountS=0;
          deviceCountS = 0;
          actionCountS=0;
          break;
        case 3:
          EEPROM.write(deviceCountS * 2 + 1, commandReceived[2]);
          EEPROM.write(deviceCountS * 2 + 2, commandReceived[3]);
          deviceCountS++;
          break;
        case 4:
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + 1, commandReceived[3]);
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + 2, commandReceived[4]);
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + 3, commandReceived[5]);
          portCountS++;
          break;
        case 5:
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + actionCountS * 4 + 1,commandReceived[2]);
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + actionCountS * 4 + 2,commandReceived[3]);
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + actionCountS * 4 + 3,commandReceived[4]);
          EEPROM.write(deviceCountS * 2 + portCountS * 3 + actionCountS * 4 + 4,commandReceived[5]);
          actionCountS++;
          break;
        case 6:
          loadSettingsFromEeprom();
          break;
      }
    }
    else
    {
      sendCommandViaMax(commandReceived);
    }
  }
}

void loop()
{
  sendOneBigByteViaMax(1,32767 );
  return;
  checkMax();
  checkSerial();
  /*int test[]={1,0,8,2,0,0};
    sendCommandViaMax(test);
    delay(2000);*/
}
