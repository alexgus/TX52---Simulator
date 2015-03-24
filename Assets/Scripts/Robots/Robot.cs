using UnityEngine;
using System.Collections;

public abstract class Robot : MonoBehaviour, ComController, RobotController {

    /// <summary>
    /// id of the robot
    /// </summary>
    private string id { get; set; }

    /// <summary>
    /// Defined if this robot is selected or not
    /// </summary>
    private bool selected { get; set;  }

    //public Robot(string i)
    //{
    //    this.id = id;
    //}

    public void select()
    {
        this.selected = true;
    }

    public void unselect()
    {
        this.selected = false;
    }

    public void follow(GameObject g)
    {
        
    }

    public void avoid(GameObject g)
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

    public abstract void Move(Direction dir, float? distance = null);
    
    
    public abstract void Rotate(Direction dir, float? angle = null);

    public abstract void Stop(Stop axe);
}
