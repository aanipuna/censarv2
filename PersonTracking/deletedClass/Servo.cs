using System;
using System.Collections;

namespace PersonTracking
{
    class Servo
    {
        private const int BAUDRATE = 57142;
        private string DEVICENAME = "";

        //locals
        private int port_num;
        public ArrayList sevoIds = new ArrayList();

        int dxl_comm_result = ServoConst.COMM_TX_FAIL;
        byte dxl_error = 0;

        public Servo(String port)
        {
            DEVICENAME = port;

            // Initialize PortHandler Structs
            // Set the port path
            // Get methods and members of PortHandlerLinux or PortHandlerWindows
            port_num = dynamixel.portHandler(DEVICENAME);

            // Initialize PacketHandler Structs
            dynamixel.packetHandler();

            // Open port
            if (dynamixel.openPort(port_num))
            {
                Console.WriteLine("Succeeded to open the port!");
            }
            else
            {
                Console.WriteLine("Failed to open the port!");
                return;
            }

            // Set port baudrate
            if (dynamixel.setBaudRate(port_num, BAUDRATE))
            {
                Console.WriteLine("Succeeded to change the baudrate!");
            }
            else
            {
                Console.WriteLine("Failed to change the baudrate!");
                return;
            }

        }

        //TODO serch all servos 
        public void addServoIDs()
        {

        }

        //Enable torque 
        //call this for all servos
        public void enableTorque(byte servoID)
        {
            dynamixel.write1ByteTxRx(port_num, ServoConst.PROTOCOL_VERSION, servoID, ServoConst.ADDR_MX_TORQUE_ENABLE, ServoConst.TORQUE_ENABLE);

            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, ServoConst.PROTOCOL_VERSION)) != ServoConst.COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(ServoConst.PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, ServoConst.PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(ServoConst.PROTOCOL_VERSION, dxl_error);
            }
            else
            {
                Console.WriteLine("Dynamixel has been successfully connected");
            }
        }

        //move servo
        public void move(byte servoID, UInt16 location)
        {
            if (location > ServoConst.DXL_MAXIMUM_POSITION_VALUE || location < ServoConst.DXL_MINIMUM_POSITION_VALUE)
            {
                Console.WriteLine("Invcalid position");
            }
            dynamixel.write2ByteTxRx(port_num, ServoConst.PROTOCOL_VERSION, servoID, ServoConst.ADDR_MX_GOAL_POSITION, location);

            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, ServoConst.PROTOCOL_VERSION)) != ServoConst.COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(ServoConst.PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, ServoConst.PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(ServoConst.PROTOCOL_VERSION, dxl_error);
            }
        }

        //call for each after user
        public void desableTorque(byte servoId)
        {
            // Disable Dynamixel Torque
            dynamixel.write1ByteTxRx(port_num, ServoConst.PROTOCOL_VERSION, servoId, ServoConst.ADDR_MX_TORQUE_ENABLE, ServoConst.TORQUE_DISABLE);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, ServoConst.PROTOCOL_VERSION)) != ServoConst.COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(ServoConst.PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, ServoConst.PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(ServoConst.PROTOCOL_VERSION, dxl_error);
            }
        }

        public void setSpeed(byte servoId, UInt16 speed)
        {
            //int port_num, int protocol_version, byte id, UInt16 address, UInt32 data
            //dynamixel.write4ByteTxRx(port_num, ServoConst.PROTOCOL_VERSION, servoId, ServoConst.ADDR_MX_SPEED, speed);

            dynamixel.write2ByteTxRx(port_num, ServoConst.PROTOCOL_VERSION, servoId, ServoConst.ADDR_MX_SPEED, speed);
        }


        //get current possition
        public int getCurrentPosition(byte servoId)
        {
            UInt16 dxl_present_position = dynamixel.read2ByteTxRx(port_num,ServoConst.PROTOCOL_VERSION, servoId, ServoConst.ADDR_MX_PRESENT_POSITION);
            if ((dxl_comm_result = dynamixel.getLastTxRxResult(port_num, ServoConst.PROTOCOL_VERSION)) != ServoConst.COMM_SUCCESS)
            {
                dynamixel.printTxRxResult(ServoConst.PROTOCOL_VERSION, dxl_comm_result);
            }
            else if ((dxl_error = dynamixel.getLastRxPacketError(port_num, ServoConst.PROTOCOL_VERSION)) != 0)
            {
                dynamixel.printRxPacketError(ServoConst.PROTOCOL_VERSION, dxl_error);
            }

            return dxl_present_position;
        }


        public void relese()
        {
            // Close port
            dynamixel.closePort(port_num);
        }
    }
}
