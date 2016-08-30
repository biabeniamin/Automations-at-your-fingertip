//master
#include<SoftwareSerial.h>
#include <EtherCard.h>
static byte myip[] = { 192, 168, 0, 108 };
static byte gwip[] = { 192, 168, 0, 1 };
static byte mymac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, 0x31 };
byte Ethernet::buffer[500]; // tcp/ip send and receive buffer

const char page[] PROGMEM =
  "HTTP/1.0 503 Service Unavailable\r\n"
  "Content-Type: text/html\r\n"
  "Retry-After: 600\r\n"
  "\r\n"
  "<html>"
  "<head><title>"
  "Automations for everyone"
  "</title></head>"
  "<body>"
  "<H1>Automations for everyone</H1>"
  "<a href='?cmd=1'><button>1</button></a> "
  "<a href='?cmd=2'><button>2</button></a> "
  "<a href='?cmd=3'><button>3</button></a> "
  "<a href='?cmd=4'><button>4</button></a> "
  "<a href='?cmd=5'><button>5</button></a> "
  "<a href='?cmd=6'><button>6</button></a> "


  "</body>"
  "</html>"
  ;


int y[] = {9, 5, 6, 8};
int x[4];
#define address 0
SoftwareSerial pSerial(10,11);
int triggerPin = 4;
void setupEncj()
{
  if (ether.begin(sizeof Ethernet::buffer, mymac, 9) == 0)
    Serial.println( "Failed to access Ethernet controller");
  ether.staticSetup(myip, gwip);
}
void setup()
{
  Serial.begin(9600);
  pSerial.begin(9600);
  setupEncj();
  Serial1.begin(9600);
  pinMode(triggerPin, OUTPUT);
  digitalWrite(triggerPin, LOW);
}
void writeByteMax(int value)
{
  Serial1.write(value + 48);
}
int readByteMax()
{
  return Serial1.read()-48;
}
void sendCommandViaMax(int bytes[])
{
  digitalWrite(triggerPin, HIGH);
  delay(1);
  writeByteMax(9);
  writeByteMax(5);
  writeByteMax(6);
  writeByteMax(8);
  //mySerial.write(48+bytes[1]);
  for (int i = 0; i < 5; ++i)
  {
    writeByteMax(bytes[i]);
    Serial.print("    ");
    Serial.print(bytes[i]);
    Serial.print("    ");
  }
  Serial.println("    ");
  digitalWrite(triggerPin, LOW);
  delay(1);
}
void sendOneByteViaMax(int toAddress, int byte)
{
  x[0] = toAddress;
  x[1] = byte;
  x[2] = 0;
  x[3] = 0;
  sendCommandViaMax(x);
}

void checkMax()
{
  if (Serial1.available() > 8)
  {
    /*int test[]={1,0,8,2,0};
  sendCommandViaMax(test);*/
    while (Serial1.available())
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
        for (int i = 0; i < 5; ++i)
        {
          x[i] = readByteMax();
        }

        if (x[0] == address)
        {
          for (int i = 0; i < 5; ++i)
          {
            //pSerial.print(x[i]);
            Serial.print(x[i]);
            Serial1.print(x[i]);
          }
          Serial.println();
          //pSerial.println();
        }
      }
    }
  }
}
void checkSerial()
{
  if (pSerial.available() > 4)
  {
    for (int i = 0; i < 5; ++i)
    {
      x[i] = pSerial.read()-48;
    }
    sendCommandViaMax(x);
  }
}
void checkEncj()
{
  word pos = ether.packetLoop(ether.packetReceive());
  char* request = "GET /?cmd=1 ";
  if (pos)
  {
    char* data = (char *) Ethernet::buffer + pos;
    for (int i = 0; i < 10; ++i)
    {
      request[10] = i + 48;
      if (strncmp(request, data, strlen(request)) == 0)
      {
        Serial.print(0);
        Serial.print(2);
        Serial.print(address);
        Serial.print(i);
        Serial.print(1);
      }
    }
    memcpy_P(ether.tcpOffset(), page, sizeof page);
    ether.httpServerReply(sizeof page - 1);
  }
}
void loop() {
  checkMax();
  checkSerial();
  checkEncj();
  /*int test[]={1,0,8,2,0};
  sendCommandViaMax(test);
  delay(100);*/
}
