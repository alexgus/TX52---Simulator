/// <summary>
/// This interface defines the control you have on a robot
/// </summary>

interface RobotController {
   
    /// <summary>
    /// Moves the Robot of the given distance to the given direction
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="distance">If distance is set to null or not given, the Robot will move indefinitely until stopped</param>
   void Move(Direction dir, float? distance = null);

    /// <summary>
    /// Rotates the robot of the given angle to the given direction
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="angle">If angle is set to null or not given, The Robot will rotate indefinetely until stopped</param>
   void Rotate(Direction dir, float? angle = null);

    /// <summary>
    /// Stops the robot from {Moving, Rotating, Both}
    /// </summary>
    /// <param name="axe">The axe(s) </param>
   void Stop(Stop axe);
}
