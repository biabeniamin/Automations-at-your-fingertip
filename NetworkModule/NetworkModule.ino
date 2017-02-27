//NetworkModule
#define ADDRESS 1
#define TRIGGERED_PIN 9
#define DEVICE_TYPE 3
#include<SoftwareSerial.h>
#include <EtherCard.h>
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
static byte myip[] = { 192, 168, 0, 108 };
static byte gwip[] = { 192, 168, 0, 1 };
static byte mymac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, 0x31 };
byte Ethernet::buffer[1000];
const char page[] PROGMEM =
  "HTTP/1.0 503 Service Unavailable\r\n"
  "Content-Type: text/html\r\n"
  "Retry-After: 600\r\n"
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
void setupEncj()
{
  if (ether.begin(sizeof Ethernet::buffer, mymac, 8) == 0)
    Serial.println( "Failed to access Ethernet controller");
  ether.staticSetup(myip, gwip);
}
void setup()
{
  Serial.begin(9600);
  serial.begin(9600);
  lan.SetPins(&inputPinsCount,inputPins,&outputPinsCount,outputPins,&analogPinsCount,analogPins,analogTriggeredValue);
  lan.Register();
  setupEncj();
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
        lan.InputPinTriggered(i,1);
      }
    }
    memcpy_P(ether.tcpOffset(), page, sizeof page);
    ether.httpServerReply(sizeof page - 1);
  }
}
void loop()
{
  lan.CheckMessages();
  checkEncj();
}
