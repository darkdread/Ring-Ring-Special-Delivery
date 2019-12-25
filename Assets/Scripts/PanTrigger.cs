using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanTrigger : MonoBehaviour {
	public Transform newCamPos;
	public Vector3 minXyz = Vector3.one * 5f, maxXyz = Vector3.one * 5f;
	public Vector3 dividendXyz = Vector3.one;
	private Vector3 _minXyz = Vector3.zero, _maxXyz = Vector3.zero;
	private Vector3 _dividendXyz = Vector3.zero;
	public bool defaultCamPos = false;

	private void Awake(){
		_minXyz = minXyz;
		_maxXyz = maxXyz;
		_dividendXyz = dividendXyz;
	}

	public void GoToStoredPos(){
		if (defaultCamPos) {
			minXyz = Vector3.zero;
			maxXyz = Vector3.zero;
			dividendXyz = Vector3.one;
		} else {
			minXyz = _minXyz;
			maxXyz = _maxXyz;
			dividendXyz = _dividendXyz;
		}
	}

	public void StoreVectors(){
		_minXyz = minXyz;
		_maxXyz = maxXyz;
		_dividendXyz = dividendXyz;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			CameraPan.instance.StartPan(this, newCamPos);
		}
	}
}
