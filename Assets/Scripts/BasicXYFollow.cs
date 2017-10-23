using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicXYFollow : MonoBehaviour {

	public Transform target;
	
	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3 (target.position.x, target.position.y, this.transform.position.z);
	}
}
