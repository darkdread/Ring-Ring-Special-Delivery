using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {

	public static MainMenu theMainMenu;
	public GameObject settingsScreen;

	public GameObject firstButton;
	public bool settingsOpened = false;

	public AudioSource mainMenuAudio;

	[Header("Button Animation")]
	[SerializeField] Camera mainCam;

	[Header("Door Animation")]
	public GameObject postOffice;
	public GameObject mainMenu;
	[SerializeField] bool opened;
	public Transform officeView, exitOfficeView;

	[Header("ButtonSounds")]
	[SerializeField] AudioSource audioSource;
	public SoundClips buttonClips;

    [Header("Main Menu CutScene")]
    public PlayableDirector mainMenuTimeLine;

	public GameObject titleScreenText;
    private bool cutScenePlayed;
    private bool cutScenePlaying;
    private bool firstButtonSelected;
    private float cutSceneTimer;
    private float doorTimer;

	public void Start() {

		if (theMainMenu != null) {
			Destroy(this);
		}

		theMainMenu = this;
		// ======================================== this is for the settings screen so that the volume can be updated
		settingsOpened = false;
		if (settingsScreen.activeInHierarchy) settingsScreen.SetActive(false);
		mainMenuAudio.Play();
		Time.timeScale = 1;
		mainCam = Camera.main;
		audioSource = GetComponent<AudioSource>();
	}

	void Update() {
		if (ControlConfigMenu.configMenuOpen) {
			return;
		}

		// If options animation is playing and you want to exit to setings
		if (OptionsAnimation.optionsAnimPlaying && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button"))
			|| OptionsAnimation.optionsAnimPlaying && Input.GetButtonDown("Cancel")) {
			settingsOpened = false;
			OptionsAnimation.optionsAnimPlaying = false;
			settingsScreen.SetActive(false);
			EventSystem.current.SetSelectedGameObject(firstButton);
			return;
		}

		OpenedDoor();
		if(menuCamera.instance.newView == null) menuCamera.instance.newView = exitOfficeView;

		// to open and close the menu after the door has opened
		if(opened && menuCamera.instance.delayTime<=0 && doorTimer > 6.3f) 
		{
			mainMenu.SetActive(true);

			if(!firstButtonSelected)
			{
				EventSystem.current.SetSelectedGameObject(firstButton);
				firstButtonSelected = true;
			}
			
		}
		else if(!opened) mainMenu.SetActive(false);
	}

	public void Settings() {
		if (!settingsOpened) {
			OptionsAnimation.optionsAnimPlaying = true;
			settingsScreen.SetActive(true);
			settingsOpened = true;
		} else if (settingsOpened) {
			settingsScreen.SetActive(false);
			settingsOpened = false;
		}
	}

	// set the animation state of the post office

	public void OpenedDoor() {

		if(cutScenePlaying)
		{
			doorTimer += Time.deltaTime;

			if(doorTimer >= 3.3f && doorTimer < 3.5f)
			{
				opened = true;
			}
		}

		if (!opened) {
			if (Input.anyKeyDown) {
				cutScenePlaying = true;
				titleScreenText.SetActive(false);
				if(cutScenePlayed) opened = true;
                // menuCamera.instance.newView = officeView;
				menuCamera.instance.delayTime = 0.8f;
			
                if(!cutScenePlayed)
                {
                    mainMenuTimeLine.Play();
                }
			}
		} else if (opened) {
            cutSceneTimer += Time.deltaTime;

            if(cutSceneTimer >= 3f)
            {
                menuCamera.instance.gameObject.GetComponent<CinemachineBrain>().enabled = false;
            }

			if (!OptionsAnimation.optionsAnimPlaying && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button") || Input.GetButtonDown("Cancel"))) {
                if(cutScenePlayed || cutSceneTimer >= 3f)
                {
                    menuCamera.instance.gameObject.GetComponent<CinemachineBrain>().enabled = false;
                    opened = false;
                    menuCamera.instance.newView = exitOfficeView;
                    EventSystem.current.SetSelectedGameObject(null);
                    mainMenu.SetActive(false);
                    cutSceneTimer = 0;
                    cutScenePlayed = true;
					firstButtonSelected = false;
                }
			}
		}
		postOffice.GetComponent<Animator>().SetBool("Opened", opened);
	}
	public void LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}
	public void ExitGame() {
		Application.Quit();
		print("Quit");
	}

	// play the button sound
	public void ButtonSound(){
		audioSource.clip = buttonClips.clip[0];
		audioSource.Play();
	}
}
