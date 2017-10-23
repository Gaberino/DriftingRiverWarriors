using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour {

	public static CamControl instance;

	public float lerpSpeed;
	public Transform player;
	public List<Transform> camInterests;
	Vector2 sumOfInterests;
	public float interestFocusAmount = 1f;
	Vector2 truePos;

	// Use this for initialization
	void Start () {
		instance = this;
		camInterests = new List<Transform>();
	}

	void Update(){
		sumOfInterests = Vector2.zero;
		if (camInterests.Count > 0){
			//summarize into v2

			foreach (Transform t in camInterests){
				sumOfInterests += new Vector2(t.position.x, t.position.y);
			}
			sumOfInterests /= camInterests.Count;
		}
	}

	void FixedUpdate(){
		if (sumOfInterests.magnitude > 0) {
			Vector2 withInterestVector = Vector2.Lerp ((Vector2)player.position, sumOfInterests, interestFocusAmount); //creates a v2 that is between the player pos and the sum of interests by interest amount
			truePos = Vector2.Lerp (transform.position, withInterestVector, lerpSpeed);
		} else {
			truePos = Vector2.Lerp (transform.position, (Vector2)player.position, lerpSpeed);
		}

		transform.position = truePos;

		transform.position = new Vector3(transform.position.x, transform.position.y, -10);

	}

}
