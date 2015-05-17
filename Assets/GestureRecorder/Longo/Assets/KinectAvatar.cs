using UnityEngine;
using System.Collections;

public class KinectAvatar : DummyAvatar {
	// the nite user id
	private uint userID = 0;

	// the analyzer linked to this avatar
	public GestureAnalyzer Analyzer { get; internal set; }

	void Awake() {
		MapJoints();

		Analyzer = new GestureAnalyzer();
	}


	void Update() {
		if (userID > 0) {
			// updates the joint positions
			UpdateSkeleton();
		}
	}


	public void Reset() {
		// resets the avatar to the default position
		for (int i = 0; i < joints.Length; i++) {
			joints[i].rotation = initialRotations[i];
		}
	}


	public void SetID(uint id) {
		// changes the user id
		userID = id;

		CancelInvoke();

		if (id > 0) {
			// resets the analyzer update
			InvokeRepeating("UpdateState", 0, GestureAnalyzer.UPDATE_INTERVAL);
		}
	}


	private void UpdateState() {
		if (Analyzer != null) {
			// gives the current state to the analyzer
			Analyzer.Update(new State(joints));
		}
	}


	private void UpdateSkeleton() {
		for (int i = 0; i < joints.Length; i++) {
			UpdateJoint(i);
		}
	}


	private void UpdateJoint(int joint) {
		// updates the orientation (and therefore position) of the given joint
		if (KinectManager.instance.IsJointOrientationTracked(userID, joint)) {
			Quaternion rotJoint = KinectManager.instance.GetJointOrientation(userID, joint);

			if (rotJoint != Quaternion.identity) {
				rotJoint *= initialRotations[joint];
				joints[joint].rotation = Quaternion.Slerp(joints[joint].rotation, rotJoint, Time.deltaTime * 10.0f);
			}
		}
	}
}
