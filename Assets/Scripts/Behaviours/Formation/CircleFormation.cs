using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleFormation : MonoBehaviour {
	
	private GameObject center;
	
	public float droneSpeed = 10;
	
	private float radius; // = 10f;
	
	private float angleBetweenDrone;
	private float distanceBetweenDrone;
	
	List<GameObject> drones = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		center = GameObject.FindGameObjectWithTag("Center");
	
		drones.AddRange( GameObject.FindGameObjectsWithTag("Drone") );
		
		radius = drones.Count * 1.25f;
		
		float squareRadius = radius * radius;
		
		angleBetweenDrone = 360 / drones.Count;
		distanceBetweenDrone = Mathf.Sqrt(squareRadius*2 - squareRadius*2*Mathf.Cos(angleBetweenDrone)); 


	}
	
	// Update is called once per frame
	void Update () {
		Vector3 goal = center.transform.position;
		
		float distanceToGoal = Vector3.Distance(goal, rigidbody.position); // distance to goal
		
		Vector3 direction = goal - rigidbody.position;
		
		// Kinematic
		/*if(distanceToGoal > radius)
			rigidbody.velocity = direction;
		else if (distanceToGoal < radius)
			rigidbody.velocity = direction * -1;*/
			
		// Steering
		if(distanceToGoal > radius)
			rigidbody.AddForce(direction.normalized * droneSpeed);
		else if (distanceToGoal < radius)
			rigidbody.AddForce(direction.normalized * droneSpeed * -1);

		GameObject nearestDrone = null;
		float nearestDroneDistance = Mathf.Infinity;
		/*GameObject secondNearestDrone = null;
		float secondNearestDroneDistance = Mathf.Infinity;*/
			
		foreach (GameObject d in drones) {
			if(d != this.gameObject) {
				float distanceToDrone = Vector3.Distance(d.transform.position, rigidbody.position); // distance to other drone
				//Debug.Log(distanceToDrone);
				
				if(distanceToDrone < nearestDroneDistance) {
					/*secondNearestDrone = nearestDrone;
					secondNearestDroneDistance = nearestDroneDistance;*/
					nearestDrone = d;
					nearestDroneDistance = distanceToDrone;
				}
			}				
		}
		
		/*Debug.Log("D1 : "+nearestDroneDistance+" D2 : "+secondNearestDroneDistance);
		
		if(nearestDroneDistance != secondNearestDroneDistance) {
			rigidbody.velocity += (secondNearestDrone.rigidbody.position - rigidbody.position).normalized;
		}*/
		
		Vector3 direction2 = new Vector3(0f,0f);
		
		if(nearestDrone != null)
			direction2 = nearestDrone.rigidbody.position - rigidbody.position;
		
		// Kinematic
		/*if(nearestDroneDistance > distanceBetweenDrone)
			rigidbody.velocity += direction2 * 2f;
		else if (nearestDroneDistance < distanceBetweenDrone)
			rigidbody.velocity += direction2 * 2f * -1;*/
			
		// Steering
		if(nearestDroneDistance > distanceBetweenDrone)
			rigidbody.AddForce(direction2.normalized * droneSpeed * 0.5f);
		else if (nearestDroneDistance < distanceBetweenDrone)
			rigidbody.AddForce(direction2.normalized * droneSpeed * 0.5f * -1);
	}
}
