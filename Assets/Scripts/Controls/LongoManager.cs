using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LongoManager : MonoBehaviour, GestureListener {

    public List<Robot> robots;

    private Robot selected;
    private int index = 0;

    private bool isForwarding = false;

    public void Awake()
    {
        
    }
	void Start () {
        if (this.robots.Count > 0)
            this.selected = this.robots[index];
	}

	void switchBehaviour(string behaviour)
	{
		

	}

    void GestureListener.OnComplete(DynGesture gesture)
    {
        Debug.Log("On complete : "+gesture.Name);
        switch (gesture.Name)
        {
            case "Clap":
                selected.Stop(Stop.FULLSTOP);
                Debug.Log("robot count : "+this.robots.Count+" cur index = "+index);
                if(index < this.robots.Count){
                    selected = this.robots[++index];
                }
                else
                {
                    index = 0;
                    selected = this.robots[index];
                }
                Debug.Log("new index= " + index);
                selected.Stop(Stop.FULLSTOP);
                break;
            case "forward":
                if (isForwarding)
                {
                    selected.Stop(Stop.MOVE);
                    isForwarding = false;
                }
                else
                {
                    selected.Move(Direction.FORWARD, null);
                    isForwarding = true;
                }
                break;
            case "lever-bras-droit":
                selected.Rotate(Direction.RIGHT, null);
                break;
            case "lever-bras-gauche":
                selected.Rotate(Direction.LEFT, null);
                break;
        }
    }

    void GestureListener.OnRecomplete(DynGesture gesture)
    {
        Debug.Log("On recomplete : " + gesture.Name);
        switch (gesture.Name)
        {
            case "Clap":
                break;
            case "forward":
                break;
            case "lever-bras-droit":
                break;
            case "lever-bras-gauche":
                break;
        }
    }

    void GestureListener.OnRelease(DynGesture gesture)
    {
        Debug.Log("On release : " + gesture.Name);
        switch (gesture.Name)
        {
            case "Clap":
                break;
            case "forward":
                break;
            case "lever-bras-droit":
                selected.Stop(Stop.ROTATE);
                break;
            case "lever-bras-gauche":
                selected.Stop(Stop.ROTATE);
                break;
        }
    }
}
