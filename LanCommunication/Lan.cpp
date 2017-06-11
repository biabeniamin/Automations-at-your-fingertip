#include "Lan.h"
Lan::Lan(int address, int deviceType, int triggerPin, void(*writeFunct)(int), int(*readFunct)(), int(*countFunct)())
{
	_lanComm = new LanCommunication(triggerPin, writeFunct, readFunct, countFunct);
	_address = address;
	_deviceType = deviceType;
}

Lan::~Lan()
{
}

void Lan::SetPins(int *inputPinsCount, rPin *inputPins, int *outputPinsCount, rPin *outputPins, int *analogPinsCount, rPin *analogPins, int *analogTriggeredValue)
{
	_inputPinsCount = inputPinsCount;
	_inputPins = inputPins;
	_outputPins = outputPins;
	_outputPinsCount = outputPinsCount;
	_analogPins = analogPins;
	_analogPinsCount = analogPinsCount;
	_analogTriggeredValue = analogTriggeredValue;
	_isAnalogTriggered = (int*)malloc(*_analogPinsCount * sizeof(int));
	_lastAnalogValue= (int*)malloc(*_analogPinsCount * sizeof(int));
	_lastInputPinValues= (int*)malloc(*_inputPinsCount * sizeof(int));
	for (int i = 0; i < *_analogPinsCount; i++)
	{
		_isAnalogTriggered[i] = 1;
	}
	for (int i = 0; i < *_inputPinsCount; ++i)
	{
		_lastInputPinValues[i]=1;
		if(_inputPins[i].initializing==1)
			pinMode(_inputPins[i].pinNumber, INPUT_PULLUP);
	}
	for (int i = 0; i < *_outputPinsCount; ++i)
	{
		if (_outputPins[i].initializing == 1)
			pinMode(_outputPins[i].pinNumber, OUTPUT);
	}
}
void Lan::SetOutputPinChanged(void(*pinChangedFunct)(int,int))
{
	_outputPinChanged = pinChangedFunct;
	_isOutoutPinChangedFunctDefined = 1;
}
void Lan::Register()
{
	int data[6] = { MASTER_ADDRESS, 0, _address, _deviceType, 0 };
	_lanComm->SendCommand(data);
	//toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
	//Serial.println(*_outputPinsCount);
	for (int i = 0; i < *_outputPinsCount; ++i)
	{
		data[0] = MASTER_ADDRESS;
		data[1] = 1;
		data[2] = _address;
		data[3] = _outputPins[i].pinNumber;
		data[4] = 1;
		data[5] = 0;
		_lanComm->SendCommand(data);
	}
	for (int i = 0; i < *_analogPinsCount; ++i)
	{
		data[0] = MASTER_ADDRESS;
		data[1] = 1;
		data[2] = _address;
		data[3] = _analogPins[i].pinNumber;
		data[4] = 2;
		data[5] = 0;
		_lanComm->SendCommand(data);
	}
	//toAddress,typeOfResponse,address,pinNumber,pinType(0-input,1-output,2-analog)
	for (int i = 0; i < *_inputPinsCount; ++i)
	{
		data[0] = MASTER_ADDRESS;
		data[1] = 1;
		data[2] = _address;
		data[3] = _inputPins[i].pinNumber;
		data[4] = 0;
		data[5] = 0;
		_lanComm->SendCommand(data);
	}
	data[0] = MASTER_ADDRESS;
	data[1] = 4;
	data[2] = _address;
	data[3] = 0;
	data[4] = 0;
	data[5] = 0;
	_lanComm->SendCommand(data);
}
void Lan::CheckMessages()
{
	if (_lanComm->IsCommandAvailable())
	{
		if (_lanComm->ReadCommand())
		{
			int *bytes = _lanComm->GetLastCommand();
			if (bytes[0] == _address)
			{
				switch (bytes[1])
				{
					case 0:
						if (_isOutoutPinChangedFunctDefined)
						{
							_outputPinChanged(bytes[2], bytes[3]);
						}
						else
						{

							Serial.println("pin change");
							Serial.print("pin=");
							Serial.print(bytes[2]);
							Serial.print(" com=");
							Serial.print(bytes[3]);
							switch (bytes[3])
							{
							case 0:
								digitalWrite(bytes[2], LOW);
								break;
							case 1:
								digitalWrite(bytes[2], HIGH);
								break;
							case 2:
								int status = digitalRead(bytes[2]);
								if (status == 1)
									status = 0;
								else
									status = 1;
								digitalWrite(bytes[2], status);
								break;
							}
						}
						break;
					case 1:
						if (_address != 0)
							delay(10);
						Register();
						break;
					case 2:
						for (int i = 0; i < *_analogPinsCount; ++i)
						{
							if (_analogPins[i].pinNumber == bytes[2])
							{
								_analogTriggeredValue[i] = bytes[3];
							}
						}
						break;
				}
			}
		}
	}
}
int count;
void Lan::CheckAnalogPins()
{
	for (int i = 0; i < *_analogPinsCount; ++i)
	{
		int value = map(analogRead(A0 + _analogPins[i].pinNumber), 0, 1024, 0, 9);
		/*Serial.print(value);
		Serial.print(" ");
		Serial.print(isAnalogTriggered[i]);
		Serial.print(" ");
		Serial.println(analogTriggeredValue[i]);*/
		if (value>_analogTriggeredValue[i] && _isAnalogTriggered[i] == 0)
		{
			//masterAddress,type,myAddress,pin,value,[0-positiveTriggered,1-negTrr,2-value changed]
			int data[6] = { MASTER_ADDRESS, 2, _address, _analogPins[i]. pinNumber, value, 0 };
			_lanComm->SendCommand(data);
			_isAnalogTriggered[i] = 1;
		}
		else if (value <= _analogTriggeredValue[i] && _isAnalogTriggered[i] == 1)
		{
			//masterAddress,type,myAddress,pin,value,[0-positiveTriggered,1-negTrr,2-value changed]
			int data[6] = { MASTER_ADDRESS, 2, _address, _analogPins[i].pinNumber, value, 1 };
			_lanComm->SendCommand(data);
			_isAnalogTriggered[i] = 0;
		}
		else if (value != _lastAnalogValue[i])
		{
			int data[6] = { MASTER_ADDRESS, 2, _address, _analogPins[i].pinNumber, value, 2 };
			//_lanComm->SendCommand(data);
		}
		_lastAnalogValue[i] = value;
	}
}
void Lan::InputPinTriggered(int pin, int value)
{
	int data[6] = { MASTER_ADDRESS, 2, _address, pin, value, 0 };
	_lanComm->SendCommand(data);
}
void Lan::CheckInputPins()
{
	for (int i = 0; i < *_inputPinsCount; ++i)
	{
		int value = digitalRead(_inputPins[i].pinNumber);
		if(_inputPins[i].activateOnSwitch==1)
		{
			if(value!=_lastInputPinValues[i])
			{
				InputPinTriggered(_inputPins[i].pinNumber, value);
				delay(1000);
			}
		}
		else
		{
			if (value == 0)
			{
				//Serial.println("trig");
				//masterAddress,respondType,fromAddress,pinNumber,value
				InputPinTriggered(_inputPins[i].pinNumber, value);
				delay(1000);
			}
		}
		_lastInputPinValues[i]=value;
	}
}