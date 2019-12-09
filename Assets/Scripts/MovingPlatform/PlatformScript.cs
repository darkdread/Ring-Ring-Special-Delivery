using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this is for the designer to play around with the differnt kinds of platforms
public enum platformType { Sideways, Vertical, Diagonal }

public class PlatformScript : MonoBehaviour {

	public platformType currentPlatformType;

	[Header("DiagonalMovementValues")]

	[Range(1, 20)]
	public float platformSpd = 5.0f;
	[Range(1, 20)]
	public float platformDistX = 5.0f;
	[Range(1, 20)]
	public float platformDistZ = 5.0f;
	[Range(1, 20)]
	public float platformDelay = 3.0f;
	private bool platformPaused;
	private bool platformReturn;
	private float platformTimer;
	private float platformLimitX;
	private float platformLimitZ;

	[Header("HorizontalMovement")]
	public float platformDist = 5.0f;
	private float platformLimit;


	// Start is called before the first frame update
	void Start() {
		platformLimitX = transform.position.x + platformDistX;
		platformLimitZ = transform.position.z + platformDistZ;
		platformLimit = transform.position.x + platformDist;
		if (currentPlatformType == platformType.Vertical) platformLimit = transform.position.y + platformDist;
	}

	// Update is called once per frame
	void Update() {
		if (currentPlatformType == platformType.Sideways) {
			HorizontalMovement();
		} else if (currentPlatformType == platformType.Diagonal) {
			DiagonalMovement();
		} else if (currentPlatformType == platformType.Vertical) {
			VerticalMovement();
		}
	}


	void DiagonalMovement() {

		platformTimer += Time.deltaTime;

		//if platform has not reached X and Z destinations and is not returning while delay is over
		if (!platformReturn && platformTimer >= platformDelay) {
			if (transform.position.x < platformLimitX) {
				transform.position = new Vector3(transform.position.x + Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
			}

			if (transform.position.z < platformLimitZ) {
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * platformSpd);
			}

			//transform.position = new Vector3(transform.position.x + Time.deltaTime * platformSpd, transform.position.y, transform.position.z + Time.deltaTime * platformSpd);
		}
		//if platform has reached its destination 
		if (transform.position.x >= platformLimitX && transform.position.z >= platformLimitZ && !platformPaused) {
			platformPaused = true;
			platformTimer = 0;
		} else if (platformReturn) {
			platformPaused = false;
			//transform.position = new Vector3(transform.position.x - Time.deltaTime * platformSpd, transform.position.y, transform.position.z - Time.deltaTime * platformSpd);

			if (transform.position.x > platformLimitX - platformDistX) {
				transform.position = new Vector3(transform.position.x - Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
			}

			if (transform.position.z > platformLimitZ - platformDistZ) {
				transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - Time.deltaTime * platformSpd);
			}

			//if platform has reached its initial position
			if (transform.position.x <= platformLimitX - platformDistX && transform.position.z <= platformLimitZ - platformDistZ) {
				platformReturn = false;
				platformTimer = 0;
			}
		}
		if (platformPaused) {
			if (platformTimer >= platformDelay) {
				platformReturn = true;
			}
		}
	}

	void HorizontalMovement() {
		platformTimer += Time.deltaTime;

		//Position values clamped
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, platformLimit - platformDist, platformLimit), transform.position.y, transform.position.z);

		//if platform has not reached destination and not returning and delay is over
		if (transform.position.x < platformLimit && !platformReturn && platformTimer >= platformDelay) {
			transform.position = new Vector3(transform.position.x + Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
		} else if (platformReturn) {
			platformPaused = false;
			transform.position = new Vector3(transform.position.x - Time.deltaTime * platformSpd, transform.position.y, transform.position.z);

			//if platform has reached its initial position
			if (transform.position.x <= platformLimit - platformDist) {
				platformReturn = false;
				platformTimer = 0;
			}
		}
		  //if platform has reached its destination 
		  else if (transform.position.x >= platformLimit && !platformPaused) {
			platformPaused = true;
			platformTimer = 0;
		}

		if (platformPaused) {
			if (platformTimer >= platformDelay) {
				platformReturn = true;
			}
		}
	}
	void VerticalMovement() {
		platformTimer += Time.deltaTime;

		//Position values clamped
		transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, platformLimit - platformDist, platformLimit), transform.position.z);

		//if platform has not reached destination and not returning and delay is over
		if (transform.position.y < platformLimit && !platformReturn && platformTimer >= platformDelay) {
			transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * platformSpd, transform.position.z);
		} else if (platformReturn) {
			platformPaused = false;
			transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * platformSpd, transform.position.z);

			//if platform has reached its initial position
			if (transform.position.y <= platformLimit - platformDist) {
				platformReturn = false;
				platformTimer = 0;
			}
		}
		  //if platform has reached its destination 
		  else if (transform.position.y >= platformLimit && !platformPaused) {
			platformPaused = true;
			platformTimer = 0;
		}

		if (platformPaused) {
			if (platformTimer >= platformDelay) {
				platformReturn = true;
			}
		}
	}
}


