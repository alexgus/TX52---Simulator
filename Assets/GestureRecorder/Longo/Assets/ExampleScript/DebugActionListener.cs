using UnityEngine;
using System.Collections;

public class DebugActionListener : MonoBehaviour, GestureListener {
	public void OnComplete(DynGesture gesture) {
		Debug.Log(gesture.Name + ": Complete");
	}

	public void OnRecomplete(DynGesture gesture) {
		Debug.Log(gesture.Name + ": Recomplete");
	}

	public void OnRelease(DynGesture gesture) {
		Debug.Log(gesture.Name + ": Release");
	}
}
