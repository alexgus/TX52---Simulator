using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynGesture {
	// the gesture's movements
	public GestureAnalyzer.Movement Movement { get; set; }

	// the gesture's name
	public string Name { get; set; }

	// indicates whether or not the gesture is a repetitive one
	public bool IsRepetitive { get; set; }

	// indicates whether or not we have to make sure the others joints haven't moved during the gesture
	public bool IgnoreOtherJoints { get; set; }

	public GestureListener Listener { private get; set; }

	// the state at which we are when completing the last checkpoint
	private State lastCheckpointState;

	// the date when we completed the last checkpoint
	private float lastCheckpointDate = -1.0f;

	// indicates whether or not we started a checkpoint yet
	private bool checkpointStarted = false;

	// indicates whether or not we completed the gesture
	private bool gestureEnded = false;

	// the current checkpoint we're at
	private int currentCheckpoint = 0;


	public void Update(State state, Vector4[] deltaPos) {
		State.JointTransform[] joints = state.getJoints();
		State.JointTransform joint;
		Vector4 diff;

		// indicates whether or not we are at the beginning of the first checkpoint
		bool initialPosition = currentCheckpoint == 0;

		// indicates whether or not we are at the end of the current checkpoint
		bool finalPosition = true;

		// compares the position of the joints that define the gesture with the current state
		foreach (GestureAnalyzer.JointMovement mv in Movement.checkpoints[currentCheckpoint].jointMovements) {
			joint = joints[mv.joint];

			if (initialPosition) {
				diff = joint.position - mv.initialPosition;

				// if the position aren't roughtly the same, then we're not at the position we want
				if (diff.magnitude > 0.15) {
					initialPosition = false;
				}
			}

			if (finalPosition) {
				diff = joint.position - mv.finalPosition;

				// if the position aren't roughtly the same, then we're not at the position we want
				if (diff.magnitude > 0.15) {
					finalPosition = false;
				}
			}
		}

		if (initialPosition) {
			// the gesture begins
			lastCheckpointDate = Time.time;
			lastCheckpointState = state;
			checkpointStarted = true;
		}
		else if (checkpointStarted && Time.time > lastCheckpointDate + Movement.checkpoints[currentCheckpoint].duration * 1.10f) {
			// the gesture timed out
			checkpointStarted = false;
			currentCheckpoint = 0;

			if (IsRepetitive && gestureEnded) {
				// if it was a repetitive gesture that was pending, we stop it
				if (Listener != null) {
					Listener.OnRelease(this);
				}

				gestureEnded = false;
			}

			return;
		}

		if (IsRepetitive) {
			// the gesture is a repetitive gesture

			if (finalPosition) {
				if (checkpointStarted && checkGesturePercents(state)) {
					// we are at the final position of the checkpoint and the movements were validated

					if (currentCheckpoint + 1 != Movement.checkpoints.Count) {
						// if there are more checkpoints, we move to the next one
						lastCheckpointDate = Time.time;
						lastCheckpointState = state;
						currentCheckpoint++;
					}
					else {
						// gesture done, start over

						if (! gestureEnded) {
							// gesture done for the first time
							if (Listener != null) {
								Listener.OnComplete(this);
							}

							gestureEnded = true;
						}
						else if (Listener != null) {
							Listener.OnRecomplete(this);
						}

						lastCheckpointDate = Time.time + 0.5f; // gives the user a half second margin to go back to the initial position
						lastCheckpointState = state;
						currentCheckpoint = 0;
					}
				}
			}
		}
		else {
			// the gesture is not a repetitive gesture

			if (finalPosition) {
				if (!gestureEnded && checkpointStarted && checkGesturePercents(state)) {
					// we are at the final position of the checkpoint and the movements were validated

					if (currentCheckpoint + 1 != Movement.checkpoints.Count) {
						// if there are more checkpoints, we move to the next one
						lastCheckpointDate = Time.time;
						lastCheckpointState = state;
						currentCheckpoint++;
					}
					else {
						// gesture done
						gestureEnded = true;
						checkpointStarted = false;

						if (Listener != null) {
							Listener.OnComplete(this);
						}
					}
				}
			}
			else if (gestureEnded) {
				// the user's moved from the final position after having completed the gesture
				checkpointStarted = false;
				currentCheckpoint = 0;
				gestureEnded = false;

				if (Listener != null) {
					Listener.OnRelease(this);
				}
			}
		}
	}


	/*
	 * checks (if needed) if the joints that aren't contributing to the gesture haven't moved during the gesture
	 */
	private bool checkGesturePercents(State state) {
		if (IgnoreOtherJoints) {
			return true;
		}

		State.JointTransform[] currJoints = state.getJoints();
		State.JointTransform[] prevJoints = lastCheckpointState.getJoints();
		
		float[] magnitudes = new float[KinectWrapper.Constants.BODY_PARTS_COUNT];
		float totalMagnitude = 0.0f;
		int i;

		// computes the movement magnitude of each joint
		for (i = 0; i < magnitudes.Length; i++) {
			magnitudes[i] = (currJoints[i].position - prevJoints[i].position).magnitude;
		}

		// marks the joints of the movement, and computes the total magnitude
		foreach (GestureAnalyzer.JointMovement mv in Movement.checkpoints[currentCheckpoint].jointMovements) {
			totalMagnitude += magnitudes[mv.joint];
			magnitudes[mv.joint] *= -1.0f;
		}

		for (i = 0; i < magnitudes.Length; i++) {
			// if the joint moved (and is not on the gesture movements) more than 5% of the total gesture movements
			// we invalidate the gesture
			if (magnitudes[i] >= GestureAnalyzer.Margin(i, Movement.checkpoints[currentCheckpoint].duration) && magnitudes[i] / totalMagnitude > 0.05f) {
				return false;
			}
		}

		return true;
	}
}
