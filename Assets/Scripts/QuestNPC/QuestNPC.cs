using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestNPC", menuName = "ScriptableObjects/QuestNPC", order = 1)]
public class QuestNPC : ScriptableObject
{
    public string npcName;
    public string questName;
    public float goldTime, silverTime, bronzeTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
