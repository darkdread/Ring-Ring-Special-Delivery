using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour {
	public GameObject settingsScreen;
	public bool settingsOpened = false;

	// audio related
	public AudioSource currentAudio;

	//set this to close
	public GameObject objectToClose;
	public Animator menuButtons;
	public void Start() {
		// ======================================== this is for the settings screen so that the volume can be updated
		settingsOpened = false;
		if (settingsScreen.activeInHierarchy) settingsScreen.SetActive(false);
		if (objectToClose != null) if (objectToClose.activeInHierarchy) objectToClose.SetActive(false);
		if (currentAudio != null)
			if (!currentAudio.isPlaying) currentAudio.Play();
		if (currentAudio == null) currentAudio = null;

	}

	void Update() {

	}

	public void LoadScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
	}

	public void Settings() {
		if (!settingsOpened) {
			// Starts options animation
			OptionsAnimation.optionsAnimPlaying = true;
			// Invoke("ShowSettings", 0.5f);
			settingsOpened = true;
		} else if (settingsOpened) {
			settingsScreen.SetActive(false);
			settingsOpened = false;
		}
	}

	void ShowSettings() {
		settingsScreen.SetActive(true);
	}



	public void ExitGame() {
		Application.Quit();
		print("Quit");
	}
}
