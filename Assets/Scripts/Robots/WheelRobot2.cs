using UnityEngine;

public class WheelRobot2 : Robot
{

    public Vector3 centerOfMass;
    public float brake;
    public float motor;
    public float steer;
    public WheelCollider wFL, wFR, wBL, wBR;
    public override void Move(Direction dir, float? distance = null)
    {
        switch (dir)
        {
            case Direction.FORWARD:
                
                wBR.brakeTorque = 0.0f;
                wBL.brakeTorque = 0.0f;
                wBL.motorTorque = motor;
                wBR.motorTorque = motor;
                break;

            case Direction.BACKWARD:
                wBR.brakeTorque = 0.0f;
                wBL.brakeTorque = 0.0f;
                 wBL.motorTorque = -1*motor;
                 wBR.motorTorque = -1 * motor;
                break;
            case Direction.LEFT: break;
            case Direction.RIGHT: break;
            default: break;
        }
    }

    public override void Rotate(Direction dir, float? angle = null)
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        this.GetComponentInChildren<Rigidbody>().centerOfMass = centerOfMass;
    }
    public override void Stop(Stop axe)
    {
        switch (axe)
        {
            case global::Stop.MOVE: 
                wBL.motorTorque = 0.0f;
                wBR.motorTorque = 0.0f;
                wBR.brakeTorque = brake;
                wBL.brakeTorque = brake;
                break;

            case global::Stop.ROTATE: break;
            case global::Stop.FULLSTOP: break;
        }
    }
}
