using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour {
	public float length;



	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}
	void CreateTable(int row, int column, float length) {


		// 	for (int i = 0; i < totalQuest; i++) {
		// 		Transform entryTransform = Instantiate(questTemplate, questContainer);
		// 		RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

		// 		Color color = oddColor;
		// 		if (i % 2 == 0) {
		// 			color = evenColor;
		// 		}

		// 		entryTransform.GetComponent<Image>().color = color;

		// 		entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * (i + 1));
		// 		entryTransform.gameObject.SetActive(true);
		// 		// content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y);
		// 		// // content.sizeDelta = new Vector2(content.sizeDelta.x, content.sizeDelta.y + addContent);

		// 		// // passing the stored data to the endscreen
		// 		entryTransform.Find("Name").GetComponent<Text>().text = GameManager.instance.nameStored[i];
		// 		entryTransform.Find("Time").GetComponent<Text>().text = GameManager.instance.timeStored[i].ToString();
		// 		entryTransform.Find("Grade").GetComponent<Text>().text = GameManager.instance.gradeStored[i];

		// 		yield return new WaitForSeconds(.2f);
		// 	}
		// }
	}
}
