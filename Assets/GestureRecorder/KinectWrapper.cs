using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

// Wrapper class that holds the various structs, variables, and dll imports
// needed to set up a model with the Kinect.
public class KinectWrapper {
	public static class Constants {
		public const int SkeletonCount = 6;

		public const float MinTimeBetweenSameGestures = 0.0f;
		public const float PoseCompleteDuration = 1.0f;
		public const float ClickStayDuration = 2.5f;

		public const int HEAD = 0;
		public const int NECK = 1;
		public const int LEFT_SHOULDER = 2;
		public const int RIGHT_SHOULDER = 3;
		public const int LEFT_ELBOW = 4;
		public const int RIGHT_ELBOW = 5;
		public const int LEFT_HAND = 6;
		public const int RIGHT_HAND = 7;
		public const int HIPS = 8;
		public const int LEFT_HIP = 9;
		public const int RIGHT_HIP = 10;
		public const int LEFT_KNEE = 11;
		public const int RIGHT_KNEE = 12;
		public const int LEFT_FOOT = 13;
		public const int RIGHT_FOOT = 14;
		public const int BODY_PARTS_COUNT = 15;
	};

	// Struct to store color RGB888
	public struct ColorRgb888 {
		public byte r;
		public byte g;
		public byte b;
	}

	// Struct to store the joint's poision.
	public struct SkeletonJointPosition {
		public float x, y, z;
	}

	// Struct that will hold the joints orientation.
	public struct SkeletonJointOrientation {
		public float x, y, z, w;
	}

	// Struct that combines the previous two and makes the transform.
	public struct SkeletonJointTransformation {
		public int jointType;
		public SkeletonJointPosition position;
		public float positionConfidence;
		public SkeletonJointOrientation orientation;
		public float orientationConfidence;
	}

