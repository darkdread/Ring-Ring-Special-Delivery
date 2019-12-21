using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanTrigger : MonoBehaviour {
	public Transform newCamPos;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			CameraPan.instance.posToMove = newCamPos;
			CameraPan.instance.canPan = true;
			CameraPan.instance.PanCamera();
		}
	}
}
