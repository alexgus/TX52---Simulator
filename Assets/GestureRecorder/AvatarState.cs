using UnityEngine;
using System.Collections;

public class AvatarState : MonoBehaviour {
   

    public GameObject avatar;
    private KinectAvatar kAvatar;
    public KinectAvatar.MODEAvatar mode;
   

	// Use this for initialization
	void Start () {
        kAvatar = avatar.GetComponent<KinectAvatar>();
        kAvatar.Mode = mode;
    }
}

