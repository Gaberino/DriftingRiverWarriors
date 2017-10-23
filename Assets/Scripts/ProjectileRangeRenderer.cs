using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//this goes on the arrows themselves
public class ProjectileRangeRenderer : MonoBehaviour {

	public float renderMultiplier = 10f;

	private Projectile myProj;
	private LineRenderer myLR;

	void Start(){
		myLR = this.GetComponent<LineRenderer>();
		myProj = this.GetComponent<Projectile>();
	}


	// Update is called once per frame
	void Update () {
		if (!myProj.fired){
			myLR.SetPosition(0, this.transform.position + this.transform.up * this.transform.localScale.y / 2); //set point 1 to tip of arrow
			myLR.SetPosition(1, (this.transform.position + this.transform.up * this.transform.localScale.y / 2) + (Vector3)myProj.velocity * myProj.life * renderMultiplier); //this ends where the arrow will fall
		}
		else {
			Debug.Log("Ley line seal");
			myLR.enabled = false;
			Destroy(this);
		}
	}
}
