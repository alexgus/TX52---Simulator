using UnityEngine;
using System.Collections;

public class CircleMovement : MonoBehaviour {

	float angle =0;
	public float speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
	public float radius=5;
	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}



	void Update()
	{
		angle += speed*Time.deltaTime; //if you want to switch direction, use -= instead of +=
		//position.x = Mathf.Cos(angle)*radius;
		//position.y = Mathf.Sin(angle)*radius;
		float distanceToTarget = Vector3.Distance(target.transform.position, rigidbody.position);
		if (distanceToTarget > radius-5 && distanceToTarget < radius+5)
			turnAround ();
		else
			findGoodPosition ();
		//this.transform.position = new Vector3(Mathf.Cos(angle)*radius + target.transform.position.x, 0, Mathf.Sin(angle)*radius + target.transform.position.z);
	}

	void findGoodPosition(){
		float distanceToTarget = Vector3.Distance(target.transform.position, rigidbody.position); // distance to goal
		Vector3 direction = target.transform.position - rigidbody.position;
		if(distanceToTarget > radius)
			rigidbody.AddForce(direction.normalized * speed);
		else if (distanceToTarget < radius)
			rigidbody.AddForce(direction.normalized * speed * -1);
		//Vector3 direction2 = new Vector3 (Mathf.Cos (angle) * radius, 0, Mathf.Sin (angle) * radius);
		//rigidbody.AddForce(direction2.normalized * speed);
	}

	void turnAround(){
		Vector3 direction = target.transform.position - rigidbody.position;
		Vector3 up = Vector3.up;
		Vector3 direction2 = Vector3.Cross(direction, up);
		//Vector3 direction2 = new Vector3 (Mathf.Cos (angle) * radius, 0, Mathf.Sin (angle) * radius);
		rigidbody.AddForce(direction2.normalized * speed);
	}
}
