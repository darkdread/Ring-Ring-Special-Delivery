using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	public static CameraPan instance;
	public Transform playerPos;
	public Transform posToMove;
	public bool canPan;

	Vector3 offset;
	// Start is called before the first frame update
	void Awake() {
		instance = this;
		canPan = false;
	}
	private void Start() {
		CalculateOffest();
	}
	// Update is called once per frame
	void Update() {
		//FollowPlayer();
		if (posToMove != null && canPan) PanCamera();
	}

	void CalculateOffest() {
		offset = new Vector3(0, transform.position.y - playerPos.position.y, 0);
	}
	public void FollowPlayer() {
		transform.position = Vector3.Lerp(transform.position, playerPos.position + offset, Time.deltaTime * 3);
		print("FollowPlayer");
	}
	public void PanCamera() {
		while (transform.position != posToMove.position) {
			transform.position = Vector3.Lerp(transform.position, posToMove.position, Time.deltaTime * 3);
			transform.rotation = Quaternion.Lerp(transform.rotation, posToMove.rotation, Time.deltaTime * 3);
			print("Panning");
			CalculateOffest();
			return;
		} 
	}
}
