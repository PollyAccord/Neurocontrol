//#include <SoftwareSerial.h>
//SoftwareSerial BluetoothSerial(13, 8); //RX, TX
#include <Wire.h>
#include <LiquidCrystal_I2C.h> // Подключение библиотеки
//#include <LiquidCrystal_PCF8574.h>

LiquidCrystal_I2C lcd(0x3F, 16, 2);

//инициализация номеров управляющих контактов
int M1 = 3; //left forward
int M2 = 5; //left backward
int M3 = 6; //right forward
int M4 = 11; //right backward

byte Speed = 100; //скорость 
char val;


//отладка
void setup() {
//BluetoothSerial.begin(115200); //для связи с блютуз 
Serial.begin(115200);
//Serial.println("Start");
//настройка пинов
pinMode(M1, OUTPUT);
pinMode(M2, OUTPUT);
pinMode(M3, OUTPUT);
pinMode(M4, OUTPUT);
//настройка дисплея
lcd.init(); // Инициализация дисплея
lcd.backlight(); // Подключение подсветки
lcd.clear();
lcd.leftToRight();
lcd.setCursor(0,0);
lcd.print("Let's begin");
}


//функции для моторов

void R_MOVE() { 
digitalWrite (M4,LOW);
digitalWrite (M3,HIGH);
}


void L_MOVE() { 
digitalWrite (M1,HIGH);
digitalWrite (M2,LOW);
}


void L_BACK() { 
digitalWrite (M2,HIGH);
digitalWrite (M1,LOW);
}

void R_BACK() { 
digitalWrite (M3,LOW);
digitalWrite (M4,HIGH);
}

void Stop() {  
digitalWrite (M1,LOW);
digitalWrite (M2,LOW);
digitalWrite (M3,LOW);
digitalWrite (M4,LOW);
}



void loop() {
  
 //delay (3000); //задержка на 1 сек
//BluetoothSerial.println("Ничего не происходит");
 if (Serial.available()>0) {
 Serial.println("Что-то начало происходить");
 //получение информации из основного кода
   //val = BluetoothSerial.read()-'0';
   val = Serial.read();
   Serial.println(val);
   //BluetoothSerial.println(val);
   //Stop();
 
   switch(val) {
    case '1': {
      R_MOVE(); L_MOVE(); 
      Serial.println("Поехали");
      lcd.clear();
      lcd.leftToRight();
      lcd.setCursor(0, 0); 
      lcd.print("Moving"); 
      lcd.setCursor(0, 1); 
      lcd.print("forward"); 
      break;  } //движение вперёд
      
    case '2': {
      R_BACK(); L_BACK(); 
      lcd.clear();
      lcd.leftToRight();
      lcd.setCursor(0, 0); 
      lcd.print("Moving"); 
      lcd.setCursor(0, 1); 
      lcd.print("backward"); 
      break; } //движение назад
      
    case '3': {
      L_MOVE(); R_BACK();
      lcd.clear();
      lcd.leftToRight();
      lcd.setCursor(0, 0); 
      lcd.print("Turning"); 
      lcd.setCursor(0, 1); 
      lcd.print("left"); 
      break;} //налево
      
    case '4': {
      R_MOVE(); L_BACK(); 
      lcd.clear();
      lcd.leftToRight();
      lcd.setCursor(0, 0); 
      lcd.print("Turning"); 
      lcd.setCursor(0, 1); 
      lcd.print("right"); 
      break; } //направо
      
    case '5': { 
      Stop(); 
      lcd.clear();
      lcd.leftToRight();
      lcd.setCursor(0, 0); 
      lcd.print("Standing");  
      break; } //остановка
   }
 }
 delay(50);
}
