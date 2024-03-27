/*
  UDP_Servidor_Multicast
  Created:  2022/09/01
  Last modified: 2022/09/03 23:55:15
  Version: 1.0

  Author: Miguel Grassi
  Contact: migrassi@gmail.com

  Description:
  Requiere un cable con extremo desnudo o soldado a una plaquita PCB conectada al pin
  del Touch0 (GPIO 4 del ESP32 - 7º pin de la derecha, contando desde abajo en NodeMCU)

  Se suscribe a un grupo UDP Multicast (239.1.2.3)
  Ver: https://www.iana.org/assignments/multicast-addresses/multicast-addresses.xhtml

      Al iniciar imprime por puerto serie el IP y puerto UDP que está escuchando

      Cada vez que recibe un mensaje por UDP imprime en el puerto serie:
      - El tipo de mensaje (Unicast, Multicast o Broadcast)
      - El IP y puerto UDP del Sender
      - El IP y puerto UDP propio
      - El Payload (contenido) del mensaje

      Retransmite al grupo UDP lo que recibe en el puerto serie

      Si recibe un número entero o float lo guarda en la variable salto

      Cuando recibe contacto en el Touch 0 manda un al valor guardado en la var salto por el UDP.

  Ver proyecto completo en https://github.com/migrassi/ESP32-Unity-UDP

  (c) 2022 - https://MiguelGrassi.com.ar

*/

#include <Arduino.h> // Esta linea no hace falta si se usa el entorno Arduino
#include "WiFi.h"
#include "AsyncUDP.h"


const char * ssid = "S7YesicaAndroid";
const char * password = "yesica666";

const int puerto = 1234;

AsyncUDP udp;
String miBuffer;
float auxi;
String salto = "1";
String value1 = "1";
String value2 = "2";

int threshold = 40;// Original 60
bool touch0detected = false;
void gotTouch0()
{
  touch0detected = true;
}

#define POTENTIOMETER_PIN  33  // ESP32 pin GIOP36 (ADC0) connected to Potentiometer pin
int sensorValue = 0;        // value read from the pot
int outputValue = 0;        // value output to the PWM (analog
//r1 34
//r2 35
//r3 32
//r4 33
////////////// TFT SCREEN ///////////////////////////////////////
#include <SPI.h>
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#define SCREEN_WIDTH 128 // OLED display width, in pixels
#define SCREEN_HEIGHT 64 // OLED display height, in pixels
// Declaration for an SSD1306 display connected to I2C (SDA, SCL pins)
#define OLED_RESET     -1 // Reset pin # (or -1 if sharing Arduino reset pin)
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);
///////////////////////////////////////////////////////////////////
void setup()
{
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  if (WiFi.waitForConnectResult() != WL_CONNECTED) {
    Serial.println("WiFi Failed");
    while (1) {
      delay(1000);
    }
  }

  ///////////// TFT DISPLAY /////////////
  // SSD1306_SWITCHCAPVCC = generate display voltage from 3.3V internally
  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    Serial.println(F("SSD1306 allocation failed"));
    for (;;); // Don't proceed, loop forever
  }

  touchAttachInterrupt(T0, gotTouch0, threshold);
  if (udp.listenMulticast(IPAddress(239, 1, 2, 3), puerto)) {
    Serial.print("Escuchando UDP en IP: ");
    Serial.println(WiFi.localIP());
    Serial.print("  Puerto:  ");
    Serial.println(puerto);
    udp.onPacket([](AsyncUDPPacket packet) {

      /////// Display values on the screen
      display.clearDisplay();

      display.setTextSize(1);             // Normal 1:1 pixel scale
      display.setTextColor(WHITE);        // Draw white text
      display.setCursor(0, 0);
      //display.println(myIP);// Start at top-left corner
      display.println(F("Client connected ?"));

      display.setTextSize(2);             // Draw 2X-scale text
      display.setTextColor(WHITE);
      display.setCursor(5, 16);
      //display.println(F("BELLY"));

      display.setTextSize(4);             // Draw 2X-scale text
      display.setTextColor(WHITE);
      display.setCursor(6, 35);
      //display.println(6 * norm);

      display.display();
      delay(50);

      Serial.print("Tipo de paquete UDP: ");
      Serial.print(packet.isBroadcast() ? "Broadcast" : packet.isMulticast() ? "Multicast" : "Unicast");
      Serial.print(", De: ");
      Serial.print(packet.remoteIP());
      Serial.print(":");
      Serial.print(packet.remotePort());
      Serial.print(", To: ");
      Serial.print(packet.localIP());
      Serial.print(":");
      Serial.print(packet.localPort());
      Serial.print(", Longitud: ");
      Serial.print(packet.length());
      Serial.print(", Data: ");
      Serial.write(packet.data(), packet.length());
      Serial.println();
      //reply to the client
      packet.printf("Recibí %u bytes de datos", packet.length());
    });
    //Send multicast
    udp.print("Hola Grupo!");
  }
}

