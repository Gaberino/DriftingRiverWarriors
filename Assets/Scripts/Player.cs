using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public static Player instance;

	public Brain myBrain; //how input is sent to me, input manager
	public Weapon currentWeapon; //what weapon the player is currently using (default bow, may not make more than that)
	public Vector2 velocity = Vector2.zero; //the velocity of the player. Needs to be public to be accessed by states
	public float rotation = 0;
	public bool midAir = false; //whether or not the player is mid jump. Used for collisions and stuff
	public bool overGround = true;
	public bool stunned = false;
	public float stunDuration = 0.25f;

	public float wetness = 0f;

	private PlayerState currentState; //state the player is in right now
	private AnimationCurve p_jumpEffectCurve; //saved reference from the slotted brain. Determines the scaling effect
	private Rigidbody2D myRB; //rigidbody ref
	private PolygonCollider2D myCollider;
	public ContactFilter2D LogFilter;
	// Use this for initialization

	#region StartingStuff

	void Start () {
		//intialize singleton
		if (instance == null) {
			instance = this;
		}

		//initialize brain
		myBrain.AttachBrain(this);

		//set references
		p_jumpEffectCurve = myBrain.jumpEffectCurve;
		myRB = this.GetComponent<Rigidbody2D>();
		myCollider = this.GetComponent<PolygonCollider2D> ();

		//initialize state and weapon
		currentState = new NormalState(this);
		if (currentWeapon != null)
			currentWeapon.PickupWeapon(this);
	}

	#endregion
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		//check if on log and if so move down at the current rate of the river
//		if (overGround && !midAir && this.transform.position.magnitude > 2f) {
//			this.transform.position += Vector3.down * GameManager.gm_Singleton.riverSpeed * Time.deltaTime;
//		}
		//check if drowning
		//wetness = wetness;
		if (!overGround && !midAir) {
			//Debug.Log("ahhhhhhhh222");
			wetness += Time.deltaTime;

			if (wetness > myBrain.timeToDrown) {
				//drowned
				LakeGameManager.instance.gameState = 2;
				this.enabled = false;
			}
		}
		else {
			if (wetness > 0f && !midAir) {
				wetness -= Time.deltaTime * myBrain.dryingRate;
			}
			else if (wetness < 0f){
				wetness = 0f;
			}
			//drownTimer = (drownTimer > 0f && !midAir) ? drownTimer -= Time.deltaTime : drownTimer = 0f;
		}
		//get input and call current state
		myBrain.RunBrain();
		currentState.Run();

		//check if state should be changed
		if (!stunned){
			if (myBrain.JumpButtonDown){
				if (!midAir && overGround){
					ChangeState(new JumpState(this));
					Debug.Log("StartingJump");
				}
			}
			else if (myBrain.AttackButtonHeld && currentWeapon != null){
				if (!midAir && !currentState.GetName().Contains("AttackState")) {
					ChangeState(new AttackState(this));
					Debug.Log("StartingAttack");
				}
			}
			else if (!midAir && !currentState.GetName().Contains("NormalState")){
				ChangeState(new NormalState(this));
				Debug.Log("ReturningToNormal");
			}
		}
	}

	void FixedUpdate(){
		if (!midAir) { //move function while not jumping, so grounded or drowning
			if (!overGround)
				velocity *= myBrain.drowningSpeedModifier; //slow in water
			//move player based on velocity
			myRB.position += velocity;
			myRB.rotation = rotation;
		} else {
			this.transform.position += (Vector3)velocity;

		}

		//check if above ground
		if (myCollider.IsTouching (LogFilter))
			overGround = true;
		else
			overGround = false;
	}

//	void OnCollisionEnter2D(Collision2D col){
//		if (col.otherCollider.tag == "Enemy") {
//			Destroy (col.otherCollider.gameObject);
//			HitPlayer (1);
//		}
//	}

	void ChangeState(PlayerState targetState){
		//exit current state, then enter new state
		currentState.Exit();
		currentState = targetState;
		currentState.Enter();
	}

	public void HitPlayer(Vector2 impactForce){
		Debug.Log ("WasHit");
		velocity = impactForce;
						ChangeState(new StunnedState(this));
		//push the player and put them in hitstun
	}

	public void Teleport(Vector3 location){
		this.transform.position = location;
		currentWeapon.ReplenishAmmo (2);
	}

	public void ToggleRigidbody(bool value){
		myRB.simulated = value;
	}
}

