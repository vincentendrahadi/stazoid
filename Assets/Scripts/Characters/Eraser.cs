using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eraser : Character {

	private static int MAX_HP = 200;
	private static float COMBO_TIMER = 5;
	private static int EASY_DAMAGE = 10;
	private static int MEDIUM_DAMAGE = 15;
	private static int HARD_DAMAGE = 25;
	private static float EASY_INCREASE = 0.1f;
	private static float MEDIUM_INCREASE = 0.15f;
	private static float HARD_INCREASE = 0.25f;

	private const float ERASED_TIME = 6.0f;


	private bool isErased = false;
	private float erasedTime = 0f;


	void update() {
		// Manage button shuffle
		if (erasedTime > 0 && isErased) {
			erasedTime -= Time.deltaTime;
			if (erasedTime < 0) {
				revertKeypad ();
			}
		}
	}


	void Awake () {
		maxHp = MAX_HP;
		comboTimer = COMBO_TIMER;
		damage[Difficulty.EASY] = EASY_DAMAGE;
		damage[Difficulty.MEDIUM] = MEDIUM_DAMAGE;
		damage[Difficulty.HARD] = HARD_DAMAGE;
		specialBarIncrease [Difficulty.EASY] = EASY_INCREASE;
		specialBarIncrease [Difficulty.MEDIUM] = MEDIUM_INCREASE;
		specialBarIncrease [Difficulty.HARD] = HARD_INCREASE;
	}

	public override KeyValuePair<string, int> generateProblem (int difficulty)
	{
		int firstNumber = 0, secondNumber = 0;
		if (difficulty == Difficulty.EASY) {
			firstNumber = Random.Range (0, 10);
			secondNumber = Random.Range (0, 10);
		} else if (difficulty == Difficulty.MEDIUM) {
			firstNumber = Random.Range (10, 100);
			secondNumber = Random.Range (0, 10);
			int flip = Random.Range (0, 2);
			if (flip == 1) {
				int temp = firstNumber;
				firstNumber = secondNumber;
				secondNumber = temp;
			}
		} else if (difficulty == Difficulty.HARD) {
			firstNumber = Random.Range (10, 100);
			secondNumber = Random.Range (10, 100);
		}
		return new KeyValuePair<string, int>(firstNumber + " + " + secondNumber, firstNumber + secondNumber);
	}
	void eraseKeypad() {
		int erasedIndex = Random.Range (0, 9);
		int i = 0;
		Button[] numberButtons = GameController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			if (i != erasedIndex) {
				button.name = "Button - " + i.ToString ();
				button.GetComponentsInChildren<Text> () [0].text = i.ToString ();
				i++;
			} else {
				button.name = "Button - ";
				button.GetComponentsInChildren<Text> () [0].text = "";
			}
		}
		isErased = true;
		erasedTime = ERASED_TIME;
	}

	void revertKeypad() {
		int i = 0;
		Button[] numberButtons = GameController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			button.name = "Button - " + i.ToString ();
			button.GetComponentsInChildren<Text> ()[0].text = i.ToString ();
			i++;
		}
		erasedTime = 0;
	}

	[PunRPC]
	public override void useSpecial ()
	{
		eraseKeypad ();
		throw new System.NotImplementedException ();
	}

}
