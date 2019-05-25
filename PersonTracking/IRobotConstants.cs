/*******************************************************************************
**
* RIS CENTER
* 
* 28/03/2017 RIS Center USJP
* 
*******************************************************************************/

namespace PersonTracking
{
    class IRobotConstants
    {
        /// <summary>
        /// Starts the OI.  The Start command must be sent before any other OI Commands.  <i>This command puts Create in <b>passive</b> mode.</i><br></br>
        /// Data Bytes: 0<br></br>
        /// OpCode = 128<br></br>
        /// </summary>
        public const byte Start = 128;

        /// <summary>
        /// Sets the baud rate (in bps) at which OI commands and data are sent.  The default baud rate at power up is 57600 bps<br></br>
        /// Once the baud rate is changed, it will persist until Create is power cycled by removing the battery or the battery is dead.<br></br>
        /// You must wait 100ms after sending this command before sending additional commands at the new baud rate.<br></br>
        /// <i>Create may be in <b>passive</b>, <b>safe</b>, or <b>full</b> mode to accept this command</i><br></br>
        /// <b>If streaming data, don't request more packets than can be sent in 15ms at the requested baud rate!</b>
        /// Data Bytes: 1  (0 - 11)<br></br>
        /// Byte vs. Baud Rate
        /// 0=300
        /// 1=600
        /// 2=1200
        /// 3=2400
        /// 4=4800
        /// 5=9600
        /// 6=14400
        /// 7=19200
        /// 8=28800
        /// 9=38400
        /// 10=56700
        /// 11=115200
        /// OpCode = 129<br></br>
        /// </summary>
        public const byte Baud = 129;

        /// <summary>
        /// Enables a restricted mode of operation for Create. Create will stop if it sees a cliff, a wheel drops, or is plugged into the charger.
        ///  <i>This command puts Create into <b>safe</b> mode</i><br></br>
        /// Create must be in <b>full</b> mode to accept this command<br></br>
        /// Data Bytes: 0<br></br>
        /// OpCode = 131<br></br>
        /// </summary>
        public const byte Safe_Mode = 131;

        /// <summary>
        /// Enables unrestricted control of Create, it *will* drive over cliffs or drive while the charging cable is plugged in.
        ///  <i>This command puts Create into <b>full</b> mode</i><br></br>
        /// Create must be in <b>safe</b> mode to accept this command<br></br>
        /// Data Bytes: 0<br></br>
        /// OpCode = 132<br></br>
        /// </summary>
        public const byte Full_Mode = 132;

        /// <summary>
        /// Starts the Cover, Spot Cover or Cover & Dock demo, without playing the "demo intro" song.
        /// </summary>
        public const byte Cover = 135;
        public const byte Spot = 134;
        public const byte CoverAndDock = 143;


        /// <summary>
        /// Starts or stops a demo performed by the Create
        /// DemoCover: Create attempts to cover an entire room using random bouncing, spiraling, and wall following
        /// DemoGoHome: Identical to Cover, until the Create sees a home dock. Then it attempts to dock with the charger and go passive. (CreateOI name is 'Cover and Dock')
        /// DemoSpiral: Create covers an area by spiraling out from it's starting position, then spiraling back inwards (CreateOI name is 'Spot Cover')
        /// DemoMouse: Create finds the wall, then follows traditional Right Wall Theory
        /// DemoFigure8: Create just drives in a figure eight
        /// DemoWimp: Create drives forward if you push it, but will recoil backwards if it bumps into a wall
        /// DemoHome: Poorly named demo. If the Create sees a virtual wall, it drives towards it. If it doesn't, it turns in place. Tape off all but the front view
        /// on the IR receiver for this (and DemoTag) to work properly!
        /// DemoTag: Identical to DemoHome, except it will continue to drive to new virtual walls after contacting the first one.
        /// DemoPachelbel: Create plays Pachelbel's Canon, in sequence, in sync with the rhythm activating it's cliff sensors
        /// DemoBanjo: Plays different notes of a chord for each cliff sensor. The chord changes depending on the bumper status: no bump, one bump, both sides bumped.
        /// DemoStop: Stops the current demo
        /// </summary>
        public const byte Demo = 136;
        public const byte DemoCover = 0;
        public const byte DemoGoHome = 1;
        public const byte DemoSpiral = 2;
        public const byte DemoMouse = 3;
        public const byte DemoFigure8 = 4;
        public const byte DemoWimp = 5;
        public const byte DemoHome = 6;
        public const byte DemoTag = 7;
        public const byte DemoPachelbel = 8;
        public const byte DemoBanjoy = 9;
        public const byte DemoStop = 255;

