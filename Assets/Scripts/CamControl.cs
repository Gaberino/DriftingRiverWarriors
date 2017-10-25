using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CamControl : MonoBehaviour {

	public static CamControl instance;

	public float lerpSpeed;
	public Transform player;
	public List<Transform> camInterests;

	Vector2 sumOfInterests;
	public float interestFocusAmount = 1f;

	[Range (0, 1)]
	public float maxVignetteIntensity;
	[Range (0, 1)]
	public float startingVignetteIntensity;
	[Range (0, 1)]
	public float maxGrainIntensity;
	[Range (0, 1)]
	public float startingGrainIntensity;

	Vector2 truePos = Vector2.zero;
	Vector2 shakeOffset = Vector2.zero;
	public float minShake;
	public float maxShake;
	[Min(1)]
	public float minShakeReturnRate;
	[Min(1)]
	public float maxShakeReturnRate;
	Vector2 rumbleOffset = Vector2.zero;
	public float minRumble;
	public float maxRumble;
	public float wetnessRumbleGrowth;
	[Min(1)]
	public float rumbleReturnRate;

	private PostProcessingBehaviour myPPB;
	private PostProcessingProfile myProfile;
	private Player C_Player;
	// Use this for initialization
	void Start () {
		instance = this;
		camInterests = new List<Transform>();
		myPPB = this.GetComponent<PostProcessingBehaviour> ();
		myProfile = myPPB.profile;
		//startingGrainIntensity = myProfile.grain.settings.intensity;
		//startingVignetteIntensity = myProfile.vignette.settings.intensity;
		C_Player = Player.instance;
	}

	void Update(){
		sumOfInterests = Vector2.zero;
		var vignetteSettings = myProfile.vignette.settings;
		var grainSettings = myProfile.grain.settings;
		if (camInterests.Count > 0){
			//summarize into v2

			foreach (Transform t in camInterests){
				sumOfInterests += new Vector2(t.position.x, t.position.y);
			}
			sumOfInterests /= camInterests.Count;
		}
		float drownedPercent = C_Player.wetness / C_Player.myBrain.timeToDrown;
		if (!C_Player.midAir && !C_Player.overGround) {//in water
			
			//myProfile.vignette.enabled = true;
			vignetteSettings.intensity = Mathf.Lerp (startingVignetteIntensity, maxVignetteIntensity, drownedPercent);
			myProfile.vignette.settings = vignetteSettings;

			myProfile.grain.enabled = true;
			grainSettings.intensity = Mathf.Lerp (startingGrainIntensity, maxGrainIntensity, drownedPercent);
			myProfile.grain.settings = grainSettings;
			Rumble ();
		} else {
			//myProfile.vignette.enabled = false;
			myProfile.grain.enabled = false;
			vignetteSettings.intensity = Mathf.Lerp (0, maxVignetteIntensity, drownedPercent);
			myProfile.vignette.settings = vignetteSettings;
		}
	}

	void FixedUpdate(){
		if (sumOfInterests.magnitude > 0) {
			Vector2 withInterestVector = Vector2.Lerp ((Vector2)player.position, sumOfInterests, interestFocusAmount); //creates a v2 that is between the player pos and the sum of interests by interest amount
			truePos = Vector2.Lerp (transform.position, withInterestVector, lerpSpeed);
		} else {
			truePos = Vector2.Lerp (transform.position, (Vector2)player.position, lerpSpeed);
		}

		transform.position = truePos + shakeOffset + rumbleOffset;

		shakeOffset = new Vector2(shakeOffset.x / Random.Range(minShakeReturnRate, maxShakeReturnRate), shakeOffset.y / Random.Range(minShakeReturnRate, maxShakeReturnRate));
		rumbleOffset /= rumbleReturnRate;

		transform.position = new Vector3(transform.position.x, transform.position.y, -10);

	}

	public void Shake(){
		shakeOffset = Random.insideUnitCircle * Random.Range (minShake, maxShake);
	}

	public void Rumble(){
		float drownedPercent = C_Player.wetness / C_Player.myBrain.timeToDrown;
		rumbleOffset = Random.insideUnitCircle * (Random.Range (minRumble, maxRumble) * (drownedPercent * wetnessRumbleGrowth));
	}
}
