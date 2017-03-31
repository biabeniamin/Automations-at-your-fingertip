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

void Lan::SetPins(int *inputPinsCount, int *inputPins, int *outputPinsCount, int *outputPins, int *analogPinsCount, int *analogPins, int *analogTriggeredValue)
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
	for (int i = 0; i < *_analogPinsCount; i++)
	{
		_isAnalogTriggered[i] = 1;
	}
	if (_deviceType != 3)
	{
		for (int i = 0; i < *_inputPinsCount; ++i)
		{
			pinMode(_inputPins[i], INPUT_PULLUP);
		}
	}
	for (int i = 0; i < *_outputPinsCount; ++i)
	{
		pinMode(_outputPins[i], OUTPUT);
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
		data[3] = _outputPins[i];
		data[4] = 1;
		data[5] = 0;
		_lanComm->SendCommand(data);
	}
	for (int i = 0; i < *_analogPinsCount; ++i)
	{
		data[0] = MASTER_ADDRESS;
		data[1] = 1;
		data[2] = _address;
		data[3] = _analogPins[i];
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
		data[3] = _inputPins[i];
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

							switch (bytes[3])
							{
							case 0:
								digitalWrite(bytes[2], LOW);
								break;
							case 1:
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
							if (_analogPins[i] == bytes[2])
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
void Lan::CheckAnalogPins()
{
	for (int i = 0; i < *_analogPinsCount; ++i)
	{
		int value = map(analogRead(A0 - _analogPins[i]), 0, 1024, 0, 9);
		/*Serial.print(value);
		Serial.print(" ");
		Serial.print(isAnalogTriggered[i]);
		Serial.print(" ");
		Serial.println(analogTriggeredValue[i]);*/
		if (value>_analogTriggeredValue[i] && _isAnalogTriggered[i] == 0)
		{
			//masterAddress,type,myAddress,pin,value,[0-positiveTriggered,1-negTrr,2-value changed]
			int data[6] = { MASTER_ADDRESS, 2, _address, _analogPins[i], value, 0 };
			_lanComm->SendCommand(data);
			_isAnalogTriggered[i] = 1;
		}
		else if (value <= _analogTriggeredValue[i] && _isAnalogTriggered[i] == 1)
		{
			//masterAddress,type,myAddress,pin,value,[0-positiveTriggered,1-negTrr,2-value changed]
			int data[6] = { MASTER_ADDRESS, 2, _address, _analogPins[i], value, 1 };
			_lanComm->SendCommand(data);
			_isAnalogTriggered[i] = 0;
		}
		else if (value != _lastAnalogValue[i])
		{
			int data[6] = { MASTER_ADDRESS, 2, _address, _analogPins[i], value, 2 };
			_lanComm->SendCommand(data);
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
		int value = digitalRead(_inputPins[i]);
		if (value == 0)
		{
			//Serial.println("trig");
			//masterAddress,respondType,fromAddress,pinNumber,value
			InputPinTriggered(_inputPins[i], value);
			delay(1000);
		}
	}
}