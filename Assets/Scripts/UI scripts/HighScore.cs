using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour {

	// instance
	public static HighScore instance;
	public Color oddColor;
	public Color evenColor;

	public float templateHeight = 70f;
	public float addContent;
	public float totalQuest;
	public Transform questContainer;
	public Transform questTemplate;
	Vector2 autoScroll;

	// this is the transform of the content for the scroll rect
	public RectTransform content;
	public bool spawnedDone, called;

	public GameObject[] stars;
	public int worthGoldPoint = 3;
	public int worthSilverPoint = 2;
	public int worthBronzePoint = 1;

	void Awake() {
		spawnedDone = false;
		called = false;
		instance = this;
		// QuestGradeUpdate();
		questTemplate.gameObject.SetActive(false);
		this.gameObject.SetActive(false);
		// print("Setted");
	}



	// Update is called once per frame
	void Update() {
		// if (totalQuest <= 0) {
		// 	totalQuest = GameManager.instance.totalQuests.Length;
		// }
		if (!spawnedDone) {
			if (!called) QuestGradeUpdate();
			autoScroll.y = content.sizeDelta.y;
			// content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, autoScroll, Time.deltaTime / 2);
			// print(content.anchoredPosition.y);
		}
	}

	public int CalculateScore(Award award) {
		if (award == Award.Gold) {
			return worthGoldPoint;
		} else if (award == Award.Silver) {
			return worthSilverPoint;
		} else if (award == Award.Bronze) {
			return worthBronzePoint;
		} else {
			return 0;
		}
	}

	void QuestGradeUpdate() {
		totalQuest = GameManager.instance.quests.Count;
		StartCoroutine(InstantiateQuestGrades());
		called = true;

		int totalPoint = 0;
		int totalAcquirablePoint = 0;
		for (int i = 0; i < totalQuest; i++) {
			
			Award award = GameManager.instance.quests[i].CalculateAward();

			totalPoint += CalculateScore(award);
			totalAcquirablePoint += CalculateScore(Award.Gold);
		}

		foreach(GameObject g in stars){
			g.SetActive(true);
		}

		print((float)totalPoint / totalAcquirablePoint);
		if ((float) totalPoint / totalAcquirablePoint >= 0.7f) {
			print("gold overall");
		} else if ((float) totalPoint / totalAcquirablePoint >= 0.5f) {
			print("silver overall");
			stars[2].SetActive(false);
		} else {
			print("bronze overall");
			stars[1].SetActive(false);
			stars[2].SetActive(false);
		}
	}
	public IEnumerator InstantiateQuestGrades() {
		yield return new WaitForSeconds(1.5f);

		for (int i = 0; i < totalQuest; i++) {
			Transform entryTransform = Instantiate(questTemplate, questContainer);
			RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
			
			Color color = oddColor;
			if (i % 2 == 0){
				color = evenColor;
			}

			entryTransform.GetComponent<Image>().color = color;

			entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * (i + 1));
			entryTransform.gameObject.SetActive(true);
			// content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y);
			// // content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + addContent);

			// // passing the stored data to the endscreen
			entryTransform.Find("Name").GetComponent<Text>().text = GameManager.instance.nameStored[i];
			entryTransform.Find("Time").GetComponent<Text>().text = GameManager.instance.timeStored[i].ToString();
			entryTransform.Find("Grade").GetComponent<Text>().text = GameManager.instance.gradeStored[i];

			yield return new WaitForSeconds(.2f);
		}

		yield return new WaitForSeconds(.2f);
		spawnedDone = true;
		print("SpawnedDone");
	}
}


