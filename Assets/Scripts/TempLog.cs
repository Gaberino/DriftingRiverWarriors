using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempLog : MonoBehaviour {

	bool PlayerOn = true;
	
	// Update is called once per frame
	void Update () {
		if (!PlayerOn){
			Destroy(this.gameObject);
		}
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