        /// <summary>
        /// Controls Create's drive wheels. This command takes 4 data bytes.<br></br>
        /// These data bytes are interpreted as two 16 bit values using twos-compliment.<br></br>
        /// The first two bytes specify the average velocity of the drive wheels in millimeters per second (mm/s)<br></br>
        /// The next 2 bytes specify the radius, in millimeters, in which Create should turn.  The longer radii make Create drive straighter; shorter radii make Create turn more.<br></br>
        /// A drive command with a positive velocity and a positive radius will make Create drive forward while turning toward the left.<br></br>
        /// A negative radius will make Create turn toward the right.<br></br>
        /// Special cases for the radius make Create turn in place or drive straight.<br></br>
        /// <br></br>
        /// Note: The robot system and its environment impose restrictions that may prevent the robot from accurately carrying out some drive commands. For example, it may not be possible to drive in a large arc with a large radius of curvature.<br></br>
        /// <br></br>
        /// Data Bytes: 4<br></br>
        /// Data Bytes 1 and 2: Velocity (-500 - 500 mm/s)<br></br>
        /// Data Bytes 3 and 4: Radius (-2000 - 2000 mm)<br></br>
        /// Special cases for Radius: 32768 = straight, -1 = turn in place clockwise, 1 = Turn in place counter-clockwise<br></br>
        /// <br></br>
        /// C# usage example:<br></br>
        /// To drive in reverse at a velocity of -200mm/s while turning at a radius of 500mm:<br></br>
        /// myCreateObj.Drive(-200, 500);<br></br>
        /// <br></br>
        ///  <i>This command does not change Create's mode</i><br></br>
        /// Create must be in <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// OpCode = 137<br></br>
        /// </summary>
        public const byte Drive = 137;

        /// <summary>
        /// This command lets you control the forward and backward motion  of  Create’s  drive  wheels  independently.  It  takes 
        /// four data bytes, which are interpreted as two 16-bit signed values using two’s complement. The first two bytes specify 
        /// the  velocity  of  the  right wheel  in millimeters  per  second (mm/s), with  the high byte sent frst. The next  two bytes 
        /// specify the velocity of the  left wheel,  in the same  format. A positive velocity makes that wheel drive forward, while a 
        /// negative velocity makes it drive backward.
        /// </summary>
        public const byte DriveDirect = 145;

        /// <summary>
        /// Controls Create's LEDs.
        ///  <i>This command does not change Create's mode</i><br></br>
        /// Create must be in <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// Data Bytes: 3
        /// The advance and play light are controlled on/off with the 8s and 2s place of the first byte
        /// Second data byte specifies the color of the Power LED, 0=pure green, 255=pure red
        /// Third data byte specifies the intensity of the Power LED, 255=full intensity
        /// OpCode = 139<br></br>
        /// </summary>
        public const byte LEDs = 139;

        /// <summary>
        /// Controsl Create's digital outputs. The outputs can handle up to 20mA each.
        /// Data bytes: 1
        /// Create must be in <b>safe</b> or <b>full</b> mode.
        /// The three lowest bits of the data byte control digital outputs 2, 1, and 0
        /// </summary>
        public const byte DigitalOutputs = 147;

        /// <summary>
        /// This allows the low side drivers to be controlled with variable voltage via pulse width modulation.
        /// Data Bytes: 3
        /// Data bytes may be from 0-128, with 128 = fully on.
        /// Data byte 1 is for low side driver 2, byte 2 for LSD 1, and the final data byte for LSD 0.
        /// </summary>
        public const byte PWMLowSideDrivers = 144;

        /// <summary>
        /// Toggles Create's low side drivers, on a Roomba this controls the vacuum motors.
        /// Data Bytes: 1
        /// The three lowest bits of the data byte control drivers 2, 1, and 0. 1 = fully on, 0 = off e.g. 138 3 turns on LSD 0 and 1, but not LSD 2
        ///  <i>This command does not change Create's mode</i><br></br>
        /// Create must be in <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// OpCode = 138<br></br>
        /// </summary>
        public const byte LowSideDrivers = 138;

        /// <summary>
        /// This sends the requested IR byte out via low side driver 1, in the format a Create/Roomba would expect. See the manual before
        /// using this, besides an IR LED and a series resistor, you'll also need a pre-load resistor in parallel with both of them.
        /// Data Bytes: 1, the byte to send (0-255)
        /// </summary>
        public const byte SendIR = 151;