void loop()
{
  if (Serial.available()) {      // Si viene algo en el puerto Serial (USB),
    // udp.print(Serial.read());   // Manda por udp al grupo Multicast cada caracter que le llega
    //miBuffer = Serial.readString(); // Espera hasta el TimeOut. Es más lenta pero lee la cadena entera
    miBuffer = Serial.readStringUntil('\n');//Lee la cadena hasta el caracter indicado o toda por timeout si el caracter no está
  }

  if (miBuffer != "")//Si hay un string en miBuffer
  {
    udp.print(miBuffer);   // Lo manda por udp al grupo Multicast. Unity no recibe esto porque no se suscribe aparentemente
    udp.broadcastTo(miBuffer.c_str(), 1234); // Pero si recibe el broadcast
    auxi = miBuffer.toFloat(); // Además, si es convertible a un número, lo carga en una variable auxiliar
    if (auxi == 0) // Aunque evito que quede en cero
    {
      salto = "1";
    }
    else
    {
      salto = String(auxi);
    }

    miBuffer = "";


  }

  if (touch0detected) {
    touch0detected = false;
    Serial.print("Touch 0 detectado: ");
    Serial.print(salto);
    Serial.println();
    udp.broadcastTo(salto.c_str(), 1234);
  }

  sensorValue = analogRead(POTENTIOMETER_PIN);
  outputValue = map(sensorValue, 1023, 0, 20, 0);

  if (outputValue >= 3 )
  {
    digitalWrite (LED_BUILTIN, HIGH);
    //sendValues();//SEND DATA
    udp.broadcastTo(String(outputValue).c_str(), 1234);
    Serial.println(2);

    /////// Display values on the screen
    display.clearDisplay();

//    if (invertScreen == true)
//    {
//      display.invertDisplay(true);
//    }

    display.setTextSize(1);             // Normal 1:1 pixel scale
    display.setTextColor(WHITE);        // Draw white text
    display.setCursor(0, 0);
    //display.println(myIP);// Start at top-left corner
    display.println(F("Client connected ?"));

    display.setTextSize(2);             // Draw 2X-scale text
    display.setCursor(5, 16);
    display.setTextColor(WHITE);
    //display.println(F("BELLY"));

    display.setTextSize(4);
    display.setCursor(6, 35);
    display.println(2);

    display.display();
  }
  else if (outputValue < 3 )
  {
    digitalWrite (LED_BUILTIN, LOW);
    Serial.println(1);
    udp.broadcastTo(String(outputValue).c_str(), 1234);

    /////// Display values on the screen
    display.clearDisplay();
//
//    if (invertScreen == true)
//    {
//      display.invertDisplay(true);
//    }

    display.setTextSize(1);             // Normal 1:1 pixel scale
    display.setTextColor(WHITE);        // Draw white text
    display.setCursor(0, 0);
    //display.println(myIP);// Start at top-left corner
    //display.println(F("Client connected ?"));

    display.setTextSize(2);             // Draw 2X-scale text
    display.setCursor(5, 16);
    display.setTextColor(WHITE);
    //display.println(F("BELLY"));

    display.setTextSize(4);
    display.setCursor(6, 35);
    display.println(1);

    display.display();

  }

  Serial.print("outpuValue: ");
  Serial.print(outputValue);
  Serial.println();
}

//void sendValues() {
//  String builtString = String("");
//  builtString += String((int)outputValue);
//
//  udp.broadcastTo(builtString.c_str(), 1234);
//  Serial.print("builtString ");
//  Serial.print(builtString);
//  Serial.println();
//}
