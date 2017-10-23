using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	//this enemy will just move toward the player from wherever they spawned
	public int enemyHealth = 1;
	public int scoreValue = 10;
	public float hitForce = 1f;
	public float chaseSpeed =  1f;
	Rigidbody2D enemyRB;
	Vector2 moveVector = Vector2.zero;
	// Use this for initialization
	void Start () {
		enemyRB = this.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		//something?

	}

	void FixedUpdate(){
		moveVector = Player.instance.GetComponent<Rigidbody2D> ().position - enemyRB.position;
		moveVector.Normalize ();

		enemyRB.position += moveVector * chaseSpeed;
	}

	public virtual void DamageEnemy(int value, string aggressor){
		enemyHealth -= value;
		if (enemyHealth < 1) {
			DieAndTeleplayer (GameObject.Find (aggressor));
		}
	}

	protected virtual void DieAndTeleplayer(GameObject playerToTele){
		enemyRB.simulated = false;
		playerToTele.GetComponent<Player>().Teleport(this.transform.position);
		LakeGameManager.instance.score += scoreValue;
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
