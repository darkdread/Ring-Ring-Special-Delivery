using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this is for the designer to play around with the differnt kinds of platforms
public enum platformType { Sideways, Vertical, Diagonal, FowardBack }

public class PlatformScript : MonoBehaviour {

	public Transform targetPos, originalPos;
	public Vector3 platformDist;
	public bool moveToTarget;
	public float speed;

	void Start() {
		targetPos.position = transform.position + platformDist;
		originalPos.position = transform.position;
		targetPos.parent = null;
		originalPos.parent = null;
	}

	void Update() {
		setTarget();
		HorizontalMovement();
	}

	void setTarget() {
		float distToTarget = Vector3.Distance(targetPos.position, transform.position);
		float distToOriginaPos = Vector3.Distance(originalPos.position, transform.position);
		if (distToTarget < 0.1f) moveToTarget = false;
		else if (distToOriginaPos < 0.1f) moveToTarget = true;
	}

	void HorizontalMovement() {
		if (moveToTarget) {
			transform.position = Vector3.MoveTowards(transform.position, targetPos.position, speed * Time.deltaTime);
		} else {
			transform.position = Vector3.MoveTowards(transform.position, originalPos.position, speed * Time.deltaTime);
		}
	}
}

