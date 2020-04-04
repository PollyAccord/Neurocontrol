//#include <SoftwareSerial.h>
//SoftwareSerial BluetoothSerial(13, 8); //RX, TX


//инициализация номеров управляющих контактов
int M1 = 3; int M2 = 4; int M3 = 6; int M4 = 11;

byte Speed = 100; //скорость 
int val;


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
}


//функции для моторов

void R_MOVE() { 
digitalWrite (M4,LOW);
digitalWrite (M3,HIGH);
}


void L_MOVE() { 
digitalWrite (M1,LOW);
digitalWrite (M2,HIGH);
}


void L_BACK() { 
digitalWrite (M2,LOW);
digitalWrite (M1,HIGH);
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
   val = Serial.parseInt();
   Serial.println(val);
   //BluetoothSerial.println(val);
   //Stop();
   switch(val) {
    case 1: {R_MOVE(); L_MOVE(); break; Serial.println("Поехали"); } //движение вперёд
    case 2: {R_BACK(); L_BACK(); break; } //движение назад
    case 3: {L_MOVE(); R_BACK();break;} //налево
    case 4: {R_MOVE(); L_BACK(); break; } //направо
    case 5: { Stop(); break; } //остановка
   }
 }
 delay(50);
}
