using UnityEngine;
using System.Collections;

public class KeyboardControl : MonoBehaviour {

    public bool selected;
    private float deadZone = 0.5f;
    private Robot robot;
	// Use this for initialization
	void Start () {
        robot = GetComponent<Robot>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //If this controler isn't selected, don't execute the code
        //The controler could also be managed by another script (ControlerManager ?) and be disabled id not used
        if (!selected)
            return;

        float hor = GetHorizontal();
        float ver = GetVertical();

        if (hor < 0)
        {
            //Rotate left
            robot.Rotate(Direction.LEFT);
            Debug.Log("LEFT");

        }
        else if (hor > 0)
        {
            //Rotate Right
            robot.Rotate(Direction.RIGHT);
            Debug.Log("RIGHT");
        }
        else
        {
            //Don't Rotate (stop if it was rotating), No order was given
            robot.Stop(Stop.ROTATE);
            Debug.Log("STOP ROTATING");
        }

        if (ver < 0)
        {
            //Move left
            robot.Move(Direction.BACKWARD);
            Debug.Log("BACKWARD");

        }
        else if (ver > 0)
        {
            //Move Right
            robot.Move(Direction.FORWARD);
            Debug.Log("FORWARD");
        }
        else
        {
            //Don't Move (stop if it was rotating), No order was given
            robot.Stop(Stop.MOVE);
            Debug.Log("STOP MOVING");
        }

      /*  if (ver == 0 && hor == 0)
        {
            robot.Stop(Stop.MOVE);
            Debug.Log("STOP MOVING");
            robot.Stop(Stop.ROTATE);
            Debug.Log("STOP ROTATING");
        }*/
	}

    protected float GetHorizontal()
    {
        float rawHor = Input.GetAxis("Horizontal");
        if (Mathf.Abs(rawHor) > deadZone)
        {
            return rawHor;
        }
        else
        {
            return 0.0f;
        }

    }
    protected float GetVertical()
    {
        float rawVer= Input.GetAxis("Vertical");
        if (Mathf.Abs(rawVer) > deadZone)
        {
            return rawVer;
        }
        else
        {
            return 0.0f;
        }
    }
}
