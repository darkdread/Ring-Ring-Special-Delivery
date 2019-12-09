using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerPos : MonoBehaviour {
	// components
	private GameManager theGameManager;

	// for maker related properties
	[Header("marker")]
	public Transform marker;
	public Transform nextMarkerPos;

	public bool turnOff;

	// Start is called before the first frame update
	void Start() {
		//accessing components
		theGameManager = GameManager.instance;

		//setting variables
		marker = theGameManager.tutorialMarker.transform; // get the marker which is stored in the Gamemanager
	}

	void Update() {

	}

	// changes the marker Position
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PlayerController>() != null) {
			// this is for setting the marker
			if ((nextMarkerPos != null) && Quest.talkedToFirstNpc) marker.position = nextMarkerPos.position;
			marker.gameObject.SetActive(true);
			if(turnOff) marker.gameObject.SetActive(false);
		}
	}
}
