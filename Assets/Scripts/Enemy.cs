using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	//this enemy will just move toward the player from wherever they spawned
	public float spawnTelegraphTime = 2f;
	private float telegraphTimer = 0f;
	public int enemyHealth = 1;
	public int scoreValue = 10;
	public float hitForce = 1f;
	public float chaseSpeed =  1f;
	Rigidbody2D enemyRB;
	Vector2 moveVector = Vector2.zero;

	public GameObject TempLogPrefab;

	bool hasSpawned = false;
	private ParticleSystem myPS;
	private SpriteRenderer mySR;
	// Use this for initialization
	void Start () {
		enemyRB = this.GetComponent<Rigidbody2D> ();
		myPS = this.GetComponent<ParticleSystem>();
		mySR = this.GetComponent<SpriteRenderer>();

		mySR.enabled = false;
		enemyRB.simulated = false;
	}
	
	// Update is called once per frame
	void Update () {
		//something?
		if (telegraphTimer > spawnTelegraphTime){
			hasSpawned = true;
			mySR.enabled = true;
			enemyRB.simulated = true;
			Destroy(myPS);
		}

		telegraphTimer += Time.deltaTime;
	}

	void FixedUpdate(){
		if (hasSpawned){
		moveVector = Player.instance.GetComponent<Rigidbody2D> ().position - enemyRB.position;
		moveVector.Normalize ();

		enemyRB.position += moveVector * chaseSpeed;
		}
	}

	public virtual void DamageEnemy(int value, string aggressor){
		if (hasSpawned){
			enemyHealth -= value;
			if (enemyHealth < 1) {
				DieAndTeleplayer (GameObject.Find (aggressor));
			}
		}
	}

	protected virtual void DieAndTeleplayer(GameObject playerToTele){
		enemyRB.simulated = false;

		GameObject newTempLog = Instantiate(TempLogPrefab, this.transform.position, Quaternion.identity);
		newTempLog.name = "TempLog";
		playerToTele.GetComponent<Player>().Teleport(this.transform.position);
		LakeGameManager.instance.score += scoreValue;
		ParticleOverlord.instance.SpawnParticle(this.transform.position, "TeleportParticle");
		Destroy (this.gameObject);
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.collider.tag.Contains("Player")){
			if (!Player.instance.midAir){
				Player.instance.HitPlayer(moveVector * hitForce);
			}
		}
	}
}
