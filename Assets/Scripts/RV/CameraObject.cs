using UnityEngine;
using System.Collections;

public class CameraObject : MonoBehaviour {

	public NetworkPlayer netPlayer;
	public Vector3 position = Vector3.zero;
	public Quaternion rotation = Quaternion.identity;
	//public Transform target = Camera.main.transform;
	// Use this for initialization
	Vector3 lastPosition;
	float minimumMovement = .05f;
	void Start () {
	}
	void Update () {
		if (networkView.isMine)
		{
			Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			float speed = 5;
			transform.Translate(speed * moveDir * Time.deltaTime);
			if (Vector3.Distance(transform.position, lastPosition) > minimumMovement)
			{
				lastPosition = transform.position;
				networkView.RPC("SetPosition", RPCMode.Others, transform.position);
			}
		}
	}
	void Awake()
	{
		if (!networkView.isMine)
			enabled = false;
	}
	[RPC]
	void SetPosition(Vector3 newPosition)
	{
		transform.position = newPosition;
	}
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 pos = transform.position;
			stream.Serialize(ref pos);
		}
		else
		{
			Vector3 receivedPosition = Vector3.zero;
			stream.Serialize(ref receivedPosition);
			transform.position = receivedPosition;
		}
	}
}