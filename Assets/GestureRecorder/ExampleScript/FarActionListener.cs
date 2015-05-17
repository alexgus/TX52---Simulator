using UnityEngine;
using System.Collections;

public class FarActionListener : MonoBehaviour, GestureListener {
	public Transform[] Cube;

	private float timer;
	private bool isVisible = false;

	void Start() {
		Manager.SetVisibility(this.gameObject, false);
	}

	void Update() {
		if (isVisible) {
			float p;

			if (timer > 0.0f) {
				timer -= Time.deltaTime;
			}

			if (timer < 0.0f) {
				timer = 0.0f;
			}

			p = 1.2f - timer;

			Cube[0].localPosition = new Vector3(p, 0.0f, 0.0f);
			Cube[1].localPosition = new Vector3(-p, 0.0f, 0.0f);
			Cube[2].localPosition = new Vector3(0.0f, 0.0f, p);
			Cube[3].localPosition = new Vector3(0.0f, 0.0f, -p);
		}
	}

	public void OnComplete(DynGesture gesture) {
		timer = 1.0f;
		isVisible = true;
		Manager.SetVisibility(this.gameObject, isVisible);
	}

	public void OnRecomplete(DynGesture gesture) {}

	public void OnRelease(DynGesture gesture) {
		isVisible = false;
		Manager.SetVisibility(this.gameObject, isVisible);
	}
}
