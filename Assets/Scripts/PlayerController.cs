using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public bool firstMove;

	[Header("Components")]
	[SerializeField] SphereCollider col;
	[SerializeField] Rigidbody rb;
	public LayerMask groundLayers;
	//========================================================
	[Header("For Jumping")]
	public float jumpForce;
	public int jumpCount; // for double jumping
	public int jumpLimit;
	public bool isOnWall;
    private bool isLanding;
    private float landingTimer;
	[SerializeField] bool jumpButtonPressed; // to ensure that the jump trigger doesnt double jump instantly
	[SerializeField] float inputLag = 0;
	//========================================================
	private bool isWalking;
	public float moveSpeed;
	private float horizontalInput, verticalInput;
	public bool canRotate;
	public Camera mainCam;
	public int stopTime;
	private float storedHInput, storedVInput;

	//========================================================
	[Header("Dash")]
	public bool isDashing;
	public int dashLimit;
	public float airDashSpeed, dashCount;
	public float dashTime;
	private Coroutine dashCoroutine;
	[SerializeField] bool dashButtonPressed;
	//========================================================
	[Header("Character Turning")]
	public float turnSpeed;
	//========================================================
	[Header("Gravity Modifier")]
	public float initialGravity;
	public float gravityStrength;
	public float jumpGravity;
	//========================================================
	[Header("Wall Jump")]

	// this part is for the tutorial canWallJump will be true for all other parts of the map
	public bool canWallJump;

	public LayerMask wallJumpLayer;
	public Transform leftSide, rightSide;
	public float wallJumpForce;
	public float wallJumpForwardForce;
	public float wallJumpHorizontalRaycastLength;
	public float wallJumpVerticalRaycastLength;
	public float wallJumpHeight;
	public float timeSinceWallJump;
	//========================================================
	[Header("Slide")]
	public LayerMask slopeLayerMask;
	[SerializeField] bool isSliding;
	public float slideSpeed;
	//========================================================
	[Header("Grapple")]
	public CatchCo.Insula.Puzzle.ProbeRope Rope;
	private Vector3 _movementVector;
	public float MaxVelocity;
	//========================================================
	[Header("Preference Setting")]
	// prefernce 1 and 2
	public bool mode1;
	public bool mode2;
	//========================================================
	[Header("Particle Effects")]
	public GameObject jumpEffect, dashEffect, landingEffect;

	[Header("Camera Effects")]
	public GameObject diveBombEffect;
	public Transform divePos, originalPos;
	// leg effects
	public GameObject leftBooster, rightBooster, leftTrail, rightTrail;

	// for the wallJump
	public GameObject walljumpEffect;
	private Transform effectPos;
	//========================================================
	[Header("Gliding")]
	public float glideFallSpeed;
	//========================================================
	[Header("Respawning")]
	public Transform checkPoint;

	//========================================================
	[Header("Animation")]
	public Animator anim;

	[Header("Ground Marker")]
	public GameObject groundMarker;

	public GameObject bar1, bar2, bar3, bar4;

	[Header("Audio")]
	AudioSource playerAudio;
	public AudioClip playerJumpSfx;
	public AudioClip playerDoubleJumpSfx;
	public AudioClip playerWallJumpSfx;
	public AudioClip playerDashSfx;

	[Header("Tutorial")]
	public Tutorial theTutorial;

	[Header("Particle System")]
	public ParticleSystem dustCloudTrail;

	public Transform abovePlayer;


	// Start is called before the first frame update
	void Start() {
		// Accessing components
		rb = GetComponent<Rigidbody>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		mainCam = Camera.main;
		playerAudio = GetComponent<AudioSource>();
		GameManager.isQuestReceived = false;

		// Assigning Variables
		turnSpeed = 0f;
		isDashing = false;
		canRotate = true;
		canWallJump = true;
		initialGravity = jumpGravity;
		jumpLimit = 2;
		dashLimit = 2;
		// if(GameManager.paused){
		// 	GameManager.paused = false;
		// 	Time.timeScale = 1;
		// }

		//spawning of the effect transform
		effectPos = new GameObject().transform;
		effectPos.name = "WallJumpEffectPos";
		if (GameManager.instance.gameObject.activeInHierarchy == false) {
			GameManager.instance.gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update() {
		if (DialogueManager.isDialogue || GameManager.instance.gameEnded) {
			isWalking = false;
			Animations();
			return; // stops the player from doing anything when he is in dialogue
		}
		timeSinceWallJump += Time.deltaTime;
		Respawn();
		Animations();
		GroundMarker();
		DiveBombController();
		DashIndicator();

		// print("JUMP AXIS: " + Input.GetAxis("Jump"));

		if (GameManager.paused) {
			return;
		}

		Inputs();

		if (canRotate && !isSliding) {
			Walking(moveSpeed);
		}

        // For Landing Particle
        landingTimer += Time.deltaTime;

		if (!IsGrounded()) {
            isLanding = true;
			// Prevents the dust cloud trail particle system from emitting
			var emissionModule = dustCloudTrail.emission;
			emissionModule.enabled = false;
			print("DustOff");

			// zooms camera to certain distance
			if (!CameraCloseUp.hitWall) ThirdPersonCamera.CamZoom(20.0f);

			// Gliding();

			if (timeSinceWallJump >= 0.2f) {
				ISeeWallIRotate();
			}

			if (timeSinceWallJump <= 0.2f) {
				return;
			}

			if (canRotate) {
				Walking(moveSpeed / 2);
			} else {
				if (isDashing && isOnWall) {
					isDashing = false;
					rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
					print("XD");
				}
			}

			//gives an initial sense of floatiness
			if (gravityStrength < initialGravity) gravityStrength += Time.deltaTime;
			Mathf.FloorToInt(gravityStrength);

			if (Input.GetKeyDown(KeyCode.Space) || jumpButtonPressed) {
				if (!WallJump()) {
					// Second jump
					if (jumpCount < jumpLimit) {
						Jump();
						emissionModule.enabled = true;
						GameObject tempEffect = Instantiate(jumpEffect, transform.position, jumpEffect.transform.rotation);

						// Sets the rotation of the jump effect
						var main = tempEffect.GetComponent<ParticleSystem>().main;

						// Must be in radians
						main.startRotationZ = transform.eulerAngles.y * Mathf.Deg2Rad;
					}
				}
			}
		}

		if (IsGrounded()) {

			// Allow the dust cloud trail particle system to emit
			var emissionModule = dustCloudTrail.emission;
			emissionModule.enabled = true;

			// set the position of the checkpoint to the players position
			// checkPoint.position = transform.position;
			boosterControl(); // will remove once the art asets come
                              // Reset the jump

            SoundControl();
            if(jumpCount > 0)
            {
                Instantiate(landingEffect, transform.position, landingEffect.transform.rotation);
            }
            
            if(isLanding && landingTimer > 0.5f)
            {
                Instantiate(landingEffect, transform.position, landingEffect.transform.rotation);
                isLanding = false;
                landingTimer = 0;
            }

			jumpCount = 0;
			dashCount = 0;

			if (ThirdPersonCamera.camDistance > 10.15f) {
				//always zoom to a little over the default camDistance
				ThirdPersonCamera.CamZoom(10.1f);
			}


			// You can always rotate when you're on the ground.
			canRotate = true;
			// gravityStrength = initialGravity;
			if (Input.GetKeyDown(KeyCode.Space) || jumpButtonPressed) {
				Jump();
			}

			if (!isWalking && rb.velocity.y <= 0 && !isSliding) {
				// rb.velocity = new Vector3(0, 0, 0);
			}
		}

		if (dashCount < dashLimit && !IsGrounded()) {
			AirDash();
		}
	}

	public void FixedUpdate() {
		rb.AddForce(Physics.gravity * gravityStrength, ForceMode.Acceleration);
		// Grapple();
	}

	// movement methods ================================================================

	void Jump() {
		if (GameManager.instance.waitTime <= 0 && Quest.talkedToFirstNpc) {
			gravityStrength = jumpGravity;
			float forwardForce = verticalInput == 0 && horizontalInput == 0 ? 0 : moveSpeed / 2;

			rb.velocity = new Vector3(0, jumpForce, 0);
			rb.AddRelativeForce(new Vector3(0, 0, forwardForce), ForceMode.Impulse);
			isDashing = false;
			jumpCount++;

			// this is to close the catCanvas for the tutorial
			if (theTutorial != null && theTutorial.ttType != tutorialType.dashTutorial && theTutorial.ttType != tutorialType.walljump) {
				CatScript.instance.catCanvas.SetActive(false);
			}

			if (jumpCount == 1) {
				playerAudio.PlayOneShot(playerJumpSfx);
			} else {
				playerAudio.PlayOneShot(playerDoubleJumpSfx);
			}
		}
	}

	void Grapple() {
		Vector3 previousPosition = transform.position;

		_movementVector = rb.velocity;

		// solve the rope with the new distance
		Rope.Solve(_movementVector * Time.deltaTime);

		// the rope may have rotational velocity
		_movementVector += Rope.GetRopeVelocity(_movementVector);

		// likewise the rope might also retract
		_movementVector += (Rope.GetRopePullDistance(previousPosition));

		// clamp the velocity to make life more predicatable
		_movementVector = _movementVector.normalized * Mathf.Min(_movementVector.magnitude, MaxVelocity / Time.deltaTime);

		rb.velocity = _movementVector;
	}

	void ISeeWallIRotate() {
		RaycastHit hit;

		// Detect front/back walls.
		Vector3 facingDir = transform.forward;
		Vector3 backDir = -transform.forward;
		isOnWall = false;

		if (Physics.Raycast(transform.position, facingDir, out hit, wallJumpVerticalRaycastLength, wallJumpLayer)) {

			Vector3 surfaceParallel = facingDir - hit.normal * Vector3.Dot(facingDir, hit.normal);
			surfaceParallel.Normalize();
			// effectPos.position = hit.point;
			print("forward wall");
			Vector3 lookRotation = Vector3.RotateTowards(facingDir, surfaceParallel, 999, 999);
			print(lookRotation);

			if (lookRotation == Vector3.zero){
				lookRotation = hit.normal.RotateY(Mathf.PI/2);
				print("allo b0ss" + lookRotation);
			}

			transform.rotation = Quaternion.LookRotation(lookRotation);
			// effectPos.rotation = Quaternion.LookRotation(Vector3.RotateTowards(facingDir, hit.normal, 999, 999));
			// effectPos.rotation = Quaternion.LookRotation(-transform.position);
			isOnWall = true;
		}

		// if (Physics.Raycast(transform.position, backDir, out hit, wallJumpVerticalRaycastLength, wallJumpLayer)) {

		// 	Vector3 surfaceParallel = backDir - hit.normal * Vector3.Dot(backDir, hit.normal);
		// 	surfaceParallel.Normalize();
		// 	// effectPos.position = hit.point;
		// 	print("back wall");
		// 	transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(backDir, surfaceParallel, 999, 999));
		// 	effectPos.rotation = Quaternion.LookRotation(Vector3.RotateTowards(backDir, hit.normal, 999, 999));
		// 	// effectPos.rotation = Quaternion.LookRotation(-transform.position);
		// 	isOnWall = true;
		// }

		// for the rotation of the particle so that it will face the direction that the player is jumping to
		if (Physics.Raycast(transform.position, leftSide.forward, out hit, wallJumpVerticalRaycastLength, wallJumpLayer)) {
			effectPos.rotation = Quaternion.LookRotation(Vector3.RotateTowards(facingDir, hit.normal, 999, 999));
		}
		if (Physics.Raycast(transform.position, rightSide.forward, out hit, wallJumpVerticalRaycastLength, wallJumpLayer)) {
			effectPos.rotation = Quaternion.LookRotation(Vector3.RotateTowards(facingDir, hit.normal, 999, 999));
		}

		// Detect left/right walls.
		// left ray
		Ray leftRay = new Ray(leftSide.position, leftSide.forward);
		if (Physics.Raycast(leftRay, wallJumpHorizontalRaycastLength, wallJumpLayer)) {
			isOnWall = true;
		}

		//right ray
		Ray rightRay = new Ray(rightSide.position, rightSide.forward);
		if (Physics.Raycast(rightRay, wallJumpHorizontalRaycastLength, wallJumpLayer)) {
			isOnWall = true;
		}
		canRotate = !isOnWall;
	}

	public bool WallJump() {
		if (canWallJump) {
			float forwardForce = verticalInput == 0 ? 0 : wallJumpForwardForce;
			// left walljump
			Vector3 leftVector = new Vector3(wallJumpForce, wallJumpHeight, 0);
			Vector3 rightVector = new Vector3(-wallJumpForce, wallJumpHeight, 0);

			// print("I am wall jump");

			// left ray
			Ray leftRay = new Ray(leftSide.position, leftSide.forward);
			if (Physics.Raycast(leftRay, wallJumpHorizontalRaycastLength, wallJumpLayer)) {
				rb.velocity = Vector3.zero;
				rb.AddRelativeForce(leftVector, ForceMode.Impulse);

				print("I am left wall");
				transform.Rotate(Vector3.up, 90f);

				timeSinceWallJump = 0f;
				playerAudio.PlayOneShot(playerWallJumpSfx);

				// for the effect
				effectPos.position = new Vector3(leftSide.position.x, leftSide.position.y, leftSide.position.z);
				// effectPos.rotation = Quaternion.Euler(leftSide.forward);
				Instantiate(walljumpEffect, effectPos.position, effectPos.rotation);
				return true;
			}

			//right ray
			Ray rightRay = new Ray(rightSide.position, rightSide.forward);
			if (Physics.Raycast(rightRay, wallJumpHorizontalRaycastLength, wallJumpLayer)) {
				rb.velocity = Vector3.zero;
				rb.AddRelativeForce(rightVector, ForceMode.Impulse);

				print("I am right wall");
				transform.Rotate(Vector3.up, -90f);

				timeSinceWallJump = 0f;
				playerAudio.PlayOneShot(playerWallJumpSfx);

				// for the effect
				effectPos.position = new Vector3(rightSide.position.x, rightSide.position.y, rightSide.position.z);
				// effectPos.rotation = Quaternion.Euler(rightSide.forward);
				Instantiate(walljumpEffect, effectPos.position, effectPos.rotation);
				return true;
			}
			return false;
		} else return false;
	}
	void Gliding() {

		if (Input.GetButton("Glide") && rb.velocity.y < 0.1f) {
			rb.velocity = new Vector3(rb.velocity.x, -glideFallSpeed, rb.velocity.z);

			playerAudio.clip = GameManager.instance.playerClips.glideClip;
			if (!playerAudio.isPlaying) {
				playerAudio.Play();
			}
			if (!IsGrounded()) {
				// for the visual effects
				leftBooster.SetActive(true);
				rightBooster.SetActive(true);

				leftTrail.SetActive(false);
				rightTrail.SetActive(false);
			}
		} else if (Input.GetButtonUp("Glide")) {
			playerAudio.Stop();
		} else {
			leftBooster.SetActive(false);
			rightBooster.SetActive(false);

			leftTrail.SetActive(true);
			rightTrail.SetActive(true);
		}
	}

	void boosterControl() {
		leftBooster.SetActive(false);
		rightBooster.SetActive(false);

		leftTrail.SetActive(true);
		rightTrail.SetActive(true);
	}

	void SoundControl() {
		// to stop glide sounds
		if (Input.GetButton("Glide")) {
			playerAudio.Stop();
		}
	}

	void AirDash() {
		if ((Input.GetKeyDown(KeyCode.T) || dashButtonPressed && !IsGrounded() && !isSliding)) {
			if (dashCoroutine != null) {
				StopCoroutine(dashCoroutine);
			}
			if (theTutorial != null && theTutorial.ttType == tutorialType.dashTutorial) {
				CatScript.instance.catCanvas.SetActive(false);
			}
			dashCoroutine = StartCoroutine(Dash());
			dashCount++;
		}
	}

	private IEnumerator Dash() {
		Vector3 forward = mainCam.transform.forward;
		Vector3 right = mainCam.transform.right;

		GameManager.instance.motionBlur.intensity.value = 2;

		isDashing = true;
		canRotate = false;
		gravityStrength = 0;
		yield return new WaitForSeconds(.1f);
		//transform.rotation = Quaternion.Euler(0, mainCam.transform.localEulerAngles.y, 0);
		rb.velocity = new Vector3(0, 0, 0);
		yield return new WaitForSeconds(.1f);
		GameObject dashParticle = Instantiate(dashEffect, transform.position, dashEffect.transform.rotation);
		playerAudio.PlayOneShot(playerDashSfx);
		// diveBombEffect.SetActive(true);
		dashParticle.transform.parent = transform;
		dashParticle.transform.localRotation = Quaternion.Euler(0, 180, 0);
		dashParticle.transform.localPosition = new Vector3(0, 0.9f, -8);
		dashParticle.transform.parent = null;

		rb.AddRelativeForce(Vector3.forward * airDashSpeed, ForceMode.Impulse);
		yield return new WaitForSeconds(dashTime);
		if (isDashing) {
			Vector3 desiredMoveDir = (forward * verticalInput + right * horizontalInput).normalized * moveSpeed;
			rb.velocity = new Vector3(desiredMoveDir.x, rb.velocity.y, desiredMoveDir.z);
			gravityStrength = initialGravity;
			canRotate = true;
			isDashing = false;
		}

		Destroy(dashParticle);
		// diveBombEffect.SetActive(false);

		GameManager.instance.motionBlur.intensity.value = GameManager.instance.motionBlurDefaultIntensity;
	}

	///<summary>////
	/// i put this in the player controller as it uses the players rb to set the values and move the particles accordingly
	///</summary>///
	void DiveBombController() {
		float speed = rb.velocity.magnitude;
		float moveIn = 20f;
		if (speed > moveIn) {
			diveBombEffect.transform.position = Vector3.Lerp(diveBombEffect.transform.position, divePos.position, Time.deltaTime * 5);
		} else {
			diveBombEffect.transform.position = Vector3.Lerp(diveBombEffect.transform.position, originalPos.position, Time.deltaTime);
		}
	}

	void DashIndicator() {

		// if (IsGrounded()) {
		// 	bar1.gameObject.SetActive(false);
		// 	bar2.gameObject.SetActive(false);
		// 	bar3.gameObject.SetActive(false);
		// 	bar4.gameObject.SetActive(false);
		// } else {
		switch (dashCount) {
			case 0:
				bar1.gameObject.SetActive(true);
				bar2.gameObject.SetActive(true);
				bar3.gameObject.SetActive(true);
				bar4.gameObject.SetActive(true);
				break;
			case 1:
				bar1.gameObject.SetActive(false);
				bar2.gameObject.SetActive(true);
				bar3.gameObject.SetActive(true);
				bar4.gameObject.SetActive(true);
				break;
			case 2:
				bar1.gameObject.SetActive(false);
				bar2.gameObject.SetActive(false);
				bar3.gameObject.SetActive(true);
				bar4.gameObject.SetActive(true);
				break;
			default:
				bar1.gameObject.SetActive(true);
				bar2.gameObject.SetActive(true);
				bar3.gameObject.SetActive(true);
				bar4.gameObject.SetActive(true);
				break;
		}
		if (IsGrounded()) {
			bar1.gameObject.SetActive(false);
			bar2.gameObject.SetActive(false);
			bar3.gameObject.SetActive(false);
			bar4.gameObject.SetActive(false);
		}
	}

	// sets the input values
	void Inputs() {
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		isWalking = (horizontalInput != 0 || verticalInput != 0) ? true : false;

		if (horizontalInput != 0 || verticalInput != 0) {
			if (!firstMove) {
				CatScript.instance.setCatDialogue(" Move analog to move ", false);
				firstMove = true;
			}
		}

		if (horizontalInput == 0 || verticalInput == 0) {
			stopTime -= 1;
		} else if (isWalking) {
			// Store 3 frames
			stopTime = 3;
			storedHInput = horizontalInput;
			storedVInput = verticalInput;
		}

		// for controller
		jumpButtonPressed = (ControlConfig.controlConfigInstance.CharacterAction(ControlConfig.jumpAction) && inputLag <= 0);
		dashButtonPressed = (ControlConfig.controlConfigInstance.CharacterAction(ControlConfig.dashAction) && inputLag <= 0);
		if (jumpButtonPressed || ControlConfig.controlConfigInstance.CharacterAction(ControlConfig.jumpAction)) inputLag = 0.05f;
		if (dashButtonPressed || ControlConfig.controlConfigInstance.CharacterAction(ControlConfig.dashAction)) inputLag = 0.05f;

		inputLag -= Time.deltaTime;
	}

	void Walking(float moveSpeed) {
		// print("IsWalking");

		if (mode1) {
			Vector3 forward = mainCam.transform.forward;
			Vector3 right = mainCam.transform.right;

			forward.y = 0f;
			right.y = 0f;

			forward.Normalize();
			right.Normalize();

			Vector3 desiredMoveDir = (forward * verticalInput + right * horizontalInput).normalized * moveSpeed;

			if (isWalking) {
				// There's a stored frame
				if (stopTime > 0) {
					float vInput = verticalInput == 0 ? storedVInput : verticalInput;
					float hInput = horizontalInput == 0 ? storedHInput : horizontalInput;
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((forward * vInput + right * hInput).normalized * moveSpeed), .3f);
				} else {
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDir), .3f);
				}
			}

			if (!IsGrounded()) {
				if (rb.velocity.magnitude < moveSpeed + 3) {
					rb.velocity += new Vector3(desiredMoveDir.x, 0, desiredMoveDir.z) * Time.deltaTime / 2.5f;
				}
			} else {
				rb.velocity = new Vector3(desiredMoveDir.x, rb.velocity.y, desiredMoveDir.z);
			}
		} else if (mode2) {
			transform.rotation = Quaternion.Euler(0, mainCam.transform.localEulerAngles.y, 0);
			if (IsGrounded()) rb.velocity = ((transform.forward * verticalInput + transform.right * horizontalInput).normalized * moveSpeed);
		}
	}

	public void ResetPlayer() {
		isDashing = false;
		rb.velocity = Vector3.zero;
	}

	// movement respawning ================================================================
	void Respawn() {
		if (Input.GetKeyDown(KeyCode.Z)) {
			StartCoroutine(GameManager.instance.Respawn());
		}
	}

	void Animations() {
		anim.SetBool("Walking", isWalking && IsGrounded());
		anim.SetBool("Jump", !IsGrounded());
	}
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Building") {
			// Sets the checkpoint to the center of the building's collider
			// checkPoint.position = other.transform.TransformPoint(other.GetComponent<BoxCollider>().center) +
			// 	Vector3.up * (other.GetComponent<BoxCollider>().size.y * other.transform.lossyScale.y);
			checkPoint.position = other.transform.TransformPoint(other.GetComponent<BoxCollider>().center);
			// print("setted");
		}
		//if (other.gameObject.layer == 10) canRotate = false;
		//reset jumpcount
		if (other.gameObject.layer == 13) {
			jumpCount = 0;
			dashCount = 0;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.layer == 13) {
			isSliding = true;
			if (isSliding && !Input.GetKey(KeyCode.Space)) {
				Vector3 slopeForward = other.transform.forward;
				transform.rotation = Quaternion.FromToRotation(Vector3.up, -other.transform.forward);
				rb.velocity = new Vector3(0, slopeForward.y, slopeForward.z) * slideSpeed;
				print("Slope");
			}
		}
	}
	void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == 10) canRotate = true;

		// this is for the slidinglayer
		if (other.gameObject.layer == 13) {
			isSliding = false;
		}
	}

	public bool IsGrounded() {
		// return Physics.CheckSphere(col.bounds.center, col.radius * 1f, groundCheck);
		return Physics.CheckCapsule(col.bounds.center, new Vector3(col.bounds.center.x, col.bounds.center.y, col.bounds.center.z), col.radius * 1.5f, groundLayers) && Mathf.Abs(rb.velocity.y) < 1f;
	}

	///<summary>
	///Spawns ground marker on ground if player is going to land on ground.
	/// 
	///Uses IsGrounded().
	///</summary>
	void GroundMarker() {

		RaycastHit hit;
		Image groundMarkerImage = groundMarker.GetComponentInChildren<Image>();
		float lerpRate = .1f;

		if (!IsGrounded() && Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayers)) {

			groundMarker.SetActive(true);
			// this makes it visible 
			groundMarker.transform.position = hit.point + Vector3.up * 0.1f;
			groundMarker.transform.rotation = transform.rotation;
			groundMarkerImage.color = Color.Lerp(groundMarkerImage.color, Color.white, lerpRate);

			//this is to make the marker start fading before the player reaches the ground
			if (transform.position.y <= groundMarker.transform.position.y + 2f) {
				groundMarkerImage.color = Color.Lerp(groundMarkerImage.color, Color.clear, lerpRate);
			}
		} else {
			Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayers);
			// this makes it invisible 
			if (IsGrounded()) {
				groundMarker.transform.position = hit.point + Vector3.up * 0.2f;
				groundMarkerImage.color = Color.Lerp(groundMarkerImage.color, Color.clear, lerpRate);
			} else if (hit.collider == null && !IsGrounded())
				groundMarkerImage.color = Color.Lerp(groundMarkerImage.color, Color.clear, lerpRate * 2);
			// groundMarker.SetActive(false);
		}
	}
}
