using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    private float initialYPos;
    // Start is called before the first frame update
    void Awake()
    {
        initialYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(initialYPos, initialYPos + 2, Mathf.PingPong(Time.time, 1)), transform.position.z);
    }
}
