using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuCamera : MonoBehaviour {
	public static menuCamera instance;
	public Transform newView;
	public float delayTime = 0.3f;
	// Start is called before the first frame update

	void Awake() {
		if (newView == null) newView = null;
	}
	void Start() {
		if (instance != null) Destroy(this);
		instance = this;
	}

	// Update is called once per frame
	void Update() {
		delayTime -= Time.deltaTime;
		if (delayTime <= 0) {
			transform.position = Vector3.Lerp(transform.position, newView.position, Time.deltaTime * 10);
			transform.rotation = Quaternion.Lerp(transform.rotation, newView.rotation, Time.deltaTime * 10);
		}

	}
}
