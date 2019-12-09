using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    public float zoomSpeed = 5.0f;

    private ThirdPersonCamera mainCam;

    private float initialCamDistance;
    private bool isColliding;
    private bool atOrigin;
    private Vector3 currentCamPos;
    private Vector3 playerPos;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = ThirdPersonCamera.instance;
        initialCamDistance = ThirdPersonCamera.camDistance;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCam.transform.position;

        if(ThirdPersonCamera.camDistance <= initialCamDistance && !isColliding && !atOrigin)
            {
                
                print("NOT PLAYER");
                ThirdPersonCamera.camDistance += Time.deltaTime * zoomSpeed;
                isColliding = false;
            }

        atOrigin = ThirdPersonCamera.camDistance <= 7.0f ? false : true;
        
    }

    private void OnCollisionStay(Collision other) {
        if(ThirdPersonCamera.camDistance >= 3.0f)
        {
            ThirdPersonCamera.camDistance -= Time.deltaTime * zoomSpeed;
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        StartCoroutine("StartDelay");
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1.5f);
        isColliding = false;
    }

}