        /// <summary>
        /// Specifies a song to be played later. Songs can be up to 16 notes long.
        /// Data bytes: 3-34
        /// The first data byte specifies the song slot to save the song in (0-15). The second byte specifies the song length (1-16).
        /// All following bytes specify first the note (31-127) or a rest, then the duration in 64ths of a second.  The Notes class defines the value for most notes
        ///  <i>This command does not change Create's mode</i><br></br>
        /// Create must be in <b>passive</b>, <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// OpCode = 140<br></br>
        /// </summary>
        public const byte Song = 140;

        /// <summary>
        /// Plays one of 16 Songs.
        /// If th requested song has not been specified yet (with OpCode 140 - OpCode.Song), then the Play command does nothing<br></br>
        ///  <i>This command does not change Create's mode</i><br></br>
        /// Create must be in <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// OpCode = 141<br></br>
        /// </summary>
        public const byte Play = 141;

        /// <summary>
        /// Requests a packet of Sensor Data Bytes. The user can select 0-6 for groups of packets, or 7-42 for individual Sensor packets. 6 = all sensors
        ///  <i>This command does not change Create's mode</i><br></br>
        /// Create must be in <b>passive</b>, <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// Data bytes: 1
        /// OpCode = 142<br></br>
        /// </summary>
        public const byte Sensors = 142;

        /// <summary>
        /// Requests a list of packets from the Create.  This is identical to the Sensors (142) OpCode, except it allows you to request multiple packets/groups of packets at once.
        /// Data Bytes: 2-255 (though you'd never need more than 43)
        /// The first byte tells the Create how many packets you're requesting, the following bytes are the packet IDs.
        /// </summary>
        public const byte QueryList = 149;

        /// <summary>
        /// This command starts a continuous stream of data packets.  The list of packets requested is sent every 15 ms, which is 
        /// the rate iRobot Create uses to update data. This  is  the best method of  requesting sensor data if  you 
        /// are controlling Create over a wireless network (which has poor real-time characteristics) with software running on a desktop computer)
        /// </summary>
        public const byte Stream = 148;

        /// <summary>
        /// This command lets you stop and restar t the steam without clearing the list of requested packets.
        /// </summary>
        public const byte Pause_Resume_Stream = 150;




        //////////////////////////////////////////////////////////////////////////////////////////////
        //This is where I need to restart including the entire OI spec
        //////////////////////////////////////////////////////////////////////////////////////////////




        /// <summary>
        /// Puts Create to sleep, the same as the normal "power" button press.  To wake,  use the <b>wake</b> function, or set Create.IO.RtsEnable Off, then on for >500 MS
        ///  <i>This command puts Create into <b>passive</b> mode</i><br></br>
        /// Create must be in <b>safe</b> or <b>full</b> mode to accept this command<br></br>
        /// Data Bytes: 0<br></br>
        /// OpCode = 133<br></br>
        /// </summary>
        public const byte Power = 133;


        /// <summary>
        /// Undocumented Command.
        /// Hard reset.  Akin to removing the battery & reinserting it.
        /// </summary>
        public const byte Reset = 7;
        /// <summary>
        /// Tells the create to wait a specified amount of time. This takes an input of one unsigned byte, which is the time (in 0.1s increments) to wait.  Note that this isn't 
        /// especially accurate, as the Create only updates it's sensors every 15ms.
        /// </summary>
        public const byte waitTime = 155;
        /// <summary>
        /// This command causes iRobot Create to wait until it has traveled the specified distance in mm. When Create travels forward, the distance is incremented. When Create travels
        ///backward, the distance is decremented. If the wheels are passively rotated in either direction, the distance is incremented. Until Create travels the specified distance,
        ///its state does not change, nor does it react to any inputs, serial or otherwise.
        ///Distance is specified as a 16-bit signed integer, high byte first.
        /// </summary>
        public const byte waitDistance = 156;
        /// <summary>
        /// This command causes Create to wait until it has rotated through specified angle in degrees. When Create turns counterclockwise, the angle is incremented. When Create
        ///turns clockwise, the angle is decremented. Until Create turns through the specified angle, its state does not change, nor does it react to any inputs, serial or otherwise.
        ///Angle is specified as a 16-bit signed integer, high byte first.
        ///In my experince, the Create tends to turn 1 degree too far while spinning in place at 100 mm/s
        /// </summary>
        public const byte waitAngle = 157;

        /// <summary>
        /// Tells the Create to wait for a specified event.  Send the 2's complement of an event to tell the create to wait until the event is NOT occurring, e.g. wait !digitalInput0
        /// will be a wait time of 0ms unless that digial input is high, in which case it will wait for that input to clear.
        /// </summary>
        public const byte waitEvent = 158;
        public const byte eventPlayButton = 17;

    }
}
