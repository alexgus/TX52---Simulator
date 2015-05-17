using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	// the second menu where you can add a new gesture
	public RectTransform newBehaviorMenu;

	// the avatar using kinect
	public GameObject kinectAvatar;

	// an example of gesture already added
	public GameObject dummyGesture;

	// the amount of behavior
	private int behaviorCount = 0;

	void Start() {
		dummyGesture.SetActive(false);
	}


	public void OnNewGestureClick() {
		// opens the other menu
		newBehaviorMenu.GetComponent<NewBehaviorMenu>().Reset();

		this.transform.localPosition = new Vector3(-300, 0, 0);
		newBehaviorMenu.transform.localPosition = new Vector3(0, 0, 0);
	}


	public void NewGesture(DynGesture ges) {
		// saves the new gesture
		Manager.instance.gestures.Add(ges);

		GameObject newGes = (GameObject)Instantiate(dummyGesture);
		newGes.SetActive(true);
		newGes.GetComponentInChildren<Text>().text = ges.Name;
		newGes.transform.SetParent(dummyGesture.transform.parent);
		newGes.transform.position = dummyGesture.transform.position + new Vector3(0, -35 * behaviorCount, 0);

		behaviorCount++;
	}


	public void OnTestClick() {
		kinectAvatar.GetComponent<KinectAvatar>().Analyzer.Mode = GestureAnalyzer.ModeEnum.ANALYSING;
	}
}
