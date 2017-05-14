//NetworkModule
#define ADDRESS 1
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 3
#include<SoftwareSerial.h>
#include <EtherCard.h>
#include <Lan.h>
SoftwareSerial serial(10, 11);
int inputPinsCount =6;
rPin inputPins[] = {{.pinNumber=0,.initializing=0},
{.pinNumber=1,.initializing=0},
{.pinNumber=2,.initializing=0},
{.pinNumber=3,.initializing=0},
{.pinNumber=4,.initializing=0},
{.pinNumber=5,.initializing=0}};
int outputPinsCount = 4;
rPin outputPins[] = {{.pinNumber=6,.initializing=0},
{.pinNumber=7,.initializing=0},
{.pinNumber=8,.initializing=0},
{.pinNumber=9,.initializing=0}};
int analogPinsCount = 0;
rPin analogPins[] = {{.pinNumber=0,.initializing=0}};
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
Lan lan(ADDRESS, DEVICE_TYPE, TRIGGERED_PIN, &writeLan, &readLan, &countLan);
static byte myip[] = { 192, 168, 0, 108 };
static byte gwip[] = { 192, 168, 0, 1 };
static byte mymac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, 0x31 };
byte Ethernet::buffer[1000];
int notificationId = 0;
const char page[] PROGMEM =
  "HTTP/1.1 200 OK\r\n"
  "Content-Type: text/html\r\n"
  "\r\n"
  "<html>"
  "<head><title>"
  "Automations for everyone"
  "</title>"
  "<style>button {width:70px;height:70px;background-color:#F2D8C2;}</style>"
  "</head>"
  "<body style='background-color: #20448c;'>"
  "<div style='margin:auto;width:50%;text-align:center;background-color:#517EA6;padding:20px;'>"
  "<h1>Automations for everyone</h1>"
  "<a href='?cmd=1'><button>1</button></a> "
  "<a href='?cmd=2'><button>2</button></a> "
  "<a href='?cmd=3'><button>3</button></a> "
  "<a href='?cmd=4'><button>4</button></a> "
  "<a href='?cmd=5'><button>5</button></a> "
  "<a href='?cmd=6'><button>6</button></a> "

  "</div>"
  "</body>"
  "</html>"
  ;
const char notificationResponse0[] PROGMEM ="HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n0";
const char notificationResponse6[] PROGMEM ="HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n6";
const char notificationResponse7[] PROGMEM ="HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n7";
const char notificationResponse8[] PROGMEM ="HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n8";
const char notificationResponse9[] PROGMEM ="HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n9";
char *notificationResponses[]={notificationResponse0,notificationResponse6,notificationResponse7,notificationResponse8,notificationResponse9};
void setupEncj()
{
  if (ether.begin(sizeof Ethernet::buffer, mymac, 3) == 0)
    Serial.println( "Failed to access Ethernet controller");
  ether.staticSetup(myip, gwip);
}
void setup()
{
  Serial.begin(9600);
  serial.begin(9600);
  lan.SetPins(&inputPinsCount, inputPins, &outputPinsCount, outputPins, &analogPinsCount, analogPins, analogTriggeredValue);
  lan.Register();
  lan.SetOutputPinChanged(outputPinChanged);
  setupEncj();
}
void outputPinChanged(int pinNumber,int value)
{
  Serial.println(pinNumber);
  if(pinNumber>5)
  {
    if(value==1)
    {
      notificationId=pinNumber-5;
    }
  }
}
void checkEncj()
{
  word pos = ether.packetLoop(ether.packetReceive());
  char* request = "GET /?cmd=1 ";
  char* notificationCheck = "GET /notif ";
  if (pos)
  {
    char* data = (char *) Ethernet::buffer + pos;
    if (strncmp(notificationCheck, data, strlen(notificationCheck)) == 0)
    {
      memcpy_P(ether.tcpOffset(), notificationResponses[notificationId], sizeof notificationResponse0);
      ether.httpServerReply(sizeof notificationResponse0 - 1);
      notificationId = 0;
    }
    else
    {
      for (int i = 0; i < 10; ++i)
      {
        request[10] = i + 48;
        if (strncmp(request, data, strlen(request)) == 0)
        {
          Serial.println(i);
          lan.InputPinTriggered(i, 1);
        }
      }
      memcpy_P(ether.tcpOffset(), page, sizeof page);
      ether.httpServerReply(sizeof page - 1);
    }
  }
}
void loop()
{
  lan.CheckMessages();
  checkEncj();
}
