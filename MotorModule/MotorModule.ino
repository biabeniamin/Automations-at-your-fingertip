#define COLS 3
#define ROWS 4
#define ADDRESS 4
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 5
#include<SoftwareSerial.h>
#include <Lan.h>
#include <EEPROM.h>
SoftwareSerial serial(10, 11);
int inputPinsCount = 0;
rPin inputPins[1] = {{.pinNumber=0,.initializing=0}};
int outputPinsCount = 4;
rPin outputPins[] = {{.pinNumber=2,.initializing=1},
{.pinNumber=3,.initializing=1},
{.pinNumber=4,.initializing=1},
{.pinNumber=5,.initializing=1}};
int analogPinsCount = 0;
rPin analogPins[1] = {{.pinNumber=0,.initializing=0}};
int analogTriggeredValue[] = {4};
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
  lan.SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan.Register();
}
void loop()
{
  lan.CheckMessages();
  lan.CheckAnalogPins();
  //lan.CheckInputPins();
}
