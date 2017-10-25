using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempLog : MonoBehaviour {

	bool PlayerOn = true;

	private SpriteRenderer mySR;
	private Color ogColor;

	public float sinkTime;
	private float timer;
	// Update is called once per frame
	void Start(){
		mySR = this.GetComponent<SpriteRenderer> ();
		ogColor = mySR.color;
	}

	void Update () {
		if (!PlayerOn){
			Destroy(this.gameObject);
		}
		if (timer >= sinkTime) {
			Destroy(this.gameObject);
		}

		mySR.color = Color.Lerp (ogColor, Color.clear, timer / sinkTime);

		timer += Time.deltaTime;
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player"){
			PlayerOn = false;
		}
		else {
			PlayerOn = true;
		}
	}
}
