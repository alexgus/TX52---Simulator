using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NewBehaviorMenu : MonoBehaviour {
	// the main menu
	public RectTransform mainMenu;

	// the avatar using Kinect
	public GameObject kinectAvatar;

	// the dummy avatar gameobject
	public GameObject dummyAvatarGameObject;

	// ui stuff
	public GameObject actionButton;
	public GameObject beginButton;
	public GameObject previewButton;
	public GameObject saveButton;
	public GameObject nameText;
	public GameObject checkpointNb;
	public Toggle isRepetitiveToggle;
	public Toggle ignoreOthersToggle;
	public Toggle isAPoseToggle;

	private Text beginButtonText;

	// the dummy avatar
	private DummyAvatar dummyAvatar;

	// contains the movement recorded by the analyzer; used on save
	private GestureAnalyzer.Movement recordedMovement;

	// the countdown used before recording
	private int timer;

	// the movements to preview
	private List<List<GestureAnalyzer.JointMovement>> previewMovements;

	// the duration of each checkpoints
	private List<float> previewDurations;

	// the current preview checkpoint
	private int currentPreview = -1;

	// the current preview checkpoint percentage
	private float previewPercentage;

	// the currently selected action
	private int currentAction = 0;

	void Start() {
		Manager.SetVisibility(dummyAvatarGameObject, false);
		beginButtonText = beginButton.GetComponentInChildren<Text>();
		dummyAvatar = dummyAvatarGameObject.GetComponent<DummyAvatar>();
	}

	void Update() {
		if (currentPreview >= 0) {
			// updates the preview
			previewPercentage += Time.deltaTime / previewDurations[currentPreview];

			// moves the joints
			foreach (GestureAnalyzer.JointMovement movement in previewMovements[currentPreview]) {
				dummyAvatar.GetJoint(movement.joint).position = Vector4.Lerp(movement.initialAbsolutePosition, movement.finalAbsolutePosition, previewPercentage);
			}

			if (previewPercentage >= 1) {
				// reset colors
				foreach (GestureAnalyzer.JointMovement m in previewMovements[currentPreview]) {
					dummyAvatar.GetJoint(m.joint).renderer.material.color = Color.white;
				}

				previewPercentage = 0;
				currentPreview++;

				if (currentPreview >= previewDurations.Count) {
					// ends the preview
					currentPreview = -1;
					previewDurations = null;
					previewMovements = null;

					Manager.SetVisibility(dummyAvatarGameObject, false);
					Manager.SetVisibility(kinectAvatar, true);
				}
				else {
					// starts next checkpoint
					foreach (GestureAnalyzer.JointMovement m in previewMovements[currentPreview]) {
						dummyAvatar.GetJoint(m.joint).renderer.material.color = Color.green;
					}
				}
			}
		}
	}

	public void Reset() {
		// resets the state of the form
		// Important: I think Unity might have a way to do it automaticly but I haven't looked up
		nameText.GetComponent<InputField>().text = "";
		checkpointNb.GetComponent<InputField>().text = "0";
		beginButtonText.text = "Begin";
		beginButton.GetComponent<Button>().interactable = true;
		previewButton.GetComponent<Button>().interactable = false;
		saveButton.GetComponent<Button>().interactable = false;
		ignoreOthersToggle.isOn = false;
		isAPoseToggle.isOn = false;
		isRepetitiveToggle.isOn = false;
		currentPreview = -1;
		currentAction = 0;
		actionButton.GetComponentInChildren<Text>().text = "Action: " + Manager.instance.Actions[currentAction].name;
	}

	public void OnMainMenuClick() {
		// changes menu
		this.transform.localPosition = new Vector3(-300, 0, 0);
		mainMenu.transform.localPosition = new Vector3(0, 0, 0);
	}

	public void OnActionClick() {
		// switches to the next action
		currentAction = (currentAction + 1) % Manager.instance.Actions.Length;

		actionButton.GetComponentInChildren<Text>().text = "Action: " + Manager.instance.Actions[currentAction].name;
	}

	public void OnBeginClick() {
		// starts the countdown
		timer = 5;

		beginButton.GetComponent<Button>().interactable = false;

		for (int i = 1; i < timer; i++) {
			Invoke("Timer", i);
		}

		Timer();
	}

	public void OnPreviewClick() {
		previewMovements = new List<List<GestureAnalyzer.JointMovement>>();
		previewDurations = new List<float>();
		currentPreview = 0;
		previewPercentage = 0;

		// prepares the preview movements

		foreach (GestureAnalyzer.MovementCheckpoint checkpoint in recordedMovement.checkpoints) {
			previewMovements.Add(checkpoint.jointMovements);
			previewDurations.Add(checkpoint.duration);
		}

		foreach (GestureAnalyzer.JointMovement m in previewMovements[currentPreview]) {
			dummyAvatar.GetJoint(m.joint).renderer.material.color = Color.green;
		}

		// adds an extra empty checkpoint in order to stay in the preview mode a bit longer at the end
		previewDurations.Add(0.5f);
		previewMovements.Add(new List<GestureAnalyzer.JointMovement>());

		Manager.SetVisibility(dummyAvatarGameObject, true);
		Manager.SetVisibility(kinectAvatar, false);
	}

	public void OnSaveClick() {
		if (isAPoseToggle.isOn) {
			// removes the movements and keep only the final position
			GestureAnalyzer.MovementCheckpoint lastCheckpoint = recordedMovement.checkpoints[recordedMovement.checkpoints.Count - 1];
			List<GestureAnalyzer.JointMovement> moves = lastCheckpoint.jointMovements;
			GestureAnalyzer.JointMovement m;

			for (int i = 0; i < moves.Count; i++) {
				m = moves[i];
				m.initialAbsolutePosition = m.finalAbsolutePosition;
				m.initialPosition = m.finalPosition;
				moves[i] = m;
			}

			lastCheckpoint.jointMovements = moves;

			recordedMovement.checkpoints.Clear();
			recordedMovement.checkpoints.Add(lastCheckpoint);
		}

		// creates the new gesture
		DynGesture ges = new DynGesture();
		ges.Name = nameText.GetComponent<InputField>().text;
		ges.Movement = recordedMovement;
		ges.IgnoreOtherJoints = ignoreOthersToggle.isOn || isAPoseToggle.isOn;
		ges.IsRepetitive = isRepetitiveToggle.isOn;
		ges.Listener = Manager.instance.Actions[currentAction].listener;

		mainMenu.GetComponent<MainMenu>().NewGesture(ges);
        mainMenu.GetComponent<MainMenu>().registerGesture(ges);

		OnMainMenuClick();
	}

	void Timer() {
		// countdown
		timer--;

		if (timer > 0) {
			beginButtonText.text = "" + timer;
		}
		else {
			beginButtonText.text = "Go!";

			KinectAvatar ava = kinectAvatar.GetComponent<KinectAvatar>();

			ava.Analyzer.Record(OnRecordEnded, int.Parse(checkpointNb.GetComponent<InputField>().text));
		}
	}

	void OnRecordEnded(GestureAnalyzer.Movement movement) {
		recordedMovement = movement;

		beginButtonText.text = "Restart";

		saveButton.GetComponent<Button>().interactable = true;
		beginButton.GetComponent<Button>().interactable = true;
		previewButton.GetComponent<Button>().interactable = true;
	}
}
