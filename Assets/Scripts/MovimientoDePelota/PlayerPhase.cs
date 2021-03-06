﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerPhase : MonoBehaviour {

	public GameObject Player1, Player2, Player3;
	GameObject currentPlayer;
	public Canvas shootUI;
	public Canvas Leader;
	public Slider fuerzaSlider;
	public Camera cam;
	public KeyCode Right;
	public KeyCode Left;
	public KeyCode Up;
	public KeyCode Down;
	public KeyCode Go;
	public KeyCode NoGo;

	public GameObject Arrow;

	public int phase;
	Vector3 direccion;
	Vector3 CamDir;
	public float camvel;
	public float offsetCamMag;
	float tiroFuerza;
	int fuerzaSliderDir = 1;
	public float yOffset;
	bool playerChanger;

	// Use this for initialization
	void Start () {
		Leader.enabled = false;
		currentPlayer = Player1;
		direccion = Vector3.left;
		CamDir = direccion;
		phase = 0;
		playerChanger = false;
		Player1.GetComponent<ExplosionBallScript> ().StartTurn ();
		Player1.GetComponent<AllBallsNeedThis> ().isWating = false;
		Player2.GetComponent<AllBallsNeedThis> ().isWating = true;
		Player3.GetComponent<AllBallsNeedThis> ().isWating = true;
	}

	// Update is called once per frame
	void Update () {
		/*if(Player1.GetComponent<Rigidbody> ().velocity == Vector3.zero)
			print ("Player 1 veolciti: " + Player1.GetComponent<Rigidbody> ().velocity);*/

		//print ("p1: " + Player1.GetComponent<Rigidbody> ().velocity + " p2: " + Player2.GetComponent<Rigidbody> ().velocity + " p3: " + Player3.GetComponent<Rigidbody> ().velocity );
		cam.transform.position = currentPlayer.transform.position - new Vector3 (CamDir.x, CamDir.y + yOffset, CamDir.z) * offsetCamMag;
		cam.transform.LookAt (currentPlayer.transform.position);
		switch (phase) {
		case 0:
			shootUI.enabled = false;

			if (!Arrow.activeSelf)
				Arrow.SetActive (true);

			Arrow.transform.position = currentPlayer.transform.position + direccion * 2;
			Arrow.transform.rotation.SetFromToRotation (currentPlayer.transform.position, Arrow.transform.position);

			Arrow.transform.rotation.eulerAngles.Set (0f, cam.transform.rotation.eulerAngles.y, 0f);
			if (Input.GetKey (Left)) {
				direccion = Quaternion.AngleAxis (camvel * Time.deltaTime, Vector3.up) * direccion;
				CamDir = Quaternion.AngleAxis (camvel * Time.deltaTime, Vector3.up) * CamDir;

			}
			if (Input.GetKey (Right)) {
				direccion = Quaternion.AngleAxis (-camvel * Time.deltaTime, Vector3.up) * direccion;
				CamDir = Quaternion.AngleAxis (-camvel * Time.deltaTime, Vector3.up) * CamDir;
			}
			if (Input.GetKey (Up)) {
				CamDir = Quaternion.AngleAxis (camvel * Time.deltaTime, Vector3.Cross (Vector3.up, CamDir)) * CamDir;
			}
			if (Input.GetKey (Down)) {
				CamDir = Quaternion.AngleAxis (-camvel * Time.deltaTime, Vector3.Cross (Vector3.up, CamDir)) * CamDir;
			}
			if (Input.GetKeyDown (Go)) {
				phase = 1;
			}
			break;
		case 1:
			
			shootUI.enabled = true;
			fuerzaSlider.value += fuerzaSliderDir * currentPlayer.GetComponent<AllBallsNeedThis> ().PresicionPersonaje * Time.deltaTime;
			if (fuerzaSlider.value >= 1)
				fuerzaSliderDir = -1;

			if (fuerzaSlider.value <= 0)
				fuerzaSliderDir = 1;

			if (Input.GetKeyDown (Go)) {
				phase = 2;
			}
			if (Input.GetKeyDown (NoGo)) {
				phase = 0;
			}
			break;	
		case 2:
			if (Arrow.activeSelf)
				Arrow.SetActive (false);

			if (!playerChanger) {
				float levantamiento = fuerzaSlider.value - 0.5f;
				if (levantamiento <= 0)
					levantamiento = 0;
				cam.gameObject.GetComponent<Leaderboard> ().SumarPuntos (currentPlayer);
				currentPlayer.GetComponent<AllBallsNeedThis> ().lastStart.position = currentPlayer.transform.position;
				currentPlayer.GetComponent<Rigidbody> ().AddForce ((direccion + new Vector3(0f, currentPlayer.GetComponent<AllBallsNeedThis>().floorGluines * levantamiento, 0f)) * fuerzaSlider.value * currentPlayer.GetComponent<AllBallsNeedThis> ().FuerzaPersonaje, ForceMode.Impulse);
				StartCoroutine (changePlayer ());
			}
			if (Input.GetKey (Left)) {
				direccion = Quaternion.AngleAxis (camvel * Time.deltaTime, Vector3.up) * direccion;
				CamDir = Quaternion.AngleAxis (camvel * Time.deltaTime, Vector3.up) * CamDir;

			}
			if (Input.GetKey (Right)) {
				direccion = Quaternion.AngleAxis (-camvel * Time.deltaTime, Vector3.up) * direccion;
				CamDir = Quaternion.AngleAxis (-camvel * Time.deltaTime, Vector3.up) * CamDir;
			}
			if (Input.GetKey (Up)) {
				CamDir = Quaternion.AngleAxis (camvel * Time.deltaTime, Vector3.Cross (Vector3.up, CamDir)) * CamDir;
			}
			if (Input.GetKey (Down)) {
				CamDir = Quaternion.AngleAxis (-camvel * Time.deltaTime, Vector3.Cross (Vector3.up, CamDir)) * CamDir;
			}
			//print (currentPlayer.GetComponent<Rigidbody> ().velocity);
			break;
		}//fin switch

		if(Input.GetKey (KeyCode.Tab))
		{
			Leader.enabled = true;
		}
		if(Input.GetKeyUp (KeyCode.Tab))
		{
			Leader.enabled = false;
		}
	}

	IEnumerator changePlayer(){
		playerChanger = true;
		yield return new WaitForSeconds (1f);
		while (currentPlayer.GetComponent<Rigidbody> ().velocity != Vector3.zero) {
			yield return null;
		}
		if (currentPlayer == Player1) {
			if (!Player2.GetComponent<AllBallsNeedThis> ().done) {
				currentPlayer = Player2;
				Player2.GetComponent<BlinkBallScript> ().StartTurn();
				Player1.GetComponent<ExplosionBallScript> ().notMyTurn ();
				Player1.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player2.GetComponent<AllBallsNeedThis> ().isWating = false;
				Player3.GetComponent<AllBallsNeedThis> ().isWating = true;
			} else if(!Player3.GetComponent<AllBallsNeedThis>().done){
				currentPlayer = Player3;
				Player3.GetComponent<GravityBallScript> ().StartTurn();
				Player1.GetComponent<ExplosionBallScript> ().notMyTurn ();
				Player1.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player2.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player3.GetComponent<AllBallsNeedThis> ().isWating = false;
			}
		}else if (currentPlayer == Player2) {
			if (!Player3.GetComponent<AllBallsNeedThis> ().done) {
				currentPlayer = Player3;
				Player3.GetComponent<GravityBallScript> ().StartTurn();
				Player1.GetComponent<ExplosionBallScript> ().notMyTurn ();
				Player1.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player2.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player3.GetComponent<AllBallsNeedThis> ().isWating = false;
			} else if(!Player1.GetComponent<AllBallsNeedThis>().done){
				currentPlayer = Player1;
				Player1.GetComponent<ExplosionBallScript> ().StartTurn();
				Player1.GetComponent<AllBallsNeedThis> ().isWating = false;
				Player2.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player3.GetComponent<AllBallsNeedThis> ().isWating = true;
			}
		}else if (currentPlayer == Player3) {
			if (!Player1.GetComponent<AllBallsNeedThis> ().done) {
				currentPlayer = Player1;
				Player1.GetComponent<ExplosionBallScript> ().StartTurn();
				Player1.GetComponent<AllBallsNeedThis> ().isWating = false;
				Player2.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player3.GetComponent<AllBallsNeedThis> ().isWating = true;
			} else if(!Player2.GetComponent<AllBallsNeedThis>().done){
				currentPlayer = Player2;
				Player2.GetComponent<BlinkBallScript> ().StartTurn();
				Player1.GetComponent<ExplosionBallScript> ().notMyTurn ();
				Player1.GetComponent<AllBallsNeedThis> ().isWating = true;
				Player2.GetComponent<AllBallsNeedThis> ().isWating = false;
				Player3.GetComponent<AllBallsNeedThis> ().isWating = true;
			}
		}
		phase = 0;
		playerChanger = false;
	}

	public void SwitchHoles(int wich){
		for(int i = 0; i < 10; i++){
			string numero;
			if(i < 10){
				numero = "0" + i.ToString();
			}else{
				numero = i.ToString();
			}
			string NombreObjeto = "Hole_" + numero;
			if (i == wich) {
				if(GameObject.Find (NombreObjeto) != null)
					GameObject.Find (NombreObjeto).GetComponentInChildren<LimitBoxes> ().currentHole = true;
			} else {
				if(GameObject.Find (NombreObjeto) != null)
					GameObject.Find (NombreObjeto).GetComponentInChildren<LimitBoxes> ().currentHole = false;
			}
		}
	}
}
