#include <Encoder.h>
#include <Button.h>

Encoder myEnc(2, 3);
long oldPosition  = -999;
Button button1(5);

int volume = 50;

void setup() {
  Serial.begin(9600);
  button1.begin();

}


void resetVol(int val) {
  myEnc.write(val);
  oldPosition = val;
  
}

void loop() {
  while (Serial.available()) {
    byte v = Serial.read();
    resetVol(v);
    Serial.read();
  }

  if (button1.pressed()) {
    resetVol(50);
  }

  long newPosition = myEnc.read();
  if (newPosition != oldPosition) {
    oldPosition = newPosition;
    if (newPosition < 0) {
      resetVol(0);
      newPosition = 0;
    }
    if (newPosition > 100) {
      resetVol(100);
      newPosition = 100;
    }
    Serial.write(newPosition);
  }


}
