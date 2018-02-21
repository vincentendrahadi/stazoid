using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	[SerializeField]
	private Text ProblemText;
	[SerializeField]
	private Text AnswerText;
	[SerializeField]
	private Text JudgeText;

	[SerializeField]
	private Button[] numberButtons;

	void Start () {
		Screen.orientation = ScreenOrientation.Portrait;

		ProblemText.text = "7 x 7";
		AnswerText.text = "0";
		JudgeText.text = "";

		foreach (Button button in numberButtons) {
			button.onClick.AddListener (delegate {
				addNumberToAnswer (button.name [9]);
			});
		}
	}

	void addNumberToAnswer(char number) {
		if (AnswerText.text != "0") {
			AnswerText.text += number;
		} else {
			AnswerText.text = "" + number;
		}
	}

	void generateNewProblem(int difficulty) {

	}

	public void judgeAnswer() {
		if (int.Parse (AnswerText.text) == 49) {
			JudgeText.text = "Hit";
		} else {
			JudgeText.text = "Miss";
		}
	}

	public void backspace() {
		AnswerText.text = AnswerText.text.Substring (0, AnswerText.text.Length - 1);
		if (AnswerText.text == "") {
			AnswerText.text = "0";
		}
	}

}
