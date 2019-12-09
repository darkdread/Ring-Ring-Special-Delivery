using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
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
        platformLimit = transform.position.y + platformDist;
    }

    // Update is called once per frame
    void Update()
    {
        platformTimer += Time.deltaTime;

        //Position values clamped
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, platformLimit - platformDist, platformLimit), transform.position.z);
        
        //if platform has not reached destination and not returning and delay is over
        if(transform.position.y < platformLimit && !platformReturn && platformTimer >= platformDelay)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * platformSpd, transform.position.z);
        }

        else if (platformReturn)
        {
            platformPaused = false;
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * platformSpd, transform.position.z);
        
            //if platform has reached its initial position
            if(transform.position.y <= platformLimit - platformDist)
            {
                platformReturn = false;
                platformTimer = 0;
            }
        }
        //if platform has reached its destination 
        else if (transform.position.y >= platformLimit && !platformPaused)
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
