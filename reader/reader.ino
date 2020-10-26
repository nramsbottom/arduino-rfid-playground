#include <SPI.h>
#include <MFRC522.h>

MFRC522 mfrc522(10, 9); // MFRC522 mfrc522(SS_PIN, RST_PIN)

void setup() {

  Serial.begin(115200);
  SPI.begin();      // Init SPI bus
  mfrc522.PCD_Init();   // Init MFRC522

}

void loop() {

  // Look for new cards
  if ( ! mfrc522.PICC_IsNewCardPresent()) {
    return;
  }

  // Select one of the cards
  if ( ! mfrc522.PICC_ReadCardSerial()) {
    return;
  }

  //Reading from the card
  String tag = "";
  for (byte i = 0; i < mfrc522.uid.size; i++)
  {
    tag.concat(String(mfrc522.uid.uidByte[i] < 0x10 ? "0" : ""));
    tag.concat(String(mfrc522.uid.uidByte[i], HEX));
  }
  tag.toUpperCase();

  Serial.print("CARD");
  Serial.print("|");
  Serial.println(tag);

  delay(2000);
}
