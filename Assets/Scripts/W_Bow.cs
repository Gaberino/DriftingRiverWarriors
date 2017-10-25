using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Weapons/Bow")]
public class W_Bow : Weapon {
	GameObject bowPrefab; //set it to weapon prefab
	public GameObject arrowPrefab;
	public Vector3 bowOffset;
	public float arrowSpeed;
	public float arrowSpeedToLife;
	Vector2 originalStickPosition;
	Vector2 currentStickPosition;
	Vector2 trajectory;
	BowBehaviour myBowBehaviour;
	Projectile arrowProjectile;

	Transform interestTransform;

	public override void PickupWeapon (Player pickupPlayer)
	{
		base.PickupWeapon (pickupPlayer);
		ammo = startingAmmo;
		LakeGameManager.instance.ammoNum = startingAmmo;
		bowPrefab = weaponPrefab;
		GameObject spawnBow = Instantiate (bowPrefab, w_Player.transform.position, Quaternion.identity);
		spawnBow.transform.parent = w_Player.transform;
		spawnBow.transform.localEulerAngles = Vector3.zero;
		spawnBow.transform.localPosition = bowOffset;
		myBowBehaviour = spawnBow.GetComponent<BowBehaviour> ();
		GameObject interestTransformObj = new GameObject("Bow Interest Object");
		interestTransform = interestTransformObj.transform;
		//interestTransform.parent = w_Player.transform;
		//interestTransform.position = w_Player.transform.position;
	}

	public override void StartupWeapon ()
	{
		originalStickPosition = w_Player.myBrain.AimingInput;
		trajectory = Vector2.zero;
		if (ammo > 0) {
			GameObject spawnArrow = Instantiate (arrowPrefab, w_Player.transform.position + bowOffset, Quaternion.identity);
			spawnArrow.transform.parent = w_Player.transform;
			spawnArrow.transform.localEulerAngles = Vector3.zero;
			spawnArrow.transform.localPosition = bowOffset + myBowBehaviour.middleStringAnchor.localPosition + Vector3.up * (spawnArrow.transform.localScale.y / 2);
			arrowProjectile = spawnArrow.GetComponent<Projectile> ();
			arrowProjectile.hitForce = weaponDamage;
			arrowProjectile.OwnerName = w_Player.gameObject.name;
			CamControl.instance.camInterests.Add (spawnArrow.transform);
		}
		CamControl.instance.camInterests.Add(interestTransform);
	}

	public override void RunWeapon ()
	{
		//Debug.Log (trajectory);

		currentStickPosition = w_Player.myBrain.AimingInput;
		trajectory = originalStickPosition - currentStickPosition;

		w_Player.rotation = Mathf.Atan2(trajectory.normalized.y, trajectory.normalized.x) * 57.2958f - 90f;
		myBowBehaviour.drawAmount = trajectory.magnitude;
		if (ammo > 0) {
			arrowProjectile.transform.localPosition = myBowBehaviour.middleStringAnchor.localPosition + Vector3.up * (arrowProjectile.transform.localScale.y);
			arrowProjectile.velocity = trajectory * arrowSpeed;
			arrowProjectile.life = trajectory.magnitude * arrowSpeed * arrowSpeedToLife;
		}
		interestTransform.position = w_Player.transform.position + ((Vector3)trajectory * aimingCameraMove);
		//Debug.Log (interestTransform.position);
	}

	public override void StowWeapon ()
	{
		Debug.Log ("stowingweapon");
		if (ammo > 0) {
			if (trajectory.magnitude > 0.1f) {
				arrowProjectile.transform.parent = null;
				arrowProjectile.fired = true;
				ammo -= 1;
				LakeGameManager.instance.ammoNum = ammo;
			} else {
				CamControl.instance.camInterests.Remove (arrowProjectile.transform);
				Destroy (arrowProjectile.gameObject);
			}
		}
		interestTransform.position = w_Player.transform.position;
		CamControl.instance.camInterests.Remove(interestTransform);
		myBowBehaviour.drawAmount = 0f;
		originalStickPosition = Vector2.zero;
	}

	public override void ReplenishAmmo(int amount){
		ammo += amount;
		LakeGameManager.instance.ammoNum = ammo;
	}
}