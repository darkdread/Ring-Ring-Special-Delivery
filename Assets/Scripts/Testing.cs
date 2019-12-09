using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.VFX;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] VisualEffect vfx;

    void Start()
    {
        vfx = GetComponent<VisualEffect>();
        vfx.Play();
        Destroy(gameObject,1f);
    }
}
