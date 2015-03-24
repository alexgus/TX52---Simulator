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
        switch (dir)
        {
            case Direction.FORWARD:
                /*JointMotor m = new JointMotor();
                m.force = 1000;
                m.targetVelocity = 100;
                hingeJoint.motor = m;*/

                w1.transform.Rotate(Vector3.down * maxSpeed);
                w2.transform.Rotate(Vector3.down * maxSpeed);
                w3.transform.Rotate(Vector3.up * maxSpeed);
                w4.transform.Rotate(Vector3.up * maxSpeed);
                w1.rigidbody.AddRelativeTorque(Vector3.down * maxSpeed);
                w2.rigidbody.AddRelativeTorque(Vector3.down * maxSpeed);
                w3.rigidbody.AddRelativeTorque(Vector3.up * maxSpeed);
                w4.rigidbody.AddRelativeTorque(Vector3.up * maxSpeed);
                break;
            case Direction.BACKWARD:
                /*JointMotor n = new JointMotor();
                n.force = -1000;
                n.targetVelocity = -100;
                hingeJoint.motor = n;*/

                w1.transform.Rotate(Vector3.up * maxSpeed);
                w2.transform.Rotate(Vector3.up * maxSpeed);
                w3.transform.Rotate(Vector3.down * maxSpeed);
                w4.transform.Rotate(Vector3.down * maxSpeed);
                w1.rigidbody.AddRelativeTorque(Vector3.up * maxSpeed);
                w2.rigidbody.AddRelativeTorque(Vector3.up * maxSpeed);
                w3.rigidbody.AddRelativeTorque(Vector3.down * maxSpeed);
                w4.rigidbody.AddRelativeTorque(Vector3.down * maxSpeed);
                break;
            default: break;
        }
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
                w1.rigidbody.AddRelativeTorque(Vector3.down * turnSpeed);
                w2.rigidbody.AddRelativeTorque(Vector3.down * turnSpeed);
                w3.rigidbody.AddRelativeTorque(Vector3.down * turnSpeed);
                w4.rigidbody.AddRelativeTorque(Vector3.down * turnSpeed);
                break;
            case Direction.RIGHT: 
                w1.transform.Rotate(Vector3.up * turnSpeed);
                w2.transform.Rotate(Vector3.up * turnSpeed);
                w3.transform.Rotate(Vector3.up * turnSpeed);
                w4.transform.Rotate(Vector3.up * turnSpeed);
                w1.rigidbody.AddRelativeTorque(Vector3.up * turnSpeed);
                w2.rigidbody.AddRelativeTorque(Vector3.up * turnSpeed);
                w3.rigidbody.AddRelativeTorque(Vector3.up * turnSpeed);
                w4.rigidbody.AddRelativeTorque(Vector3.up * turnSpeed);
                break;
            default: break;
        }
    }
    public override void Stop(Stop axe)
    {

    }
}
