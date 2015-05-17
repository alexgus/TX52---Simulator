using UnityEngine;
using System;
using System.Collections.Generic;

public class GestureAnalyzer {
	// the modes availablefor the analyzer
	public enum ModeEnum { WAITING, RECORDING, ANALYSING };

	// the analyzer will work at 1/UPDATE_INTERVAL fps
	public const float UPDATE_INTERVAL = 0.05f;

	// a movement magnitude inferior at STILL_MARGIN is considered as immobile
	public const float STILL_MARGIN = 0.4f;

	// indicates how many frame without movement we needs to consider that the user is not moving
	private const int STILL_REQUIREMENT = 10;

	// the current mode of the analyzer
	public ModeEnum Mode { get; set; }

	// the function to call when the analyzer recorded a movement
	private Action<Movement> recordCallback;

	// the amount of checkpoint when recording
	private int recordCheckpoints;

	// the previous state
	private State prevState = null;

	// the list of the state since the user moved
	private List<State> movement;

	// amount of frame since the user is not moving
	private int stillUntil = 0;

	// indicates when the user started moving
	private int movedAt = -1;

	// the average last position of each joint (is a sum, need to be divided)
	private Vector4[] averagePosition;


	public struct Movement {
		// the time it takes to make the whole move
		public float duration;
		public List<MovementCheckpoint> checkpoints;
	}

	public struct MovementCheckpoint {
		// the time it takes to make this move, in seconds
		public float duration;
		public List<JointMovement> jointMovements;
	}

	public struct JointMovement {
		// the joint id
		public int joint;
		// indicates how important this joint movement is on the overall checkpoint movement
		public float percent;
		// the initial position of the joint in the "spine-space"
		public Vector4 initialPosition;
		// the final position of the joint in the "spine-space"
		public Vector4 finalPosition;
		// the initial world position of the joint (not pertinent)
		public Vector4 initialAbsolutePosition;
		// the final world position of the joint (not pertinent)
		public Vector4 finalAbsolutePosition;
	}

	public GestureAnalyzer() {
		Mode = ModeEnum.WAITING;
		movement = new List<State>();
	}


	public void Record(Action<Movement> callback, int checkpoints) {
		averagePosition = new Vector4[KinectWrapper.Constants.BODY_PARTS_COUNT];

		SetMode(ModeEnum.RECORDING);
		recordCallback = callback;
		recordCheckpoints = checkpoints;
	}

	private void SetMode(ModeEnum m) {
		Mode = m;

		// resets everything when changing mode
		prevState = null;
		movement.Clear();
		stillUntil = 0;
		movedAt = -1;
	}

	public void Update(State state) {
		if (Mode == ModeEnum.WAITING) {
			return;
		}

		State.JointTransform[] curr = state.getJoints();
		State.JointTransform[] last10 = null;

		if (prevState != null && Mode == ModeEnum.ANALYSING) {
			// when analysing, we give each gesture the current state and the delta moves since last state
			Vector4[] deltaPos = new Vector4[KinectWrapper.Constants.BODY_PARTS_COUNT];
			State.JointTransform[] prevJoints = prevState.getJoints();

			for (int i = 0; i < deltaPos.Length; i++) {
				deltaPos[i] = curr[i].position - prevJoints[i].position;
			}

			// lets each gesture analyse for itself
			foreach (DynGesture gesture in Manager.instance.gestures) {
				gesture.Update(state, deltaPos);
			}
		}
		else if (movement.Count >= 10 && Mode == ModeEnum.RECORDING) {
			last10 = movement[movement.Count - 10].getJoints();
			bool isStill = true;

			// checking if we've moved or not since the last frame
			for (int j = 0; j < curr.Length && isStill; j++) {
				isStill &= IsStill(j, averagePosition[j] / 10.0f, curr[j].position, 5.0f * UPDATE_INTERVAL);
			}

			if (isStill) {
				stillUntil++;
			}
			else {
				// if we moved, reset the "still counter" and mark the movement
				stillUntil = 0;

				if (movedAt < 0) {
					movedAt = movement.Count;
				}
			}

			// if the user has moved and is now still
			if (stillUntil == STILL_REQUIREMENT && movedAt >= 0) {
				if (Mode == ModeEnum.RECORDING) {
					AnalyzeForRecord(movement);
				}

				movement.Clear();

				return;
			}
		}


		if (Mode == ModeEnum.RECORDING) {
			for (int j = 0; j < curr.Length; j++) {
				averagePosition[j] += curr[j].position - (last10 != null ? last10[j].position : Vector4.zero);
			}

			movement.Add(state);
		}

		prevState = state;
	}

