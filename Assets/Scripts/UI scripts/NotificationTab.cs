using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationTab : MonoBehaviour {
	public Animator anim;
	public float timer;
	public static NotificationTab instance;
	public Text theText;
	// Start is called before the first frame update
	void Start() {
		anim = GetComponent<Animator>();
		instance = this;
	}

	// Update is called once per frame
	void Update() {
		if (!DialogueManager.isDialogue) timer -= Time.deltaTime;
        
		if (timer <= 0) {
			SetAnimState("Notif", false);
		}
	}

	public void SetAnimState(string animState, bool toSet) {
		anim.SetBool(animState, toSet);
	}
}
