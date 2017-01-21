#include <SoftwareSerial.h>
#define address 1
SoftwareSerial mySerial(10, 11);
void setup() {
  Serial.begin(9600);
  mySerial.begin(9600);
  pinMode(6,OUTPUT);
  digitalWrite(6,HIGH);
}
int x[6];
int y[]={9,5,6,8};
int readOneByteMax()
{
  int aux=mySerial.read();
  return aux;
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
            Serial.print(" ");
          }
          if(x[1]==99)
          {
            Serial.print("value=");
            Serial.print(x[2]*256+x[3]);
          }
          Serial.println();
        }
      }
    }
  }
}
void loop() {
  checkMax();
}
