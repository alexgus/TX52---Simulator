using UnityEngine;
using System.Collections;

public class Boid3D_2 : MonoBehaviour{
	
	private int boidsNumber;
	public GameObject[] boidsList;
	public GameObject goal;
	public float speed;
	//public GameObject boid;
	
	
	// Use this for initialization
	void Start () {
		boidsList = GameObject.FindGameObjectsWithTag ("Drone");
		goal = GameObject.FindGameObjectWithTag ("Center");
		
	}
	
	// Update is called once per frame
	void Update () {

		move_boid_to_new_positions ();
		
		
	}
	
	//
	private void move_boid_to_new_positions(){
		Vector3 v1 = new Vector3();
		Vector3 v2 = new Vector3();
		Vector3 v3 = new Vector3();
		Vector3 direction = new Vector3 ();
		
		//boidsList = GameObject.FindGameObjectsWithTag ("Boid");
		v1 = rule1 ();
		v2 = rule2 ();
		v3 = rule3 ();
		Vector3 vectorBoids = v1 + v2 + v3;
		//vectorBoids.Normalize();
		direction = goTo();

		//this.rigidbody.velocity = new Vector3((this.rigidbody.velocity.x + vectorBoids.x + direction.x)*Time.deltaTime*speed, (this.rigidbody.velocity.y + vectorBoids.y + direction.y)*Time.deltaTime*speed, (this.rigidbody.velocity.z + vectorBoids.z + direction.z)*Time.deltaTime*speed);
		this.rigidbody.AddForce ((vectorBoids + direction)*speed);



		//this.transform.position = this.transform.position + this.rigidbody.velocity;


	}
	
	
	private Vector3 rule1(){
		Vector3 pcj = new Vector3(0,0,0);
		//boidsList = GameObject.FindGameObjectsWithTag ("Boid");
		
		foreach(GameObject b in boidsList)
		{
			if(b != this && Vector3.Distance(b.transform.position, this.transform.position) < 1){
				pcj.x = pcj.x + b.transform.position.x;
				pcj.y = pcj.y + b.transform.position.y;
				pcj.z = pcj.z + b.transform.position.z;
			}
			boidsNumber = boidsNumber +1;
		}
		pcj = pcj / (boidsNumber-1);
		
		Vector3 finalVector = new Vector3 (0, 0, 0);
		finalVector.x = (pcj.x - this.transform.position.x) / 100;
		finalVector.y = (pcj.y - this.transform.position.y) / 100;
		finalVector.z = (pcj.z - this.transform.position.z) / 100;
		return finalVector;
	}
	
	private Vector3 rule2(){
		Vector3 c = new Vector3 (0, 0, 0);
		//boidsList = GameObject.FindGameObjectsWithTag ("Boid");
		
		foreach (GameObject b in boidsList) {
			if (b != this) 
			{
				Vector3 differenceVector = new Vector3();
				differenceVector = b.transform.position - this.transform.position;
				float distance = differenceVector.magnitude;
				//if ((b.transform.position.magnitude - bj.transform.position.magnitude) < 1)
				float randDist = Random.Range(2f, 5f);
				if(distance < randDist)
				{
					c.x = c.x - (b.transform.position.x - this.transform.position.x);
					c.y = c.y - (b.transform.position.y - this.transform.position.y);
					c.z = c.z - (b.transform.position.z - this.transform.position.z);
				}
			}
		}
		return c;
	}
	
	private Vector3 rule3()
	{
		Vector3 pvj = new Vector3 (0, 0, 0);
		//boidsList = GameObject.FindGameObjectsWithTag ("Boid");
		
		foreach (GameObject b in boidsList) {
			if (b != this) {
				pvj.x = pvj.x + b.rigidbody.velocity.x;
				pvj.x = pvj.y + b.rigidbody.velocity.y;
				pvj.z = pvj.z + b.rigidbody.velocity.z;
			}
			boidsNumber = boidsNumber + 1;
		}
		
		pvj = pvj / (boidsNumber - 1);
		
		Vector3 finalVector = new Vector3 (0, 0, 0);
		finalVector.x = (pvj.x - this.rigidbody.velocity.x) / 8;
		finalVector.y = (pvj.y - this.rigidbody.velocity.y) / 8;
		finalVector.z = (pvj.z - this.rigidbody.velocity.z) / 8;
		
		return finalVector;
	}
	
	private Vector3 goTo()
	{
		Vector3 direction = new Vector3 (goal.transform.position.x - this.transform.position.x, goal.transform.position.y - this.transform.position.y, goal.transform.position.z - this.transform.position.z);
		direction.Normalize();
		return direction;
	}
}
