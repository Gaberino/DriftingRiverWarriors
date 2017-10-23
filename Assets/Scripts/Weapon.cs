using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ScriptableObject {
	public int startingAmmo;
	public int ammo;
	public int weaponDamage;
	public GameObject weaponPrefab;
	public Vector2 weaponOffset;
	public float aimingCameraMove = 1f;
	protected Player w_Player;

	public virtual void PickupWeapon(Player pickupPlayer){
		w_Player = pickupPlayer;
	}

	public virtual void StartupWeapon(){} //called in the enter of the attack state

	public virtual void RunWeapon(){} //called in the run of the attack state

	public virtual void StowWeapon(){} //called in the exit of the attack state

	public virtual void ReplenishAmmo(int amount){
	}
}
