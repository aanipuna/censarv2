/**
 *  28/03/2017 RIS Center USJP
 * 
 * This class is use to connect and send commnads to IRobot
 * */

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PersonTracking
{
    class IRobot
    {
        /**
         * IO - Handdle all serial port communications
         * portNumber - store port number
         * baudRate - baud rate for port communications 
         * */
        private SerialPort IO;
        private String portNumber;
        private int baudRate; //115200
        private static IRobot irobot = null;

        //keep comand that need to buffer to the irobot
        IEnumerable<byte> sendbuff;
        //runnig states
        Boolean runnning = false;
        Boolean init_flag = false;

        private IRobot(String portNumber, int baudRate, int mode)
        {
            this.portNumber = portNumber;
            this.baudRate = baudRate;
            this.sendbuff = this.createDriveCommand(0,0); // default runner
            this.SetPort();

            //if mode is invalid set to safe mode
            if(mode != 1 || mode != 2)
            {
                mode = 2;
            }
            this.selectMode(mode);
            this.init_flag = true;
            this.action();
        }

        /**
         * Initialize IRobot and return IRobot object to use
         * 
         * @param portNumber port number of IRobot
         * @param baudRate Baud rate for communicate 
         * @param mode operation mode (1 - full 2 - safe)
         * 
         * @return IRobot object
         * **/
        public static IRobot init(String portNumber, int baudRate, int mode)
        {
            if(irobot != null)
            {
                return irobot;
            }
            else
            {
                irobot = new IRobot(portNumber,baudRate,mode);
                return irobot;
            }
        }

        /**
         * Openport and start IRobot to send command 
         * **/
        private bool SetPort()
        {
            try
            {
                if (IO != null)
                {
                    IO.Close();//Just in case port is already taken
                }
                IO = new SerialPort(portNumber, baudRate, Parity.None, 8, StopBits.One);
                IO.DtrEnable = false;
                IO.Handshake = Handshake.None;
                IO.RtsEnable = false;
                IO.DataReceived += new SerialDataReceivedEventHandler(SensorDataReceived);

                IO.Open();
                this.debug("open");
                return SendCommand(new byte[] { IRobotConstants.Start });
            }
            catch
            {
                IO.Close();
                return false;
            }
        }

        /**
         * select mode to operate Irobot
         * 
         * @param mode operation mode (1 - full 2 - safe)
         * 
         * @return success or not
         * **/
         private bool selectMode(int mode)
        {
            bool success = false;

            //full mode
            if (mode == 1)
            {
                success = this.SendCommand(new byte[] { IRobotConstants.Full_Mode });
            }
            //safe mode
            else if (mode == 2)
            {
                success = this.SendCommand(new byte[] { IRobotConstants.Safe_Mode });
            }
            return success;
        }

        /**
         * Send commands to serial port 
         * @param commandCollection command list to send
         * 
         * @return command sent success or not 
         * **/
        private bool SendCommand(IEnumerable<byte> commandCollection)
        {
            try
            {
                byte[] commandArr = commandCollection.ToArray();
                IO.Write(commandArr, 0, commandArr.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /**
         * Handle sensor data
         * **/
        private void SensorDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //this.debug("in data recevier");
            if (IO.IsOpen == false)
            {
                return;
            }
            try
            {
                var numOfBytes = IO.BytesToRead;
                byte[] sensorsData = new byte[numOfBytes];
                IO.Read(sensorsData, 0, numOfBytes);
                //this.debug(Encoding.Default.GetString(sensorsData));
            }
            catch (Exception ex)
            {
                this.debug("fail to get sensor data");
            } 
        }

        /** control methods **/

        /**action thread**/
        private void action()
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
                    this.debug("init_error");
                }

            }

        }

        private void action_thred()
        {

            while (runnning)
            {
                Thread.Sleep(5);
                SendCommand(sendbuff);

                //TODO sensor handle
                //if (flagReceive == true)
                //{
                //    flagReceive = false;

                //    this.SendSensorCommand();
                //}


            }

        }

        /**
         * Driver directly
         * 
         * @param leftSpeed speed of the left wheel
         * @param rightSpeed speed of the right wheel
         * 
         * **/
        public void driveDirect(int leftSpeed, int rightSpeed)
        {
            this.sendbuff = createDriveCommand(leftSpeed,rightSpeed);
            this.debug("Driving direct left = " + leftSpeed + " right = " + rightSpeed);
        }
        /**
         * Stop drive
         * 
         * **/
        public void stopDrive()
        {
            this.sendbuff = createDriveCommand(0, 0);
            this.debug("Drive stop");
        }

        /**
         * Create drive command
         * **/
        private IEnumerable<byte> createDriveCommand(int left, int right)
        {
            List<byte> sendbuff = new List<byte>();

            sendbuff.Add(IRobotConstants.DriveDirect);
            sendbuff.AddRange(DecimalToHighLowBytes(right));
            sendbuff.AddRange(DecimalToHighLowBytes(left));
            return sendbuff;
        }
        /**
         * Play tone 
         * **/
        public void playDetectTone()
        {
            byte[] notes = new byte[3] { 90,102,103 };
            byte[] lengths = new byte[3] { 32, 32, 33 };
            this.sendbuff = createToneCommand(notes,lengths);
            this.debug("playing detect tone");
        }

        /**
         * Create tone command
         * **/
        private IEnumerable<byte> createToneCommand(byte[] toneNumbers, byte[] toneLengths)
        {
            List<byte> sendbuff = new List<byte>();

            sendbuff.Add(IRobotConstants.Song);
            sendbuff.Add(0);

            int numberOfNotes = toneNumbers.Length;

            sendbuff.Add((byte)numberOfNotes);

            for(int i = 0; i < numberOfNotes; i++)
            {
                sendbuff.Add(toneNumbers[i]);
                sendbuff.Add(toneLengths[i]);
            }

            sendbuff.Add(IRobotConstants.Play);
            sendbuff.Add(0);

            return sendbuff;
        }

        /**
         * Convert decimal to High and low bytes
         * 
         * @param decimalNum decimal number need to convert to byte
         * 
         * @return high and low byte set of given decimal
         * **/
        private IEnumerable<byte> DecimalToHighLowBytes(int decimalNum)
        {
            byte highByte = (byte)(decimalNum >> 8);
            byte lowByte = (byte)(decimalNum & 255);
            var commands = new List<byte>() { highByte, lowByte };
            return commands;
        }

        //debug writer
        private void debug(String message)
        {
            Console.WriteLine(message);
        }

        //close port 
        public bool close()
        {
            this.runnning = false;
            Thread.Sleep(20);
            if (IO.IsOpen == false)
            {
                return false;
            }
            else
            {
                try
                {
                    if (SendCommand(new byte[] { IRobotConstants.Safe_Mode, 7 }))
                    {
                        IO.Close();
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

    }
}
