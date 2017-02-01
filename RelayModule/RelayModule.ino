//relay module
#define ASCIIVALUES 0
#include<SoftwareSerial.h>
SoftwareSerial serial(10, 11);
int y[] = {9, 5, 6, 8};
int x[5];
int address = 2;
int triggerPin = 9;
int masterAddress = 0;
//type(0-master,1-relay,2-keyboard,3-network)
int deviceType = 1;
int inputPinsCount = 1;
int inputPins[1] = {8};
int outputPinsCount = 1;
int outputPins[1] = {7};
int analogPinsCount = 1;
int analogPins[1] = {0};
int analogTriggeredValue[] = {7};
int isAnalogTriggered[]={1};
void registerInLan()
{
  //toAddress,typeOfResponse(0-register,1-PinRegister),fromAddress,type(0-master,1-relay,2-keyboard,3-network)
  int data[6] = {masterAddress, 0, address, deviceType, 0};
  sendCommandViaMax(data);
  //toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
  for (int i = 0; i < outputPinsCount; ++i)
  {
    data[0] = masterAddress;
    data[1] = 1;
    data[2] = address;
    data[3] = outputPins[i];
    data[4] = 1;
    data[5] = 0;
    sendCommandViaMax(data);
  }
  for (int i = 0; i < analogPinsCount; ++i)
  {
    data[0] = masterAddress;
    data[1] = 1;
    data[2] = address;
    data[3] = analogPins[i];
    data[4] = 2;
    data[5] = 0;
    sendCommandViaMax(data);
  }
  //toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
  for (int i = 0; i < inputPinsCount; ++i)
  {
    data[0] = masterAddress;
    data[1] = 1;
    data[2] = address;
    data[3] = inputPins[i];
    data[4] = 0;
    data[5] = 0;
    sendCommandViaMax(data);
  }
  data[0] = masterAddress;
  data[1] = 4;
  data[2] = address;
  data[3] = 0;
  data[4] = 0;
  data[5] = 0;
  sendCommandViaMax(data);
}
void setup()
{
  serial.begin(9600);
  Serial.begin(9600);
  pinMode(triggerPin, OUTPUT);
  digitalWrite(triggerPin, LOW);
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
void writeByteMax(int value)
{
#if ASCIIVALUES==1
  Serial.print(value);
  serial.write(value + 48);
#else
  Serial.print(value);
  serial.write(value);
#endif
}
int readByteMax()
{
#if ASCIIVALUES==1
  return serial.read() - 48;
#else
  return serial.read();
#endif
}
void sendCommandViaMax(int bytes[])
{
  digitalWrite(triggerPin, HIGH);
  delay(100);
  writeByteMax(y[0]);
  writeByteMax(y[1]);
  writeByteMax(y[2]);
  writeByteMax(y[3]);
  for (int i = 0; i < 6; ++i)
  {
    writeByteMax( bytes[i]);
  }
  digitalWrite(triggerPin, LOW);
  delay(1);
  Serial.println();
}
void sendOneByteViaMax(int address, int byte)
{
  x[0] = address;
  x[1] = byte;
  x[2] = 0;
  x[3] = 0;
  x[4] = 0;
  x[5] = 0;
  sendCommandViaMax(x);
}
void checkMax()
{
  if (serial.available() > 9)
  {

    while (serial.available())
    {
      for (int i = 0; i < 3; ++i)
      {
        x[i] = x[i + 1];
      }
      x[3] = readByteMax();
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
          x[i] = readByteMax();
        }
        if (x[0] == address)
        {
          Serial.println("int2");
          Serial.print(x[0]);
          Serial.print(x[1]);
          Serial.print(x[2]);
          Serial.println(x[3]);
          switch (x[1])
          {
            case 0:

              switch (x[3])
              {
                case 0:
                  digitalWrite(x[2], LOW);
                  break;
                case 1:
                  digitalWrite(x[2], HIGH);
                  break;
                case 2:
                  int status = digitalRead(x[2]);
                  if (status == 1)
                    status = 0;
                  else
                    status = 1;
                  digitalWrite(x[2], status);
                  break;
              }
              break;
            case 1:
              if (address != 0)
                registerInLan();
              break;
            case 2:
              for (int i = 0; i < analogPinsCount; ++i)
              {
                if(analogPins[i]==x[2])
                {
                  analogTriggeredValue[i]=x[3];
                  Serial.println("limit setted");
                  Serial.println(x[3]);
                }
              }
              break;
          }
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
    Serial.print(value);
    Serial.print(" ");
    Serial.print(isAnalogTriggered[i]);
    Serial.print(" ");
    Serial.println(analogTriggeredValue[i]);
    if(value>analogTriggeredValue[i] && isAnalogTriggered[i]==0)
    {
      int data[6] = {masterAddress, 2, address, inputPins[i], value, 0};
      sendCommandViaMax(data);
      isAnalogTriggered[i]=1;
      Serial.println("anal pin trig");
      Serial.println(value);
    }
    else if(value<=analogTriggeredValue[i] && isAnalogTriggered[i]==1)
    {
      int data[6] = {masterAddress, 2, address, inputPins[i], value, 1};
      sendCommandViaMax(data);
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
      int data[6] = {masterAddress, 2, address, inputPins[i], value, 0};
      sendCommandViaMax(data);
      delay(500);
    }
  }
  checkAnalogPins();
}
