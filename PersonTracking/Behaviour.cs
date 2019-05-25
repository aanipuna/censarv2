using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using ConsoleApplication3;
using ConsoleApp4;

namespace PersonTracking
{
    class Behaviour
    {
        public const char neck = Servo.Servo_1;
        public const char head = Servo.Servo_2;
        public static Boolean run = true;
        //public  ServoControl servoShoulder;
        //public  ServoControl servoNeck;////comented because add new servo class
        public Servo servos = null;
        public Greating greate = null;
        //private IRobotCreate iRobot;
        private static int rate;
        public delegate void SensorDataHandler(IRobotCreate.SensorData sensorData);
        public event SensorDataHandler sensorDataHandler;
        public Behaviour()
        {

        }
        public void init()
        {
            double speed, inputSpeed;
            

            /* servoShoulder = new ServoControl("COM4");
             servoShoulder.add(1);
             servoNeck = new ServoControl("COM5");
             servoNeck.add(2);
             servoShoulder.Start();
             servoNeck.Start();
             servoShoulder.SetSpeed(20);
             servoNeck.SetSpeed(40);
             servoShoulder.setTargetPosition(1500);
             servoNeck.setTargetPosition(1100);*///comented because add new servo class
                                                 //create object using new class
            servos = new Servo("COM17");
            servos.enableTorque(neck, Servo.Torque_1);
            servos.enableTorque(head, Servo.Torque_1);
            servos.setSpeed(neck, 10);//set speed to 10ms
            servos.setSpeed(head, 10);//set speed to 10ms
            greate = new Greating();//create greating object
            //iRobot = new IRobotCreate();
            IRobotCreate.init(115200, "COM7");
            IRobotCreate.sensorDataHandler += new IRobotCreate.SensorDataHandler(onSensorData);
            //Console.WriteLine("Enter iRobot Speed follwed by .00");
            //if (double.TryParse(Console.ReadLine(), out inputSpeed))
            //{
            //    speed = inputSpeed;
            //    rate = (int)(500 * speed / 100.0);

            //}
            //else
            //{
            //    Console.WriteLine("Wrong number format!. setting speed to default 20");
            //    rate = (int)(500 * 20 / 100.0);
            //}

            //Thread child = new Thread(randomMotion);
            //child.Start();
        }

        //static void Main(string[] args)
        //{
        //    Behaviour behaviour = new Behaviour();
        //    behaviour.init();
        //    Thread child = new Thread(behaviour.randomMotion);
        //    child.Start();
        //}

        public void randomMotion()
        {
            Thread iRobot = new Thread(baseRand);
            iRobot.Start();
            Thread head = new Thread(headRand);
            head.Start();
        }
        public void baseRand()
        {
            while (run)
            {
                greate.setvolume(100);
               
                greate.setVoice(Greating.Zira);
                greate.speak("hello");
                greate.great();

                Random rand = new Random();
                int num = rand.Next(10, 20);
                int time = rand.Next(2, 7);

                Thread.Sleep(2000);
                IRobotCreate.SetDrive(-num, num);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(0, 0);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(num, -num);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(0, 0);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(num, -num);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(0, 0);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(-num, num);
                Thread.Sleep(2000);
                IRobotCreate.SetDrive(0, 0);
                Thread.Sleep(time * 1000);
            }
            IRobotCreate.SetDrive(0, 0);
        }
        public void headRand()
        {
            Random rand = new Random();
            while (run)
            {
                int num = rand.Next(1000, 2000);
                int time = rand.Next(5, 7);

                Console.WriteLine("Sleeping" + num);
                Thread.Sleep(num);
                Console.WriteLine("Going left");
               // servoShoulder.setTargetPosition(1200);
               // servoNeck.setTargetPosition(1400);
               //servos.

                Thread.Sleep(time * 1000);
                Console.WriteLine("Going right");
                //servoShoulder.setTargetPosition(1700);
                //servoNeck.setTargetPosition(1500);
                servos.move(neck, 80);
                servos.move(head, 80);
                Thread.Sleep(time * 1000);
                //servoShoulder.setTargetPosition(1500);
                // servoNeck.setTargetPosition(1100);
                servos.move(neck, 30);
                servos.move(head, 30);
                Thread.Sleep(time * 1000);
            }
        }
        public void SetIRobotDrive(int left, int right)
        {
            IRobotCreate.SetDrive(left, right);
        }
        private void onSensorData(IRobotCreate.SensorData sensorData)
        {
            if (this.sensorDataHandler != null)
            {
                sensorDataHandler(sensorData);
            }
        }
    }
}
