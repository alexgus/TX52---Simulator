using UnityEngine;
using System.Collections;

public class DummyAvatar : MonoBehaviour {
	public Transform Hips;
	public Transform Neck;
	public Transform Head;
	public Transform LeftUpperArm;
	public Transform LeftElbow;
	public Transform LeftHand;
	public Transform RightUpperArm;
	public Transform RightElbow;
	public Transform RightHand;
	public Transform LeftThigh;
	public Transform LeftKnee;
	public Transform LeftFoot;
	public Transform RightThigh;
	public Transform RightKnee;
	public Transform RightFoot;
	public Transform Root;

	protected Transform[] joints;
	protected Quaternion[] initialRotations;

	void Awake() {
		MapJoints();
	}

	protected void MapJoints() {
		joints = new Transform[KinectWrapper.Constants.BODY_PARTS_COUNT];

		joints[KinectWrapper.Constants.HIPS] = Hips;
		joints[KinectWrapper.Constants.NECK] = Neck;
		joints[KinectWrapper.Constants.HEAD] = Head;
		joints[KinectWrapper.Constants.LEFT_SHOULDER] = LeftUpperArm;
		joints[KinectWrapper.Constants.LEFT_ELBOW] = LeftElbow;
		joints[KinectWrapper.Constants.LEFT_HAND] = LeftHand;
		joints[KinectWrapper.Constants.RIGHT_SHOULDER] = RightUpperArm;
		joints[KinectWrapper.Constants.RIGHT_ELBOW] = RightElbow;
		joints[KinectWrapper.Constants.RIGHT_HAND] = RightHand;
		joints[KinectWrapper.Constants.LEFT_HIP] = LeftThigh;
		joints[KinectWrapper.Constants.LEFT_KNEE] = LeftKnee;
		joints[KinectWrapper.Constants.LEFT_FOOT] = LeftFoot;
		joints[KinectWrapper.Constants.RIGHT_HIP] = RightThigh;
		joints[KinectWrapper.Constants.RIGHT_KNEE] = RightKnee;
		joints[KinectWrapper.Constants.RIGHT_FOOT] = RightFoot;

		initialRotations = new Quaternion[joints.Length];

		for (int i = 0; i < joints.Length; i++) {
			initialRotations[i] = joints[i].rotation;
		}
	}

	public Transform GetJoint(int joint) {
		return joints[joint];
	}
}
