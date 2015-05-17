using UnityEngine;
using System;

public class State {
	// the joints of the state
	private JointTransform[] joints;

	// the matrix converting into the spine-centered space
	private Matrix4x4 spinMat;

	public class JointTransform {
		// the absolute position of the joint (not really used)
		public Vector4 absolutePosition;

		// the position of the joint in the spine-centered space
		public Vector4 position;

		// the orientation of the bone (in world space!)
		public Quaternion rotation;

		// the angle between the bone and its parent
		public float parentAngle;
	};

	public State(Transform[] jointTransforms) {
		Transform spine = jointTransforms[KinectWrapper.Constants.HIPS];

		// transform matrix
		// world position => spine-centered position
		Matrix4x4 mat = new Matrix4x4();
		mat.SetColumn(0, -spine.forward);
		mat.SetColumn(1, -spine.right);
		mat.SetColumn(2, spine.up);
		mat.SetColumn(3, new Vector4(spine.position.x, spine.position.y, spine.position.z, 1));

		spinMat = mat.inverse;

		joints = new JointTransform[jointTransforms.Length];

		for (int i = 0; i < jointTransforms.Length; i++) {
			joints[i] = new JointTransform();
			joints[i].absolutePosition = ToPoint4(jointTransforms[i].position);
			joints[i].position = spinMat * joints[i].absolutePosition;
			joints[i].rotation = jointTransforms[i].rotation;
			joints[i].parentAngle = Quaternion.Angle(jointTransforms[i].parent.rotation, jointTransforms[i].rotation);
		}
	}


	public JointTransform[] getJoints() {
		return joints;
	}

	private Vector4 ToPoint4(Vector3 point) {
		return new Vector4(point.x, point.y, point.z, 1);
	}
}