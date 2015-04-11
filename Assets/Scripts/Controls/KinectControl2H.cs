using System.Collections.Generic;
using UnityEngine;

public class KinectControl2H : MonoBehaviour
{
    [Range(0, 1000)]
    public float tolerance;

    private KinectInteractionManager leftManager;
    private KinectInteractionManager rightManager;
    private Vector2 handOrigin;
    public List<Robot> robot;

    private bool leftDing =false;
    private IEnumerator<Robot> itRobot;
    private Robot curRobot;

    // Use this for initialization
    private void Start() {
        leftManager = new KinectInteractionManager();
        rightManager = new KinectInteractionManager();
        leftManager.init(0, Screen.width, Screen.height, tolerance);
        rightManager.init(1, Screen.width, Screen.height, tolerance);
        leftManager.listenBehaviour(KinectInteractionBehaviour.GripAndFollow);
        rightManager.listenBehaviour(KinectInteractionBehaviour.GripAndFollow);

        handOrigin = new Vector2();

        itRobot = robot.GetEnumerator();
        itRobot.MoveNext();
        curRobot = itRobot.Current;
	}

    // Update is called once per frame
    private void Update()
    {
        rightManager.updateListenedBehaviours();
        leftManager.updateListenedBehaviours();
        BehaviourData rightData = rightManager.getBehaviourDataOf(KinectInteractionBehaviour.GripAndFollow);
        BehaviourData leftData = leftManager.getBehaviourDataOf(KinectInteractionBehaviour.GripAndFollow);

        GripAndFollowState rState = (GripAndFollowState)rightData.state;
        GripAndFollowState lState = (GripAndFollowState)leftData.state;

        if (curRobot == null)
        {
            Debug.Log("Curr Robot is null");
            return;
        }
        switch (rState)
        {
            case GripAndFollowState.OPEN_DETECTED: Debug.Log("R OPEN_DETECTED"); curRobot.Stop(Stop.FULLSTOP);  break;
            case GripAndFollowState.DING: Debug.Log("R DING"); handOrigin = rightData.currPosition; break;
            case GripAndFollowState.FOLLOW: 
                Debug.Log("R Follow");
                float ver= rightData.currPosition.y - handOrigin.y;
                float hor = rightData.currPosition.x - handOrigin.x;
                Debug.Log("R RobotControl1H (" + hor + " , " + ver + (" )"));
                if (ver > tolerance)
                {
                    curRobot.Move(Direction.BACKWARD);
                }
                else if (ver < -tolerance)
                {
                    curRobot.Move(Direction.FORWARD);
                }
                else
                {
                    curRobot.Stop(Stop.MOVE);
                }

                if (hor > tolerance)
                {
                    curRobot.Rotate(Direction.RIGHT);
                }
                else if (hor < -tolerance)
                {
                    curRobot.Rotate(Direction.LEFT);
                }
                else
                {
                    curRobot.Stop(Stop.ROTATE);
                }
                break;
            case GripAndFollowState.NONE: Debug.Log("R NONE"); break;
        }

        switch (lState)
        {
            case GripAndFollowState.OPEN_DETECTED: Debug.Log("L OPEN_DETECTED"); leftDing = false; break;
            case GripAndFollowState.DING: Debug.Log("L DING");
                if (!leftDing)
                {
                    nextRobot();
                }
                leftDing = true; break;
            case GripAndFollowState.FOLLOW: Debug.Log("L Follow"); leftDing = false;break ;
            case GripAndFollowState.NONE: Debug.Log("L NONE"); leftDing = false; break;
        }
    }

    
    private Robot nextRobot(){
        Debug.Log("Next Robot");
        if(itRobot.MoveNext()){
            curRobot = itRobot.Current;
        }
        else{
            itRobot.Reset();
            curRobot = itRobot.Current;
        }
        return curRobot;
    }
}
