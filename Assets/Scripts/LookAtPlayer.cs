using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {
	[SerializeField] Transform thePlayer;
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		// if (thePlayer = null) thePlayer = FindObjectOfType<PlayerController>();
		transform.LookAt(thePlayer.transform.position, Vector3.up);
	}
}
