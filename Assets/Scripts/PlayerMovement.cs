using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private float InputX;
	private float InputZ;
	private float movementSpeed;

	[SerializeField]
	private float playerSpeed = 20;

	[SerializeField]
	private float playerRotateSpd = 0.5f;

	[SerializeField]
	private float allowPlayerRotate = 0f;

	[SerializeField]
	private bool blockPlayerRotate = false;

	[SerializeField]
	private bool isGrounded;

	[SerializeField]
	private Vector3 desiredMoveDir;

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private CharacterController charController;


	private float verticalVelo;
	private Vector3 moveVector;


	public AnimationCurve jumpFallOff;
	public float jumpMultiplier;
	bool isJumping;
	public KeyCode jumpKey;


	private Vector3 moveDirection;
	private float playerGravity = -9.5f;

	private LayerMask waterMask;
	private Collider playerCollider;

	public bool isOnBridge;

	void Start() {
		charController = GetComponent<CharacterController>();
		playerCollider = GetComponent<Collider>();
		cam = Camera.main;
		blockPlayerRotate = false;
	}

	private void Update() {
		//print(GameSystem.IsPaused() + " " + FishingSystem.isFishing + " " + FishingSystem.isCastingHook + " " + FishingSystem.isReeling + " " + CameraCinematic.isRunning + " " + WatchTower.onTower  + " " + FishThrowing.isThrowing);
		InputMagnitude();
		//JumpInput(); //uncomment this for jump

		charController.Move(new Vector3(0f, playerGravity * Time.deltaTime, 0f));

	}

	void PlayerMoveAndRotation() {
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");

		Camera camera = Camera.main;
		Vector3 forward = cam.transform.forward;
		Vector3 right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize();
		right.Normalize();

		desiredMoveDir = forward * InputZ + right * InputX;

		float isMovingZ = System.Math.Sign(InputZ);
		float isMovingX = System.Math.Sign(InputX);

		Vector3 instantMoveDir = forward * isMovingZ + right * isMovingX;

		RaycastHit hit;

		if (!blockPlayerRotate) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDir), playerRotateSpd);
		}

		if (!isOnBridge) {
			if (Physics.Raycast(transform.position + instantMoveDir * 3 + transform.up * 10, Vector3.down, out hit, Mathf.Infinity)) {

				switch (hit.collider.tag) {
					case "Water":
					case "Bridge":
					case "Trap":
					case "Boss":
						charController.Move(Vector3.zero);
						return;
				}


			}
		}

		if (!blockPlayerRotate) {
			charController.SimpleMove(desiredMoveDir * playerSpeed);
		}
	}

	private void OnDrawGizmos() {
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");

		Vector3 forward = cam.transform.forward;
		Vector3 right = cam.transform.right;

		forward.Normalize();
		right.Normalize();

		float isMovingZ = System.Math.Sign(InputZ);
		float isMovingX = System.Math.Sign(InputX);

		Vector3 instantMoveDir = forward * isMovingZ + right * isMovingX;

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position + instantMoveDir * 3 + transform.up * 10, Vector3.down * 1000f);
	}


	void InputMagnitude() {
		//Calculate Input Vectors
		InputX = Input.GetAxis("Horizontal");
		InputZ = Input.GetAxis("Vertical");

		float currentSpeed = new Vector2(InputX, InputZ).sqrMagnitude;

		//Calculate Input Magnitude
		movementSpeed = currentSpeed;

		//Physically Move Player
		if (movementSpeed > allowPlayerRotate) {
			PlayerMoveAndRotation();
		} else if (movementSpeed < allowPlayerRotate) {
		} else if (movementSpeed == 0 || !Input.GetButton("Horizontal") || !Input.GetButton("Vertical")) {

		}


	}

	private void JumpInput() {
		if (Input.GetKeyDown(jumpKey) && !isJumping) {
			isJumping = true;
			StartCoroutine(JumpEvent());
		}
	}

	private IEnumerator JumpEvent() {

		//prevents jittering when trying to jump over tall objects
		charController.slopeLimit = 90.0f;

		float timeinAir = 0.0f;

		do {

			float jumpForce = jumpFallOff.Evaluate(timeinAir);
			charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
			//playerGravity = -20.0f;
			timeinAir += Time.deltaTime;
			yield return null;
		}
		while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

		charController.slopeLimit = 45.0f;
		isJumping = false;
	}
}
