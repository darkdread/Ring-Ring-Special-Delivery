using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPan : MonoBehaviour
{
    
	private void OnTriggerEnter(Collider other) {
		// if(other.GetComponent<CameraPan>() != null) {
		// 	other.GetComponent<CameraPan>().canPan = false;
		// 	other.GetComponent<CameraPan>().posToMove = null;
		// }
	}
}
