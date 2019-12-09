using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCloseUp : MonoBehaviour {
	public GameObject player;
	private float zoomSpeed = 8.0f;
	private float initialCamDist;
	// private float initialCamDist, minCamDist;

	public int noOfRays;
	public float rayDeltaY;
	public float cameraMinYAbovePlayer;
	public float cameraMinSnapDistance;

	public LayerMask zoomMask;

	// private bool isColliding;
	private bool zoomOut;

	private Vector3 playerPos;
	private Vector3 previousCamPos;

	private RaycastHit hit, distancedHit, reversedHit, distancedReversedHit;
	private bool reversingRay;

	//this is to stop the zoom amt from increasing when the camera tis touching a wall
	// static itself
	public static bool hitWall;

	// Start is called before the first frame update
	void Start() {
		// minCamDist = .5f;
		initialCamDist = ThirdPersonCamera.camDistance;
		playerPos = player.transform.position;
		previousCamPos = transform.position;
	}

	// Update is called once per frame
	void Update() {

		if (!GameManager.paused) {
			// transform.position = ThirdPersonCamera.instance.transform.position;
			playerPos = player.transform.position;

			// Player to camera ray. 
			hitWall = Physics.Raycast(playerPos, transform.position - playerPos, out hit, Vector3.Distance(playerPos, transform.position), zoomMask);
			Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, zoomMask);

			Debug.DrawRay(playerPos, transform.position - playerPos, Color.red);

			// Targeted either wall.
			if (hit.collider != null) {
				// print(hit.collider);
				// Hit wall, zoom in.
				Vector3 snappedPosition = new Vector3(hit.point.x + hit.normal.x * 0.5f, hit.point.y, hit.point.z + hit.normal.z * 0.5f);
				
				// Only snap if the distance is between cameraMinSnapDistance.
				if (Vector3.Distance(playerPos, snappedPosition) > cameraMinSnapDistance){
					return;
				}

				// Shoot multiple raycast in upper y direction.
				float maxHeight = 0f;
				for (int i = 1; i <= noOfRays; i++) {
					RaycastHit newHit;
					Debug.DrawRay(playerPos, transform.position - playerPos + Vector3.up * i * rayDeltaY, Color.blue);
					Physics.Raycast(playerPos, transform.position - playerPos + Vector3.up * i * rayDeltaY, out newHit, Vector3.Distance(playerPos, transform.position), zoomMask);
					if (newHit.collider == hit.collider) {
						maxHeight = i;
					} else {
						break;
					}
				}

				if (snappedPosition.y < playerPos.y + cameraMinYAbovePlayer) {
					float shiftY = 0f;
					
					if (maxHeight * rayDeltaY > cameraMinYAbovePlayer){
						shiftY = cameraMinYAbovePlayer;
						ThirdPersonCamera.lockY = false;
					} else {
						shiftY = maxHeight * rayDeltaY;
						ThirdPersonCamera.lockY = true;
					}

					ThirdPersonCamera.camHeight = shiftY;
					snappedPosition += Vector3.up * ThirdPersonCamera.camHeight;
				}

				ThirdPersonCamera.instance.AdjustCamera(snappedPosition);
				// print("snap");
			} else {
				ThirdPersonCamera.lockY = false;
				if (ThirdPersonCamera.camDistance < initialCamDist) {
					ThirdPersonCamera.camDistance += Time.deltaTime * zoomSpeed;
					if (ThirdPersonCamera.camHeight > 0f) {
						ThirdPersonCamera.camHeight -= Time.deltaTime * zoomSpeed;
					}
					print("zoom out");
				}
			}

	
			// print(string.Format("reversingRay: {0}", reversingRay));



			// if (hit.collider != null) {
			// 	// print(hit.collider.name);

			// 	// Distanced ray
			// 	Vector3 pos = ThirdPersonCamera.CalculateCameraPosition(ThirdPersonCamera.camDistance + Time.deltaTime * zoomSpeed);
			// 	Physics.Raycast(pos, playerPos - pos, out distancedHit, 10f, zoomMask);

			// 	if (!hit.collider.CompareTag("Player") && ThirdPersonCamera.camDistance >= minCamDist) {
			// 		// Zoom in.
			// 		ThirdPersonCamera.camDistance -= Time.deltaTime * zoomSpeed;
			// 		// hit.collider.GetComponent<MeshRenderer>().material = GameManager.instance.wallInvisMaterial;
			// 		print("zoomin2");
			// 		reversingRay = false;
			// 	} else {
			// 		float distBetweenPlayer = Vector3.Distance(playerPos, transform.position);
			// 		// Reverse ray.
			// 		if (Physics.Raycast(playerPos, transform.position - playerPos, out reversedHit, distBetweenPlayer, zoomMask) && ThirdPersonCamera.camDistance >= minCamDist) {
			// 			// Zoom in.
			// 			ThirdPersonCamera.camDistance -= Time.deltaTime * zoomSpeed;
			// 			print("zoomin3");
			// 			reversingRay = true;
			// 		}

			// 		// Distanced reverse ray
			// 		Vector3 reversePos = playerPos - (transform.position - pos);
			// 		// print(reversePos);
			// 		Physics.Raycast(reversePos, transform.position - reversePos, out distancedReversedHit, distBetweenPlayer + 1f, zoomMask);
			// 	}

			// 	// Zoom out logic.
			// 	if (ThirdPersonCamera.camDistance < initialCamDist) {

			// 		if (reversingRay) {
			// 			print(string.Format("reversedHit.collider: {0}, distancedReversedHit.collider: {1}", reversedHit.collider, distancedReversedHit.collider));
			// 			if (reversedHit.collider == null && distancedReversedHit.collider == null) {
			// 				ThirdPersonCamera.camDistance += Time.deltaTime * 5;
			// 				print("Zoomout1");
			// 			}
			// 		} else if (distancedHit.collider.CompareTag("Player")) {
			// 			ThirdPersonCamera.camDistance += Time.deltaTime * 5;

			// 			print("Zoomout2");
			// 		}

			// 	}
			// }
		}
		// Collider[] objects = Physics.OverlapSphere(transform.position, 0.01f);

		// foreach (Collider c in objects) {
		// 	print(c.name);
		// }

		// Vector3 positionAfterZoomOut = (Vector3)ThirdPersonCamera.GetCameraValues(ThirdPersonCamera.camDistance + Time.deltaTime * zoomSpeed)["position"];
		// print(positionAfterZoomOut);

		// if (objects.Length > 0 && objects[0].CompareTag("Building") && ThirdPersonCamera.camDistance > minCamDist) {
		// 	ThirdPersonCamera.camDistance -= Time.deltaTime * zoomSpeed;
		// 	print("zoomin1");
		// 	return;
		// } else if (ThirdPersonCamera.camDistance < initialCamDist && (hit.collider == null || hit.collider.CompareTag("Player")) &&
		//  (Physics.OverlapSphere(positionAfterZoomOut, 0.1f).Length == 0 || Physics.OverlapSphere(positionAfterZoomOut - (positionAfterZoomOut - playerPos) / 2, 0.5f).Length == 0)) {
		// 	ThirdPersonCamera.camDistance += Time.deltaTime * zoomSpeed;
		// 	print("zoominout1");
		// 	return;
		// }
		// if (Physics.Raycast(currentCamPos, playerPos - currentCamPos, 10.0f, playerMask) && !isColliding)
		// {
		//     print("PLAYER");
		//     if(ThirdPersonCamera.camDistance <= initialCamDist)
		//     {
		//         ThirdPersonCamera.camDistance += Time.deltaTime * zoomSpeed; 
		//         currentCamPos = ThirdPersonCamera.instance.transform.position; 
		//         isColliding = false;
		//     }
		// }
		// if (Physics.Raycast(currentCamPos, playerPos - currentCamPos, 10.0f, ~playerMask & ~(1 << 9)))
		// {
		//     if(ThirdPersonCamera.camDistance >= 3.0f)
		//     {
		//         ThirdPersonCamera.camDistance -= Time.deltaTime * zoomSpeed;  
		//         currentCamPos = ThirdPersonCamera.instance.transform.position;
		//         isColliding = true;
		//     }
		// }
	}

	// void OnTriggerStay(Collider other) {
	// 	if (other.tag == "Building") {
	// 		other.GetComponent<MeshRenderer>().material = GameManager.instance.wallInvisMaterial;
	// 	}
	// }
	// void OnTriggerExit(Collider other) {
	// 	if (other.tag == "Building") {
	// 		other.GetComponent<MeshRenderer>().material = GameManager.instance.wallNormalMaterial;
	// 	}
	// }
	private void OnCollisionStay(Collision other) {
		if (ThirdPersonCamera.camDistance >= 6.0f) {
			ThirdPersonCamera.camDistance -= Time.deltaTime * zoomSpeed;
			previousCamPos = ThirdPersonCamera.instance.transform.position;
			// isColliding = true;
		}
	}

	private void OnCollisionExit(Collision other) {
		// isColliding = false;
	}

	private void OnDrawGizmos() {
		// Vector3 positionAfterZoomOut = (Vector3)ThirdPersonCamera.GetCameraValues(ThirdPersonCamera.camDistance + Time.deltaTime * zoomSpeed)["position"];
		// Gizmos.DrawSphere(positionAfterZoomOut - (positionAfterZoomOut - playerPos) / 2, 0.1f);
		Gizmos.DrawSphere(transform.position, 2f);
	}
}
