using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace PersonTracking
{
    public static class IRobotCreate
    {
        public delegate void SensorDataHandler(SensorData sensorData);
        public static event SensorDataHandler sensorDataHandler;

        private static SerialPort myPort;
        private static Boolean runnning = false;
        private static Boolean init_flag = false;
        private static bool fullmode()
        {
            if (myPort.IsOpen == true)
            {
                if (passive())
                {
                    return SendCommand(new byte[] { 132 });
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool passive()
        {
            try
            {
                return SendCommand(new byte[] { 128 });
            }
            catch { return false; }
        }

        private static bool SendCommand(IEnumerable<byte> commandCollection)
        {
            try
            {
                byte[] commandArr = commandCollection.ToArray();
                myPort.Write(commandArr, 0, commandArr.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private struct I_RobotPara
        {
            public int leftSpeed;
            public int rightSpeed;
            public I_RobotPara(int leftSpeed, int rightSpeed)
            {
                this.leftSpeed = leftSpeed;
                this.rightSpeed = rightSpeed;
            }
        }
        public struct SensorData
        {
            public WheelDrop wheelDrop;
            public Bump bump;
            public LightBump lightBump;
        }
        private static SensorData sensorData;
        public struct WheelDrop
        {
            public Boolean left;
            public Boolean righ;
        }
        public struct Bump
        {
            public Boolean left;
            public Boolean right;
        }
        public struct LightBump
        {
            public Boolean right;
            public Boolean frongRight;
            public Boolean centerRight;
            public Boolean centerLeft;
            public Boolean frontLeft;
            public Boolean left;

        }
        private static I_RobotPara robotPara;

        private enum SensorMode
        {
            BumpsAndWheelDrop,
            LightBumper

        }
        private static SensorMode sensorMode;
        public static void init(int bourate, String port)
        {

            robotPara = new I_RobotPara(0, 0);
            sensorMode = new SensorMode();
            sensorMode = SensorMode.BumpsAndWheelDrop;
            sensorData = new SensorData();


            myPort = new SerialPort();
            myPort.PortName = port;
            myPort.BaudRate = bourate;

            myPort.Parity = System.IO.Ports.Parity.None;
            myPort.DataBits = 8;
            myPort.StopBits = System.IO.Ports.StopBits.One;
            myPort.DtrEnable = false;
            myPort.Handshake = Handshake.None;
            myPort.RtsEnable = false;
            myPort.DataReceived += new SerialDataReceivedEventHandler(SensorDataReceived);

            // Timeout Ask the person who is familiar with later about the number of seconds
            myPort.ReadTimeout = 10;
            myPort.WriteTimeout = 10;

            try
            {
                myPort.Open();
                fullmode();
                init_flag = true;
                action();
            }
            catch
            {
                debug("error");
            }
        }


        public static void SendSensorCommand()
        {
            List<byte> sendbuff = new List<byte>();
            sendbuff.Add(142);
            if (sensorMode == SensorMode.BumpsAndWheelDrop) sendbuff.Add(7);
            if (sensorMode == SensorMode.LightBumper) sendbuff.Add(45);
            SendCommand(sendbuff);

            //this.SendCommand(this.createSensorCommand());
        }

        public static void SetDrive(int left, int right)
        {
            if (left > 500) {
                left = 500;
            }
            else if (left < -500) {
                left = -500;
            }

            if (right > 500) {
                right = 500;
            }
            else if (right < -500) {
                right = -500;
            }

            if (left == 0 && right == 0) {

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                

                while (stopwatch.Elapsed.Milliseconds < 700)
                {
                    robotPara.leftSpeed = (robotPara.leftSpeed / 2);
                    robotPara.rightSpeed = (robotPara.rightSpeed / 2);
                }
                stopwatch.Reset();
            }
            robotPara.leftSpeed = left;
            robotPara.rightSpeed = right;
        }


        private static IEnumerable<byte> createDriveCommand(int left, int right)
        {
            List<byte> sendbuff = new List<byte>();

            sendbuff.Add(145);
            sendbuff.AddRange(DecimalToHighLowBytes(right));
            sendbuff.AddRange(DecimalToHighLowBytes(left));
            return sendbuff;

        }

        private static IEnumerable<byte> DecimalToHighLowBytes(int decimalNum)
        {
            byte highByte = (byte)(decimalNum >> 8);
            byte lowByte = (byte)(decimalNum & 255);
            var commands = new List<byte>() { highByte, lowByte };
            return commands;
        }
        public static bool my_close()
        {
            runnning = false;
            Thread.Sleep(10);
            if (myPort.IsOpen == false)
            {
                return false;
            }
            else
            {
                try
                {
                    if (SendCommand(new byte[] { 131, 7 }))
                    {
                        myPort.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch { return false; }
            }
        }

        private static void action()
        {
            if (runnning == false)
            {

                if (init_flag)
                {
                    runnning = true;
                    Task.Run(() =>
                    {
                        action_thred();
                    });
                }
                else
                {
                    debug("init_error");
                }

            }

        }
        private static Boolean flagReceive = true;
        private static void action_thred()
        {

            while (runnning)
            {
                Thread.Sleep(5);
                SendCommand(createDriveCommand(robotPara.leftSpeed, robotPara.rightSpeed));

                if (flagReceive == true)
                {
                    flagReceive = false;

                    SendSensorCommand();
                }


            }

        }
        private static void debug(String message)
        {
            Console.WriteLine(message);

        }
        private static void SensorDataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            //!If serial port is not open, do not process.
            if (myPort.IsOpen == false)
            {
                return;
            }

            try
            {
                //! Read the received data.
                // string data = myPort.ReadExisting();
                var numOfBytes = myPort.BytesToRead;
                byte[] sensorsData = new byte[numOfBytes];
                myPort.Read(sensorsData, 0, numOfBytes);

                setSensorData(sensorsData);
                /*
                this.debug("Reception");
                foreach (var item in sensorsData)
                {
                    this.debug(item.ToString());
                }*/

                if (sensorDataHandler != null)
                {
                    sensorDataHandler(sensorData);
                }


            }
            catch (Exception ex)
            {
                debug("error");
            }

            flagReceive = true;

            if (sensorMode == SensorMode.BumpsAndWheelDrop) sensorMode = SensorMode.LightBumper;
            else sensorMode = SensorMode.BumpsAndWheelDrop;
        }
        private static void setSensorData(byte[] sensorsData)
        {
            if (sensorMode == SensorMode.BumpsAndWheelDrop)
            {
                int jyu = sensorsData[0];
                //
                for (int i = 0; i < 16; i++)
                {
                    int data = jyu % 2;
                    if (i == 0) sensorData.bump.right = booleanConvert(data);
                    if (i == 1) sensorData.bump.left = booleanConvert(data);
                    if (i == 2) sensorData.wheelDrop.righ = booleanConvert(data);
                    if (i == 3) sensorData.wheelDrop.left = booleanConvert(data);

                    jyu = jyu / 2;
                }
            }
            else
            {
                int jyu = sensorsData[0];
                //
                for (int i = 0; i < 16; i++)
                {
                    int data = jyu % 2;
                    if (i == 0) sensorData.lightBump.left = booleanConvert(data);
                    if (i == 1) sensorData.lightBump.frontLeft = booleanConvert(data);
                    if (i == 2) sensorData.lightBump.centerLeft = booleanConvert(data);
                    if (i == 3) sensorData.lightBump.centerRight = booleanConvert(data);
                    if (i == 4) sensorData.lightBump.frongRight = booleanConvert(data);
                    if (i == 5) sensorData.lightBump.right = booleanConvert(data);

                    jyu = jyu / 2;
                }
            }
        }

        private static Boolean booleanConvert(int num)
        {
            Boolean data = true;
            if (num == 0) data = false;
            return data;
        }

        public static void stopAll()
        {
            IRobotCreate.SetDrive(0, 0);
        }
    }
}
