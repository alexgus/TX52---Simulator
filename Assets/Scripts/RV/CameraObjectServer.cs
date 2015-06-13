using UnityEngine;
using System.Collections;

public class CameraObjectServer : MonoBehaviour {
	public NetworkPlayer theOwner;
	//float lastClientHInput = 0f;
	//float lastClientVInput = 0f;
	//float serverCurrentHInput = 0f;
	//float serverCurrentVInput = 0f;
	string m_NickName = "";
	int hashid;
	public bool followHim = false;


	Quaternion serverCurrentRotation = Quaternion.identity;
	// Use this for initialization
	void Start () {
	}
	void Awake()
	{

		if (Network.isClient)
			enabled = false;
	}
	public string getName()
	{
		return m_NickName;
	}
	void onGUI() {
	}
	public bool isLocal()
	{
		return (theOwner == Network.player);
	}
	[RPC]
	void disconnected()
	{
		Debug.LogError ("Le joueur " + getName () + " s'est déconnecté");
		this.tag = null;
		Destroy (gameObject);
		Network.RemoveRPCs(theOwner);
		Network.DestroyPlayerObjects(theOwner);
		CameraObjectServer.Destroy (this);

	}
	[RPC]
	void SetPlayer(NetworkPlayer player, string name)
	{
		theOwner = player;
		m_NickName = name;
		this.tag = "Player" ;
		if (player == Network.player) {
			enabled = true;
			followHim = true;
			PlayersList.view = this;
			//Debug.LogError ("Mon nom est "+ getName ());
		}
		/*else
			Debug.LogError ("Connexion de " + getName());*/
	}
	void OnGUI()
	{

	}
	void Update()
	{
			if (/*theOwner != null && */Network.player == theOwner) {
			//if (followHim){
			
				Vector3 newPos = this.transform.position;
				newPos.y = newPos.y - 2f;
				Camera.main.transform.position = newPos;
				this.transform.rotation = Camera.main.transform.rotation;		
				
				Quaternion r = this.transform.rotation;
				if (Network.isServer) {
						SendRotationInput (r);
			//			SendMovementInput (HInput, VInput);
				} else if (Network.isClient) {
			//			networkView.RPC ("SendMovementInput", RPCMode.Server, HInput, VInput);
						networkView.RPC ("SendRotationInput", RPCMode.Server, r);
				}
		}
		if(Network.isServer)
		{
			//Vector3 moveDirection = new Vector3(serverCurrentHInput, 0, serverCurrentVInput);
			//float speed = 5;
			//transform.Translate(speed * moveDirection * Time.deltaTime);
			this.transform.rotation = serverCurrentRotation;
		}

		if (followHim) {

			
			Vector3 newPos = this.transform.position;
			newPos.y = newPos.y - 2f;
			Camera.main.transform.position = newPos;
			Camera.main.transform.rotation = this.transform.rotation;

				}
	}
	[RPC]
	void SendMovementInput(float HInput, float VInput)
	{
		//serverCurrentHInput = HInput;
		//serverCurrentVInput = VInput;
	}
	[RPC]
	void SendRotationInput(Quaternion r)
	{
		serverCurrentRotation = r;
	}
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting)
		{
			
			Quaternion rot = this.transform.rotation;
			Vector3 position = this.transform.position;
			stream.Serialize(ref rot);
			stream.Serialize(ref position);
		}
		else
		{
			Quaternion rot = Quaternion.identity;
			Vector3 position = Vector3.zero;
			stream.Serialize(ref rot);
			stream.Serialize(ref position);
			this.transform.rotation = rot;
			this.transform.position = position;
		}
	}

}