	// generates a set of move from the previously saved moves
	private void AnalyzeForRecord(List<State> moves) {
		// ignore the latest states because they should all be roughly the same
		int totalDuration = moves.Count - movedAt - 1;

		State.JointTransform[] start = moves[movedAt].getJoints();
		State.JointTransform[] end = moves[totalDuration].getJoints();

		Movement move = new Movement();
		move.duration = (totalDuration - STILL_REQUIREMENT) * UPDATE_INTERVAL;
		//move.jointMovements = AnalyzeForRecord(start, end);
		move.checkpoints = new List<MovementCheckpoint>();

		MovementCheckpoint check;
		int checkpointDuration = (totalDuration - STILL_REQUIREMENT) / (recordCheckpoints + 1);

		for (int i = 0; i < recordCheckpoints + 1; i++) {
			// generates a movement for each checkpoint
			end = moves[movedAt + (i + 1) * checkpointDuration].getJoints();

			check = new MovementCheckpoint();
			check.duration = checkpointDuration * UPDATE_INTERVAL;
			check.jointMovements = AnalyzeForRecord(start, end, check.duration);

			move.checkpoints.Add(check);

			start = end;
		}

		// sends it to the callback
		recordCallback(move);

		Mode = ModeEnum.WAITING;
	}

	// generates for each moving joint a JointMovement going from start to end
	private List<JointMovement> AnalyzeForRecord(State.JointTransform[] start, State.JointTransform[] end, float duration) {
		List<JointMovement> components = new List<JointMovement>();
		JointMovement m;
		Vector4 diff;
		int i;

		float totalMagnitude = 0;

		for (i = 0; i < start.Length; i++) {
			diff = end[i].position - start[i].position;

			// if the joint didn't moved much, we just ignore it
			if (diff.magnitude >= Margin(i, duration)) {
				m = new JointMovement();
				m.joint = i;
				m.initialPosition = start[i].position;
				m.finalPosition = end[i].position;

				// world position are not very pertinent, they are used to preview the movement
				// (it only works because the preview avatar is at the same position than the user)
				m.initialAbsolutePosition = start[i].absolutePosition;
				m.finalAbsolutePosition = end[i].absolutePosition;
				m.percent = diff.magnitude;

				totalMagnitude += diff.magnitude;

				components.Add(m);
			}
		}

		// convert the movement magnitude into a percentage
		for (i = 0; i < components.Count; i++) {
			m = components[i];
			m.percent = 100.0f * m.percent / totalMagnitude;
			components[i] = m;
		}

		return components;
	}


	private bool IsStill(int joint, Vector4 prevPos, Vector4 pos, float duration) {
		return (prevPos - pos).magnitude < Margin(joint, duration);
	}


	// returns the margin we chose to ignore for the given joint
	// foots and knees tends to not be very precise and to move a lot
	// so we increase the margin for them
	public static float Margin(int joint, float duration) {
		if (joint == KinectWrapper.Constants.LEFT_FOOT || joint == KinectWrapper.Constants.RIGHT_FOOT || joint == KinectWrapper.Constants.LEFT_KNEE || joint == KinectWrapper.Constants.RIGHT_KNEE) {
			return 1.5f * STILL_MARGIN * duration;
		}

		return STILL_MARGIN * duration;
	}
}