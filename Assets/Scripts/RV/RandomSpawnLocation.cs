using UnityEngine;
using System.Collections;

public class RandomSpawnLocation : MonoBehaviour {

	// Use this for initialization
	void Start () {

		/*transform.position.x = ;
		transform.position.y = ;
		transform.position.z = ;*/
		Vector3 temp = transform.position;
		temp.x = Random.Range(-25, 25);
		temp.y = Random.Range(7, 15);
		temp.z = Random.Range(-25, 25);
		transform.position = temp;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
