using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {
	
	private Vector3 lastVelocity;
	
	private GameObject[] helices;
	
	void Start() {
		
		lastVelocity = Vector3.zero;
		helices = new GameObject[4];
		helices = GameObject.FindGameObjectsWithTag("Helice");
	}

	void Update() {
		//transform.Rotate(2, 0, 0);
		//transform.Rotate(Vector3.right * Time.deltaTime * 100);
	}

	void FixedUpdate () {
		
		//Vector3 speedVector = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z);
		
		Vector3 acceleration = rigidbody.velocity - lastVelocity;
		lastVelocity = rigidbody.velocity;
		
		
		/*if(speedVector.magnitude!=3)
			transform.Rotate(rigidbody.velocity.y, -rigidbody.velocity.x, 0);
			
		else {
			Quaternion deltaRotation = Quaternion.Euler(new Vector3(-rigidbody.rotation.eulerAngles.x,-rigidbody.rotation.eulerAngles.y,0) * Time.deltaTime * 100);
			rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);			
			//rigidbody.MoveRotation(Quaternion.identity);
			
			//rigidbody.rotation.eulerAngles.x;
			
			//transform.rotation.x = Mathf.Lerp(transform.rotation.x, transform.rotation.x + 1, Time.deltaTime);
		}*/
		//Quaternion deltaRotation = Quaternion.Euler(new Vector3(speedVector.x,speedVector.y,0) * Time.deltaTime * 100);
		//rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
		
		
		if(acceleration.magnitude > 0) {
		
			for (var i = 0; i<helices.Length; i++)
			{
				helices[i].transform.rotation = helices[i].transform.rotation * Quaternion.Euler( 25, 0, 0 );
			}
		}
		else {
			
			for (var i = 0; i<helices.Length; i++)
			{
				helices[i].transform.rotation = helices[i].transform.rotation * Quaternion.Euler( 10, 0, 0 );
			}
		}
		
		/*if(acceleration.magnitude > 0)
			//transform.rotation = Quaternion.identity * Quaternion.Euler( speedVector );
			transform.Rotate( acceleration.y * 10, -acceleration.x * 10, 0 );
		else
			transform.Rotate( -acceleration.y * 10, acceleration.x * 10, 0);
			//transform.rotation = Quaternion.identity;
			
		//print(acceleration.y);*/
		
		if(Input.GetKey(KeyCode.Space)) {
			rigidbody.AddForce(0,30,0);
			//rigidbody.AddRelativeForce(0,0,-10);
		}
		if(Input.GetKey(KeyCode.C)) {
			rigidbody.AddForce(0,-30, 0);
			//rigidbody.AddRelativeForce(0,0,10);
		}
	}
}
