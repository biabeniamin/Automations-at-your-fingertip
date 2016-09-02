/*
 * EEPROM Read
 *
 * Reads the value of each byte of the EEPROM and prints it
 * to the computer.
 * This example code is in the public domain.
 */

#include <EEPROM.h>

// start reading from the first byte (address 0) of the EEPROM
int address = 0;
byte value;

void setup() {
  // initialize serial and wait for port to open:
  Serial.begin(9600);
 
    // wait for serial port to connect. Needed for native USB port only
  
}

void loop() {
  // read a byte from the current address of the EEPROM
    for (int i = 0 ; i <50; i++)
          {
            Serial.print(EEPROM.read(i));
            Serial.print(" ");
          }
          
  
  Serial.println();
  while(1);

  /***
    Advance to the next address, when at the end restart at the beginning.

    Larger AVR processors have larger EEPROM sizes, E.g:
    - Arduno Duemilanove: 512b EEPROM storage.
    - Arduino Uno:        1kb EEPROM storage.
    - Arduino Mega:       4kb EEPROM storage.

    Rather than hard-coding the length, you should use the pre-provided length function.
    This will make your code portable to all AVR processors.
  ***/
  address = address + 1;
  if (address == EEPROM.length()) {
    address = 0;
  }

  /***
    As the EEPROM sizes are powers of two, wrapping (preventing overflow) of an
    EEPROM address is also doable by a bitwise and of the length - 1.

    ++address &= EEPROM.length() - 1;
  ***/

  delay(500);
}
