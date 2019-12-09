using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum Award {
	Gold,
	Silver,
	Bronze,
	Participation
}

public class Quest : MonoBehaviour {
	// this is the only for the first npc
	public bool firstNpc;
	public static bool talkedToFirstNpc;
	public string firstNotif;
	public float timerSet;

	[Header("Npc Data")]
	public QuestNPC questNPC;
	public string questName;
	public QuestData questData;
	public bool isQuestReceived, isQuestCompleted;
	public Transform destination;
	public Transform arrowIcon;

	private float totalQuestTime;
	private float questTimer;

	private float initialYPos;

	private Vector3 hoverVelocity;

	[Header("Dialogue for Npc")]
	public Dialogue dialogue;

	//this is to store the hint positions
	[Header("Hints")]
	public Transform questHintHolder;
	public List<Transform> questHints;
	public int questHintCurrent;

	[Header("UI")]
	public Image questMarker;
	public Text questMarkerText;
	public Canvas questInteractCanvas;
	public Vector3 offset;

	private void Start() {
		initialYPos = arrowIcon.position.y;
		hoverVelocity = Vector3.up * GameManager.instance.hoverSpeed * Time.deltaTime;
		isQuestReceived = false;
		totalQuestTime = questData.AwardBronze;
		talkedToFirstNpc = false;

		questMarker.transform.parent = GameManager.instance.markerCanvas;

		foreach(Transform hint in questHintHolder){
			questHints.Add(hint);
		}
	}

	private void Update() {
		//calling functions
		// ShowInteractUI();

		if (GameManager.paused || isQuestCompleted) {
			return;
		}

		if (!isQuestReceived && !GameManager.isInCutscene){
			UpdateQuestMarker();
		}

		if (isQuestReceived && !DialogueManager.isDialogue) {
			// print("NOT PAUSED");
			// Maybe update quest timer here
			questTimer += Time.deltaTime;
			GameManager.instance.sliderManager.masterValue = questTimer / totalQuestTime;
			GameManager.instance.timeText.text = ((int)questTimer).ToString();
		}
	}

	private void UpdateQuestMarker(){
		questMarker.transform.position = GameManager.GetTargetIndicator(transform.position + offset); ;
		questMarkerText.text = Mathf.Floor(Vector3.Distance(GameManager.instance.player.transform.position, transform.position) * 10).ToString();
	}

	public void UpdateArrowIcon() {
		// Hover down.
		if (arrowIcon.position.y > initialYPos + GameManager.instance.hoverDistance) {
			hoverVelocity = -Vector3.up * GameManager.instance.hoverSpeed * Time.deltaTime;
		} else if (arrowIcon.position.y < initialYPos - GameManager.instance.hoverDistance) {
			// Hover Up.
			hoverVelocity = Vector3.up * GameManager.instance.hoverSpeed * Time.deltaTime;
		}

		arrowIcon.position += hoverVelocity;
	}

	public void ReceiveQuest() {
		isQuestReceived = true;

		// Show dialogue box.
		DialogueManager.instance.dialogueCanvas.SetActive(true);

		// Show quest beam.
		GameManager.instance.questDestinationBeam.SetActive(true);
		GameManager.instance.questDestinationBeam.transform.position = destination.position;

		DialogueManager.instance.StartDialogue(dialogue);
		DialogueManager.instance.nameText.text = dialogue.name;

		// Hide quest arrow.
		arrowIcon.gameObject.SetActive(false);

		GameManager.ShowQuestInteractButton(false);
		GameManager.ShowQuestMarkers(false);

		//set the target for the waypoint
		GameManager.instance.wayPointTarget = destination;
		GameManager.instance.SpawnSlider(questData);

		// first npc
		if (firstNpc) {
			// NotificationTab.instance.SetAnimState("Notif", true);
			// NotificationTab.instance.timer = timerSet;
			// NotificationTab.instance.theText.text = firstNotif;
			// CatScript.instance.setCatDialogue("Talk to Npcs near post office to get quests", true);
			talkedToFirstNpc = true;
		}
	}

	public void ShowMarker(bool show){
		questMarker.gameObject.SetActive(show);
	}

	public void ShowInteractButton(bool show) {
		questInteractCanvas.gameObject.SetActive(show);
	}


	public void CompleteQuest() {
		isQuestCompleted = true;

		// Hide quest beam.
		GameManager.instance.questDestinationBeam.SetActive(false);

		GameManager.ShowQuestInteractButton(true);
		GameManager.ShowQuestMarkers(true);

		// Grading
		GameManager.instance.gradeText.text = Enum.GetName(typeof(Award), CalculateAward());
		// GameManager.instance.gradeBar.SetActive(true);
		GameManager.instance.nameStored.Add(questName);
		GameManager.instance.timeStored.Add(questTimer);
		GameManager.instance.gradeStored.Add(GameManager.instance.gradeText.text);
		GameManager.instance.ShowQuestUi(false);

		// Show tutorial for question marks.
		if (GUITutorial.instance.GuiClosed == 0){
			StartCoroutine(GUITutorial.instance.OpenGuiInstuctions(1));
		}

		foreach (Quest q in GameManager.instance.quests) {
			if (!q.isQuestCompleted) {
				return;
			}
		}

		StartCoroutine(GameManager.instance.callEndScreen());
	}

	public Award CalculateAward() {
		if (questTimer < questData.AwardGold) {
			return Award.Gold;
		} else if (questTimer < questData.AwardSilver) {
			return Award.Silver;
		} else if (questTimer < questData.AwardBronze) {
			return Award.Bronze;
		} else {
			return Award.Participation;
		}
	}
}
