using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

	private void Awake() {
        instance = this;

        // Launch analysis
        importGestures();

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

    private void importGestures()
    {
        string folderPath = "./Assets/RecordedGesture/";

        string[] gesturesFiles = Directory.GetFiles(folderPath, "*.xml");
        foreach (string s in gesturesFiles)
        {
            Debug.Log("Loading gesture : " + s);
            // Load old gesture and display it
            DynGesture d = Exporter.import(s);
            //GestureToListenerMapper mapper = new GestureToListenerMapper(Manager.instance.Actions, "./Assets/mapping.xml");
            d.Listener = Manager.instance.Actions[0].listener;
          //  d.Listener = Manager.instance.Actions[mapper.GetActionIndexFromGestureName(s)].listener;
            Manager.instance.gestures.Add(d);
        }
    }
}
