using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pencil : Character {

	private static int MAX_HP = 200;
	private static float COMBO_TIMER = 5;
	private static int EASY_DAMAGE = 10;
	private static int MEDIUM_DAMAGE = 15;
	private static int HARD_DAMAGE = 25;
	private static float EASY_INCREASE = 0.1f;
	private static float MEDIUM_INCREASE = 0.15f;
	private static float HARD_INCREASE = 0.25f;


	private bool isShuffled = false;
	private float shuffledTime = 0f;

	private int[] shuffleKeypadArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

	void update() {
		// Manage button shuffle
		if (shuffledTime > 0 && isShuffled) {
			shuffledTime -= Time.deltaTime;
			if (shuffledTime < 0) {
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


	void Shuffle(int[] array) {
		int n = array.Length;
		for (int i = 0; i < n; i++)	{
			int r = i + Random.Range(0, n - i);
			int temp = array[r];
			array[r] = array[i];
			array[i] = temp;
		}
	}
	void shuffleKeypad() {
		int i = 0;
		Shuffle (shuffleKeypadArray);
		Button[] numberButtons = GameController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			button.name = "Button - " + shuffleKeypadArray [i].ToString ();
			button.GetComponentsInChildren<Text> ()[0].text = shuffleKeypadArray [i].ToString ();
			i++;
		}
		isShuffled = true;
		shuffledTime = 5.0f;
	}

	void revertKeypad() {
		int i = 0;
		Button[] numberButtons = GameController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			button.name = "Button - " + i.ToString ();
			button.GetComponentsInChildren<Text> ()[0].text = i.ToString ();
			i++;
		}
		shuffledTime = 0;
	}

	[PunRPC]
	public override void useSpecial ()
	{
		shuffleKeypad();
		throw new System.NotImplementedException ();
	}

}
