using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	public static CameraPan instance;
	public Transform posToMove;
	// Start is called before the first frame update
	void Awake() {
		instance = this;
	}

	// Update is called once per frame
	void Update() {
		if (posToMove != null) PanCamera();
	}

	public void PanCamera() {
		while (transform.position != posToMove.position) {
			transform.position = Vector3.Lerp(transform.position, posToMove.position, Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, posToMove.rotation, Time.deltaTime);
			return;
		}
	}
}
