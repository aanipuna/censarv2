using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace PersonTracking
{
    class ServoControl
    {
        private SerialPort serialPort1 = new SerialPort();
        private bool runningServo;
        Servo servo = new Servo();

        public ServoControl(String port)
        {
            if (port != null)
            {
                serialPort1.PortName = port;
                int baurate = 57600;
                serialPort1.BaudRate = baurate;
                serialPort1.DataBits = 8;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Encoding = Encoding.Unicode;

                try
                {
                    serialPort1.Open();
                    Console.WriteLine(port + "Connected to servo.");
                }
                catch (Exception)
                {
                    Console.WriteLine("Connection failed.");
                }

            }
        }

        private void Dispose()
        {
            if (runningServo == true)
            {
                runningServo = false;
                serialPort1.Close();
            }
        }

        public void add(int id)
        {
            if (id >= 0 && id <= 253)
            {
                servo.set_id(id);
            }
            else
            {
                Console.WriteLine("Registration failed because the servo ID is invalid. (Lol)");
            }
        }

        private byte[] getSYNC_WRITEcommand()
        {

            int num = 1;

            byte[] param = new byte[8 + 5 * num];
            int size = 0;
            param[size++] = 0xFF;
            param[size++] = 0xFF;
            param[size++] = (byte)(int)(0xFE);
            param[size++] = (byte)(4 + 5 * num);
            param[size++] = (byte)0x83;
            param[size++] = 0x1E;
            param[size++] = 0x04;

            int id = servo.get_id();
            int position_target = servo.get_target();
            int speed = servo.get_speed();

            param[size++] = (byte)id;

            param[size++] = (byte)(position_target & 0xFF);
            param[size++] = (byte)((position_target >> 8) & 0xFF);

            param[size++] = (byte)(speed & 0xFF);
            param[size++] = (byte)((speed >> 8) & 0xFF);

            param[size++] = calc_checksum_robotis(param);

            return param;
        }

        private byte calc_checksum_robotis(byte[] packet)
        {
            int checksum = 0;

            for (int i = 2; i < packet.Length - 1; i++)
            {
                checksum += (int)packet[i];
            }

            checksum = (~checksum) & 0xFF;

            return (byte)(checksum & 0xFF);
        }

        public void setTargetPosition(int targetPosition)
        {
            servo.set_target(targetPosition);
        }

        public void SetSpeed(int speed)
        {
            servo.set_speed(speed);
        }

        public int PositionToDegree(int position)
        {

            int zero_position = servo.get_zero_position();

            int upper = servo.get_servo_upper_limit();

            double diff = servo.get_degree_range() / (double)upper;

            double position_diff = position - zero_position;

            double degree = diff * position_diff;

            return (int)degree;
        }

        public int DegreeToPosition(String name, double degree, int zeroPosition)
        {
            if (degree >= servo.get_degree_range() / 2)
            {
                degree = servo.get_degree_range() / 2;
            }
            if (degree <= -servo.get_degree_range() / 2)
            {
                degree = -servo.get_degree_range() / 2;
            }

            int target = (int)(((servo.get_degree_range() / 2 + degree) / servo.get_degree_range()) * (double)servo.get_servo_upper_limit());

            target += (zeroPosition - servo.get_servo_upper_limit() / 2);

            return target;
        }

        public void Start()
        {
            if (runningServo == false)
            {
                runningServo = true;
                Task.Run(() =>
                {                                                           //  this method like update method in unity c#
                    action_thred();
                });
            }
        }

        public void Stop()
        {
            this.Dispose();
        }

        private void action_thred()
        {

            while (runningServo)
            {
                byte[] param = this.getSYNC_WRITEcommand();
                this.SerialWrite(param);
            }
        }

        private void SerialWrite(byte[] packet)
        {

            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write(packet, 0, packet.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }

    }

    class Servo
    {
        private int ID;
        private int target = 512;
        private int old_target = 0;
        private int speed = 500;

        private int angle_lower_limit = 0;
        private int angle_upper_limit = 1723;
        private int servo_upper_limit = 1723;
        private int speed_limit = 1023;
        private int degree_range = 300;

        private int presentPosition = 0;
        private int old_presentPosition = 0;

        private int zero_position = 512;

        public Servo()
        {
        }

        public Servo(int angle_limit, int degree_range)
        {
            this.angle_upper_limit = angle_limit;
            this.servo_upper_limit = angle_limit;

            this.degree_range = degree_range;
        }

        public int get_degree_range()
        {
            return this.degree_range;
        }

        public int get_servo_upper_limit()
        {
            return this.servo_upper_limit;
        }

        public void set_id(int ID)
        {
            this.ID = ID;
        }

        public int get_upper_limit()
        {
            return this.angle_upper_limit;
        }

        public int get_lower_limit()
        {
            return this.angle_lower_limit;
        }

        public void set_presentPosition(int position)
        {
            if (position < 0 || position > this.servo_upper_limit) position = this.old_presentPosition;
            this.presentPosition = position;
            this.old_presentPosition = this.presentPosition;

        }

        public int get_presentPosition()
        {
            return this.presentPosition;
        }

        public void set_zero_position(int zero_position)
        {
            this.zero_position = zero_position;

            if (this.zero_position >= this.servo_upper_limit)
            {
                this.zero_position = this.servo_upper_limit;
            }
        }

        public int get_zero_position()
        {
            return this.zero_position;
        }

        public void set_upper_limit(int upper_limit)
        {
            this.angle_upper_limit = upper_limit;
            if (this.angle_upper_limit >= this.servo_upper_limit) this.angle_upper_limit = this.servo_upper_limit;
        }

        public void set_lower_limit(int lower_limit)
        {
            this.angle_lower_limit = lower_limit;
            if (this.angle_lower_limit <= 0) this.angle_lower_limit = 0;
        }

        public void set_target(int target)
        {
            if (target <= this.angle_lower_limit) target = this.angle_lower_limit;
            if (target > this.angle_upper_limit) target = this.angle_upper_limit;

            this.target = target;
            this.old_target = this.target;
        }

        public void set_speed(int speed)
        {
            if (speed < 0) speed = 0;
            if (speed > this.speed_limit) speed = this.speed_limit;

            this.speed = speed;
        }

        public int get_target()
        {
            return this.target;
        }

        public int get_id()
        {
            return this.ID;
        }

        public int get_speed()
        {
            return this.speed;
        }
    }
}
