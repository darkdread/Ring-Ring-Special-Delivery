using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CatScript : MonoBehaviour {
	public static CatScript instance;
	private Pathfinding.AIDestinationSetter aIDestinationSetter;

	// this is where the cat should go without helping

	[Header("CatDialogue")]
	public GameObject catCanvas;
	public Text tutorialDialogue;

	// Start is called before the first frame update
	void Start() {
		instance = this;
		catCanvas.SetActive(false);
		aIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
	}

	// Update is called once per frame
	void Update() {

		if (Input.GetKeyDown(KeyCode.F) && !GameManager.instance.player.firstMove) {
			setCatDialogue("Move analog to move", true);
		}

		if (aIDestinationSetter.target != null){
			Transform player = GameManager.instance.player.abovePlayer;

			// If the destination is player.
			if (player == aIDestinationSetter.target){
				// If cat is near player.
				if (Vector3.Distance(transform.position, player.position) < 5f) {
					transform.LookAt(player);
				}
			} else {
				// If cat is at hint.
				if (Vector3.Distance(transform.position, aIDestinationSetter.target.position) < 5f) {
					SetDestination(GameManager.instance.player.abovePlayer);
				}
			}
		}

		if (!Quest.talkedToFirstNpc) setCatDialogue("Talk to Npcs near post office to get quests", true);
	}

	public void SetDestination(Transform target){
		aIDestinationSetter.target = target;
	}

	//==============================================================================================================
	// this are for the cat dialogue
	public void setCatDialogue(string ttDialogue, bool setCanvas) {
		tutorialDialogue.text = ttDialogue;
		catCanvas.SetActive(setCanvas);
	}
}
