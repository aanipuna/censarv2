using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Servo
    {
        public const char Servo_1 = '1';
        public const int Torque_1 = 1;
        public const int Torque_2 = 2;
        public const int Torque_3 = 3;
        public const int Torque_4 = 4;
        public const char Servo_2 = '2';
        public const char Led_1 = '1';
        public const char Led_2 = '2';
        private static string status = "";
        int servo_1Speed = 10;
        int servo_2Speed = 10;
        int servo_1Torque = 1;
        int servo_2Torque = 1;
        static SerialPort _serialPort;
        System.Threading.Thread readThread;
        private object brak;

        public Servo(String port)
        {
            readThread = new System.Threading.Thread(Read);
            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = port;
            _serialPort.BaudRate = 250000;
            _serialPort.ReadTimeout = 1000;
            _serialPort.WriteTimeout =1000;
            try
            {
                _serialPort.Open();
            }
            catch (System.IO.IOException ex)
            {
                throw ex; ;
            }
            catch(Exception) {
            }
            readThread.Start();
            readThread.Join();
        }
        public Boolean isopen(){
            if (_serialPort.IsOpen)
                return true;
            return false;
        }
        public void enableTorque(char servoID,int Torque)
        {
            switch (servoID) {
                case Servo_1:
                    servo_1Torque = Torque;
                    break;
                case Servo_2:
                    servo_2Torque = Torque;
                    break;
                default:
                    break;
            }

        }
        public void move(char servoID, int location)
        {
            switch (servoID)
            {
                case Servo_1:
                    Write("1;1;" + servo_1Speed + ";" + location + ";" + servo_1Torque+";");
                    break;
                case Servo_2:
                    Write("1;2;" + servo_2Speed + ";" + location + ";" + servo_2Torque+";");
                    break;
                default:
                    break;
            }
        }
        public void desableTorque(char servoId)
        {
            switch (servoId)
            {
                case Servo_1:
                    servo_1Torque = 0;
                    break;
                case Servo_2:
                    servo_2Torque = 0;
                    break;
                default:
                    break;
            }
        }
        public void setSpeed(char servoId, int speed)
        {
            switch (servoId)
            {
                case Servo_1:
                    servo_1Speed = speed;
                    break;
                case Servo_2:
                    servo_2Speed = speed;
                    break;
                default:
                    break;
            }
        }
            public void relese()
        {
            Write("1;1;10;90;0;");
            Write("1;2;10;90;0;");
            Write("2;1;2;10;");
            Write("2;2;2;10;");
            readThread.Abort();
            _serialPort.Close();
        }
        private static void Write(string message )
        {
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.WriteLine(message);
                Console.WriteLine(message);
            }
            catch (TimeoutException ex) {
                throw ex;
            }
            finally {
               
            }

        }

        private static void Read()
        {


            try
            {
                try
                {
                    if (_serialPort.IsOpen)
                        status = _serialPort.ReadLine();
                    Console.WriteLine("input");
                    Console.WriteLine((string)status);
                }
                catch (System.IO.IOException ex)
                {
                    throw ex;
                }
                finally
                {

                }

            }
            catch (TimeoutException time)
            {
                throw time;
            }
            catch (Exception) { 
                
            }

        }
        public int getAngle(char servoId) {
            Read();
            string[] angle = status.Split(';');
            switch (servoId)
            {
                case '1':
                    return Int32.Parse(angle[0]);

                    
                case '2':
                    return Int32.Parse(angle[1]);
                default:
                    return 0;
            }

            
        }
        public void enableLED(char led) {
            switch (led) {
                case Led_1:
                    Write("2;1;2;255;");
                    break;
                case Led_2:
                    Write("2;2;2;255;");
                    break;
                default:
                    break;
                     
            }
        }
        public void disableLED(char led)
        {
            switch (led)
            {
                case '1':
                    Write("2;1;3;1;");
                    break;
                case '2':
                    Write("2;2;3;1;");
                    break;
                default:
                    break;
            }
            }
        public void blinkLED(char led,int blink)
        {
            switch (led)
            {
                case '1':
                    Write("2;1;1;"+blink+";");
                    break;
                case '2':
                    Write("2;2;1;" + blink + ";");
                    break;
                default:
                    break;
            }
        }
        public void fadeLED(char led,int fade)
        {
            switch (led)
            {
                case '1':
                    Write("2;1;2;" + fade + ";");
                    break;
                case '2':
                    Write("2;2;2;" + fade + ";");
                    break;
                default:
                    break;
            }
        }
       
    }
}
