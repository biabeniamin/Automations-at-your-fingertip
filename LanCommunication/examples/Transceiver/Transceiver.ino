#include <LanCommunication.h>
#include<SoftwareSerial.h>
#define ADDRESS 2
#define TRIGGERED_PIN 9
SoftwareSerial serial(10, 11);
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
LanCommunication lan(ADDRESS,TRIGGERED_PIN,&writeLan,&readLan,&countLan);
void setup() {
  pinMode(13,OUTPUT);
  Serial.begin(9600);
  serial.begin(9600);
}

void loop() {
  //lan.Blink();
  int x[]={2,0,7,2,0,0};
  lan.SendCommand(x);
  delay(1000);

}
void switchP(int pin)
{
  int s=digitalRead(pin);
  if(s==0)
    digitalWrite(pin,1);
  else
  digitalWrite(pin,0);
}
