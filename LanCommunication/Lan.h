#ifndef Lan_h
#define Lan_h
#endif // !Lan_h
#include <LanCommunication.h>
#define MASTER_ADDRESS 0
typedef struct
{
	int pinNumber;
	int initializing;
	int activateOnSwitch;
} rPin;
class Lan
{
public:
	Lan(int,int, int, void(*)(int), int(*)(), int(*)());
	void SetPins(int*, rPin*, int*, rPin*, int*, rPin*, int*);
	void SetOutputPinChanged(void(*)(int, int));
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
	rPin *_inputPins;
	int *_lastInputPinValues;
	int *_outputPinsCount;
	rPin *_outputPins;
	int *_analogPinsCount;
	rPin *_analogPins;
	int *_analogTriggeredValue;
	int *_isAnalogTriggered;
	int *_lastAnalogValue;
	int _isOutoutPinChangedFunctDefined = 0;
	void(*_outputPinChanged)(int,int);
};
