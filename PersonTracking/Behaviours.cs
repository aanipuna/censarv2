using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PersonTracking
{
    class Behaviours
    {
        //servo handle
        private static Servo servoHandle;
        public const byte neck = 2;
        public const byte shoulder = 3;
        private static  Boolean run = true;
        public static Boolean detect = false;

        //irobot handle
        private static IRobot irobotHandle;


        public static void init(String headCom, String irCom)
        {
            //create new servo handle and init nessary values
            servoHandle = new Servo(headCom);
            servoHandle.setSpeed(neck,40);
            servoHandle.setSpeed(shoulder, 20);
            servoHandle.enableTorque(neck);
            servoHandle.enableTorque(shoulder);

            //create ironto handle with full mode
            irobotHandle = IRobot.init(irCom, 115200, 1);

        }

        public static void randomMotions()
        {
            Thread head = new Thread(headRand);
            head.Start();
            Thread irBase = new Thread(baseRand);
            irBase.Start();
        }

        private static void headRand()
        {
            Random rand = new Random();
            while (run)
            {
                int neckpos = rand.Next(1100, 1500);
                int shopos = rand.Next(1200, 1700);
                //int time = rand.Next(3, 4);

                servoHandle.move(shoulder, Convert.ToUInt16(shopos));
                servoHandle.move(neck, Convert.ToUInt16(neckpos));

                Thread.Sleep(2000);

                while (detect)
                {
                    Thread.Sleep(500);
                }
            }
        }

        private static void baseRand()
        {
            Random rand = new Random();
            while (run)
            {
                int num = rand.Next(20, 30);

                int direc = rand.Next(1, 4);

                Console.WriteLine("-----------" + direc);

                if (direc == 1)
                {
                    irobotHandle.driveDirect(num, num);
                }
                else if (direc == 2 )
                {
                    irobotHandle.driveDirect(-num, -num);
                }
                else if (direc == 3)
                {
                    irobotHandle.driveDirect(num, -num);
                }
                else
                {
                    irobotHandle.driveDirect(-num, num);
                }
                Thread.Sleep(2000);

                while (detect)
                {
                    Thread.Sleep(500);
                }

            }
            irobotHandle.driveDirect(0, 0);
        }

        public static void moveHead(byte sid, UInt16 location)
        {
            servoHandle.move(sid, location);
        }

        public static void moveBase(int left, int right)
        {
            irobotHandle.driveDirect(left,right);
        }

        public static void closeAll()
        {
            if (detect)
            {
                Thread.Sleep(500);
            }
            if (run)
            {
                run = false;
                Thread.Sleep(2500);
            }

            servoHandle.desableTorque(neck);
            servoHandle.desableTorque(shoulder);
            servoHandle.relese();
            irobotHandle.driveDirect(0,0);
            irobotHandle.close();

        }
    }
}
