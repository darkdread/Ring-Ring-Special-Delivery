using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCam : MonoBehaviour {
	public Camera mainCam;

	// Start is called before the first frame update
	void Start() {
		mainCam = Camera.main;
	}

	// Update is called once per frame
	void Update() {
		FaceCamera();
	}
	void FaceCamera() {
		if (mainCam == null) mainCam = Camera.main; // as it does not initialize when the game starts
		transform.LookAt(transform.position + mainCam.transform.rotation * -Vector3.back, mainCam.transform.rotation * -Vector3.down);
	}
}
