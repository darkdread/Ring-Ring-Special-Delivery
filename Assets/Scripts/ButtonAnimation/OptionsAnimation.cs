using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsAnimation : MonoBehaviour
{
    // Static value is adjusted inside LevelManager & GameManager
    // One Line per script 
    public static bool optionsAnimPlaying;
    // public static bool optionsAnimPlaying;

    private float btnVelocity = 0.0f;


    // Original y coordinates of buttons
    private float originalOptionsY;
    private float originalContinueY;

    private Transform optionsTrans;
    private Transform continueTrans;
    private Transform backToMainTrans;

    private RectTransform continueRectTrans;
    private RectTransform optionsRectTrans;

    // Start is called before the first frame update
    void Start()
    {
        // Finding gameobjects 
        // Exact Names please
        optionsTrans = transform.Find("Options");
        continueTrans = transform.Find("Continue");
        backToMainTrans = transform.Find("BackToMain");

        // If buttons can't be found
        if(!optionsTrans || !continueTrans || !backToMainTrans)
        {
            continueTrans = transform.Find("StartGame");
            backToMainTrans = transform.Find("Quit");
            return;
        }

        continueRectTrans = continueTrans.GetComponent<RectTransform>();
        optionsRectTrans = optionsTrans.GetComponent<RectTransform>();

        originalContinueY = continueRectTrans.anchoredPosition.y;
        originalOptionsY = optionsRectTrans.anchoredPosition.y;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(optionsAnimPlaying)
        {
            // Moving Buttons
            MoveButtonY(continueRectTrans, 70f, 0.2f);
            MoveButtonY(optionsRectTrans, 20f, 0.2f);

            // Fading Text
            FadeOutText(continueTrans, true);
            FadeOutText(backToMainTrans, true);

            // Options Image Fade
            OptionsImageFade(optionsTrans, false);


        }
        else if (!optionsAnimPlaying)
        {
            // ResetAnimation();
            ReverseAnimation();
        }
        
    }

    void MoveButtonY(RectTransform btnRectTrans, float moveY, float timeTaken)
    {
        btnRectTrans.anchoredPosition = new Vector2(btnRectTrans.anchoredPosition.x, Mathf.SmoothDamp(btnRectTrans.anchoredPosition.y, moveY, ref btnVelocity, timeTaken, 500f, Time.fixedUnscaledDeltaTime));
    }

    void FadeOutText(Transform btnTrans, bool fading)
    {
        Button curButton = btnTrans.GetComponent<Button>();

        // Set button state to disabled for disabled animation to play
        curButton.interactable = !fading;

    }

    // Only for Options!
    void OptionsImageFade(Transform btnTrans, bool fading)
    {
        Button curButton = btnTrans.GetComponent<Button>();

        // Set button state to disabled for disabled animation to play
        curButton.interactable = fading;

    }

    void ResetAnimation()
    {
        Button continueBtn = continueTrans.GetComponent<Button>();
        Button optionsBtn = optionsTrans.GetComponent<Button>();
        Button backToMainBtn = backToMainTrans.GetComponent<Button>();

        // Reset Y positions
        continueRectTrans.anchoredPosition = new Vector2(continueRectTrans.anchoredPosition.x, originalContinueY);
        optionsRectTrans.anchoredPosition = new Vector2(optionsRectTrans.anchoredPosition.x, originalOptionsY);

        // Set Interactable for animations
        continueBtn.interactable = true;
        optionsBtn.interactable = true;
        backToMainBtn.interactable = true;

        
    }

    void ReverseAnimation()
    {
        // Moving Buttons
            MoveButtonY(continueRectTrans, originalContinueY, 0.2f);
            MoveButtonY(optionsRectTrans, originalOptionsY, 0.2f);

            // Fading Text
            FadeOutText(continueTrans, false);
            FadeOutText(backToMainTrans, false);

            // Options Image Fade
            OptionsImageFade(optionsTrans, true);
    }
}
