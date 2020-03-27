#include <SoftwareSerial.h>
SoftwareSerial BluetoothSerial(7, 8); //RX, TX


//инициализация номеров управляющих контактов
int M1 = 3; int M2 = 4; int M3 = 6; int M4 = 11;

byte Speed = 100; //скорость 


//отладка
void setup() {
BluetoothSerial.begin(9600); //для связи с блютуз 
//Serial.begin(9600);
//настройка пинов
pinMode(M1, OUTPUT);
pinMode(M2, OUTPUT);
pinMode(M3, OUTPUT);
pinMode(M4, OUTPUT);
}


//функции для моторов

void R_MOVE(int Speed) { 
digitalWrite (M4,LOW);
analogWrite (M3,Speed);
}


void L_MOVE(int Speed) { 
digitalWrite (M1,LOW);
analogWrite (M2,Speed);
}


void L_BACK(int Speed) { 
digitalWrite (M2,LOW);
analogWrite (M1,Speed);
}

void R_BACK(int Speed) { 
digitalWrite (M3,LOW);
analogWrite (M4,Speed);
}

void Stop() {  
digitalWrite (M1,LOW);
digitalWrite (M2,LOW);
digitalWrite (M3,LOW);
digitalWrite (M4,LOW);
}



void loop() {
  int val;
 delay (3000); //задержка на 3 сек
// Serial.println("Ничего не происходит");
 if (BluetoothSerial.available()>0) {
 // Serial.println("Что-то начало происходить");
 //получение информации из основного кода
   //val = BluetoothSerial.read()-'0';
   val = BluetoothSerial.read();
   Serial.println(val);
   Stop();
   switch(val) {
    case '1': {R_MOVE(Speed); L_MOVE(Speed); break; Serial.println("Поехали"); } //движение вперёд
    case '2': {R_BACK(Speed); L_BACK(Speed); break; } //движение назад
    case '3': {L_MOVE(Speed); R_BACK(Speed);break;} //налево
    case '4': {R_MOVE(Speed); L_BACK(Speed); break; } //направо
    case '5': { Stop(); break; } //остановка
   }
 }
}
