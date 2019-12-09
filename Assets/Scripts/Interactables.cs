using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractableTypes { mailBox, comepletionStar, randomInteractable };

public class Interactables : MonoBehaviour {

	public InteractableTypes interactableTypes;
	public float timer;

	// Start is called before the first frame update
	void Start() {

	}

	///<summary>
	/// this is to animate the mailbox and change the animator state
	///</summary>
	public void animStateChange(string animState, bool toAnim) {
		if (interactableTypes == InteractableTypes.mailBox) {
			Animator anim = GetComponent<Animator>();
			anim.SetBool(animState, toAnim);
		}
	}

	///<summary>
	/// this is to animate the star for when the player completes quest,rotate speed is for how fast the start spins when it spawns
	///</summary>
	public IEnumerator starAnim(Vector3 height, float rotateSpeed, float translateSpeed) {
		if (interactableTypes == InteractableTypes.comepletionStar) {
			while (transform.position != height) {
				transform.Translate(translateSpeed, 0, 0);
				transform.Rotate(rotateSpeed, 0, 0);
				yield return new WaitForSeconds(0);
				timer += Time.deltaTime;
				if (timer >= 1.05) Instantiate(GameManager.instance.starParticle, transform.position, Quaternion.identity);
				print("isRunning");
			}
		}
	}


	// this is for the random interactables in the scene. for things like dashin and just running pass the interactables
	void randomAnim(string animState, bool toAnim) {
		Collider[] player = Physics.OverlapSphere(transform.position, 10f, 12);
		
		foreach(Collider c in player){

		}
		if (interactableTypes == InteractableTypes.randomInteractable) {
			Animator anim = GetComponent<Animator>();
			anim.SetBool(animState, toAnim);
		}
	}
}
