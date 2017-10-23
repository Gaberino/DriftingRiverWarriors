using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowBehaviour : MonoBehaviour {

	public float drawAmount = 0;
	public float drawModifier = 1;
	public Transform leftStringAnchor;
	public Transform rightStringAnchor;
	public Transform middleStringAnchor;

	LineRenderer myLR;
	private Vector3 originalMidStringLocalPos;
	// Use this for initialization
	void Start () {
		myLR = this.GetComponent<LineRenderer> ();
		originalMidStringLocalPos = middleStringAnchor.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		middleStringAnchor.localPosition = originalMidStringLocalPos + (Vector3.down * (drawAmount * drawModifier));

		myLR.SetPosition (0, leftStringAnchor.position);
		myLR.SetPosition (1, middleStringAnchor.position);
		myLR.SetPosition (2, rightStringAnchor.position);
	}
}
