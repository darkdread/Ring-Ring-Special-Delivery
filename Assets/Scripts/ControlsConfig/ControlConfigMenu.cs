using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlConfigMenu : MonoBehaviour
{
    public Sprite[] padButtonSprites;
    public Sprite[] padTriggerSprites;

    // Jump, Dash, Interact, Hint Images
    public Image[] buttonImages;

    public GameObject configScreen;
    public GameObject controlsBtn;
    public GameObject anyKey;

    public static bool configMenuOpen;

    // Update is called once per frame
    void Update()
    {
        // Shows any key text if changing input
        anyKey.SetActive(ControlConfig.changingInput);

        // For Jump Button        
        if(ControlConfig.jumpAction.isTriggerInput)
        {
            buttonImages[0].sprite = padTriggerSprites[ControlConfig.jumpAction.whichTrigger];
        }
        else if (!ControlConfig.jumpAction.isTriggerInput)
        {
            buttonImages[0].sprite = padButtonSprites[ControlConfig.jumpAction.whichButton];
        }

        // For Dash Button        
        if(ControlConfig.dashAction.isTriggerInput)
        {
            buttonImages[1].sprite = padTriggerSprites[ControlConfig.dashAction.whichTrigger];
        }
        else if (!ControlConfig.dashAction.isTriggerInput)
        {
            buttonImages[1].sprite = padButtonSprites[ControlConfig.dashAction.whichButton];
        }

        // For Interact Button        
        if(ControlConfig.interactAction.isTriggerInput)
        {
            buttonImages[2].sprite = padTriggerSprites[ControlConfig.interactAction.whichTrigger];
        }
        else if (!ControlConfig.interactAction.isTriggerInput)
        {
            buttonImages[2].sprite = padButtonSprites[ControlConfig.interactAction.whichButton];
        }

        // For Hint Button        
        if (ControlConfig.hintAction.isTriggerInput)
        {
            buttonImages[3].sprite = padTriggerSprites[ControlConfig.hintAction.whichTrigger];
        }
        else if (!ControlConfig.interactAction.isTriggerInput)
        {
            buttonImages[3].sprite = padButtonSprites[ControlConfig.hintAction.whichButton];
        }

    }

    public void ShowConfigMenu()
    {
        configScreen.SetActive(!configMenuOpen);
        configMenuOpen = configScreen.activeSelf;

        if(!configMenuOpen)
        {
            EventSystem.current.SetSelectedGameObject(controlsBtn);
        }
    }

    public void SelectFirstButton()
    {
        EventSystem.current.SetSelectedGameObject(buttonImages[0].transform.parent.gameObject);
    }
}
