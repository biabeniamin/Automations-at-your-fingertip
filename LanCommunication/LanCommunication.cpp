#include "LanCommunication.h"
//#define DO_DEBUG
LanCommunication::LanCommunication(int triggerPin, void(*writeFunct)(int), int(*readFunct)(), int(*countFunct)())
{
	_writeFunct = writeFunct;
	_readFunct = readFunct;
	_countFunct = countFunct;
	_triggerPin = triggerPin;
	pinMode(_triggerPin, OUTPUT);
	_deactivateMax();
}

LanCommunication::~LanCommunication()
{
}
void LanCommunication::_writeByte(int value)
{
#if ASCII_VALUES==1
	_writeFunct(value + 48);
#else
	_writeFunct(value);
#endif
}
int LanCommunication::_readByte()
{
#if ASCII_VALUES==1
	return _readFunct() - 48;
#else
	return _readFunct();
#endif
}
int LanCommunication::_readCount()
{
	return _countFunct();
}
void LanCommunication::_activateMax()
{
	digitalWrite(_triggerPin, HIGH);
}
void LanCommunication::_deactivateMax()
{
	digitalWrite(_triggerPin, LOW);
}
void LanCommunication::_writeVerificationBytes()
{
	for (int i = 0; i < VERIFICATION_BYTE_COUNT; i++)
	{
		_writeByte(_verificationBytes[i]);
	}
}
void LanCommunication::SendCommand(int bytes[COMMUNICATION_BYTE_COUNT])
{
	_activateMax();
	_writeVerificationBytes();
	for (int i = 0; i < COMMUNICATION_BYTE_COUNT; i++)
	{
#ifdef DO_DEBUG
		Serial.print(bytes[i]);
#endif // DO_DEBUG
		_writeByte(bytes[i]);
	}
#ifdef DO_DEBUG
	Serial.println();
#endif // DO_DEBUG
	//delay(1);
	_deactivateMax();
}
void LanCommunication::SendByte(int address, int byte)
{
	int bytes[COMMUNICATION_BYTE_COUNT] = { address,byte };
	SendCommand(bytes);
}
int LanCommunication::IsCommandAvailable()
{
	return _countFunct() >= VERIFICATION_BYTE_COUNT + COMMUNICATION_BYTE_COUNT ? 1 : 0;
}
int LanCommunication::_checkVerificationBytes()
{
	int receivedBytes[VERIFICATION_BYTE_COUNT] = { 0 };
	int founded = 0;
	while (_countFunct() > 0 && founded == 0)
	{
		for (int i = 0; i < VERIFICATION_BYTE_COUNT - 1; ++i)
		{
			receivedBytes[i] = receivedBytes[i + 1];
		}
		receivedBytes[VERIFICATION_BYTE_COUNT - 1] = _readByte();
		int isOk = 1;
		for (int i = 0; i < VERIFICATION_BYTE_COUNT; i++)
		{
			if (_verificationBytes[i] != receivedBytes[i])
			{
				isOk = 0;
				break;
			}
		}
		if (isOk)
		{
			founded = 1;
			break;
		}
	}
	return founded;
}
int LanCommunication::ReadCommand()
{
	int ok = 0;
	if (IsCommandAvailable())
	{
		if (_checkVerificationBytes())
		{
			ok = 1;
			for (int i = 0; i < COMMUNICATION_BYTE_COUNT; i++)
			{
				_bytesInLastCommand[i] = _readFunct();
			}
		}
	}
	return ok;
}
int* LanCommunication::GetLastCommand()
{
	return _bytesInLastCommand;
}
