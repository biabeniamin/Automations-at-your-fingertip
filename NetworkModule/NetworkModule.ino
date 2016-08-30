//NetworkModule
#include <SoftwareSerial.h>
#include <EtherCard.h>
static byte myip[] = { 192, 168, 0, 108 };
static byte gwip[] = { 192, 168, 0, 1 };
static byte mymac[] = { 0x74, 0x69, 0x69, 0x2D, 0x30, 0x31 };
byte Ethernet::buffer[500];
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
  
SoftwareSerial mySerial(10, 11);
int y[] = {9, 5, 6, 8};
int x[5];
int address = 1;
int triggerPin = 7;
int masterAddress = 0;
//type(0-master,1-relay,2-keyboard,3-network)
int deviceType = 3;
#define inputPinsCount 5
int inputPins[inputPinsCount]={1,2,3,4,5};
#define outputPinsCount 1
int outputPins[outputPinsCount]={8};
void registerInLan()
{
  //toAddress,typeOfResponse(0-register,1-PinRegister),fromAddress,type(0-master,1-relay,2-keyboard,3-network)
  int data[5] = {masterAddress, 0, address, deviceType};
  sendCommandViaMax(data);
  //toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
  for(int i=0;i<outputPinsCount;++i)
  {
    data[0]=masterAddress;
    data[1]=1;
    data[2]=address;
    data[3]=outputPins[i];
    data[4]=1;
    sendCommandViaMax(data);
  }
  //toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
  for(int i=0;i<inputPinsCount;++i)
  {
    data[0]=masterAddress;
    data[1]=1;
    data[2]=address;
    data[3]=inputPins[i];
    data[4]=0;
    sendCommandViaMax(data);
  }
  data[0] = masterAddress;
  data[1] = 4;
  data[2] = address;
  data[3] = 0;
  data[4] = 0;
  sendCommandViaMax(data);
}
void setupEncj()
{
  if (ether.begin(sizeof Ethernet::buffer, mymac, 9) == 0)
    Serial.println( "Failed to access Ethernet controller");
  ether.staticSetup(myip, gwip);
}
void setup()
{
  Serial.begin(9600);
  mySerial.begin(9600);
  pinMode(triggerPin, OUTPUT);
  digitalWrite(triggerPin, LOW);
  registerInLan();
  setupEncj();
  for(int i=0;i<inputPinsCount;++i)
  {
    pinMode(inputPins[i],INPUT_PULLUP);
  }
  for(int i=0;i<outputPinsCount;++i)
  {
    pinMode(outputPins[i],OUTPUT);
  }
}
void sendCommandViaMax(int bytes[])
{
  digitalWrite(triggerPin, HIGH);
  delay(1);
  mySerial.write(y[0] + 48);
  mySerial.write(y[1] + 48);
  mySerial.write(y[2] + 48);
  mySerial.write(y[3] + 48);
  //mySerial.write(48+bytes[1]);
  for (int i = 0; i < 5; ++i)
  {
    mySerial.write(48 + bytes[i]);
    Serial.print(bytes[i]);
    Serial.print("    ");
  }
  Serial.println("    ");
  digitalWrite(triggerPin, LOW);
  delay(1);
}
void sendOneByteViaMax(int address, int byte)
{
  x[0] = address;
  x[1] = byte;
  x[2] = 0;
  x[3] = 0;
  x[4] = 0;
  sendCommandViaMax(x);
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
      x[3] = mySerial.read() - 48;
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
          x[i] = mySerial.read() - 48;
        }
        if (x[0] == address)
        {
          switch(x[1])
          {
            case 0:
              switch(x[3])
              {
                case 0:
                  digitalWrite(x[2],LOW);
                  break;
                case 1:
                  digitalWrite(x[2],HIGH);
                  break;
                case 2:
                  int status=digitalRead(x[2]);
                  if(status==1)
                    status=0;
                  else
                    status=1;
                  digitalWrite(x[2],status);
                  break;
              }
              break;
            case 1:
              registerInLan();
              break;
          }
          for (int i = 0; i < 5; ++i)
          {
            Serial.print(x[i]);
            Serial.print(" ");
          }
          Serial.println();
        }
      }
    }
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
        int com[]={0,2,address,i,1};
        sendCommandViaMax(com);
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
  checkEncj();
  if(Serial.available()>0)
  {
    registerInLan();
    int afas=Serial.read();
  }
  /*for(int i=0;i<inputPinsCount;++i)
  {
    int value=digitalRead(inputPins[i]);
    if(value==0)
    {
      //masterAddress,respondType,fromAddress,pinNumber,value
      int data[5] = {masterAddress, 2, address, inputPins[i],value};
      sendCommandViaMax(data);
      delay(500);
    }
  }*/
}
