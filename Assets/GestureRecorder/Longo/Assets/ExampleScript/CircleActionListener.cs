using UnityEngine;
using System.Collections;

public class CircleActionListener : MonoBehaviour, GestureListener {
	public Transform[] Cube;

	private float angle = 0.0f;
	private bool isVisible = false;

	void Start () {
		Manager.SetVisibility(this.gameObject, false);
	}
	
	void Update () {
		if (isVisible) {
			angle += Time.deltaTime;
			
			float c = Mathf.Cos(angle);
			float s = Mathf.Sin(angle);

			Cube[0].localPosition = new Vector3(c, 0.0f, s);
			Cube[1].localPosition = new Vector3(-c, 0.0f, -s);
		}
	}

	public void OnComplete(DynGesture gesture) {
		isVisible = true;
		Manager.SetVisibility(this.gameObject, isVisible);
	}

	public void OnRecomplete(DynGesture gesture) {
		Color c = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
		
		Cube[0].renderer.material.color = c;
		Cube[1].renderer.material.color = c;
	}

	public void OnRelease(DynGesture gesture) {
		isVisible = false;
		Manager.SetVisibility(this.gameObject, isVisible);
	}
}
