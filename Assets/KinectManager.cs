using UnityEngine;
using System;
using System.Collections;

public class KinectManager : MonoBehaviour {
	// the instance of the manager
	public static KinectManager instance;

	// the players avatar (up to 2 players)
	public GameObject[] objectPlayers = new GameObject[2];
	private KinectAvatar[] players = new KinectAvatar[2];

	// below this confidence percentage, we considered the joint as not tracked
	public float minimumConfidence = 0.5f;

	// indicates which players are calibrated
	private bool[] isPlayerCalibrated = new bool[2];

	// players NiTE ids
	private uint[] playerID = new uint[2];

	// NiTE stuff
	private short[] oniUsers = new short[KinectWrapper.Constants.SkeletonCount];
	private short[] oniStates = new short[KinectWrapper.Constants.SkeletonCount];
	private Int32 oniUsersCount = 0;

	private KinectWrapper.SkeletonJointPosition jointPosition;


	void Awake() {
		// Look for OpenNI & NiTE
		if (KinectWrapper.CheckOpenNIPresence()) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	// Use this for initialization
	void Start() {
		KinectWrapper.Init(false, false);

		KinectWrapper.StartLookingForUsers(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		KinectWrapper.SetSkeletonSmoothing(0.7f);

		instance = this;

		DontDestroyOnLoad(gameObject);
	}

	// Update is called once per frame
	void Update() {
		oniUsersCount = oniUsers.Length;
		KinectWrapper.Update(oniUsers, oniStates, ref oniUsersCount);

		// Process the new, lost and calibrated user(s)
		if (oniUsersCount > 0) {
			for (int i = 0; i < oniUsersCount; i++) {
				uint userId = (uint) oniUsers[i];
				short userState = oniStates[i];

				switch (userState) {
					/*case 1: // new user
						break;*/

					/*case 2: // calibration started
						break;*/

					case 3: // calibration succeeded
						OnCalibrationSuccess(userId);
						break;

					/*case 4: // calibration failed
						break;*/

					case 5: // user lost
						OnUserLost(userId);
						break;
				}
			}
		}
	}

	public bool IsJointOrientationTracked(uint userID, int joint) {
		return KinectWrapper.GetJointOrientationConfidence(userID, joint) >= minimumConfidence;
	}

	public Quaternion GetJointOrientation(uint userID, int joint) {
		Quaternion rotJoint = Quaternion.identity;

		KinectWrapper.GetSkeletonJointOrientation(userID, joint, true, ref rotJoint);

		return rotJoint;
	}

	public bool IsJointPositionTracked(uint userID, int joint) {
		return KinectWrapper.GetJointPositionConfidence(userID, joint) >= minimumConfidence;
	}

	public Vector3 GetJointPosition(uint userID, int joint) {
		if (KinectWrapper.GetJointPosition(userID, joint, ref jointPosition)) {
			return new Vector3(jointPosition.x, jointPosition.y, jointPosition.z);
		}

		return Vector3.zero;
	}


	private void OnCalibrationSuccess(uint userId) {
		if (!isPlayerCalibrated[0]) {
			NewUser(0, userId);
		}
		else if (!isPlayerCalibrated[1]) {
			NewUser(1, userId);
		}
		else {
			KinectWrapper.StopLookingForUsers();
		}
	}

	private void OnUserLost(uint userId) {
		for (int i = 0; i < 2; i++) {
			if (playerID[i] == userId) {
				isPlayerCalibrated[i] = false;
				playerID[i] = 0;
				players[i].SetID(0);
				players[i].Reset();
			}
		}

		KinectWrapper.StartLookingForUsers(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
	}

	private void NewUser(int player, uint userId) {
		if (objectPlayers[player] != null && objectPlayers[player].activeInHierarchy) {
			playerID[player] = userId;

			players[player] = objectPlayers[player].GetComponent<KinectAvatar>();
			players[player].SetID(userId);
		}
	}

	private void OnApplicationQuit() {
		KinectWrapper.Shutdown();
		instance = null;
	}
}
