#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <MedianFilter.h>
#include <ArduinoOSCWiFi.h>

const char* ssid = "ESPdatos";
const char* pwd = "respiracion";
const char* host = "192.168.4.2";
const int publish_port = 6969;

// OLED display dimensions
#define SCREEN_WIDTH 128 // OLED display width, in pixels
#define SCREEN_HEIGHT 64 // OLED display height, in pixels
// Declaration for an SSD1306 display connected to I2C (SDA, SCL pins)
#define OLED_RESET -1
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

#define ANALOG_IN 34

int calTime = 40000;//10.000  10 seg

// Median filter variables
MedianFilter medianValues(32, 0); // first value can be up to 255
int medianFilteredValue;
int EMA = 0;
float EMAconstant = 0.1;
int finalValue;// 0.0 - 1.0, percent of range, second value is a preset - i leave it zero

// calibration variables?????? 
//I don't understand if max and min are the values of the capacity that the stretchable has by default, (200-3900)
//or if it is the range of values that I use when the stretchable is being used, i.e., at rest and in exhalation (700-400)
int maxPotValue = 700;// 39000; //0;// // initialize very big 0
int minPotValue = 400;// 200;//900;// // initialize very big 2000
int potRange = 0;
int potThreshold = 0;
float thresholdAmount = 0.5;//// 0.0 to 1.0, percent of range
int avgPotReading;
int lastPotValue = 0;
int crossedThreshold = 0;

// indicator variables
int ledPin = 2;
int count = 0;

void setup() {
  Serial.begin(9600);

  // Connect to Wifi
  WiFi.disconnect(true, true);
  delay(1000);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, pwd);
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }
  Serial.println();
  Serial.print("WiFi connected, IP = ");
  Serial.println(WiFi.localIP());////////////////

  pinMode(ANALOG_IN, INPUT); // Potentiometer pin
  pinMode(ledPin, OUTPUT);
  pinMode(4, INPUT); // Set touch pin 4 as input for touch sensor

  Serial.println("Setup complete");

  // Initialize the OLED display
  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) { // Address 0x3C for 128x64
    Serial.println(F("SSD1306 allocation failed"));
    for (;;);}
  display.clearDisplay();
  display.setTextSize(1);
  display.setTextColor(SSD1306_WHITE);
  display.setCursor(0, 0);
  display.println("ESP32 Touch Test");
  display.println("WiFi Status: " + String(WiFi.status()));
  display.println("IP Address: " + WiFi.localIP().toString());
  display.display(); // Show initial text

  calibrate();//Execute calibration on startup
}

void loop() {

  int touchValue = touchRead(4); // Read the touch value from GPIO 4
  if ((touchValue >= 8 && touchValue <= 12) || (touchValue >= 20 && touchValue <= 30)) {
    calibrate(); // Execute calibration
  }

  // Send raw potentiometer value over OSC
  OscWiFi.send(host, publish_port, "/belly", lastPotValue);///is calibration values! RAW
  OscWiFi.update();
  
  // *******  get simple average of (X) distances
  avgPotReading = averagePotValue(10);// average (X) readings

  // Display the avgPotReading on OLED
  display.clearDisplay();
  display.setTextColor(WHITE);
  display.setTextSize(1); // Set text size back to 1 for other messages
  display.setCursor(0, 10);
  display.print("Avg Pot Reading: ");
  display.setCursor(0, 24);
  display.setTextSize(2);
  display.println(avgPotReading);
  display.display();

  ////////////////////////////////////////////////
  // *******  filter readings -- as needed
  // get median of distances
  medianValues.in(avgPotReading);
  medianFilteredValue = medianValues.out();
  // get low pass -- or Exponential Moving Average
  EMA = (int)(EMAconstant * medianFilteredValue) + ((1.0 - EMAconstant) * EMA);
  
  // *******  decide which value you want to use
  // can be avgBrightnessReading OR medianFilteredValue OR EMA
  finalValue = avgPotReading;

  // ******* check <finalValue> against <xxxxxxThreshold>
  crossedThreshold = checkThreshold(finalValue);

  switch (crossedThreshold) {
    case (0):// Below threshold code here
      Serial.println("Value is BELOW threshold");
      OscWiFi.send(host, publish_port, "/chest", 1); // Send 0 for below threshold
      OscWiFi.update(); // Update to ensure data is sent
      break;

    case (1):// Above threshold code here
      Serial.println("Value is ABOVE threshold");
      OscWiFi.send(host, publish_port, "/chest", 2); // Send 0 for below threshold
      OscWiFi.update(); // Update to ensure data is sent
      display.setTextSize(2); // Set text size to 2 for the message
      display.setCursor(0, 50);
      display.println("Threshold Crossed!");
      break;
  }

  Serial.print("Average Potentiometer Value: ");
  Serial.println(avgPotReading);

  delay(100);
}

void calibrate() {
  unsigned long start = millis();
  digitalWrite(ledPin, HIGH);
  Serial.println("Calibrating potentiometer...");
  display.clearDisplay(); // Clear the display before starting calibration
  display.setTextSize(1);
  display.setTextColor(SSD1306_WHITE);
  display.setCursor(0, 0);
  display.println("Calibrating...");
  display.display(); // Display the initial message
  delay(500); // Delay to stabilize the display

  do {
    int potValue = analogRead(ANALOG_IN); // Read potentiometer value
    lastPotValue = potValue; // Store raw potentiometer value
    calibrate(potValue);
    OscWiFi.send(host, publish_port, "/calibration", lastPotValue); // Envía valores crudos durante la calibración
    OscWiFi.update(); // Actualiza para asegurarse de que se envíen los datos
    Serial.print("Raw values : ");
    Serial.println(lastPotValue);

    delay(50); // Ajusta el retardo según sea necesario para controlar la frecuencia de actualización
    // Print raw min and max inputs on OLED screen
    display.clearDisplay();
    display.setTextSize(1); // Tamaño de texto 1 para "Raw:"
    display.setTextColor(SSD1306_WHITE);
    display.setCursor(0, 0);
    display.print("Raw: ");
    display.setTextSize(5); // Tamaño de texto 3 para lastPotValue
    display.setTextColor(SSD1306_WHITE);
    display.setCursor(0, 24); // Mueve el cursor al siguiente renglón
    display.println(lastPotValue); // Imprime el valor de lastPotValue
    display.display(); // Actualiza la pantalla
    delay(50); // Adjust delay as needed to control refresh rate
  } while (millis() - start < calTime);

  digitalWrite(ledPin, LOW);
  Serial.println("Calibration complete");
}

void calibrate(int potValue) {
  maxPotValue = max(maxPotValue, potValue);
  minPotValue = min(minPotValue, potValue);
  potRange = maxPotValue - minPotValue;
  potThreshold = minPotValue + int(thresholdAmount * potRange);
}

int averagePotValue(int times) {
  int sum = 0;
  for (int i = 0; i < times; i++) {
    sum += analogRead(ANALOG_IN);
    delay(10);
  }
  return sum / times;
}

void displayCalibrationValues() {
  display.clearDisplay();
  display.setTextColor(WHITE);
  display.setTextSize(1);
  display.setCursor(0, 0);
  display.print("Calibrating...");
  display.display();
}

int checkThreshold(int value) {
  if (value > potThreshold)
    return 1; // Above threshold
  else
    return 0; // Below threshold
}
