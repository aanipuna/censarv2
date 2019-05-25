using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonTracking
{
    class ServoConst
    {
        // Control table address
        public const int ADDR_MX_TORQUE_ENABLE = 24;                  // Control table address is different in Dynamixel model
        public const int ADDR_MX_GOAL_POSITION = 30;
        public const int ADDR_MX_SPEED = 32;
        public const int ADDR_MX_PRESENT_POSITION = 36;

        // Protocol version
        public const int PROTOCOL_VERSION = 1;                   // See which protocol version is used in the Dynamixel

        public const int TORQUE_ENABLE = 1;                   // Value for enabling the torque
        public const int TORQUE_DISABLE = 0;                   // Value for disabling the torque
        public const int DXL_MINIMUM_POSITION_VALUE = 100;                 // Dynamixel will rotate between this value
        public const int DXL_MAXIMUM_POSITION_VALUE = 4000;                // and this value (note that the Dynamixel would not move when the position value is out of movable range. Check e-manual about the range of the Dynamixel you use.)
        public const int DXL_MOVING_STATUS_THRESHOLD = 10;                  // Dynamixel moving status threshold

        public const int COMM_SUCCESS = 0;                   // Communication Success result value
        public const int COMM_TX_FAIL = -1001;               // Communication Tx Failed
    }
}
