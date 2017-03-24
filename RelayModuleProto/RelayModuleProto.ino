//relay module
#define ADDRESS 3
#define TRIGGERED_PIN 2
#define DEVICE_TYPE 1
#include<SoftwareSerial.h>
#include <Lan.h>
SoftwareSerial serial(0,1);
int inputPinsCount = 0;
int inputPins[1] = {8};
int outputPinsCount = 1;
int outputPins[1] = {9};
int analogPinsCount = 0;
int analogPins[1] = {0};
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
