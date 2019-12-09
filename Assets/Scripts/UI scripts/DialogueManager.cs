using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
	// instance
	public static DialogueManager instance;
	public Queue<string> sentences;

	public Text dialogueText;
	public Text nameText;


	[Header("Dialogue")]
	public GameObject dialogueCanvas;

	public AudioSource voiceSource;
	public AudioClip[] voiceCLip;
	public int tickRate = 3;
	private Coroutine storedSentence;

	[Header("DialogueAnim")]
	public GameObject dialogueParent;
	public static bool isDialogue;

	[Header("NextButton")]
	public Button next;
	// Start is called before the first frame update
	void Start() {

		// if (instance == null) {
		instance = this;

		// } else Destroy(this.gameObject);

		sentences = new Queue<string>();
		isDialogue = false;
	}

	void Update() {
		Anim();
	}
	public void StartDialogue(Dialogue dialogue) {
		sentences.Clear();
		Cursor.visible = true;
		foreach (string sentence in dialogue.sentences) {
			sentences.Enqueue(sentence);
		}
		DisplayNextSentence();
		isDialogue = true;
		GameManager.instance.eventManager.SetSelectedGameObject(next.gameObject);
	}
	public void DisplayNextSentence() {
		if (sentences.Count == 0) {
			EndDialogue();
			// dialogueCanvas.SetActive(false);
			return;
		}
		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		storedSentence = StartCoroutine(TypeSentence(sentence));
		GameManager.instance.waitTime = .5f;
	}
	public void EndDialogue() {
		Cursor.visible = false;
		print("End");
		isDialogue = false;
		GameManager.instance.waitTime = .5f;
		GameManager.instance.eventManager.SetSelectedGameObject(null);
		GameManager.isQuestReceived = true;
		GameManager.instance.ShowQuestUi(true);
		NotificationTab.instance.SetAnimState("Notif", true);
		StopCoroutine(storedSentence);
		
		// this is for the gui tutorial
		if (GUITutorial.instance.GuiClosed == -1) {
			StartCoroutine(GUITutorial.instance.OpenGuiInstuctions(0));
		}
	}

	private IEnumerator TypeSentence(string sentence) {
		dialogueText.text = "";
		int wait = 0;

		foreach (char letter in sentence.ToCharArray()) {
			dialogueText.text += letter;
			wait++;

			if (wait >= tickRate) {
				wait = Random.Range(0, tickRate - 1);
				voiceSource.clip = voiceCLip[Random.Range(0, voiceCLip.Length)];
				voiceSource.Play();
				voiceSource.pitch = Random.Range(0.6f, 1f);
			}

			// AudioSource.PlayClipAtPoint(voiceCLip,GameManager.instance.player.transform.position);
			yield return null;
		}
	}
	void Anim() {
		dialogueParent.GetComponent<Animator>().SetBool("isDialogue", isDialogue);
	}
}
