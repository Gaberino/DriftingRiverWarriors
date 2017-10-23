using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public string OwnerName;

	public int hitForce;

	public float SlowTimeSearchRadius;

	public float life;
	private float elapsedLife;

	public bool useFlightCurve = true;
	public AnimationCurve flightCurve;
	private Vector3 originalScale;

	public bool fired = false;
	public Vector2 velocity;

	private Rigidbody2D myRB;
	// Use this for initialization
	void Start () {
		myRB = this.GetComponent<Rigidbody2D> ();
		originalScale = this.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (fired) {
			elapsedLife += Time.deltaTime;
			if (elapsedLife > life)
				Remove();
		
			if (useFlightCurve) {
				this.transform.localScale = originalScale * flightCurve.Evaluate (elapsedLife / life);
			}
		}
	}

	void FixedUpdate () {
		if (fired) {
			myRB.position += velocity;

//			if (Physics2D.OverlapCircle(this.transform.position, SlowTimeSearchRadius).IsTouchingLayers (GameManager.gm_Singleton.enemyLayer)) {
//				GameManager.gm_Singleton.slowTime = true;
//			} else {
//				GameManager.gm_Singleton.slowTime = false;
//			}
		}
	}

	void Remove(){
		CamControl.instance.camInterests.Remove (this.transform);
		Destroy (this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other){
		if (fired) {
			if (other.name != OwnerName) {
				if (other.tag == "Player") {
					other.GetComponent<Player> ().HitPlayer (velocity * hitForce);
				//	GameManager.gm_Singleton.slowTime = false;
					Remove();
				}
				else if (other.tag == "Enemy") {
					other.GetComponent<Enemy>().DamageEnemy(hitForce, OwnerName);
				//	GameManager.gm_Singleton.slowTime = false;
					Remove();
				}
			}
		}
	}
}
