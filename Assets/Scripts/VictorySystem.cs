using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictorySystem : MonoBehaviour
{
    public GameObject victoryCanvas;
    public Text timeText;
    public Text titleText;

    public float goldTime;
    public float silverTime;

    private float levelTimer;
    private bool alreadyWon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        levelTimer += Time.deltaTime;
        // print(Time.time);
    }

    private void OnTriggerEnter(Collider other) {
        victoryCanvas.SetActive(true);

        if(!alreadyWon)
        {
            if(levelTimer <= goldTime)
            {
                titleText.text = "Gold";
            }
            else if(levelTimer <= silverTime)
            {
                titleText.text = "Silver";
            }
            else 
            {
                titleText.text = "Bronze";
            }
        }
        
        alreadyWon = true;
        timeText.text = "Time: " + levelTimer.ToString("F3") + " Seconds";
    }
}
