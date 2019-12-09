using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPoint : MonoBehaviour {

	Camera mainCam;
	public Image img;
	public Transform target;
	public Vector3 offset;
	public Text meter;


	void start() {
		mainCam = Camera.main;
        print("started");
	}

	// Update is called once per frame
	void Update() {
		wayPoint();
	}

	void wayPoint() {
		float minX = img.GetPixelAdjustedRect().width / 2;
		float maxX = Screen.width - minX;


		float minY = img.GetPixelAdjustedRect().height / 2;
		float maxY = Screen.width - minX;

		Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

		// this is for when the target is behind the player
		if (Vector3.Dot((target.position - transform.position), transform.forward) < 0) {
			// checks if the target is behind the player
			if (pos.x < Screen.width / 2) {
				pos.x = maxX;
			} else {
				pos.x = minX;
			}
		}

		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.y = Mathf.Clamp(pos.y, minY, maxY);
		img.transform.position = pos;
		meter.text = ((int)Vector3.Distance(target.position, transform.position)).ToString();
	}
}
