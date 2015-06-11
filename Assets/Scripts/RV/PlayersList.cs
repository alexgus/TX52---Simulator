using UnityEngine;
using System.Collections;

public class PlayersList : MonoBehaviour {

	public static CameraObjectServer view = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
		
		#if UNITY_STANDALONE_WIN	
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
//		Debug.LogError (players.Length);
				for (int i = 0; i < players.Length; i++) {
						//	GUI.Label (new Rect (200, 100 + (50 * (i+1)), 100, 50), players[i].ToString());
						
						CameraObjectServer obj = (CameraObjectServer)players [i].GetComponent<CameraObjectServer> ();
			if (obj != null){		
			if (obj.isLocal ())
								GUI.color = Color.yellow;
						else
								GUI.color = Color.white;
						if (GUI.Button (new Rect (200, 100 + (50 * i), 100, 50), obj.getName ())) {
								if (view != null)
										view.followHim = false;
								view = obj;
								view.followHim = true;
						}
				}
	}
		
		#endif
		}
}
