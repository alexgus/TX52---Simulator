using UnityEngine;

public class WheelRobot : Robot
{
    public float maxSpeed= 10.0f; //The maximum speed of the robot
    public float turnSpeed = 100.0f; //The turning speed of the robot

    public GameObject w1,w2,w3,w4;

    public void avoid(GameObject g)
    {
    }

    public void follow(GameObject g)
    {
    }

    public void goTo(GameObject g)
    {
    }

    public void group(string scheme)
    {
    }

    public int sensorSignal()
    {
        return 0;
    }

    
    public override void Move(Direction dir, float? distance = null)
    {
        JointMotor m = new JointMotor();
        JointMotor n = new JointMotor();

        switch (dir)
        {
            case Direction.FORWARD:
                m.force = 1;
                m.targetVelocity = 360;

                n.force = 1;
                n.targetVelocity = -360;

                w1.transform.Rotate(Vector3.up * maxSpeed);
                w2.transform.Rotate(Vector3.up * maxSpeed);
                w3.transform.Rotate(Vector3.down * maxSpeed);
                w4.transform.Rotate(Vector3.down * maxSpeed);

                break;
            case Direction.BACKWARD:
                m.force = 1;
                m.targetVelocity = -360;

                n.force = 1;
                n.targetVelocity = 360;

                w1.transform.Rotate(Vector3.down * maxSpeed);
                w2.transform.Rotate(Vector3.down * maxSpeed);
                w3.transform.Rotate(Vector3.up * maxSpeed);
                w4.transform.Rotate(Vector3.up * maxSpeed);
                break;
            default: break;
        }

        w1.hingeJoint.motor = m;
        w2.hingeJoint.motor = m;
        w3.hingeJoint.motor = n;
        w4.hingeJoint.motor = n;

        w1.hingeJoint.useMotor = true;
        w2.hingeJoint.useMotor = true;
        w3.hingeJoint.useMotor = true;
        w4.hingeJoint.useMotor = true;
    }
    public override void Rotate(Direction dir, float? angle = null)
    {
        
        switch (dir)
        {
            case Direction.LEFT: 
                w1.transform.Rotate(Vector3.down * turnSpeed);
                w2.transform.Rotate(Vector3.down * turnSpeed);
                w3.transform.Rotate(Vector3.down * turnSpeed);
                w4.transform.Rotate(Vector3.down * turnSpeed);
                break;
            case Direction.RIGHT: 
                w1.transform.Rotate(Vector3.up * turnSpeed);
                w2.transform.Rotate(Vector3.up * turnSpeed);
                w3.transform.Rotate(Vector3.up * turnSpeed);
                w4.transform.Rotate(Vector3.up * turnSpeed);
                break;
            default: break;
        }
    }
    public override void Stop(Stop axe)
    {
        w1.hingeJoint.useMotor = false;
        w2.hingeJoint.useMotor = false;
        w3.hingeJoint.useMotor = false;
        w4.hingeJoint.useMotor = false;

    }
}
