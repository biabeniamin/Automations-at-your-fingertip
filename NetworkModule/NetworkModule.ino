#include <SPI.h>
#include <Ethernet.h>
#include <Lan.h>
byte mac[] = {
  0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED
};
IPAddress ip(192, 168, 0, 108);
EthernetServer server(80);
#define ADDRESS 1
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 3
int inputPinsCount = 10;
int inputPins[] = {0,1,2,3,4,5,6,7,8,9};
int outputPinsCount = 0;
int outputPins[0] = {};
int analogPinsCount = 0;
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
  Serial1.begin(9600);
  Serial.begin(9600);
  Ethernet.begin(mac, ip);
  server.begin();
  lan=new Lan(ADDRESS,DEVICE_TYPE,TRIGGERED_PIN, &writeLan, &readLan, &countLan);
  lan->SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan->Register();
}

void writeHtml(EthernetClient client)
{
  client.println("HTTP/1.1 200 OK");
  client.println("Content-Type: text/html");
  client.println("Connection: close");
  client.println();
  client.println("<!DOCTYPE HTML>");
  client.println("<html>");
  client.println("<body>");
  for (int i = 0; i < 10; ++i)
  {
    client.print("<a href='?cmd=");
    client.print(i);
    client.print("'><button>");
    client.print(i);
    client.println("</button></a>");
  }
  client.println("</body>");
  client.println("</html>");
}
void pinTriggered(int pin)
{
  Serial.println(pin);
  lan->InputPinTriggered(pin,1);
}
void loop() {
  EthernetClient client = server.available();
  if (client) {
    boolean currentLineIsBlank = true;
    char buffer[100];
    int posBuffer = 0;
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        if (posBuffer < 11)
          buffer[posBuffer++] = c;
        //Serial.write(c);
        if (c == '\n' && currentLineIsBlank) {
          buffer[posBuffer] = '\0';
          writeHtml(client);
          char url[] = "GET /?cmd=1";
          for (int i = 0; i < 10; ++i)
          {
            url[10] = i + 48;
            if (strcmp(buffer, url) == 0)
              pinTriggered(i);
              
          }

          break;
        }
        if (c == '\n') {
          currentLineIsBlank = true;
        }
        else if (c != '\r') {
          currentLineIsBlank = false;
        }
      }
    }
    delay(1);
    client.stop();
  }
}

