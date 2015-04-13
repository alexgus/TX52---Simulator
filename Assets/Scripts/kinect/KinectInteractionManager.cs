
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum KinectInteractionBehaviour
{
    GripAndFollow = 0
}

public enum GripAndFollowState
{
    NONE = 0,
    OPEN_DETECTED,
    DING,
    FOLLOW
}

public struct BehaviourData
{
    public int state;
    public float startTime;
    public Vector2 currPosition;
    public Vector2 lastPosition;
    public Vector2 screenPosition;
}

public class KinectInteractionManager {

    private KinectInteractionClient client;
    private const float _DEFAULT_TOLERANCE = 100.0f;
    private const int _DEFAULT_SCREEN_WIDTH = 1024;
    private const int _DEFAULT_SCREEN_HEIGHT = 720;

    private float tolerance;
    private Vector2 screen_dimension;

    private Dictionary<KinectInteractionBehaviour, BehaviourData> listenedBehaviours = new Dictionary<KinectInteractionBehaviour, BehaviourData>();

	// Use this for initialization
    public void init(int handType, int screenWidth = _DEFAULT_SCREEN_WIDTH, int screenHeight = _DEFAULT_SCREEN_HEIGHT, float tolerance = _DEFAULT_TOLERANCE)
    {
        try
        {
            // Test the Hand Type given 
            if (handType != 0 && handType != 1)
            {
                throw new Exception ("Client will just accept hand type equals to 0 (left hand) or 1 (right hand)");
            }

            this.tolerance = tolerance;
            this.screen_dimension = new Vector2(screenWidth, screenHeight);

            // Instantiate the client & run it 
            client = new KinectInteractionClient();
            client.Run(handType);

        }
        catch (Exception e)
        {
            Debug.Log("Error : " + e);
        }
	}

    public void listenBehaviour(KinectInteractionBehaviour bi)
    {
        BehaviourData data = new BehaviourData();
        this.listenedBehaviours.Add(bi, data);
    }

	// Should be called once per frame
	public void updateListenedBehaviours () {

        List <KinectInteractionBehaviour> keys = new List <KinectInteractionBehaviour> (listenedBehaviours.Keys);
        foreach (KinectInteractionBehaviour k in keys)
        {
            KinectInteractionBehaviour behaviour_key = k;
            BehaviourData behaviour_data = listenedBehaviours[k];

            // Get the position of hand on the screen
            behaviour_data.currPosition = client.handPosition;
            switch (behaviour_key)
            {
                case KinectInteractionBehaviour.GripAndFollow :
                    this.handleGripAndFollowBehaviour(ref behaviour_data);  
                    break;

                // ...
                // add here your new behaviours
                
                default:
                    continue;
            }

            // As BehaviourData is a struct, the dictionary is composed by value of it and not references.
            listenedBehaviours[k] = behaviour_data;
        }
	}

    public BehaviourData getBehaviourDataOf(KinectInteractionBehaviour id)
    {
        return listenedBehaviours[id];
    }

    private Vector2 positionToGUI(Vector2 p)
    {
        return new Vector2(
            p.x * screen_dimension.x,
            (1f - p.y) * screen_dimension.y
        );
    }

    private bool HandIsMoved(Vector2 p1, Vector2 p2)
    {
        return (
            Mathf.Abs(p1.x - p2.x) > tolerance ||
            Mathf.Abs(p1.y - p2.y) > tolerance
        );
    }

    private bool isDING = false;

    private void handleGripAndFollowBehaviour (ref BehaviourData data) {
        // a switch to handle state's changings
        switch ((GripAndFollowState)data.state)
        {
            case GripAndFollowState.NONE:
                if (client.HandIsOpen())
                {
                    isDING = false;
                    data.state = (int)GripAndFollowState.OPEN_DETECTED;
                    data.startTime = Time.time;
                    data.lastPosition = new Vector2(data.currPosition.x, data.currPosition.y);
                }
                break;

            case GripAndFollowState.OPEN_DETECTED:
                if (client.HandIsClose()/* || this.HandIsMoved(data.lastPosition, data.currPosition)*/)
                {
                    data.state = (int)GripAndFollowState.NONE;
                }
                if (Time.time - data.startTime > 2.0f)
                {
                    data.state = (int)GripAndFollowState.DING;
                    isDING = true;
                }
                if (isDING)
                {
                    if (client.HandIsClose())
                    {
                        data.state = (int)GripAndFollowState.FOLLOW;
                    }
                }
                break;

            case GripAndFollowState.DING:
                if (client.HandIsClose())
                {
                    data.state = (int)GripAndFollowState.FOLLOW;
                }
                break;

            case GripAndFollowState.FOLLOW:
                isDING = false;
                if (client.HandIsOpen())
                {
                    data.state = (int)GripAndFollowState.NONE;
                }
                break;
        }
        data.screenPosition = positionToGUI(data.currPosition);
    }

    public void Close()
    {
        this.client.Close();
    }
}