using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour {
	// the second menu where you can add a new gesture
	public RectTransform newBehaviorMenu;

	// the avatar using kinect
	public GameObject kinectAvatar;

	// an example of gesture already added
	public GameObject dummyGesture;

	// the amount of behavior
	private int behaviorCount = 0;

    public enum Recorder
    {
        SAVE,
        LOAD
    }

	void Start() {
		dummyGesture.SetActive(false);
        this.loadGestures();
   	}


	public void OnNewGestureClick() {
		// opens the other menu
		newBehaviorMenu.GetComponent<NewBehaviorMenu>().Reset();

		this.transform.localPosition = new Vector3(-300, 0, 0);
		newBehaviorMenu.transform.localPosition = new Vector3(0, 0, 0);
	}

    public void loadGestures()
    {
        string folderPath = "./Assets/RecordedGesture/";

        string[] gesturesFiles = Directory.GetFiles(folderPath, "*.xml");
        foreach (string s in gesturesFiles)
        {
            Debug.Log("Loading gesture : " + s);
            // Load old gesture and display it
            DynGesture d = Exporter.import(s);
            d.Listener = Manager.instance.Actions[0].listener;
            NewGesture(d);
        }
    }

    public void registerGesture(DynGesture d)
    {
        Exporter.export(d);
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
