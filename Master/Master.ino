#include <EEPROM.h>

#include <SoftwareSerial.h>
#define ASCIIVALUES 0
#define address 0
SoftwareSerial mySerial(10, 11);
int y[] = {9, 5, 6, 8};
int x[6];
int triggerPin = 9;
int progCurrentDev = 0;
int progCurrentPort = 0;
int deviceCount = 0;
int portCount = 0;
int actionCount = 0;
int devices[10][2];
int pins[20][5];
int actions[40][5];
void setTriggerLimit(int devAddress,int pinNumber,int value)
{
  int val[]={devAddress,2,pinNumber,value,0,0,0};
  sendCommandViaMax(val);
  mySerial.print("trig limit ");
  mySerial.print(devAddress);
  mySerial.print(pinNumber);
  mySerial.println(value);
}
void sendAnalogTriggerLimit()
{
  int pinStart = 0;
  for (int i = 0; i < deviceCount; ++i)
  {
    int pinEnd = devices[i][1];
    for (int j = pinStart; j < pinEnd; ++j)
    {
      if (pins[j][3]>0)
      {
        setTriggerLimit(devices[i][0],pins[j][0],pins[j][3]);
      }
    }
    pinStart+=pinEnd;
  }

}
void loadSettingsFromEeprom()
{
  deviceCount = EEPROM.read(0);
  Serial.println("dev count:");
  Serial.println(deviceCount);
  actionCount = 0;
  int lastPinCount = 0;
  for (int i = 0; i < deviceCount; ++i)
  {
    devices[i][0] = EEPROM.read(i * 2 + 1);
    devices[i][1] = EEPROM.read(i * 2 + 2) + lastPinCount;
    lastPinCount += devices[i][1];
    portCount = devices[i][1];
  }
  Serial.println("dev 1 pin count:");
  Serial.println(devices[0][1]);
  int lastActionsCount = 0;
  for (int i = 0 ; i < portCount; i++)
  {
    pins[i][0] = EEPROM.read(deviceCount * 2 + i * 4 + 1);
    pins[i][1] = EEPROM.read(deviceCount * 2 + i * 4 + 2) + lastActionsCount;
    lastActionsCount = pins[i][1];
    pins[i][2] = EEPROM.read(deviceCount * 2 + i * 4 + 3);
    pins[i][3] = EEPROM.read(deviceCount * 2 + i * 4 + 4);
    actionCount = pins[i][1];
    Serial.println("action");
    Serial.print(pins[i][0]);
    Serial.print(pins[i][1]);
    Serial.print(pins[i][2]);
    Serial.println(pins[i][3]);
  }
  /*Serial.println("dev 1 pin2:");
    Serial.println(pins[1][0]);*/
  for (int i = 0 ; i < actionCount; i++)
  {
    actions[i][0] = EEPROM.read(deviceCount * 2 + portCount * 4 + 5 * i + 1);
    actions[i][1] = EEPROM.read(deviceCount * 2 + portCount * 4 + 5 * i + 2);
    actions[i][2] = EEPROM.read(deviceCount * 2 + portCount * 4 + 5 * i + 3);
    actions[i][3] = EEPROM.read(deviceCount * 2 + portCount * 4 + 5 * i + 4);
    actions[i][4] = EEPROM.read(deviceCount * 2 + portCount * 4 + 5 * i + 5);
  }

  //send analogLimit
  sendAnalogTriggerLimit();
  /*Serial.println("dev 1 pin 2 act count:");
    Serial.print(actionCount);
    Serial.print(actions[0][0]);
    Serial.print(actions[0][1]);
    Serial.print(actions[0][2]);
    Serial.print(actions[0][3]);
    Serial.print(actions[0][4]);
    Serial.println();*/

  /*for (int i = 0 ; i < portCount; i++)
    {
    Serial.print(pins[i][0]);
    Serial.print(pins[i][1]);
    Serial.print(pins[i][2]);
    Serial.print(" ");
    }*/
    
  /*for (int i = 0 ; i < actionCount; i++)
  {
  Serial.print(actions[i][0]);
  Serial.print(actions[i][1]);
  Serial.print(actions[i][2]);
  Serial.print(actions[i][3]);
  Serial.print(actions[i][4]);
  Serial.print(" --");
  }*/
  //Serial.println(" ");
}
void executeAction(int actionId)
{
  //1 8 2 0
  //1 0 8 2 0 0
  int actionCommand[] = {actions[actionId][0], 0, actions[actionId][1], actions[actionId][2], 0, 0};
  Serial.print("address");
  Serial.println(actionId);
  if (actionCommand[3] == 3)
  {
    delay(actions[actionId][3] * 1000);
  }
  else
  {
    sendCommandViaMax(actionCommand);
  }
}
void pinTriggered(int deviceId, int pinNumber,int actionType)
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
      int actionStart = 0;
      for (int j = pinStart; j < pinEnd; ++j)
      {
        if (pins[j][0] == pinNumber)
        {
          int actionEnd = actionStart + pins[j][1];
          for (int k = 0; k < pins[j][2]; ++k)
          {
            Serial.print("action start:");
            Serial.print(actionStart);
            Serial.print("action end:");
            Serial.print(actionEnd);
            for (int l = actionStart; l < actionEnd; ++l)
            {
              if(actions[l][4]==actionType)
              {
                executeAction(l);
              }
            }
          }
          actionStart += pins[j][1];
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
  int value = mySerial.read();
  //Serial.print(value);
#if ASCIIVALUES==1
  return value - 48;
#else
  return value;
#endif
}
void writeByteMax(int value)
{
  //Serial.print(value);
#if ASCIIVALUES==1
  mySerial.write(value + 48);
#else
  mySerial.write(value);
#endif
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
  //Serial.println();
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
  x[2] = byte / 256;
  x[3] = byte % 256;
  x[4] = 0;
  x[5] = 0;
  sendCommandViaMax(x);
}
int testBuffer[4];
void checkMax()
{
  if (mySerial.available() > 9)
  {
    while (mySerial.available() > 0)
    {
      for (int i = 0; i < 3; ++i)
      {
        testBuffer[i] = testBuffer[i + 1];
      }
      testBuffer[3] = readOneByteMax();
      bool isOk = true;
      for (int i = 0; i < 4; ++i)
      {
        if (testBuffer[i] != y[i])
        {
          isOk = false;
        }
      }
      int waitCount = 0;
      if (mySerial.available() < 6)
      {
        delay(10);
        /*if(waitCount>0)
          {
          break;
          }*/
      }
      if (isOk && mySerial.available() > 5)
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
            pinTriggered(x[2], x[3],x[5]);
          }
        }
      }
    }
  }
}
int portCountS = 0, deviceCountS = 0, actionCountS = 0;
int pos = 0;
int commandReceived[7];
void checkSerial()
{

  if (Serial.available() > 5)
  {
    for (int i = 0; i < 7; ++i)
    {
      commandReceived[i] = Serial.read() - 48;
      mySerial.print(commandReceived[i]);
    }
    mySerial.println();
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
          portCountS = 0;
          deviceCountS = 0;
          actionCountS = 0;
          mySerial.println("devT");
          mySerial.println(commandReceived[2]);
          break;
        case 3:
          mySerial.println("dev");
          EEPROM.write(deviceCountS * 2 + 1, commandReceived[2]);
          EEPROM.write(deviceCountS * 2 + 2, commandReceived[3]);
          mySerial.println(deviceCountS * 2 + 1);
          mySerial.println(deviceCountS * 2 + 2);
          mySerial.println(commandReceived[3]);
          deviceCountS++;
          break;
        case 4:
          mySerial.println("pin");
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + 1, commandReceived[3]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + 2, commandReceived[4]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + 3, commandReceived[5]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + 4, commandReceived[6]);
          mySerial.println(commandReceived[6]);
          portCountS++;
          break;
        case 5:
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + actionCountS * 5 + 1, commandReceived[2]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + actionCountS * 5 + 2, commandReceived[3]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + actionCountS * 5 + 3, commandReceived[4]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + actionCountS * 5 + 4, commandReceived[5]);
          EEPROM.write(deviceCountS * 2 + portCountS * 4 + actionCountS * 5 + 5, commandReceived[6]);
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
      delay(10);
    }
  }
}
/*
 * 11100
21100
31100
41100
51100
01150
82100

 */
void loop()
{
  /*for(int i=0;i<7;i++)
  {
    for(int j=0;j<5;++j)
    {
      Serial.print(pins[i][j]);
    }
    Serial.println();
  }
  while(1);*/
  checkMax();
  checkSerial();
  /*int test[]={1,0,8,2,0,0};
    sendCommandViaMax(test);
    delay(2000);*/
}
