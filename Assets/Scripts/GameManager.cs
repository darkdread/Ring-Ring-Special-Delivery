using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

[System.Serializable]
public struct PlayerClips {
	public AudioClip jumpClip;
	public AudioClip glideClip;
}

[System.Serializable]
public struct SoundClips {
	public AudioClip[] clip;
}
public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public static bool isQuestReceived;
	public static bool isInCutscene = false;

	public PlayerController player;
	public GameObject pauseScreen;
	public SetVolume setVol;
	public Slider volSlider;
	public LevelManager levelManager;
	public static bool paused;

	//========================================================
	[Header("Quest")]
	public static Quest questCurrent;
	public QuestDataContainer questDataContainer;
	public List<Quest> quests;
	public float questRadius;
	public LayerMask questMask;
	public GameObject questDestinationBeam;
	public float hoverDistance;
	public float hoverSpeed;
	public LayerMask wallMask;
	public GameObject completeScreen;

	[Header("Respawnscreen")]
	public Image blackScreen;

	[Header("Wall Materials")]
	public Material wallNormalMaterial;
	public Material wallInvisMaterial;

	[Header("EventSystem")]
	public EventSystem eventManager;
	public GameObject firstButton;

	// the button for when the game ends
	public GameObject endButton;

	[Header("Utility")]
	public float waitTime;

	[Header("Player Sounds")]
	public PlayerClips playerClips;

	// this is to show where the player should go when he accepts a quest
	[Header("WayPoint")]
	public Camera mainCam;
	public Image img;
	public Transform wayPointTarget;
	public Vector3 offset;
	public Text meter;

	[Header("QuestTimer")]
	public GameObject timerObject;
	public Text timeText;

	public SliderManager sliderManager;

	public float goldTime;
	public float silverTime;
	public float bronzeTime;

	[Header("Quest")]
	public Transform markerCanvas;
	public Text gradeText;
	public GameObject gradeBar;
	public GameObject hintEffectPrefab;

	[Header("SettingsScreen")]

	public GameObject settingsScreen;
	public bool settingsOpened = false;

	// audio related
	public AudioSource currentAudio;

	//set this to close
	public GameObject objectToClose;
	public Animator menuButtons;

	// For player velocity after unpause
	private Vector3 savedPlayerVelocity;

	[Header("StoredData")]
	public List<float> timeStored;
	public List<string> nameStored, gradeStored;

	[Header("GameEnded and start")]

	public bool gameStart;
	public bool gameEnded = false;

	[Header("Camera effects")]
	public Volume volProf;
	private Bloom bloom;
	private float bloomDefaultIntensity;
	public MotionBlur motionBlur;
	public float motionBlurDefaultIntensity;

	[Header("FeedBacks")]
	public LayerMask mailBoxMask;
	public GameObject completionStar;
	public GameObject starParticle;

	// this is for the tutorial marker
	public GameObject tutorialMarker;
	public float markerHeight;
	public float markerLimit = 7;
	public LayerMask groundLayers;

	private float tempMarkerHeight;

	void Awake() {
		// paused = false;
		gameEnded = false;
		Time.timeScale = 1;
		instance = this;
	}
	void Start() {
		tempMarkerHeight = markerHeight;
		mainCam = Camera.main;

		// Initialize Quests
		questDataContainer = QuestDataContainer.Load(Path.Combine(Application.dataPath, "Quests.xml"));

		gameEnded = false;
		Time.timeScale = 1;

		GameObject[] objects = GameObject.FindGameObjectsWithTag("Quest");

		for (int i = 0; i < objects.Length; i++) {
			Quest quest = objects[i].GetComponent<Quest>();
			quests.Add(quest);

			foreach (QuestData questData in questDataContainer.QuestsData) {
				if (quest.questName == questData.Name) {
					quest.questData = questData;

					quest.dialogue.name = questData.Name;
					quest.dialogue.sentences = questData.Dialogue;
					print(quest.questName);
				}
			}
		}

		setVol.VolumeLevel(volSlider.value);
		isQuestReceived = false;
		paused = false;
		levelManager = GetComponent<LevelManager>();

		// for the settings screen
		settingsOpened = false;
		if (settingsScreen.activeInHierarchy) settingsScreen.SetActive(false);
		if (objectToClose != null) if (objectToClose.activeInHierarchy) objectToClose.SetActive(false);
		if (currentAudio != null)
			if (!currentAudio.isPlaying) currentAudio.Play();
		if (currentAudio == null) currentAudio = null;
		bloom = (Bloom)volProf.profile.components[4];
		bloomDefaultIntensity = bloom.intensity.value;
		motionBlur = (MotionBlur)volProf.profile.components[6];
		motionBlurDefaultIntensity = motionBlur.intensity.value;

	}

	// Update is called once per frame
	void Update() {

		TutorialMarkerAboveGround();
		if (ControlConfigMenu.configMenuOpen) {
			return;
		}

		// If options animation is playing and you want to exit to setings
		if (OptionsAnimation.optionsAnimPlaying && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button"))
			|| OptionsAnimation.optionsAnimPlaying && Input.GetButtonDown("Cancel")) {
			settingsOpened = false;
			OptionsAnimation.optionsAnimPlaying = false;
			settingsScreen.SetActive(false);
			eventManager.SetSelectedGameObject(firstButton);
			return;
		}


		if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start Button") || Input.GetButtonDown("Cancel"))
			&& !DialogueManager.isDialogue && !gameEnded) {
			Pause();
		}

		if (Input.GetKeyDown(KeyCode.Q)) {
			// isQuestReceived = false;
			// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reset the scene
			// Pause();
			StartCoroutine(callEndScreen());
		}

		// Receiving Quest
		if (Input.GetButtonDown("Interact") || ControlConfig.controlConfigInstance.CharacterAction(ControlConfig.interactAction)) {

			Collider[] questsNearPlayer = Physics.OverlapSphere(player.transform.position, questRadius, questMask);

			// if (!isQuestReceived) DialogueManager.instance.dialogueCanvas.SetActive(true);
			print(questsNearPlayer.Length);

			if (questsNearPlayer.Length > 0 && !isQuestReceived) {
				foreach (Collider c in questsNearPlayer) {
					Quest quest = c.GetComponent<Quest>();
					if (quest && !quest.isQuestReceived) {
						isQuestReceived = true;
						questCurrent = quest;
						quest.ReceiveQuest();
						player.ResetPlayer();
						print("Quest Received");
					}
				}
			}
			print("IsInteract");
		}

		if (questCurrent != null) {
			if (questCurrent.questHintCurrent < questCurrent.questHints.Count) {
				if (Input.GetKeyDown(KeyCode.N) || ControlConfig.controlConfigInstance.CharacterAction(ControlConfig.hintAction)) {
					// float closestDistance = Mathf.Infinity;
					// Transform closestTarget = null;

					// foreach (Transform t in questCurrent.questHints) {
					// 	float d = Vector3.Distance(t.position, CatScript.instance.transform.position);
					// 	if (d < closestDistance) {
					// 		closestDistance = d;
					// 		closestTarget = t;
					// 	}
					// }

					// CatScript.instance.SetDestination(closestTarget);
					CatScript.instance.SetDestination(questCurrent.questHints[questCurrent.questHintCurrent]);
					ThirdPersonCamera.SetTarget(CatScript.instance.transform, 1f);
				}

				// If player is at hint.
				if (Vector3.Distance(instance.player.transform.position, questCurrent.questHints[questCurrent.questHintCurrent].position) < 5f) {
					Instantiate(hintEffectPrefab, player.transform.position, Quaternion.identity);
					questCurrent.questHintCurrent += 1;
				}
			}
		}

		// Completing Quests
		foreach (Quest quest in quests) {
			// For the hover objects above NPC.
			// quest.UpdateArrowIcon();

			if (quest.isQuestReceived && !quest.isQuestCompleted) {
				if (Vector3.Distance(player.transform.position, quest.destination.position) <= questRadius) {
					isQuestReceived = false;
					quest.CompleteQuest();
					Collider[] mailboxes = Physics.OverlapSphere(questDestinationBeam.transform.position, 2f, mailBoxMask);

					// animates the mailbox when the player completes a quest 
					foreach (Collider c in mailboxes) {
						print(c.name);
						Interactables interactable = c.GetComponent<Interactables>();
						interactable.animStateChange("SpinMailBox", true);

						// this is for the spawning of the star when the quest is completed
						GameObject obj = Instantiate(completionStar, interactable.transform.position + Vector3.up * 2, Quaternion.Euler(0, 0, 90));
						StartCoroutine(obj.GetComponent<Interactables>().starAnim(obj.transform.position + Vector3.up * 5, 50f, .1f));
						// if(obj.GetComponent())
						Destroy(obj, 1.1f);
						print("Set");
					}
				}
			}
		}

		//utility variables
		waitTime -= Time.deltaTime;
		wayPoint();
	}

	public IEnumerator Respawn() {

		// Hide diveBombEffect
		player.diveBombEffect.transform.position = player.originalPos.position;

		// Reset player jump and dash count
		player.jumpCount = 0;
		player.dashCount = 0;

		player.GetComponent<Rigidbody>().isKinematic = true;
		player.GetComponent<PlayerController>().enabled = false;

		float fadeMaxTime = 1f;
		float fadeTime = 0f;

		while (fadeTime < fadeMaxTime) {
			fadeTime += Time.deltaTime;
			blackScreen.color = Color.Lerp(blackScreen.color, Color.black, fadeTime/fadeMaxTime);
			yield return null;
		}

		Vector3 respawnPoint = player.checkPoint.position;
		// Vector3 randomPoint = respawnPoint + (Vector3.up * 15) + Random.insideUnitSphere * 3;
		Vector3 randomPoint = respawnPoint + (Vector3.up * 5);
		// Vector3 spawnPoint = RecalculatePath(randomPoint, respawnPoint);
		Vector3 spawnPoint = randomPoint;

		fadeTime = 0f;

		while (fadeTime < fadeMaxTime) {
			fadeTime += Time.deltaTime;

			// for the fading back of the screen
			blackScreen.color = Color.Lerp(blackScreen.color, Color.clear, fadeTime/fadeMaxTime);

			// for the movement of the player
			player.transform.position = Vector3.Lerp(spawnPoint, respawnPoint, fadeTime/fadeMaxTime);
			yield return null;
		}

		player.GetComponent<Rigidbody>().isKinematic = false;
		// yield return new WaitUntil(() => player.transform.position == player.checkPoint.position);
		player.GetComponent<PlayerController>().enabled = true;
		print("respawned");
	}

	// this is to ensure that the player will not spawn in the collider
	Vector3 RecalculatePath(Vector3 randomPoint, Vector3 respawnPoint) {
		bool hitBuilding = true;

		RaycastHit hit;

		while (hitBuilding) {
			if (Physics.Raycast(randomPoint, (respawnPoint - randomPoint).normalized, out hit, Vector3.Distance(randomPoint, respawnPoint), wallMask)) {
				hitBuilding = true;
				randomPoint = respawnPoint + Random.insideUnitSphere * 5;
				print("hit");
			} else {
				hitBuilding = false;
				// calculatedPath = respawnPoint + (Vector3.up * 5) + Random.insideUnitSphere * 5;
			}
		}
		return randomPoint;
	}
	void Pause() {
		if (!paused) {

			pauseScreen.SetActive(true);
			// Cursor.visible = true;
			paused = true;
			Time.timeScale = 0;
			eventManager.SetSelectedGameObject(firstButton);
			Rigidbody playerRb = player.GetComponent<Rigidbody>();
			bloom.intensity.value = 1f;
			// To freeze player
			// savedPlayerVelocity = playerRb.velocity;
			// playerRb.isKinematic = true;

		} else if (paused) {
			bloom.intensity.value = bloomDefaultIntensity;
			eventManager.SetSelectedGameObject(null);
			pauseScreen.SetActive(false);
			eventManager.SetSelectedGameObject(null);
			waitTime = 0.5f;
			// Cursor.visible = false;
			paused = false;
			Time.timeScale = 1;
			// To unfreeze player
			// playerRb.isKinematic = false;
			// playerRb.velocity = savedPlayerVelocity;

		}

		// levelManager.Settings();
	}

	public void ContinueButton() {
		Pause();
	}

	public void LoadScene(string sceneToLoad) {
		SceneManager.LoadScene(sceneToLoad);
		if (paused) Pause();
	}

	public void ShowQuestUi(bool toShow) {
		timerObject.SetActive(toShow);
		sliderManager.gameObject.SetActive(toShow);
	}

	public IEnumerator callEndScreen() {
		yield return new WaitForSeconds(2f);
		completeScreen.SetActive(true);
		gameEnded = true;
		eventManager.SetSelectedGameObject(endButton); // to select the endbutton for when the game ends
	}

	public static Vector3 GetTargetIndicator(Vector3 target){
		Vector3 screenPos = instance.mainCam.WorldToScreenPoint(target);

		if (screenPos.z > 0 &&
		screenPos.x > 0 && screenPos.x < Screen.width &&
		screenPos.y > 0 && screenPos.y < Screen.height) {
			return screenPos;
		} else {
			// Offscreen

			// Flip img because it's behind.
			if (screenPos.z < 0) {
				screenPos *= -1;
			}

			Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

			// 0, 0 to be center instead of bottom left.
			screenPos -= screenCenter;

			float angle = Mathf.Atan2(screenPos.y, screenPos.x);
			angle -= 90 * Mathf.Deg2Rad;

			float cos = Mathf.Cos(angle);
			float sin = -Mathf.Sin(angle);

			screenPos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

			float m = cos / sin;

			Vector3 screenBounds = screenCenter * 0.9f;

			// Check up & down.
			if (cos > 0) {
				screenPos = new Vector3(screenBounds.y / m, screenBounds.y, 0);
			} else {
				screenPos = new Vector3(-screenBounds.y / m, -screenBounds.y + (screenCenter * 0.05f).y, 0);
			}

			// Out of bounds, get point on appropriate sides.

			// Right side.
			if (screenPos.x > screenBounds.x) {
				screenPos = new Vector3(screenBounds.x, screenBounds.x * m, 0);
			} else if (screenPos.x < -screenBounds.x) {
				screenPos = new Vector3(-screenBounds.x, -screenBounds.x * m, 0);
			}

			screenPos += screenCenter;

			return screenPos;
		}
	}

	void wayPoint() {

		if (isQuestReceived) {
			img.gameObject.SetActive(true);
		} else {
			img.gameObject.SetActive(false);
		}

		img.transform.position = GetTargetIndicator(wayPointTarget.position + offset);

		meter.text = ((int)Vector3.Distance(wayPointTarget.position, player.transform.position)).ToString();
	}

	///<summary>
	/// this is to control the opening of the options and what happens when it opens
	///</summary>
	public void Settings() {
		if (!settingsOpened) {
			// Starts options animation
			OptionsAnimation.optionsAnimPlaying = true;
			settingsScreen.SetActive(true);
			settingsOpened = true;
		} else if (settingsOpened) {
			settingsScreen.SetActive(false);
			settingsOpened = false;
		}
		// if (!settingsOpened) {
		// 	// Starts options animation
		// 	OptionsAnimation.optionsAnimPlaying = true;
		// 	settingsScreen.SetActive(true);
		// 	settingsOpened = true;
		// } else if (settingsOpened) {
		// 	settingsScreen.SetActive(false);
		// 	settingsOpened = false;
		// }
	}

	void buttonAnimation() {
		menuButtons.SetBool("SettingsOpened", settingsOpened);
	}
	public void SpawnSlider(QuestData questData) {
		sliderManager.ClearSliders();
		sliderManager.CreateSlider(3, 300f);

		float totalQuestTime = questData.AwardBronze;

		sliderManager.SetSliderLengthPercent(0, (float)questData.AwardGold / totalQuestTime, true);
		sliderManager.SetSliderLengthPercent(1, (float)(questData.AwardSilver - questData.AwardGold) / totalQuestTime, true);

		// for (int i = 0; i < 3; i++) {
		// 	Color razerChroma;
		// 	if (i % 3 == 0) {
		// 		razerChroma = Color.yellow;
		// 	} else if (i % 3 == 1) {
		// 		razerChroma = Color.grey;
		// 	} else {
		// 		razerChroma = new Color(171f / 255f, 132f / 255f, 99f / 255f);
		// 	}

		// 	sliderManager.SetSliderFillColor(i, razerChroma);
		// }
	}


	// this is for the marker to hit above the ground
	void TutorialMarkerAboveGround() {
		RaycastHit hit;

		// print("IsCallingg");


		if (Physics.Raycast(tutorialMarker.transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayers)) {
			tutorialMarker.transform.position = hit.point + (Vector3.up * markerHeight);
		}

		// if (markerHeight >= 7) markerLimit = 5.66f;
		// else if(markerHeight <= 5.66) markerLimit = 7;
		markerHeight = Mathf.Lerp(tempMarkerHeight, markerLimit, Mathf.PingPong(Time.time, 1f));
		// print(Mathf.Lerp(tempMarkerHeight, markerLimit, Mathf.PingPong(Time.time, 1f)));
		// print("Hitting");
	}

	public static void ShowQuestInteractButton(bool show) {
		if (isQuestReceived) {
			show = false;
		}

		// Hide all quest interact buttons.
		foreach (Quest q in instance.quests) {

			// Hide if showing but quest already received.
			if (show && q.isQuestCompleted) {
				continue;
			}

			q.ShowInteractButton(show);
		}
	}

	public static void ShowQuestMarkers(bool show){
		if (isQuestReceived){
			show = false;
		}

		// Hide all quest markers.
		foreach (Quest q in instance.quests) {

			// Hide if showing but quest already received.
			if (show && q.isQuestReceived){
				continue;
			}

			q.ShowMarker(show);
		}
	}
}
