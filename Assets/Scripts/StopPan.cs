using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	private void OnTriggerEnter(Collider other) {
		if(other.GetComponent<CameraPan>() != null) {
			other.GetComponent<CameraPan>().canPan = false;
			other.GetComponent<CameraPan>().posToMove = null;
		}
	}
}
