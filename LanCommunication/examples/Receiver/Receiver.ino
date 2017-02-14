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
  //Serial.println("read funct");
  return serial.read();
}
int countLan()
{
  return serial.available();
}
LanCommunication lan(ADDRESS, TRIGGERED_PIN, &writeLan, &readLan, &countLan);
void setup() {
  pinMode(13, OUTPUT);
  Serial.begin(9600);
  serial.begin(9600);
  delay(100);
}

void loop() {
  if (lan.IsCommandAvailable())
  {
    if (lan.ReadCommand())
    {
      int *bytes = lan.GetLastCommand();
      for (int i = 0; i < COMMUNICATION_BYTE_COUNT; i++)
      {
        Serial.print(bytes[i]);
      }
      Serial.println();
    }
  }
}