public interface PlayerState {

	string GetName(); //so we can know what state we are in within checking functions
	void Enter();
	void Run();
	void Exit();

}

public class NormalState : PlayerState { //the default player movestate
	Player stateOwner;

	public NormalState(Player inputOwner){
		stateOwner = inputOwner;
	}

	public string GetName(){
		return "NormalState";
	}

	public void Enter(){

	}

	public void Run(){
		Vector2 move = stateOwner.myBrain.MovementInput;
		//normal movement
		stateOwner.velocity = move * stateOwner.myBrain.playerSpeed * Time.deltaTime;
		//rotate to face move
		if (move.magnitude > 0.1f)
		stateOwner.rotation = Mathf.Atan2(move.y, move.x) * 57.2958f - 90f;
		//move camera
		//Camera.main.transform.position = new Vector3(stateOwner.transform.position.x, stateOwner.transform.position.y, -10);
		//check if on log and if so move down at the current rate of the river
//		if (stateOwner.overGround && !stateOwner.midAir && stateOwner.transform.position.y > 3f) {
//			stateOwner.velocity += Vector2.down * GameManager.gm_Singleton.riverSpeed * Time.deltaTime;
//		}
	}

	public void Exit(){

	}
}

public class AttackState : PlayerState {
	Player stateOwner;

	public AttackState(Player inputOwner){
		stateOwner = inputOwner;
	}

	public string GetName(){
		return "AttackState";
	}
	public void Enter(){
		stateOwner.currentWeapon.StartupWeapon ();
	}

	public void Run(){
		//half movement
		stateOwner.velocity = (stateOwner.myBrain.MovementInput * stateOwner.myBrain.playerSpeed * Time.deltaTime) / 2;
		stateOwner.currentWeapon.RunWeapon(); //camera handled in weapon
	}

	public void Exit(){
		stateOwner.currentWeapon.StowWeapon ();
	}
}

public class JumpState : PlayerState {
	Player stateOwner;
	//Vector2 jumpVector;
	float timer;
	Vector3 originalScale;

	public JumpState(Player inputOwner){
		stateOwner = inputOwner;
	}

	public string GetName(){
		return "JumpState";
	}

	public void Enter(){
		stateOwner.midAir = true;
		stateOwner.ToggleRigidbody(false); //since we toggle the rigidbody, moving the rigidbody is no longer possible via code, so we need to make sure in fixed update that if we have no rigidbody, to just move the transform instea
		//jumpVector = stateOwner.velocity;
		timer = 0;
		originalScale = stateOwner.transform.localScale;

	}

	public void Run(){
		if (timer < stateOwner.myBrain.playerJumpDuration){
			timer += Time.deltaTime;

			stateOwner.transform.localScale = originalScale * stateOwner.myBrain.jumpEffectCurve.Evaluate(timer / stateOwner.myBrain.playerJumpDuration); 
			//Camera.main.transform.position = new Vector3(stateOwner.transform.position.x, stateOwner.transform.position.y, -10);
			//stateOwner.velocity = jumpVector * Time.deltaTime;
		}

		else {
			stateOwner.midAir = false;
		}
	}

	public void Exit(){
		stateOwner.ToggleRigidbody(true);
	}
}

public class StunnedState : PlayerState {
	Player stateOwner;
		float stunTimer = 0;
		float stunDuration;
	Vector2 initialVelocity;

	public StunnedState(Player inputOwner){
		stateOwner = inputOwner;
		stunDuration = stateOwner.stunDuration;
		initialVelocity = stateOwner.velocity;
	}

	public string GetName(){
		return "StunnedState";
	}
	public void Enter(){
		stateOwner.stunned = true;
	}

	public void Run(){
							//spin
							stateOwner.rotation += Time.deltaTime * 1000f;

		//stunstate is the result of a hit, so decrease velocity by a drag function
		stateOwner.velocity = initialVelocity * (1 - (stunTimer / stunDuration));
		
		//run the stuntimer
							stunTimer += Time.deltaTime;
							if (stunTimer > stunDuration){
								stateOwner.stunned = false;
							}
	}

	public void Exit(){
		
	}
	
}