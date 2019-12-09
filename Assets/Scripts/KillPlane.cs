using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour
{
   void OnTriggerEnter(Collider other){
       if(other.tag == "Player"){
           StartCoroutine(GameManager.instance.Respawn());
       }
   }
}
