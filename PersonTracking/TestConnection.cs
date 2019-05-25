using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;

namespace PersonTracking

{
    class TestConnection
    {
        SerialPort IO;

        public bool SetPort(string portNum)
        {
            try
            {
                if (IO != null)
                {
                    IO.Close();//Just in case port is already taken
                }
                IO = new SerialPort(portNum, 115200, Parity.None, 8, StopBits.One);
                IO.DtrEnable = true;
                IO.Handshake = Handshake.None;
                IO.RtsEnable = false;

                IO.Open();
                Console.WriteLine("open");
                return SendCommand(new byte[] { 128 });
            }
            catch
            {
                IO.Close();
                Console.WriteLine("close");
                return false;
            }

        }

        public bool SendCommand(IEnumerable<byte> commandCollection)
        {
            try
            {
                var commandArr = commandCollection.ToArray();
                IO.Write(commandArr, 0, commandArr.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<byte> DecimalToHighLowBytes(int decimalNum)
        {
            byte highByte = (byte)(decimalNum >> 8);
            byte lowByte = (byte)(decimalNum & 255);
            var commands = new List<byte>() { highByte, lowByte };
            return commands;
        }
        public void close()
        {
            IO.Close();
        }


        public void SensorDataWasReceived()
        {

            var numOfBytes = IO.BytesToRead;
            byte[] sensorsData = new byte[numOfBytes];

            Console.WriteLine(" numOfBytes " + numOfBytes);

            Console.WriteLine("IO.Read" + IO.Read(sensorsData, 0, numOfBytes));
            //Console.WriteLine(IO.ReadByte());
            //set sensors…
            if(numOfBytes > 0)
            {
                Console.WriteLine("Data " + sensorsData[2]);
                Console.WriteLine("Data " + sensorsData[3]);


                int num = this.UnsignedHighLowBytesToDecimal(sensorsData[2], sensorsData[3]);
                Console.WriteLine("number " + num);
            }
            
        }


        private  int SignedHighLowBytesToDecimal(byte highByte, byte lowByte)
        {
            uint u = (uint)highByte << 8 | lowByte;
            int num = (int)(u >= (1u << 15) ? u - (1u << 16) : u);
            return num;
        }
        private  int UnsignedHighLowBytesToDecimal(byte highByte, byte lowByte)
        {
            return 256 * highByte + lowByte;
        }



        
    }

    

}
