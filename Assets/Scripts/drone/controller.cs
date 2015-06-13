using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using moteur;

public enum CommandeDrone { Haut, Bas, Droite, Gauche, Descendre, Monter, Stabilise, RotationDroite, RotationGauche, Vecteur};

public class controller : MonoBehaviour {

	private int vitesse = 0;
	private GameObject[] helices;
	private GameObject[] supports;
	private GameObject leader;

	private moteur[] moteurDrone = new moteur[4];
	private regulateur reguleDrone;

	public Text vitesseDisplay;
	public Text hauteurDsiplay;

	
	public int portNumber = 8235;
	private bool connected = false;

	public bool controlleWithVecteur = false;

	//private Vector3 testAffiche;

	private void OnConnectedToServer(){
		connected = true;
	}
	
	private void OnServerInitialized(){
		connected = true;
	}
	
	private void OnDisconnectedFromServer(){
		connected = false;
	}


	// Use this for initialization
	//http://www.droneaddict.net/comment-ca-vole/#Rsum

	private void OnGUI(){
		
		if (!connected) {
			GUILayout.Label ("Création du server ....  ");
		} else {
			GUILayout.Label ("Connections : " + Network.connections.Length.ToString ());
		}
		
	}

	void Start () {

		if (!connected) {
			Network.InitializeServer (4, portNumber, false);
			print ("ok");
		}

		vitesse = 0;
		helices = GameObject.FindGameObjectsWithTag("helice");
		supports = GameObject.FindGameObjectsWithTag("support");
		leader = GameObject.FindGameObjectWithTag("leader");
		vitesseDisplay.text = "Vitesse actuel : " + vitesse;
		hauteurDsiplay.text = "Hauteur voulue : " + vitesse;

		moteurDrone [0] = new moteur ("One", 0, supports [0],helices[0], Vector3.down);
		moteurDrone [1] = new moteur ("Two", 0, supports [1],helices[1], Vector3.up);
		moteurDrone [2] = new moteur ("Three", 0, supports [2],helices[2], Vector3.right);
		moteurDrone [3] = new moteur ("Four", 0, supports [3],helices[3], Vector3.left);

		reguleDrone = new regulateur (leader, moteurDrone);

	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		vitesseDisplay.text = "Vitesse actuel " + reguleDrone.getVitesse();
		hauteurDsiplay.text = "Hauteur voulue " + reguleDrone.getHauteur();


		reguleDrone.setCommande(CommandeDrone.Stabilise);
		reguleDrone.setControllerWithVecteur (controlleWithVecteur);

		//OTHERS keys
		if (Input.GetKey (KeyCode.UpArrow)) {
			reguleDrone.setCommande(CommandeDrone.Haut);
		}
		
		if(Input.GetKey(KeyCode.DownArrow)) {
			reguleDrone.setCommande(CommandeDrone.Bas);
		}
		
		if(Input.GetKey(KeyCode.LeftArrow)) {
			reguleDrone.setCommande(CommandeDrone.Gauche);
		}
		
		if(Input.GetKey(KeyCode.RightArrow)) {
			reguleDrone.setCommande(CommandeDrone.Droite);
		}

		if (Input.GetKey (KeyCode.B)) {
			reguleDrone.setCommande(CommandeDrone.Monter);
		}
	
		if (Input.GetKey (KeyCode.V)) {
			reguleDrone.setCommande(CommandeDrone.Descendre);
		}

		if (Input.GetKey (KeyCode.Q)) {
			reguleDrone.setCommande(CommandeDrone.RotationGauche);
		}
		
		if (Input.GetKey (KeyCode.D)) {
			reguleDrone.setCommande(CommandeDrone.RotationDroite);
		}


			movement ();


	}

	void movement(){

		reguleDrone.live ();
		/*print ("rotation cos(x) : " + (1+ (1 - Mathf.Cos(leader.transform.eulerAngles.z * Mathf.Deg2Rad))));
		print ("angle en radian : " + leader.transform.eulerAngles.z * Mathf.Deg2Rad);*/
		print ("commande : " + reguleDrone.getCommande());
	}

	[RPC]
	private void SendHauteur(int one){

		switch (one) {
		case 6:
			reguleDrone.setCommande(CommandeDrone.Monter);
		break;

		case 0:
			reguleDrone.setCommande(CommandeDrone.Stabilise);
		break;

		case -6:
			reguleDrone.setCommande(CommandeDrone.Descendre);
		break;

		}
		reguleDrone.setvitesseToUp (one);
	}
	
	[RPC]
	public void setMove(Vector3 mouvement){
		float tmp = mouvement.y;
		mouvement.y = 0;
		mouvement.z = tmp;
		//testAffiche = mouvement;
		reguleDrone.prepareMove (mouvement.x*30.0F, mouvement.z*-30.0F);
		//reguleDrone.setMoveWithVecteur (mouvement);
	}
	

}
