using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour {

	public static CameraPan instance;
	public Transform playerPos;
	public Transform posToMove;
	public bool canPan;

	private PanTrigger currentPanTrigger;

	// Start is called before the first frame update
	void Awake() {
		instance = this;
		canPan = false;
	}

	// Update is called once per frame
	void Update() {
		if (posToMove == null){
			return;
		}

		FollowPlayer();
		if (canPan) {
			PanCamera();
		}
	}

	public void StartPan(PanTrigger pt, Transform panPosition) {
		print("StartPan");
		currentPanTrigger = pt;
		posToMove = panPosition;
		canPan = true;
	}

	public Vector3 CalculateOffset() {
		Vector3 offset = new Vector3(playerPos.position.x - posToMove.position.x, playerPos.position.y - posToMove.position.y, playerPos.position.z - posToMove.position.z);
		offset.x = Mathf.Clamp(offset.x / currentPanTrigger.dividendXyz.x, currentPanTrigger.minXyz.x, currentPanTrigger.maxXyz.x);
		offset.z = Mathf.Clamp(offset.z / currentPanTrigger.dividendXyz.z, currentPanTrigger.minXyz.z, currentPanTrigger.maxXyz.z);
		offset.y = Mathf.Clamp(offset.y / currentPanTrigger.dividendXyz.y, currentPanTrigger.minXyz.y, currentPanTrigger.maxXyz.y);
		return offset;
	}

	public void FollowPlayer() {
		Vector3 offset = CalculateOffset();
		transform.position = Vector3.Lerp(transform.position, posToMove.position + offset, Time.deltaTime * 3);
		print("FollowPlayer");
	}
	public void PanCamera() {
		if (Vector3.Distance(transform.position, posToMove.position) > 0.2f) {
			transform.rotation = Quaternion.Lerp(transform.rotation, posToMove.rotation, Time.deltaTime * 3);
			print("Panning");
		}else if(Vector3.Distance(transform.position, posToMove.position) <= 0.2f){
			canPan = false;
		}
	}
}
