using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KinectInteractionListener : MonoBehaviour {
    public int handType = 0;
   // public GUIText debug;

    //public GameObject virtual_hand;

  //  public float followSpeed = 5.0f;

    private Plane XZPlane = new Plane(Vector3.up, Vector3.zero);
    private KinectInteractionManager manager;

    private Ray ray;

    

	// Use this for initialization
	void Start () {
        manager = new KinectInteractionManager();
        manager.init(handType, Screen.width, Screen.height);
        manager.listenBehaviour(KinectInteractionBehaviour.GripAndFollow);
	}
	
	// Update is called once per frame
	void Update () {
        manager.updateListenedBehaviours();

        //BehaviourData gripAndFollowData = manager.getBehaviourDataOf(KinectInteractionBehaviour.GripAndFollow);

        
        //debug.text = "State : " + (GripAndFollowState)gripAndFollowData.state;
        //debug.text += "\nPosition :" + gripAndFollowData.currPosition;

        //virtual_hand.transform.localPosition = GetPositionOnXZPlane(gripAndFollowData.screenPosition);

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        //if (gripAndFollowData.state == (int)GripAndFollowState.FOLLOW)
        //{
        //    //float moveH = virtual_hand.transform.position.x - transform.position.x;
        //    //float moveV = virtual_hand.transform.position.z - transform.position.z; 

        //    Vector3 movement = new Vector3(moveH, 0, moveV);
        //    rigidbody.AddForce(movement * followSpeed);
        //}
            

        //debug.transform.position = new Vector3(gripAndFollowData.currPosition.x/2, 1f - gripAndFollowData.currPosition.y , 0) ;
	}

    void OnApplicationQuit()
    {
        manager.Close();
    }

    

    private Vector3 GetPositionOnXZPlane(Vector2 position)
    {
        float distance;
        Ray ray = Camera.allCameras[handType].ScreenPointToRay(position);
        if (XZPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            //Just double check to ensure the y position is exactly zero
            hitPoint.y = 0;
            return hitPoint;
        }
        return Vector3.zero;
    }
}
