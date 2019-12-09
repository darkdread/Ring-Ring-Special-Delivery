using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WindArea : MonoBehaviour
{
    public float windStrength = 30.0f;
    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            playerRb.AddForce(playerRb.transform.up * windStrength, ForceMode.Force);
        }
        
    }
}
