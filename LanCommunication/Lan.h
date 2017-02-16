#ifndef Lan_h
#define Lan_h
#endif // !Lan_h
#include <LanCommunication.h>
#define MASTER_ADDRESS 0
class Lan
{
public:
	Lan(int,int, int, void(*)(int), int(*)(), int(*)());
	void SetPins(int*, int*, int*, int*, int*, int*, int*);
	void Register();
	void CheckMessages();
	void CheckAnalogPins();
	void CheckInputPins();
	void InputPinTriggered(int, int);
	~Lan();

private:
	LanCommunication *_lanComm;
	int _address;
	int _deviceType;
	int *_inputPinsCount;
	int *_inputPins;
	int *_outputPinsCount;
	int *_outputPins;
	int *_analogPinsCount;
	int *_analogPins;
	int *_analogTriggeredValue;
	int *_isAnalogTriggered;
};
