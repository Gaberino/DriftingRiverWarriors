using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager gm_Singleton;
	public int enemyLayer;

	public bool slowTime = false;

	public Transform PlayerTransform;
	private Player thePlayer;

	public Text infoText;
	public Text ammoText;
	public int ammoNum = 0;
	public Text gameOverText;
	public Text startText;
	public Text scoreText;

	public int gameState = 0; //o is not started, 1 is during, 2 is end

	public Transform killZone;//zone beneath player in which they will lose
	public float maxDistToKillzone;

	public Transform spawnZone;//zone above player at which things will spawn and drift down
	public float minSpawnZoneDistance;
	public float spawnZoneWidth;

	public float score;
	private int instanceTopScore = 0;

	public Vector2 minMaxSpawnTime;
	public Vector2 minMaxSpawnAmount;
	private float currentTimeTillSpawn = 0;
	private float currentSpawnTimer = 0;

	public float startingEnemyEncounterRate;
	private float enemyEncounterRate = 0f;
	public float enemyEncounterGrowthRate;
	public float enemyEncounterMaxRate;

	public float startingRiverSpeed = 1f;
	public float riverSpeed = 1f;
	public float riverSpeedIncreaseRate = 10f;

	public float currentGameTimeElapsed;

	public Transform[] Logs;
	public Transform[] Enemies;

	List<Transform> logsToFloat;
	// Use this for initialization
	void Start () {
		gm_Singleton = this;
		thePlayer = PlayerTransform.GetComponent<Player> ();
		thePlayer.enabled = false;
		logsToFloat = new List<Transform> ();
//		killzone.position = PlayerTransform.position + Vector3.down * maxDistToKillzone;
//		spawnZone.position = PlayerTransform.position + Vector3.up * maxSpawnZoneDistance;
	}
	
	// Update is called once per frame
	void Update () {
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
			//do the game
			if (!thePlayer.enabled) thePlayer.enabled = true;

			//move zones
			if (spawnZone.position.y - PlayerTransform.position.y < minSpawnZoneDistance) {
				//when we move the spawn zone, it also means we've reached new heights, so we can add the difference to our score.
				float preMoveY = spawnZone.position.y;
				//Debug.Log("moving");
				Debug.Log(PlayerTransform.position + (Vector3.up * minSpawnZoneDistance));
				spawnZone.position = PlayerTransform.position + (Vector3.up * minSpawnZoneDistance);

				score += (spawnZone.position.y - preMoveY) / 10f;
			}

			if (Vector3.Distance (killZone.position, PlayerTransform.position) > maxDistToKillzone)
				killZone.position = PlayerTransform.position + Vector3.down * maxDistToKillzone;

			//update rates
			if (PlayerTransform.position.y > 5f) {//don't start shit till they move
				enemyEncounterRate = (startingEnemyEncounterRate < enemyEncounterMaxRate) ? startingEnemyEncounterRate + currentGameTimeElapsed * enemyEncounterGrowthRate : enemyEncounterMaxRate;

				riverSpeed = startingRiverSpeed + (currentGameTimeElapsed * riverSpeedIncreaseRate);
			}

			//check if in killzone
			if (PlayerTransform.position.y < killZone.position.y + 1f) {
				thePlayer.enabled = false;
				gameState = 2;
			}

			//spawning
			currentSpawnTimer += Time.deltaTime;
			if (currentSpawnTimer > currentTimeTillSpawn) {
				SpawnLogs ();
				currentSpawnTimer = 0f;
				currentTimeTillSpawn = Random.Range (minMaxSpawnTime.x, minMaxSpawnTime.y);
			}

			//drowning display
			if (!thePlayer.overGround && !thePlayer.midAir) {
				infoText.text += "!";
				infoText.gameObject.SetActive (true);
			} else {
				infoText.text = "Drowning";
				infoText.gameObject.SetActive (false);
			}

			//move logs
			for (int i = 0; i < logsToFloat.Count; i++) {
				
				logsToFloat[i].position += (Vector3.down * riverSpeed * Time.deltaTime);
				if (logsToFloat [i].position.y < killZone.position.y) {
					Destroy (logsToFloat [i].gameObject, .5f);
					logsToFloat.Remove (logsToFloat [i]);
				}
			}

			//update displays with info
			scoreText.text = "Score " + Mathf.RoundToInt (score);
			ammoText.text = "Ammo: " + ammoNum;
		}

		else if (gameState == 2) {
			//do something to restart the game
			if (Mathf.RoundToInt(score) > instanceTopScore) instanceTopScore = Mathf.RoundToInt(score);
			gameOverText.text = "Sleep with the fishes pal. Your score was: " + Mathf.RoundToInt(score) + "\n Jump if you want to try again \n High Score: " + instanceTopScore;
			gameOverText.gameObject.SetActive(true);
			if (InControl.InputManager.ActiveDevice.AnyButton.IsPressed) {
				ResetGame ();
			}
		}
	}

	void SpawnLogs(){
		int numberToSpawn = Mathf.RoundToInt (Random.Range (minMaxSpawnAmount.x, minMaxSpawnAmount.y));
		for (int x = 0; x < numberToSpawn; x++) {
			Transform newLog = Instantiate (Logs [Random.Range (0, Logs.Length)], spawnZone.position + Vector3.right * Random.Range (-spawnZoneWidth, spawnZoneWidth), Quaternion.identity);
			newLog.transform.eulerAngles = Vector3.forward * (Random.Range(0f, 360f));
			logsToFloat.Add (newLog);

			if (Random.Range (0, 100) <= enemyEncounterRate) {
				Transform newEnemy = Instantiate (Enemies [Random.Range (0, Enemies.Length)], newLog.transform.position, Quaternion.identity);
				newEnemy.parent = newLog;
				newEnemy.localEulerAngles = Vector3.zero;
			}
		}
	}

	void ResetGame () {
		for (int i = 0; i < logsToFloat.Count; i++) {
			Destroy (logsToFloat [i].gameObject, .2f);
			logsToFloat.Remove (logsToFloat [i]);
		}
		score = 0;
		PlayerTransform.position = Vector3.zero;
		thePlayer.wetness = 0;
		thePlayer.currentWeapon.ammo = thePlayer.currentWeapon.startingAmmo - 1;
		thePlayer.currentWeapon.ReplenishAmmo (1);
		gameOverText.gameObject.SetActive(false);
		thePlayer.enabled = true;
		gameState = 1;
	}
}
