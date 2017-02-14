#ifndef LanCommunication_h
#define LanCummunication_h
#endif // !LanCommunication_h
#define COMMUNICATION_BYTE_COUNT 6
#define VERIFICATION_BYTE_COUNT 4
#define ASCII_VALUES 0
#include "Arduino.h"
class LanCommunication
{
public:
	LanCommunication(int, void (*)(int),int (*)(),int (*)());
	~LanCommunication();
	void SendCommand(int bytes[COMMUNICATION_BYTE_COUNT]);
	void SendByte(int,int);
	int IsCommandAvailable();
	int ReadCommand();
	int *GetLastCommand();
private:
	//functions
	void _writeVerificationBytes();
	void _writeByte(int value);
	int _readByte();
	int _readCount();
	void _activateMax();
	void _deactivateMax();
	int _checkVerificationBytes();
	//variables
	int _bytesInLastCommand[COMMUNICATION_BYTE_COUNT];
	int _verificationBytes[VERIFICATION_BYTE_COUNT] = { 9, 5, 6, 8 };
	int _triggerPin;
	void (*_writeFunct)(int);
	int(*_readFunct)();
	int(*_countFunct)();
};

