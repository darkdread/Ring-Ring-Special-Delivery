using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagonalPlatform : MonoBehaviour
{
    public float platformSpd = 2.0f;
    public float platformDistX = 5.0f;
    public float platformDistZ = 5.0f;
    public float platformDelay = 3.0f;

    private bool platformPaused;
    private bool platformReturn;
    private float platformTimer;
    private float platformLimitX;
    private float platformLimitZ;

    // Start is called before the first frame update
    void Start()
    {
        platformLimitX = transform.position.x + platformDistX;
        platformLimitZ = transform.position.z + platformDistZ;
    }

    // Update is called once per frame
    void Update()
    {
        // print("Paused : " + platformPaused);
        // print("Return : " + platformReturn);

        print(platformLimitX);
        print(transform.position.x - platformLimitX);

        platformTimer += Time.deltaTime;
        
        //if platform has not reached X and Z destinations and is not returning while delay is over
        if(!platformReturn && platformTimer >= platformDelay)
        {
            if(transform.position.x < platformLimitX)
            {
                transform.position = new Vector3(transform.position.x + Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
            }

            if(transform.position.z < platformLimitZ)
            {
                transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z + Time.deltaTime * platformSpd);
            }

            //transform.position = new Vector3(transform.position.x + Time.deltaTime * platformSpd, transform.position.y, transform.position.z + Time.deltaTime * platformSpd);
        }
        //if platform has reached its destination 
        if (transform.position.x >= platformLimitX && transform.position.z >= platformLimitZ && !platformPaused)
        {
            platformPaused = true;
            platformTimer = 0;
        }
        else if (platformReturn)
        {
            platformPaused = false;
            //transform.position = new Vector3(transform.position.x - Time.deltaTime * platformSpd, transform.position.y, transform.position.z - Time.deltaTime * platformSpd);
        
            if(transform.position.x > platformLimitX - platformDistX)
            {
                transform.position = new Vector3(transform.position.x - Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
            }

            if(transform.position.z > platformLimitZ - platformDistZ)
            {
                transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z - Time.deltaTime * platformSpd);
            }

            //if platform has reached its initial position
            if(transform.position.x <= platformLimitX - platformDistX && transform.position.z <= platformLimitZ - platformDistZ)
            {
                platformReturn = false;
                platformTimer = 0;
            }
        }
        

        if(platformPaused)
        {
            if(platformTimer >= platformDelay)
            {
                platformReturn = true;
            }
        }
    }
}
