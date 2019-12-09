using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUITutorial : MonoBehaviour {

	// Accessing components
	GameManager theGameManager;
	// Ui instructions related
	public List<GameObject> UiInstructions; // stores all the GUI instructions
	public int GuiClosed = -1; // negative one because list starts from 0
	// variables
	public static GUITutorial instance; // makes a singleton for others scripts to access

	// Start is called before the first frame update
	void Start() {
		// Asssigning variables
		instance = this;
		GuiClosed = -1;
		
		// acccesing components
		theGameManager = GameManager.instance; 
	}

	// Called this function under the dialogue manager so as to ensure that it will only appear once the dialogue is done
	public IEnumerator OpenGuiInstuctions(int index) {
		GameManager.ShowQuestMarkers(false);

		yield return new WaitForSeconds(.5f);
		if (!DialogueManager.isDialogue) {
			UiInstructions[index].SetActive(true);

			// First instruction, show slider.
			if (index == 0){
				GameManager.instance.SpawnSlider(GameManager.questCurrent.questData);
			}

			GameManager.paused = true;
			yield return new WaitForSeconds(2f);
			theGameManager.eventManager.SetSelectedGameObject(UiInstructions[index].GetComponentInChildren<Button>().gameObject); // sets the button as selected so the player can interact 
		}
	}
	public void CloseGuiInstuctions() {
		GuiClosed++;
		
		GameManager.instance.waitTime = 0.2f;
		GameManager.paused = false;

		GameManager.ShowQuestMarkers(true);
        
		UiInstructions[GuiClosed].SetActive(false); // call this last so that the rest of the functions will run
	}
}
