using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[CreateAssetMenu (fileName = "DefaultBrain")]
public class Brain : ScriptableObject {

	public float playerSpeed;
	public float playerJumpDuration;
	public float drowningSpeedModifier = 0.5f;
	public float timeToDrown = 2f;
	public float dryingRate = .1f;
	public AnimationCurve jumpEffectCurve;
	//public string horizontalInputAxisName;
	//public string verticalInputAxisName;
	//public string aimHorizontalInputAxisName;
	//public string aimVerticalInputAxisName;
	public string jumpButtonName;
	public string attackButtonName;

	public Vector2 MovementInput;
	public Vector2 AimingInput;
	public bool JumpButtonDown;
	public bool JumpButtonHeld;
	public bool JumpButtonUp;
	public bool AttackButtonDown;
	public bool AttackButtonHeld;

	public bool AttackButtonUp;

	private Player myPlayer;

	public virtual void AttachBrain(Player target){
		myPlayer = target;
	}

	public virtual void RunBrain(){
		MovementInput = new Vector2(InputManager.ActiveDevice.LeftStickX.Value, InputManager.ActiveDevice.LeftStickY.Value);
		AimingInput = new Vector2(InputManager.ActiveDevice.RightStickX.Value, InputManager.ActiveDevice.RightStickY.Value);

		JumpButtonDown = Input.GetButtonDown(jumpButtonName);
		JumpButtonHeld = Input.GetButton(jumpButtonName);
		JumpButtonUp = Input.GetButtonUp(jumpButtonName);

		AttackButtonDown = Input.GetButtonDown(attackButtonName);
		AttackButtonHeld = Input.GetButton(attackButtonName);
		AttackButtonUp = Input.GetButtonUp(attackButtonName);

	}
}
