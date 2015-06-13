using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class behaviourManager : MonoBehaviour, GestureListener {

	List<GameObject> drones;
	public GameObject prefabDrone;


    public int nbSpawn,offsetX, offsetY, offsetZ;

    public void Awake()
    {
        this.drones = new List<GameObject>();
    }
	void Start () {
        for (int i = -nbSpawn + offsetX; i <= nbSpawn + offsetX; i += nbSpawn)
        {
            for (int y = -nbSpawn + offsetY; y <= nbSpawn + offsetY; y += nbSpawn)
            {
				 GameObject drone = (GameObject) Instantiate(prefabDrone, prefabDrone.transform.position + new Vector3(i,0,y), prefabDrone.transform.rotation);
                Debug.Log("Drone is null ? " + drone == null ? "true" : "false");
                drones.Add(drone);
			}
		}
	}

	void switchBehaviour(string behaviour)
	{
		switch(behaviour)
		{
		    case "Clap":
                Debug.Log(drones.Count);
			foreach(GameObject drone in drones)
			{
                drone.GetComponent<Wandering>().enabled = true;
				//wandering.enabled =! wandering.enabled;
			}
            break;

		}

	}

    void GestureListener.OnComplete(DynGesture gesture)
    {
        Debug.Log("On complete : "+gesture.Name);
        switchBehaviour(gesture.Name);
    }

    void GestureListener.OnRecomplete(DynGesture gesture)
    {
       // throw new System.NotImplementedException();
    }

    void GestureListener.OnRelease(DynGesture gesture)
    {
        //throw new System.NotImplementedException();
    }
}
