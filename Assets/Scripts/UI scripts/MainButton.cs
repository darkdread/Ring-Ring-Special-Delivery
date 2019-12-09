using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainButton : MonoBehaviour {
	// this script is for the animation events
	// public Image buttonImage;
	RectTransform buttonTransform;

	public Transform cameraView;

	public void AnimateButton() {

		menuCamera.instance.newView = cameraView;
		menuCamera.instance.delayTime = .25f;
		// print("hehe");
	}

}
