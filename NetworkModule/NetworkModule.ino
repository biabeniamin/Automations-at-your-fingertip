//relay module
#define ADDRESS 1
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 1
//#include<SoftwareSerial.h>
#include <Lan.h>
//SoftwareSerial serial(10, 11);
int inputPinsCount = 1;
int inputPins[1] = {8};
int outputPinsCount = 1;
int outputPins[1] = {7};
int analogPinsCount = 1;
int analogPins[1] = {0};
int analogTriggeredValue[] = {4};
void writeLan(int byte)
{
  Serial1.write(byte);
}
int readLan()
{
  return Serial1.read();
}
int countLan()
{
  return Serial1.available();
}
Lan *lan;
void setup()
{
  delay(500);
  Serial1.begin(9600);
  Serial.begin(9600);
  lan=new Lan(ADDRESS,DEVICE_TYPE,TRIGGERED_PIN, &writeLan, &readLan, &countLan);
  lan->SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan->Register();
  
}
void loop() {
  lan->CheckMessages();
  lan->CheckAnalogPins();
  lan->CheckInputPins();
}
