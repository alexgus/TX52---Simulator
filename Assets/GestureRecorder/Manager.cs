using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {
	[System.Serializable]
	public class ActionCouple {
		public string name;
		public GameObject go;
		public GestureListener listener;
	}

	// the instance
	public static Manager instance;

	// the recorded gestures
	public List<DynGesture> gestures = new List<DynGesture>();

	// the actions (gesture listener) available
	public ActionCouple[] Actions;

	void Awake() {
		instance = this;

		// initializes the actions
		foreach (ActionCouple a in Actions) {
			a.listener = a.go.GetComponent(typeof(GestureListener)) as GestureListener;
		}
	}

	public static void SetVisibility(GameObject obj, bool isVisible) {
		// hides or shows the given object childs
		Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

		foreach (Renderer r in renderers) {
			r.enabled = isVisible;
		}
	}
}
