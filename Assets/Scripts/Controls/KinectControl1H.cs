using UnityEngine;

public class KinectControl1H : MonoBehaviour
{
    [Range(0, 1000)]
    public float tolerance;

    private KinectInteractionManager leftManager;
    private KinectInteractionManager rightManager;
    private Vector2 handOrigin;
    public Robot robot;

    // Use this for initialization
    private void Start() {
        leftManager = new KinectInteractionManager();
        rightManager = new KinectInteractionManager();
        leftManager.init(0, Screen.width, Screen.height, tolerance);
        rightManager.init(1, Screen.width, Screen.height, tolerance);
        leftManager.listenBehaviour(KinectInteractionBehaviour.GripAndFollow);
        rightManager.listenBehaviour(KinectInteractionBehaviour.GripAndFollow);

        handOrigin = new Vector2();
	}

    // Update is called once per frame
    private void Update()
    {
        rightManager.updateListenedBehaviours();
        BehaviourData rightData = rightManager.getBehaviourDataOf(KinectInteractionBehaviour.GripAndFollow);

        GripAndFollowState state = (GripAndFollowState)rightData.state;

        switch (state)
        {
            case GripAndFollowState.OPEN_DETECTED: Debug.Log("OPEN_DETECTED"); robot.Stop(Stop.FULLSTOP);  break;
            case GripAndFollowState.DING: Debug.Log("DING"); handOrigin = rightData.currPosition; break;
            case GripAndFollowState.FOLLOW: 
                Debug.Log("Follow");
                float ver= rightData.currPosition.y - handOrigin.y;
                float hor = rightData.currPosition.x - handOrigin.x;
                Debug.Log("RobotControl1H (" + hor + " , " + ver + (" )"));
                if (ver > tolerance)
                {
                    robot.Move(Direction.BACKWARD);
                }
                else if (ver < -tolerance)
                {
                    robot.Move(Direction.FORWARD);
                }
                else
                {
                    robot.Stop(Stop.MOVE);
                }

                if (hor > tolerance)
                {
                    robot.Rotate(Direction.RIGHT);
                }
                else if (hor < -tolerance)
                {
                    robot.Rotate(Direction.LEFT);
                }
                else
                {
                    robot.Stop(Stop.ROTATE);
                }
                break;
            case GripAndFollowState.NONE: Debug.Log("NONE"); break;
        }
    }
}
