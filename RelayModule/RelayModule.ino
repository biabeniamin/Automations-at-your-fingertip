//relay module
#define ADDRESS 2
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 1
#include<SoftwareSerial.h>
#include <Lan.h>
SoftwareSerial serial(10, 11);
int inputPinsCount = 1;
int inputPins[1] = {8};
int outputPinsCount = 1;
int outputPins[1] = {7};
int analogPinsCount = 1;
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
  Serial.begin(9600);
  delay(1000);
  lan.SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan.Register();
  
}
void loop() {
  lan.CheckMessages();
  lan.CheckAnalogPins();
  lan.CheckInputPins();
}