	// DLL Imports to pull in the necessary Unity functions to make the Kinect go.
	[DllImport("UnityInterface2")]
	public static extern int Init(bool isInitDepthStream, bool isInitColorStream);
	[DllImport("UnityInterface2")]
	public static extern void Shutdown();
	[DllImport("UnityInterface2")]
	public static extern int Update([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = Constants.SkeletonCount, ArraySubType = UnmanagedType.U2)] short[] pUsers,
									[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = Constants.SkeletonCount, ArraySubType = UnmanagedType.U2)] short[] pStates, ref int pUsersCount);

	[DllImport("UnityInterface2")]
	public static extern IntPtr GetLastErrorString();
	[DllImport("UnityInterface2")]
	public static extern int GetDepthWidth();
	[DllImport("UnityInterface2")]
	public static extern int GetDepthHeight();
	[DllImport("UnityInterface2")]
	public static extern int GetColorWidth();
	[DllImport("UnityInterface2")]
	public static extern int GetColorHeight();
	[DllImport("UnityInterface2")]
	public static extern IntPtr GetUsersLabelMap();
	[DllImport("UnityInterface2")]
	public static extern IntPtr GetUsersDepthMap();
	[DllImport("UnityInterface2")]
	public static extern IntPtr GetUsersColorMap();

	[DllImport("UnityInterface2")]
	public static extern void SetSkeletonSmoothing(float factor);

	[DllImport("UnityInterface2")]
	public static extern bool GetJointTransformation(uint userID, int joint, ref SkeletonJointTransformation pTransformation);
	[DllImport("UnityInterface2")]
	public static extern bool GetJointPosition(uint userID, int joint, ref SkeletonJointPosition pTransformation);
	[DllImport("UnityInterface2")]
	public static extern bool GetJointOrientation(uint userID, int joint, ref SkeletonJointOrientation pTransformation);
	[DllImport("UnityInterface2")]
	public static extern float GetJointPositionConfidence(uint userID, int joint);
	[DllImport("UnityInterface2")]
	public static extern float GetJointOrientationConfidence(uint userID, int joint);

	[DllImport("UnityInterface2")]
	public static extern void StartLookingForUsers(IntPtr NewUser, IntPtr CalibrationStarted, IntPtr CalibrationFailed, IntPtr CalibrationSuccess, IntPtr UserLost);
	[DllImport("UnityInterface2")]
	public static extern void StopLookingForUsers();

	public delegate void UserDelegate(uint userId);

	public static void StartLookingForUsers(UserDelegate NewUser, UserDelegate CalibrationStarted, UserDelegate CalibrationFailed, UserDelegate CalibrationSuccess, UserDelegate UserLost) {
		StartLookingForUsers(
			Marshal.GetFunctionPointerForDelegate(NewUser),
			Marshal.GetFunctionPointerForDelegate(CalibrationStarted),
			Marshal.GetFunctionPointerForDelegate(CalibrationFailed),
			Marshal.GetFunctionPointerForDelegate(CalibrationSuccess),
			Marshal.GetFunctionPointerForDelegate(UserLost)
			);
	}

	public static bool GetSkeletonJointOrientation(uint userID, int joint, bool flip, ref Quaternion pJointRot) {
		Matrix4x4 matOri = Matrix4x4.identity;
		bool bMatHasOri = false;

		if (joint == Constants.LEFT_HAND) {
			// special case - left hand
			SkeletonJointPosition posElbow = new SkeletonJointPosition();
			SkeletonJointPosition posHand = new SkeletonJointPosition();
			SkeletonJointPosition posHips = new SkeletonJointPosition();
			SkeletonJointPosition posNeck = new SkeletonJointPosition();

			bool bElbowTracked = GetJointPosition(userID, Constants.LEFT_ELBOW, ref posElbow);
			bool bHandTracked = GetJointPosition(userID, Constants.LEFT_HAND, ref posHand);
			bool bHipsTracked = GetJointPosition(userID, Constants.HIPS, ref posHips);
			bool bNeckTracked = GetJointPosition(userID, Constants.NECK, ref posNeck);

			if (bElbowTracked && bHandTracked && bHipsTracked && bNeckTracked) {
				Vector3 vElbow = new Vector3(posElbow.x, posElbow.y, posElbow.z);
				Vector3 vHand = new Vector3(posHand.x, posHand.y, posHand.z);
				Vector3 vHips = new Vector3(posHips.x, posHips.y, posHips.z);
				Vector3 vNeck = new Vector3(posNeck.x, posNeck.y, posNeck.z);

				Vector3 vx = -(vHand - vElbow);
				Vector3 vy = vNeck - vHips;

				MakeMatrixFromXY(vx, vy, ref matOri);
				bMatHasOri = true;
			}
		}
		else if (joint == Constants.RIGHT_HAND) {
			// special case - right hand
			SkeletonJointPosition posElbow = new SkeletonJointPosition();
			SkeletonJointPosition posHand = new SkeletonJointPosition();
			SkeletonJointPosition posHips = new SkeletonJointPosition();
			SkeletonJointPosition posNeck = new SkeletonJointPosition();

			bool bElbowTracked = GetJointPosition(userID, Constants.RIGHT_ELBOW, ref posElbow);
			bool bHandTracked = GetJointPosition(userID, Constants.RIGHT_HAND, ref posHand);
			bool bHipsTracked = GetJointPosition(userID, Constants.HIPS, ref posHips);
			bool bNeckTracked = GetJointPosition(userID, Constants.NECK, ref posNeck);

			if (bElbowTracked && bHandTracked && bHipsTracked && bNeckTracked) {
				Vector3 vElbow = new Vector3(posElbow.x, posElbow.y, posElbow.z);
				Vector3 vHand = new Vector3(posHand.x, posHand.y, posHand.z);
				Vector3 vHips = new Vector3(posHips.x, posHips.y, posHips.z);
				Vector3 vNeck = new Vector3(posNeck.x, posNeck.y, posNeck.z);

				Vector3 vx = vHand - vElbow;
				Vector3 vy = vNeck - vHips;

				MakeMatrixFromXY(vx, vy, ref matOri);
				bMatHasOri = true;
			}
		}
		else {
			// all other joints
			SkeletonJointOrientation oriJoint = new SkeletonJointOrientation();

			if (GetJointOrientation(userID, joint, ref oriJoint)) {
				Quaternion rotJoint = new Quaternion(oriJoint.x, oriJoint.y, oriJoint.z, oriJoint.w);
				matOri.SetTRS(Vector3.zero, rotJoint, Vector3.one);
				bMatHasOri = true;
			}
		}

		if (bMatHasOri) {
			Vector4 vZ = matOri.GetColumn(2);
			Vector4 vY = matOri.GetColumn(1);

			if (!flip) {
				vZ.y = -vZ.y;
				vY.x = -vY.x;
				vY.z = -vY.z;
			}
			else {
				vZ.x = -vZ.x;
				vZ.y = -vZ.y;
				vY.z = -vY.z;
			}

			if (vZ.x != 0.0f || vZ.y != 0.0f || vZ.z != 0.0f)
				pJointRot = Quaternion.LookRotation(vZ, vY);
			else
				bMatHasOri = false;
		}

		return bMatHasOri;
	}

	//constructs an orientation from 2 vectors: the first specifies the x axis, and the next specifies the y axis
	//uses the first vector as x axis, then constructs the other axes using cross products
	private static void MakeMatrixFromXY(Vector3 xUnnormalized, Vector3 yUnnormalized, ref Matrix4x4 jointOrientation) {
		//matrix columns
		Vector3 xCol;
		Vector3 yCol;
		Vector3 zCol;

		//set up the three different columns to be rearranged and flipped
		xCol = xUnnormalized.normalized;
		zCol = Vector3.Cross(xCol, yUnnormalized.normalized).normalized;
		yCol = Vector3.Cross(zCol, xCol);
		//yCol = yUnnormalized.normalized;
		//zCol = Vector3.Cross(xCol, yCol).normalized;

		//copy values into matrix
		PopulateMatrix(ref jointOrientation, xCol, yCol, zCol);
	}

	//populate matrix using the columns
	private static void PopulateMatrix(ref Matrix4x4 jointOrientation, Vector3 xCol, Vector3 yCol, Vector3 zCol) {
		jointOrientation.SetColumn(0, xCol);
		jointOrientation.SetColumn(1, yCol);
		jointOrientation.SetColumn(2, zCol);
	}

	public static int GetSkeletonMirroredJoint(int jointIndex) {
		switch (jointIndex) {
			case Constants.LEFT_SHOULDER:
				return Constants.RIGHT_SHOULDER;
			case Constants.LEFT_ELBOW:
				return Constants.RIGHT_ELBOW;
			case Constants.LEFT_HAND:
				return Constants.RIGHT_HAND;
			case Constants.RIGHT_SHOULDER:
				return Constants.LEFT_SHOULDER;
			case Constants.RIGHT_ELBOW:
				return Constants.LEFT_ELBOW;
			case Constants.RIGHT_HAND:
				return Constants.LEFT_HAND;
			case Constants.LEFT_HIP:
				return Constants.RIGHT_HIP;
			case Constants.LEFT_KNEE:
				return Constants.RIGHT_KNEE;
			case Constants.LEFT_FOOT:
				return Constants.RIGHT_FOOT;
			case Constants.RIGHT_HIP:
				return Constants.LEFT_HIP;
			case Constants.RIGHT_KNEE:
				return Constants.LEFT_KNEE;
			case Constants.RIGHT_FOOT:
				return Constants.LEFT_FOOT;
		}

		return jointIndex;
	}

	// copies and configures the needed resources in the project directory
	public static bool CheckOpenNIPresence() {
		bool bOneCopied = false, bAllCopied = true;

		// check openni directory and resources
		string sOpenNIPath = System.Environment.GetEnvironmentVariable("OPENNI2_REDIST");
		if (sOpenNIPath == String.Empty || !Directory.Exists(sOpenNIPath))
			throw new Exception("OpenNI directory not found. Please check the OpenNI installation.");

		sOpenNIPath = sOpenNIPath.Replace('\\', '/');
		if (sOpenNIPath.EndsWith("/"))
			sOpenNIPath = sOpenNIPath.Substring(0, sOpenNIPath.Length - 1);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (!File.Exists("OpenNI2.dll")) {
			string srcOpenNiDll = sOpenNIPath + "/OpenNI2.dll";

			if (File.Exists(srcOpenNiDll)) {
				Debug.Log("Copying OpenNI library...");
				File.Copy(srcOpenNiDll, "OpenNI2.dll");

				bOneCopied = File.Exists("OpenNI2.dll");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied OpenNI library.");
			}
		}
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (!File.Exists("libOpenNI2.dylib")) {
			string srcOpenNiDll = sOpenNIPath + "/libOpenNI2.dylib";

			if (File.Exists(srcOpenNiDll)) {
				Debug.Log("Copying OpenNI library...");
				File.Copy(srcOpenNiDll, "libOpenNI2.dylib");

				bOneCopied = File.Exists("libOpenNI2.dylib");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied OpenNI library.");
			}
		}
#endif

		if (!File.Exists("OpenNI.ini")) {
			Debug.Log("Copying OpenNI configuration...");
			TextAsset textRes = Resources.Load("OpenNI.ini", typeof(TextAsset)) as TextAsset;

			if (textRes != null) {
				string sResText = textRes.text.Replace("%OPENNI_REDIST_DIR%", sOpenNIPath);
				File.WriteAllText("OpenNI.ini", sResText);

				bOneCopied = File.Exists("OpenNI.ini");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied OpenNI configuration.");
			}
		}

		// check nite directory and resources
		string sNiTEPath = System.Environment.GetEnvironmentVariable("NITE2_REDIST64");
        if (sNiTEPath == String.Empty || !Directory.Exists(sNiTEPath))
        {
            sNiTEPath = System.Environment.GetEnvironmentVariable("NITE2_REDIST");
            if (sNiTEPath == String.Empty || !Directory.Exists(sNiTEPath))
                throw new Exception("NiTE directory not found. Please check the NiTE installation.");
        }


		sNiTEPath = sNiTEPath.Replace('\\', '/');
		if (sNiTEPath.EndsWith("/"))
			sNiTEPath = sNiTEPath.Substring(0, sNiTEPath.Length - 1);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (!File.Exists("NiTE2.dll")) {
			string srcNiteDll = sNiTEPath + "/NiTE2.dll";

			if (File.Exists(srcNiteDll)) {
				Debug.Log("Copying NiTE library...");
				File.Copy(srcNiteDll, "NiTE2.dll");

				bOneCopied = File.Exists("NiTE2.dll");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied NITE library.");
			}
		}
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (!File.Exists("libNiTE2.dylib")) {
			string srcNiteDll = sNiTEPath + "/libNiTE2.dylib";

			if (File.Exists(srcNiteDll)) {
				Debug.Log("Copying NiTE library...");
				File.Copy(srcNiteDll, "libNiTE2.dylib");

				bOneCopied = File.Exists("libNiTE2.dylib");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied NITE library.");
			}
		}
#endif

		if (!File.Exists("NiTE.ini")) {
			Debug.Log("Copying NiTE configuration...");
			TextAsset textRes = Resources.Load("NiTE.ini", typeof(TextAsset)) as TextAsset;

			if (textRes != null) {
				string sResText = textRes.text.Replace("%NITE_REDIST_DIR%", sNiTEPath);
				File.WriteAllText("NiTE.ini", sResText);

				bOneCopied = File.Exists("NiTE.ini");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied NITE configuration.");
			}
		}

		// check the unity interface library
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (!File.Exists("UnityInterface2.dll")) {
			Debug.Log("Copying UnityInterface library...");
			TextAsset textRes = Resources.Load("UnityInterface2.dll", typeof(TextAsset)) as TextAsset;

			if (textRes != null) {
				File.WriteAllBytes("UnityInterface2.dll", textRes.bytes);

				bOneCopied = File.Exists("UnityInterface2.dll");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied UnityInterface library.");
			}
		}

		if (!File.Exists("msvcp90d.dll")) {
			Debug.Log("Copying msvcp90d library...");
			TextAsset textRes = Resources.Load("msvcp90d.dll", typeof(TextAsset)) as TextAsset;

			if (textRes != null) {
				using (FileStream fileStream = new FileStream("msvcp90d.dll", FileMode.Create, FileAccess.Write, FileShare.Read)) {
					fileStream.Write(textRes.bytes, 0, textRes.bytes.Length);
				}

				bOneCopied = File.Exists("msvcp90d.dll");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied msvcp90d library.");
			}
		}

		if (!File.Exists("msvcr90d.dll")) {
			Debug.Log("Copying msvcr90d library...");
			TextAsset textRes = Resources.Load("msvcr90d.dll", typeof(TextAsset)) as TextAsset;

			if (textRes != null) {
				using (FileStream fileStream = new FileStream("msvcr90d.dll", FileMode.Create, FileAccess.Write, FileShare.Read)) {
					fileStream.Write(textRes.bytes, 0, textRes.bytes.Length);
				}

				bOneCopied = File.Exists("msvcr90d.dll");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied msvcr90d library.");
			}
		}

#endif
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
		if (!File.Exists("libUnityInterface2.dylib")) {
			Debug.Log("Copying UnityInterface library...");
			TextAsset textRes = Resources.Load("libUnityInterface2.dylib", typeof(TextAsset)) as TextAsset;

			if (textRes != null) {
				File.WriteAllBytes("libUnityInterface2.dylib", textRes.bytes);

				bOneCopied = File.Exists("libUnityInterface2.dylib");
				bAllCopied = bAllCopied && bOneCopied;

				if (bOneCopied)
					Debug.Log("Copied UnityInterface library.");
			}
		}
#endif

		return bOneCopied && bAllCopied;
	}

}

