//relay module
#define ADDRESS 2
#define TRIGGERED_PIN 2
#define DEVICE_TYPE 1
#include<SoftwareSerial.h>
#include <Lan.h>
SoftwareSerial serial(0,1);
int inputPinsCount = 1;
rPin inputPins[1] = {{.pinNumber=8,.initializing=1,.activateOnSwitch=1}};
int outputPinsCount = 1;
rPin outputPins[1] = {{.pinNumber=9,.initializing=1}};
int analogPinsCount = 1;
rPin analogPins[1] = {{.pinNumber=5,.initializing=0}};
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
  pinMode(8,INPUT_PULLUP);
  serial.begin(9600);
  //Serial.begin(9600);
  delay(1000);
  lan.SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan.Register();
  
}
void loop() {
  lan.CheckMessages();
  lan.CheckAnalogPins();
  lan.CheckInputPins();
}
