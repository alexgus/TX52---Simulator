using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	private const string typeName = "TX_Drone";
	private string gameName = "Room";
	public GameObject playerPrefab;
	private HostData[] hostList;
	private NetworkView localNetworkview;
	private Hashtable listPlayers = new Hashtable();
	private Hashtable listPlayerNetworkView = new Hashtable();

	// Lancé avec l'application
	void Start () {
		int i = Random.Range (0, 255);
		gameName = "Room " + i.ToString();
	}
	
	void OnGUI(){
		int i = 0;
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (0, 100, 100, 50), "Start Server"))
				StartServer ();
				if (GUI.Button (new Rect (100, 100, 100, 50), "Refresh Hosts")) {
					RefreshHostList ();
				}
			if (hostList != null) {
					for (i = 0; i < hostList.Length; i++) {
					if (GUI.Button (new Rect (200, 100 + (210 * i), 100, 50), hostList [i].gameName))
						JoinServer (hostList [i]);
					}
				}
			}
	}

	// Update is called once per frame
	void Update () {	}

	// Demande de rafraichissement de la liste des salles ouvertes pour l'application
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	// Executé par tout le monde
	// Traitement des Evenements avec le master server
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		//Récupération de la liste des serveurs / salles ouvertes pour l'application
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	// Executé par le serveur
	// Préviens le MasterServer de la création de la salle
	private void StartServer()
	{
		// 10 Personnes max, port 25000
		Network.InitializeServer(10, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		//MasterServer.ipAddress = "127.0.0.1"; // Lancement local
	}

	// Executé par le serveur
	// Appelé au lancement du serveur
	void OnServerInitialized()
	{
		SpawnPlayer(Network.player);
	}
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	// Executé par le client
	// Appelé lors que le client se connecte au serveur
	void OnConnectedToServer()
	{

	}

	public void SpawnPlayer(NetworkPlayer player)
	{
		Vector3 temp = Vector3.zero;
		temp.x = Random.Range(-25, 25);
		temp.y = Random.Range(7, 15);
		temp.z = Random.Range(-25, 25);
		transform.position = temp;

		GameObject newPlayerTransform = (GameObject)Network.Instantiate(playerPrefab, temp, transform.rotation, 0);
		//newPlayerTransform.tag = "Player";
	
		NetworkView theNetworkView = newPlayerTransform.networkView;
		string name = "Toto "+ Random.Range (0, 50);
		theNetworkView.RPC("SetPlayer", RPCMode.AllBuffered, player, name);
		if (Network.player == player) {
			localNetworkview = theNetworkView;
			PlayersList.view = (CameraObjectServer) newPlayerTransform.GetComponent<CameraObjectServer>();
		}
		listPlayers.Add (player, newPlayerTransform);
		listPlayerNetworkView.Add (player, theNetworkView);
	}
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Clean up after " + player.ipAddress);
		GameObject obj = (GameObject)listPlayers [player]; 
		NetworkView objnet = (NetworkView)listPlayerNetworkView [player];

		objnet.RPC ("disconnected", RPCMode.AllBuffered);
		
		GameObject.Destroy (obj);
		listPlayerNetworkView.Remove (player);
		listPlayers.Remove(player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);

	}
	// Executé par le serveur
	// Appelé lors qu'un nouveau joueur se connecte au serveur
	void OnPlayerConnected(NetworkPlayer aPlayer)
	{
		SpawnPlayer(aPlayer);
	}

}