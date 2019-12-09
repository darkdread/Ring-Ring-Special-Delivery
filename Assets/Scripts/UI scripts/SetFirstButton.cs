using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetFirstButton : MonoBehaviour
{
    public GameObject firstButton;
    public void SetSelectedButton()
    {
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}
