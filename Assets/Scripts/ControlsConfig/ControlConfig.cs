using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlConfig : MonoBehaviour
{
    struct PadButton
    {
        public string btnInput;
        public ButtonType buttonType;
    }

    public struct CharAction
    {
        public string btnInput;
        public bool isTriggerInput; 
        public int whichTrigger;
        public int whichButton;
    }

    public enum ButtonType{ UNBINDED, JUMP, DASH, INTERACT, HINT};
    private PadButton[] padButtons;
    private PadButton[] padTriggers;
    private ButtonType buttonPressed;

    public static CharAction jumpAction, dashAction, interactAction, hintAction;

    public static bool changingInput;
    private bool triggerInput;

    private float changeInputTimer;

    public static ControlConfig controlConfigInstance;

    // Start is called before the first frame update
    void Start()
    {
        if(controlConfigInstance == null)
        {
            controlConfigInstance = this;
        }

        padButtons = new PadButton[6];
        padTriggers = new PadButton[2];
        jumpAction = new CharAction();
        dashAction = new CharAction();
        interactAction = new CharAction();
        hintAction = new CharAction();

        // Clear all input
        // PlayerPrefs.DeleteAll();

        // Gets Values from PlayerPrefs
        CheckIfFirstTime();
        InitalizeInput();

    }

    // Update is called once per frame
    void Update()
    {
        // Debug Purposes
        //  for (int i = 0; i < 6; i++) {
        //          print("Button " + i + " Type: " + padButtons[i].buttonType);
        //  }

        //  for (int i = 0; i < 2; i++) {
        //          print("Trigger " + i + " Type: " + padTriggers[i].buttonType);
        //  }

        //  print("Jump Input: " + jumpAction.btnInput);
        //  print("Dash Input: " + dashAction.btnInput);
        //  print("Interact Input: " + interactAction.btnInput);

        if(changingInput)
        {
            changeInputTimer += Time.fixedUnscaledDeltaTime;
        }
        else if(!changingInput)
        {
            changeInputTimer = 0;
        }


        if(changingInput && changeInputTimer > 0.25f)
        {
            // For Buttons
            for (int i = 0; i < 6; i++) {
                if(Input.GetKeyDown("joystick button "+ i)){
                    // print("joystick button "+ i);
                    triggerInput = false;
                    SetInputBtn(i);
                    return;
                }
            }

            // For Triggers
            if(Input.GetAxis("LeftTrigger") > 0)
            {
                triggerInput = true;
                
                // 0 for Left Trigger
                SetInputBtn(0);
            }
            else if( Input.GetAxis("RightTrigger") > 0)
            {
                triggerInput = true;

                // 1 for Right Trigger
                SetInputBtn(1);
            }

        }
    }

    public bool CharacterAction(CharAction charAction)
    {
        if(charAction.isTriggerInput)
        {
            if(Input.GetAxis(padTriggers[charAction.whichTrigger].btnInput) > 0)
            {
                return true;
            }
        }
        else if (!charAction.isTriggerInput){
            if(Input.GetKeyDown(charAction.btnInput))
            {
                return true;
            }
        }

        return false;
    }

    private void SetInputBtn(int padBtnNum)
    {
        changeInputTimer = 0;

        // For Buttons
        if(!triggerInput)
        {
            // Unbinds input for Trigger
            for (int i = 0; i < 2; i++)
            {
                if(padTriggers[i].buttonType == buttonPressed)
                {
                    padTriggers[i].buttonType = padButtons[padBtnNum].buttonType;
                    PlayerPrefs.SetInt("PadTrigger" + i, (int)padButtons[padBtnNum].buttonType);
                    PlayerPrefs.Save();
                }
            }
            // Binds input to button 
            for (int i = 0; i < 6; i++)
            {
                // Check if set input exists
                if(padButtons[i].buttonType == buttonPressed)
                {
                    // Swaps input
                    padButtons[i].buttonType = padButtons[padBtnNum].buttonType;
                    PlayerPrefs.SetInt("PadBtn" + i, (int)padButtons[padBtnNum].buttonType);

                    // Sets new input
                    padButtons[padBtnNum].buttonType = buttonPressed;
                    PlayerPrefs.SetInt("PadBtn" + padBtnNum, (int)buttonPressed);
                    PlayerPrefs.Save();
                }
            }

            padButtons[padBtnNum].buttonType = buttonPressed;
            PlayerPrefs.SetInt("PadBtn" + padBtnNum, (int)buttonPressed);
            PlayerPrefs.Save();
        }

        // For Triggers
        if(triggerInput)
        {
            // Unbinds input for button 
            for (int i = 0; i < 6; i++)
            {
                // Check if set input exists
                if(padButtons[i].buttonType == buttonPressed)
                {
                    // Swaps input
                    padButtons[i].buttonType = padTriggers[padBtnNum].buttonType;
                    PlayerPrefs.SetInt("PadBtn" + i, (int)padTriggers[padBtnNum].buttonType);
                    PlayerPrefs.Save();
                }
            }

            // Binds input to Trigger 
            for (int i = 0; i < 2; i++)
            {
                // Check if set input exists
                if(padTriggers[i].buttonType == buttonPressed)
                {
                    // Swaps Input 
                    padTriggers[i].buttonType = padTriggers[padBtnNum].buttonType;
                    PlayerPrefs.SetInt("PadTrigger" + i, (int)padTriggers[padBtnNum].buttonType);

                    // Sets new input
                    padTriggers[padBtnNum].buttonType = buttonPressed;
                    PlayerPrefs.SetInt("PadTrigger" + padBtnNum, (int)buttonPressed);
                    PlayerPrefs.Save();
                }
            }

            padTriggers[padBtnNum].buttonType = buttonPressed;
            PlayerPrefs.SetInt("PadTrigger" + padBtnNum, (int)buttonPressed);
            PlayerPrefs.Save();
        }
        
        // Sets input for buttons and triggers
        switch(buttonPressed)
        {
            case ButtonType.JUMP:
                if(triggerInput)
                {
                    jumpAction.btnInput = padTriggers[padBtnNum].btnInput;
                    jumpAction.isTriggerInput = true;
                    jumpAction.whichTrigger = padBtnNum;
                    triggerInput = false;
                }
                else{
                    jumpAction.btnInput = padButtons[padBtnNum].btnInput;
                    jumpAction.isTriggerInput = false;
                    jumpAction.whichButton = padBtnNum;
                }
                break;

            case ButtonType.DASH:
                if(triggerInput)
                {
                    dashAction.btnInput = padTriggers[padBtnNum].btnInput;
                    dashAction.isTriggerInput = true;
                    dashAction.whichTrigger = padBtnNum;
                    triggerInput = false;
                }
                else{
                    dashAction.btnInput = padButtons[padBtnNum].btnInput;
                    dashAction.isTriggerInput = false;
                    dashAction.whichButton = padBtnNum;
                }
                break;

            case ButtonType.INTERACT:
                if(triggerInput)
                {
                    interactAction.btnInput = padTriggers[padBtnNum].btnInput;
                    interactAction.isTriggerInput = true;
                    interactAction.whichTrigger = padBtnNum;
                    triggerInput = false;
                }
                else{
                    interactAction.btnInput = padButtons[padBtnNum].btnInput;
                    interactAction.isTriggerInput = false;
                    interactAction.whichButton = padBtnNum;
                }
                break;

            case ButtonType.HINT:
                if (triggerInput)
                {
                    hintAction.btnInput = padTriggers[padBtnNum].btnInput;
                    hintAction.isTriggerInput = true;
                    hintAction.whichTrigger = padBtnNum;
                    triggerInput = false;
                }
                else
                {
                    hintAction.btnInput = padButtons[padBtnNum].btnInput;
                    hintAction.isTriggerInput = false;
                    hintAction.whichButton = padBtnNum;
                }
                break;
        }
        changingInput = false;
        InitalizeInput();
    }

    public void ChangeInput(string buttonType)
    {
        changingInput = true;

        switch(buttonType)
        {
            case "Jump":
                buttonPressed = ButtonType.JUMP;
                break;

            case "Dash":
                buttonPressed = ButtonType.DASH;
                break;

            case "Interact":
                buttonPressed = ButtonType.INTERACT;
                break;

            case "Hint":
                buttonPressed = ButtonType.HINT;
                break;

            default:
                Debug.LogError("No button type Assigned!");
                return;
        }
    }

    private void CheckIfFirstTime()
    {
        if(!PlayerPrefs.HasKey("InputExists"))
        {
            SetDefaultInput();
        }
    }

    public void SetDefaultInput()
    {
        PlayerPrefs.DeleteAll();

        for (int i = 0; i < 6; i++)
        {
            if(i == 0)
            {
                // 1 for enum ButtonType.JUMP
                PlayerPrefs.SetInt("PadBtn0", 1);
                
            }
            else if(i == 3)
            {
                // 3 for enum ButtonType.INTERACT
                PlayerPrefs.SetInt("PadBtn3", 3);
            }
            else if(i == 2)
            {
                // 4 for enum ButtonType.HINT
                PlayerPrefs.SetInt("PadBtn2", 4);
            }
            else
            {
                // 0 for enum ButtonType.UNBINDED
                PlayerPrefs.SetInt("PadBtn" + i, 0);
            }
        }

        // 2 for enum ButtonType.DASH
        PlayerPrefs.SetInt("PadTrigger1", 2);

        // For Initializing input values
        PlayerPrefs.SetInt("InputExists", 1);

        // Initializing pad button values
        for (int i = 0; i < 6; i++)
        {
            padButtons[i].btnInput = "joystick button " + i;
            padButtons[i].buttonType = (ButtonType)PlayerPrefs.GetInt("PadBtn" + i);
            // print("I = " + i + " " + PlayerPrefs.GetInt("PadBtn" + i));
        }

        InitalizeInput();
        SaveInput();
    }

    private void InitalizeInput(){

        // Initializing pad button values
        for (int i = 0; i < 6; i++)
        {
            padButtons[i].btnInput = "joystick button " + i;
            padButtons[i].buttonType = (ButtonType)PlayerPrefs.GetInt("PadBtn" + i);
            // print("I = " + i + " " + PlayerPrefs.GetInt("PadBtn" + i));

            // Initializing action inputs
            if(padButtons[i].buttonType == ButtonType.JUMP)
            {
                jumpAction.btnInput = padButtons[i].btnInput;
                jumpAction.isTriggerInput = false;

                // 0 - 6 for pad buttons
                jumpAction.whichButton = i;
            }
            else if(padButtons[i].buttonType == ButtonType.DASH)
            {
                dashAction.btnInput = padButtons[i].btnInput;
                dashAction.isTriggerInput = false;

                // 0 - 6 for pad buttons
                dashAction.whichButton = i;
            }   
            else if(padButtons[i].buttonType == ButtonType.INTERACT)
            {
                interactAction.btnInput = padButtons[i].btnInput;
                interactAction.isTriggerInput = false;

                // 0 - 6 for pad buttons
                interactAction.whichButton = i;
            }

            else if (padButtons[i].buttonType == ButtonType.HINT)
            {
                hintAction.btnInput = padButtons[i].btnInput;
                hintAction.isTriggerInput = false;

                // 0 - 6 for pad buttons
                hintAction.whichButton = i;
            }
        }

        // Initializing pad trigger values
        padTriggers[0].btnInput = "LeftTrigger";
        padTriggers[1].btnInput = "RightTrigger";

        for (int i = 0; i < 2; i++)
        {
            padTriggers[i].buttonType = (ButtonType)PlayerPrefs.GetInt("PadTrigger" + i);

            // Initializing action inputs
            if(padTriggers[i].buttonType == ButtonType.JUMP)
            {
                jumpAction.btnInput = padTriggers[i].btnInput;
                jumpAction.isTriggerInput = true;

                // 0 for Left, 1 for Right
                jumpAction.whichTrigger = i;
            }
            else if(padTriggers[i].buttonType == ButtonType.DASH)
            {
                dashAction.btnInput = padTriggers[i].btnInput;
                dashAction.isTriggerInput = true;

                // 0 for Left, 1 for Right
                dashAction.whichTrigger = i;
            }   
            else if(padTriggers[i].buttonType == ButtonType.INTERACT)
            {
                interactAction.btnInput = padTriggers[i].btnInput;
                interactAction.isTriggerInput = true;

                // 0 for Left, 1 for Right
                interactAction.whichTrigger = i;
            }
            else if (padTriggers[i].buttonType == ButtonType.HINT)
            {
                hintAction.btnInput = padTriggers[i].btnInput;
                hintAction.isTriggerInput = true;

                // 0 for Left, 1 for Right
                hintAction.whichTrigger = i;
            }
        }
    }

    public void SaveInput()
    {
        PlayerPrefs.Save();
    }
}