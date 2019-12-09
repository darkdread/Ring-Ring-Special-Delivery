using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

	public static ThirdPersonCamera instance;

	public Transform camTarget;
	public Transform camTransform;
	public Transform forcedCam;

	public float zoomAmt;
	public float zoomRate;

	private const float minAngleY = -50.0f;
	private const float maxAngleY = 40.0f;

	public static float camDistance = 10.0f;
	private static float currentX = 0.0f;
	private static float currentY = 0.0f;
	public static bool lockY = false;

	[Header("See Through")]
	// for the see through
	public Transform playerPos;

	PlayerController thePlayer;
	public LayerMask SeeThroughMask;

	public List<GameObject> currentGroundObjects = new List<GameObject>();
	private List<GameObject> groundObjects = new List<GameObject>();

	[Header("Back ray")]
	public LayerMask hitLayer;
	public float backRayLength;
	public static float camHeight;
	public float adjDistance;

	[Header("change height")]
	[SerializeField] bool touchWall;

	[Header("Camera force look target for time")]
	public Transform lookTarget;
	public float lookTime, lookTimeReturn;


	void Awake() {
		thePlayer = FindObjectOfType<PlayerController>();
		if (instance == null) {
			instance = this;
			currentX = 235;
			camDistance = 10.0f;
			currentX = 0.0f;
			currentY = 0.0f;
			//locks cursor to center of screen
			//  Cursor.lockState = CursorLockMode.Locked;

			// AdjustCamera(Vector3.zero);
		} else {
			Destroy(gameObject);
		}
	}


	void Update() {
		// stops the camera from turning 
		if(GameManager.instance.gameEnded) return;

		if (!GameManager.paused) {
			lookTime -= Time.deltaTime;

			if (lookTime <= 0f){
				lookTimeReturn -= Time.deltaTime;
			}

			AdjustCamera(Vector3.zero);
			
			// this is to make the camera start behind the player
			if(!GameManager.instance.gameStart){
				currentX = 180;
				GameManager.instance.gameStart = true;
			}
			// SeeThrough();
		}
	}

	public bool BackRay() {
		RaycastHit hit;
		Vector3 backDir = -transform.forward;
		return (Physics.Raycast(transform.position, backDir, out hit, backRayLength, hitLayer));
	}

	public static Vector3 CalculateCameraPosition(float distance) {
		Vector3 dir = new Vector3(0, 0, -distance);
		//change x value to currentY for vertical rotation
		Quaternion camRotation = Quaternion.Euler(0, currentX , 0);

		// Places camera on the player then applies rotation * direction
		return instance.camTarget.position + new Vector3(0f, 2f, 0f) + camRotation * dir; ;
	}

	public static void SetTarget(Transform target, float time){
		instance.lookTarget = target;
		instance.lookTime = time;
		instance.lookTimeReturn = 1f;
	}

	public void AdjustCamera(Vector3 forcePosition) {
		// Only look at target, do nothing else.
		if (lookTarget != null){
			if (lookTime > 0f){
				camTransform.rotation = Quaternion.RotateTowards(camTransform.rotation,
					Quaternion.LookRotation(CatScript.instance.transform.position - camTransform.position), 10f);
				return;
			} else if (lookTimeReturn > 0f) {
				camTransform.rotation = Quaternion.RotateTowards(camTransform.rotation,
					Quaternion.LookRotation(instance.camTarget.position - camTransform.position), 10f);
				return;
			}
		}

		Vector3 backDir = -transform.forward;

		if (forcePosition != Vector3.zero) {
			camTransform.position = Vector3.Lerp(camTransform.position, forcePosition, 0.8f);

			camTransform.rotation = Quaternion.LookRotation((camTarget.position - camTransform.position + camTransform.forward).normalized);
			// camTransform.LookAt(camTarget.position, Vector3.up);

			ThirdPersonCamera.camDistance = Vector3.Distance(camTransform.position, camTarget.position);

			return;
		}

		currentX += Input.GetAxis("Mouse X") * 15;

		if (lockY) {
			if (Input.GetAxis("Mouse Y") < 0f) {
				currentY += Input.GetAxis("Mouse Y") * 15;
			}
		} else {
			currentY += Input.GetAxis("Mouse Y") * 15;
		}

		// print(Input.GetAxis("Mouse Y"));

		currentY = Mathf.Clamp(currentY, minAngleY, maxAngleY);

		Vector3 dir = new Vector3(0, 0, -camDistance);
		//change x value to currentY for vertical rotation
		Quaternion camRotation = Quaternion.Euler(-currentY, currentX, 0);

		// Places camera on the player then applies rotation * direction
		camTransform.position = camTarget.position + camRotation * dir;

		Debug.DrawRay(camTarget.position, camTransform.position - camTarget.position, Color.blue);

		camTransform.LookAt(camTarget.position, Vector3.up);
	}

	public void SeeThrough() {
		//distance between player and the camera
		float distance = Vector3.Distance(playerPos.position, transform.position);
		RaycastHit[] hits = Physics.RaycastAll(transform.position, playerPos.position - transform.position, distance, SeeThroughMask);
		// RaycastHit[] hits = Physics.SphereCastAll(transform.position, 3f, playerPos.position - transform.position, distance - 6f, SeeThroughMask);

		foreach (RaycastHit hit in hits) {
			GameObject g = hit.collider.gameObject;

			if (!currentGroundObjects.Contains(g)) {
				currentGroundObjects.Add(g);
				g.GetComponent<MeshRenderer>().material = GameManager.instance.wallInvisMaterial;
			}

			foreach (GameObject lmao in currentGroundObjects) {
				if (lmao == g) {
					groundObjects.Add(lmao);
				}
			}
		}

		for (int i = currentGroundObjects.Count - 1; i > -1; i--) {
			GameObject curr = currentGroundObjects[i];

			if (groundObjects.IndexOf(curr) > -1) {
				continue;
			}
			curr.GetComponent<MeshRenderer>().material = GameManager.instance.wallNormalMaterial;
			currentGroundObjects.RemoveAt(i);
		}

		groundObjects.Clear();

	}

	/// <summary>
	/// Returns a dictionary with "position" and "rotation" key, with values being the
	/// position and rotation of the Third Person Camera.
	/// camDistance: Camera's distance from player
	/// </summary>
	public static Dictionary<string, object> GetCameraValues(float camDistance) {
		currentY = Mathf.Clamp(currentY, minAngleY, maxAngleY);

		Vector3 dir = new Vector3(0, 0, -camDistance);
		//change x value to currentY for vertical rotation
		Quaternion camRotation = Quaternion.Euler(-currentY, currentX, 0);

		Dictionary<string, object> dict = new Dictionary<string, object>();
		dict.Add("position", instance.camTarget.position + new Vector3(0f, 2f, 0f) + camRotation * dir);
		dict.Add("rotation", Quaternion.LookRotation((instance.camTarget.position - instance.camTransform.position).normalized));

		return dict;
	}

	public static void CamZoom(float zoomAmount) {
		// instance.zoomAmt = zoomAmount;
		// instance.StartCoroutine("CameraZoom");
	}

	private IEnumerator CameraZoom() {
		if (camDistance < zoomAmt && !BackRay()) {
			camDistance += Time.deltaTime * zoomRate;
		} else if (camDistance > zoomAmt) {
			if (thePlayer.IsGrounded()) camDistance -= Time.deltaTime * zoomRate;
			else if (!thePlayer.IsGrounded() && BackRay()) {
				camDistance -= Time.deltaTime * zoomRate;
			}
		}
		// print(camDistance);
		yield return new WaitForSeconds(Time.deltaTime);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.black;
		Gizmos.DrawSphere(camTransform.position, 0.1f);
	}
}
