using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlatform : MonoBehaviour
{
    public float platformSpd = 2.0f;
    public float platformDist = 5.0f;
    public float platformDelay = 3.0f;

    private bool platformPaused;
    private bool platformReturn;
    private float platformTimer;
    private float platformLimit;

    // Start is called before the first frame update
    void Start()
    {
        platformLimit = transform.position.x + platformDist;
    }

    // Update is called once per frame
    void Update()
    {
        platformTimer += Time.deltaTime;

        //Position values clamped
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, platformLimit - platformDist, platformLimit), transform.position.y, transform.position.z);
        
        //if platform has not reached destination and not returning and delay is over
        if(transform.position.x < platformLimit && !platformReturn && platformTimer >= platformDelay)
        {
            transform.position = new Vector3(transform.position.x + Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
        }

        else if (platformReturn)
        {
            platformPaused = false;
            transform.position = new Vector3(transform.position.x - Time.deltaTime * platformSpd, transform.position.y, transform.position.z);
        
            //if platform has reached its initial position
            if(transform.position.x <= platformLimit - platformDist)
            {
                platformReturn = false;
                platformTimer = 0;
            }
        }
        //if platform has reached its destination 
        else if (transform.position.x >= platformLimit && !platformPaused)
        {
            platformPaused = true;
            platformTimer = 0;
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
