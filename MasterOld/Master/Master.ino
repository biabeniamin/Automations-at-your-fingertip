//master
#include <EtherCard.h>
#include <EEPROM.h>
static byte myip[] = { 192,168,0,108 };
static byte gwip[] = { 192,168,0,1 };
static byte mymac[] = { 0x74,0x69,0x69,0x2D,0x30,0x31 };
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
int y[]={9,5,6,8};
int x[4];
#define address 0
int triggerPin=6;
int programmerTriggerPin=5;
void setupEncj()
{
  if (ether.begin(sizeof Ethernet::buffer, mymac,7) == 0) 
    Serial.println( "Failed to access Ethernet controller");
  ether.staticSetup(myip, gwip);
}
void setup()
{
  Serial.begin(9600);
  setupEncj();
  //mySerial.begin(9600);  pinMode(triggerPin,OUTPUT);
  digitalWrite(triggerPin,LOW);
  pinMode(programmerTriggerPin,OUTPUT);
  digitalWrite(programmerTriggerPin,LOW);
}
void sendCommandViaMax(int bytes[])
{
  digitalWrite(triggerPin,HIGH);
  delay(1);
  Serial.write(9+48);
  Serial.write(5+48);
  Serial.write(6+48);
  Serial.write(8+48);
  //mySerial.write(48+bytes[1]);
  for(int i=0;i<5;++i)
  {
    Serial.write(48+bytes[i]);
  }
  digitalWrite(triggerPin,LOW);
  delay(1);
}
void sendOneByteViaMax(int toAddress,int byte)
{
  x[0]=toAddress;
  x[1]=byte;
  x[2]=0;
  x[3]=0;
  sendCommandViaMax(x);
}
void registerDevice(int deviceAddress,int deviceType)
{
  EEPROM.write(0, 0);
  int deviceCount = EEPROM.read(0);
  EEPROM.write(0, 1);
  Serial.println("Device registered");
}
void checkMax()
{  
  if (Serial.available()>8)
  {
    Serial.println("a");
    while(Serial.available())
    {
      for(int i=0;i<3;++i)
      {
        x[i]=x[i+1];
      }
      x[3]=Serial.read()-48;
      bool isOk=true;
      for(int i=0;i<4;++i)
      {
        if(x[i]!=y[i])
        {
          isOk=false;
        }
      }
      if(isOk)
      {
        for(int i=0;i<5;++i)
        {
          x[i]=Serial.read();
        }

        if(x[0]==address)
        {
          Serial.println("test");
          switch(x[1])
          {
            case 0:
              registerDevice(x[2],x[3]);
              break;
          }
          /*for(int i=0;i<5;++i)
          {
            Serial.print(x[i]);
          }
          Serial.println();*/
        }
      }
    }
  }
}
void checkSerial()
{
  if(Serial.available()>4)
  {
    for(int i=0;i<5;++i)
    {
      x[i]=Serial.read()-48;
    }
    //sendCommandViaMax(x);
  }
}
void checkEncj()
{
  word pos=ether.packetLoop(ether.packetReceive());
  char* request="GET /?cmd=1 ";
  if (pos)
  {
    char* data = (char *) Ethernet::buffer + pos;
    for(int i=0;i<10;++i)
    {
      request[10]=i+48;
      if (strncmp(request, data,strlen(request)) == 0)
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
void checkProgrammerButton()
{
  
}
void loop() {
  checkMax();
  checkSerial();
  checkEncj();
  checkProgrammerButton();
}
