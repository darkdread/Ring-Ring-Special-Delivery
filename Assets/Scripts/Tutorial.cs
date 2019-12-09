using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tutorialType { start, normal, dashTutorial, doubleJump, walljump }
public class Tutorial : MonoBehaviour {
	public tutorialType ttType; // this is for the enum

	// these are the vairables to set the players jump dash and walljump limits
	[SerializeField] PlayerController thePlayer;
	public int jumpAmount, dashAmount;
	public bool wallJumpState;

	[Header("CatDialogue")]
	public string tutorialDialogue;


	// this is for the initialization part of the player
	public bool reset;

	///<summary>
	/// Sets the player jump and dash limit and also sets whether the player can walljump
	///</summary>
	void SetPlayerLimit(int setJump, int setDash, bool canWallJp) {
		thePlayer.jumpLimit = setJump;
		thePlayer.dashLimit = setDash;
		thePlayer.canWallJump = canWallJp;
	}

	// ------------------------------------------------------------------------------------------------
	// the below functions will handle on the ontrigger components
	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<PlayerController>() != null) {
			thePlayer = other.GetComponent<PlayerController>();

			if (Quest.talkedToFirstNpc) {
				CatScript.instance.setCatDialogue(tutorialDialogue, true);

				// this is for so that you cannot proceed unless you talked to the npc
			} else if (!Quest.talkedToFirstNpc && ttType == tutorialType.doubleJump) {
				return;
			}

			SetPlayerLimit(jumpAmount, dashAmount, wallJumpState);
			thePlayer.theTutorial = this;

			if (reset) {
				CatScript.instance.setCatDialogue(tutorialDialogue, false);
			}
			print(Quest.talkedToFirstNpc);
		}
	}

	// close the cat canvas in the player script under the dash and jump functions
	void OnTriggerStay(Collider other) {

		if ((other.GetComponent<PlayerController>() != null) && ttType == tutorialType.dashTutorial) {
			if (other.GetComponent<PlayerController>().isDashing) {
				CatScript.instance.setCatDialogue(tutorialDialogue, false);
				this.gameObject.SetActive(false);
			}
		} else if ((other.GetComponent<PlayerController>() != null) && ttType == tutorialType.walljump) {
			if (other.GetComponent<PlayerController>().WallJump()) {
				CatScript.instance.setCatDialogue(tutorialDialogue, false);
				this.gameObject.SetActive(false);
			}
		} else if ((other.GetComponent<PlayerController>() != null)) {
			GameManager.instance.player.theTutorial = this;
		}

		// this is to set the player setting when the game starts
		if ((other.GetComponent<PlayerController>() != null) && ttType == tutorialType.start) {
			SetPlayerLimit(jumpAmount, dashAmount, wallJumpState);
		}
	}
	void OnTriggerExit(Collider other) {
		if (ttType == tutorialType.start && other.GetComponent<PlayerController>() != null) {
			gameObject.SetActive(false);
		}
		if (other.GetComponent<PlayerController>() != null) {
			if (ttType == tutorialType.normal && Quest.talkedToFirstNpc) {
				gameObject.SetActive(false);
			} else if (ttType == tutorialType.doubleJump && Quest.talkedToFirstNpc) {
				gameObject.SetActive(false);
			}
		}

	}
}
