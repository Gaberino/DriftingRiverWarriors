using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LakeGameManager : MonoBehaviour {

	public static LakeGameManager instance;
	public int enemyLayer;

	public bool slowTime = false;

	public Transform PlayerTransform;
	private Player thePlayer;

	public Image wetnessBar;
	private Vector2 originalBarSize;

	public Text RoundInfoText;
	public Text ammoText;
	public int ammoNum = 0;
	public Text gameOverText;
	public Text startText;
	public Text scoreText;

	public int gameState = 0; //o is not started, 1 is during, 2 is end

	public float score;
	private int instanceTopScore = 0;

	//wave system
	public int roundNum = 0;
	public float waveIntermissionTime = 3f;
	private float intermissionTimer = 0f;

	public int perRoundEnemyIncrease = 2;
	public int maxEnemiesAtOnce = 10;

	public float enemySpawnInterval = 1f;
	private float spawnTimer = 0f;

	bool preppingNextRound = true;

	private int enemiesToSpawnThisRound = 0;

	public float currentGameTimeElapsed;

	public GameObject[] SpawnableEnemies;
	GameObject[] logs;

	// Use this for initialization
	void Start () {
		instance = this;
		thePlayer = PlayerTransform.GetComponent<Player> ();
		thePlayer.enabled = false;

		originalBarSize = wetnessBar.rectTransform.sizeDelta;
		logs = GameObject.FindGameObjectsWithTag ("Log");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if (gameState == 0) {
			//do something to start the game
			if (InControl.InputManager.ActiveDevice.AnyButton) {
				startText.transform.parent.gameObject.SetActive (false);
				gameState = 1;
				thePlayer.enabled = true;
			}
			//Debug.Log(slowTime);
		}

		else if (gameState == 1) {
			currentGameTimeElapsed += Time.deltaTime;

			if (!thePlayer.enabled) thePlayer.enabled = true;
			//Always active game stuff:
			//drowning display
			wetnessBar.rectTransform.sizeDelta = new Vector2(originalBarSize.x * (thePlayer.wetness / thePlayer.myBrain.timeToDrown), originalBarSize.y);
			//Debug.Log(originalBarSize);

			//update displays with info
			scoreText.text = "Score " + Mathf.RoundToInt (score);
			ammoText.text = "Ammo: " + ammoNum;

			//roundstart
			if (preppingNextRound) {
				if (roundNum == 0) { //just say to start
					RoundInfoText.text = "GAME START";

					if (intermissionTimer > waveIntermissionTime) {
						roundNum += 1;
						//set number of enemies
						enemiesToSpawnThisRound = roundNum * perRoundEnemyIncrease;

						//end intermission
						RoundInfoText.text = "";
						spawnTimer = enemySpawnInterval; //1 immediate spawn
						preppingNextRound = false;
					}
				} else {
					RoundInfoText.text = "ROUND " + roundNum + " OVER. PREPARE YOURSELF";

					if (intermissionTimer > waveIntermissionTime) {
						roundNum += 1;
						//enque enemies
						enemiesToSpawnThisRound = roundNum * perRoundEnemyIncrease;

						//end intermission
						RoundInfoText.text = "";
						spawnTimer = enemySpawnInterval; //1 immediate spawn
						preppingNextRound = false;
					}
				}


				intermissionTimer += Time.deltaTime;
			}


			//spawning
			else if (!preppingNextRound && enemiesToSpawnThisRound > 0) {
				spawnTimer += Time.deltaTime;

				if (spawnTimer > enemySpawnInterval && GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemiesAtOnce) {
					//spawn
					SpawnEnemy();
					enemiesToSpawnThisRound -= 1;
				}
			}

			//check for next round start
			else if (!preppingNextRound && enemiesToSpawnThisRound == 0 && GameObject.FindGameObjectWithTag("Enemy") == null) {
				intermissionTimer = 0;

				preppingNextRound = true;
			}
		}

		else if (gameState == 2) {
			//do something to restart the game
			if (Mathf.RoundToInt(score) > instanceTopScore) instanceTopScore = Mathf.RoundToInt(score);
			gameOverText.text = "Sleep with the fishes pal. Your score was: " + Mathf.RoundToInt(score) + "\n Press X if you want to try again \n High Score: " + instanceTopScore;
			gameOverText.gameObject.SetActive(true);
			if (InControl.InputManager.ActiveDevice.AnyButton.IsPressed) {
				ResetGame ();
			}
		}
	}

	void SpawnEnemy(){
		Vector3 spawnPos = logs [Random.Range (0, logs.Length)].transform.position;

		GameObject newEnemy = Instantiate (SpawnableEnemies [Random.Range (0, SpawnableEnemies.Length)], spawnPos, Quaternion.identity);
		spawnTimer = 0f;

	}

	void ResetGame () {
		//kill all enemies
		GameObject[] enemiesToKill = GameObject.FindGameObjectsWithTag("Enemy");
		for (int i = 0; i < enemiesToKill.Length; i++){
			Destroy(enemiesToKill[i]);
		}


		intermissionTimer = waveIntermissionTime;

		preppingNextRound = true;

		score = 0;
		PlayerTransform.position = Vector3.zero;
		thePlayer.wetness = 0;
		thePlayer.velocity = Vector2.zero;
		thePlayer.stunned = false;
		thePlayer.currentWeapon.ammo = thePlayer.currentWeapon.startingAmmo - 1;
		thePlayer.currentWeapon.ReplenishAmmo (1);
		gameOverText.gameObject.SetActive(false);
		thePlayer.enabled = true;
		roundNum = 0;
		gameState = 1;
	}
}
