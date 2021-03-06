﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Jayrock.Json.Conversion;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;




namespace ConsoleApp1
{
    class Program
    {
            
        // public static StreamWriter EEG = new StreamWriter(@"EEGData_" + ".csv", true);

        static Connector connect;

        static void Main(string[] args)
        {
                 

            TcpClient client;
            Stream stream;
            IDictionary eSense, blinkStrength;

            connect = new Connector();
            
            connect.setBlinkDetectionEnabled(true);


           
            int Blink = 0;
            int count = 0;
            string port;
            string meditation, attention; //algorithm dataset
            byte[] buffer = new byte[2048];
            int bytesRead;

            


            // Building command to enable JSON output from ThinkGear Connector (TGC)

            byte[] forward = Encoding.ASCII.GetBytes("1");
            byte[] backward = Encoding.ASCII.GetBytes("2");
            byte[] left = Encoding.ASCII.GetBytes("3");
            byte[] right = Encoding.ASCII.GetBytes("4");
            byte[] standing = Encoding.ASCII.GetBytes("5");

            SerialPort comPort = new SerialPort(); 
            
            //Starting connection on com port 
           //Console.WriteLine("Enter COM Port (COM6): ");
           // var port = Console.ReadLine();
            try {                             
                //comPort.PortName = port.ToString();
               comPort.PortName = "COM6";         
                comPort.BaudRate = 9600;
             
               // comPort.Parity = Parity.None;
                //comPort.StopBits = StopBits.One;
                comPort.Open();
                
            } 

            catch (Exception e)
            {
               // Console.WriteLine("Error connecting to port : " + port.ToString());
            } 




              Console.WriteLine("Do you want to see the EEG data? (1/0)");
                var button = Console.ReadLine();

           
           var com = @"{""enableRawOutput"": true, ""format"": ""Json""}";
            if (button.ToString() == "1")
                com = @"{""enableRawOutput"": false, ""format"": ""Json""}";

            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(com);

    
            //Create a TcpClient and try to connect
            try
            {
                Console.WriteLine("Connection to Headset");
                client = new TcpClient("127.0.0.1", 13854);
                stream = client.GetStream();
                Thread.Sleep(5000);
                client.Close();
            }
            catch (SocketException se) {
                Console.WriteLine("Error connecting to device.");
            }


            try
            {
                client = new TcpClient("127.0.0.1", 13854);
                stream = client.GetStream();
                Console.WriteLine("Sending configuration packet to device.");
                if (stream.CanWrite)
                    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);

                Thread.Sleep(500);
                client.Close();              
            }
            catch (SocketException se)
            {
                Console.WriteLine("Error sending configuration packet to TGC.");
            }

                        
            //Keep reading packets and parse the packets
            try
            {
                Console.WriteLine("Starting data collection.");
                client = new TcpClient("127.0.0.1", 13854);
                stream = client.GetStream();

                // Sending configuration packet to TGC
                if (stream.CanWrite)
                {
                    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
                }

              
                Console.WriteLine("Enter any key to start.");

                ConsoleKeyInfo key = Console.ReadKey(false);

                
                if (stream.CanRead)
                {
                    Console.WriteLine("Reading bytes");

                    //in its own thread
                    while (true)
                    {
                        bytesRead = stream.Read(buffer, 0, 2048);

                        string[] packets = Encoding.UTF8.GetString(buffer, 0, bytesRead).Split('\r');
                        foreach (string s in packets)


                            //Starting to analyze
                        {

                            try
                            {
                                IDictionary data = (IDictionary)JsonConvert.Import(typeof(IDictionary), s); 

                                //Check if device is ON

                                var ready = false; //the readiness of device
                                                            
                                 //Check if device is on head or not
                                if (data.Contains("eSense"))

                                    if (data["eSense"].ToString() == "{\"attention\":0,\"meditation\":0}") {                                     
                                        Console.WriteLine("Check fitting.");
                                        ready = false;
                                        break;
                                    }

                                if (data.Contains("status"))
                                    {
                                        Console.WriteLine("Device is OFF");
                                        ready = false;
                                        break;
                                    }
                               

                                //Check if device is ready
                                if (data.Contains("eSense") && !ready)
                                {
                                    if (((IDictionary)data["eSense"])["attention"].ToString() != "0" && ((IDictionary)data["eSense"])["meditation"].ToString() != "0")
                                    {
                                        ready = true;
                                        Console.WriteLine("Device is ready.\n Moving backward - press B\n Moving left/right - press F\n Moving and stop - press any key\n To close readings - CTRL+C\n");

                                    }
                                }

                                if (ready)
                                {
                                    if (Console.KeyAvailable == true)
                                    {
                                        //startRead = true;
                                        key = Console.ReadKey(true);

                                        break;
                                    }

                                    //Start reading data


                                    eSense = (IDictionary)data["eSense"];
                                    blinkStrength = (IDictionary)data["blinkStrength"];


                                    if (data.Contains("eSense"))
                                    {

                                        Console.WriteLine("Attention : " + eSense["attention"].ToString() + ", Meditation : " + eSense["meditation"].ToString());

                                        attention = eSense["attention"].ToString();
                                        meditation = eSense["meditation"].ToString();
                                        // blink = blinkStrength["blinkStrength"].ToString();

                                        switch (key.Key) {
                                            case ConsoleKey.F:
                                                Console.WriteLine("Mode:moving left/right\n");
                                                //left
                                                if (Int32.Parse(attention) > 60)
                                                {
                                                    if (button.ToString() == "1")
                                                        Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                         ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                         ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                         "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                         "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                                    // Console.WriteLine("Blink strength \n: " + data["blinkStrength"].ToString());
                                                    Console.WriteLine("Bot is moving left.\n");
                                                    //comPort.Write(left, 0, 1);
                                                    comPort.Write("3");
                                                    //string cmd = "3";
                                                    //var res = App.BluetoothManager.SendMessageAsync(cmd);
                                                    Thread.Sleep(3000);
                                                }

                                                else if ((Int32.Parse(attention) < 60) && (Int32.Parse(meditation) > 40))
                                                {
                                                    //right
                                                    if (button.ToString() == "1")
                                                        Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                        ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                        ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                        "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                        "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                                    //Console.WriteLine("Blink strength \n: " + data["blinkStrength"].ToString());
                                                    Console.WriteLine("Bot is moving right.\n");
                                                   // comPort.Write(right, 0, 1);
                                                    comPort.Write("4");
                                                   
                                                    Thread.Sleep(3000);
                                                }

                                                else
                                                {
                                                    if (button.ToString() == "1")
                                                        Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                        ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                        ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                        "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                        "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                                    Console.WriteLine("Continue reading data.\n");
                                                    comPort.Write("5");
                                                    Thread.Sleep(3000);
                                                }
                                                break;

                                            case ConsoleKey.B:
                                                Console.WriteLine("Mode: moving backward\n");
                                                if (Int32.Parse(attention) > 60)
                                                {
                                                    //backward
                                                    if (button.ToString() == "1")
                                                        Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                        ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                        ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                        "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                        "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                                    //Console.WriteLine("Blink strength: " + data["blinkStrength"].ToString());

                                                    Console.WriteLine("Bot is moving backward.\n");
                                                    // comPort.Write(backward, 0, 1);
                                                    comPort.Write("2");
                                                    Thread.Sleep(3000);
                                                }
                                                else
                                                {
                                                    if (button.ToString() == "1")
                                                        Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                              ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                              ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                              "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                              "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                                    Console.WriteLine("Continue reading data.\n");
                                                    comPort.Write("5");
                                                    Thread.Sleep(3000);
                                                }
                                                break;

                                        default:
                                                Console.WriteLine("Mode: Moving forward or stop\n");

                                                //forward
                                                if (Int32.Parse(attention)>60) { 
                                                if (button.ToString() == "1")
                                                    Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                        ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                        ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                        "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                        "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                                Console.WriteLine("Bot is moving forward.\n");
                                                    //comPort.Write(forward, 0, 1);
                                                    comPort.Write("1");
                                                // Console.WriteLine("Blink strength \n: " + ((IDictionary)data["blinkStrength"]).ToString());       
                                                Thread.Sleep(3000);
                                        }

                                            else if ((Int32.Parse(attention) < 60) && (Int32.Parse(meditation) > 40))
                                        {

                                            //stop
                                            if (button.ToString() == "1")
                                                Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + ",\n" + "Theta:" +
                                                             ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha: " +
                                                             ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                             "High Betta: " + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                             "High Gamma: " + ((IDictionary)data["eegPower"])["highGamma"].ToString());


                                            Console.WriteLine("Bot stopped.\n");
                                                    //comPort.Write(standing, 0, 1);
                                                    comPort.Write("5");
                                            //Console.WriteLine("Blink strength \n: " + data["blinkStrength"].ToString());
                                            Thread.Sleep(3000);
                                        }

                                        else
                                        {
                                            if (button.ToString() == "1")
                                                Console.WriteLine("Delta: " + ((IDictionary)data["eegPower"])["delta"].ToString() + "," + "Theta:" +
                                                      ((IDictionary)data["eegPower"])["theta"].ToString() + ",\n" + "Low Alpha: " + ((IDictionary)data["eegPower"])["lowAlpha"].ToString() + ",\n" + "HighAlpha:" +
                                                      ((IDictionary)data["eegPower"])["highAlpha"].ToString() + ",\n" + "Low Betta: " + ((IDictionary)data["eegPower"])["lowBeta"].ToString() + ",\n" +
                                                      "High Betta:" + ((IDictionary)data["eegPower"])["highBeta"].ToString() + ",\n" + "Low Gamma: " + ((IDictionary)data["eegPower"])["lowGamma"].ToString() + ",\n" +
                                                      "High Gamma:" + ((IDictionary)data["eegPower"])["highGamma"].ToString());

                                            Console.WriteLine("Continue reading data.\n");
                                                    comPort.Write("5");
                                            Thread.Sleep(3000);
                                        }
                                                break;
                                        }

                                        count = 0;
                                    }

                                    Console.WriteLine("Blink strength \n: " + data["blinkStrength"].ToString());

                                    int size = Int32.Parse(data["blinkStrength"].ToString());
                                        if (size > 30)
                                        {
                                           Blink = size;
                                            count += 1;
                                        }
                                        
                                         //Console.WriteLine("Blink strength : " + ((IDictionary)data["blinkStrength"]).ToString());
                                    }
                            }

                            catch (Exception e) {}                          
                        }
                    }
                }

                Thread.Sleep(15000);
                client.Close();

                connect.Close();

            }
            catch (SocketException se) {

                Console.WriteLine("Error in data collection.");
                comPort.Close();

            }

        }
    }
}
