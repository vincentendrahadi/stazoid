using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eraser : Character {
	private const string CONTROLLER_PATH = "Animations/Eraser/EraserController";

	private static int MAX_HP = 250;
	private static float COMBO_TIMER = 5;
	private static int EASY_DAMAGE = 7;
	private static int MEDIUM_DAMAGE = 12;
	private static int HARD_DAMAGE = 20;
	private static float EASY_INCREASE = 0.075f;
	private static float MEDIUM_INCREASE = 0.2f;
	private static float HARD_INCREASE = 0.3f;

	private const float ERASED_TIME = 5.0f;
	private const int ERASED_COUNT = 3;

	private bool isErased = false;
	private float erasedTime = 0f;
	private bool isNPC = false;

	void Update () {
		if (!isPaused) {
			// Manage button shuffle
			if (erasedTime > 0 && isErased) {
				erasedTime -= Time.deltaTime;
				if (erasedTime < 0) {
					if (isNPC == true) {
						npcRevertKeypad ();
					} else {
						revertKeypad ();
					}

				}
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

	public override string getControllerPath() {
		return CONTROLLER_PATH;
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
		if (firstNumber < secondNumber) {
			int temp = firstNumber;
			firstNumber = secondNumber;
			secondNumber = temp;
		}
		return new KeyValuePair<string, int>(firstNumber + " - " + secondNumber, firstNumber - secondNumber);
	}
	void eraseKeypad() {
		HashSet <int> toBeErased = new HashSet <int> ();
		while (toBeErased.Count < ERASED_COUNT) {
			int erasedIndex = Random.Range (0, 9);
			if (!toBeErased.Contains (erasedIndex)) {
				toBeErased.Add (erasedIndex);
			}
		}
		int i = 0;
		Button[] numberButtons = GameController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			if (toBeErased.Contains (i)) {
				button.interactable = false;
			}
			i++;
		}
		isErased = true;
		erasedTime = ERASED_TIME;
	}

	void revertKeypad() {
		Button[] numberButtons = GameController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			button.interactable = true;
		}
		erasedTime = 0;
	}

	void npcEraseKeypad() {
		isNPC = true;
		HashSet <int> toBeErased = new HashSet <int> ();
		while (toBeErased.Count < ERASED_COUNT) {
			int erasedIndex = Random.Range (0, 9);
			if (!toBeErased.Contains (erasedIndex)) {
				toBeErased.Add (erasedIndex);
			}
		}
		int i = 0;
		Button[] numberButtons = SinglePlayerController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			if (toBeErased.Contains (i)) {
				button.interactable = false;
			}
			i++;
		}
		isErased = true;
		erasedTime = ERASED_TIME;
	}

	void npcRevertKeypad() {
		isNPC = false;
		Button[] numberButtons = SinglePlayerController.Instance.getNumberButtons ();
		foreach (Button button in numberButtons) {
			button.interactable = true;
		}
		erasedTime = 0;
	}

	public override void npcUseSpecial() {
		npcEraseKeypad ();
	}

	[PunRPC]
	public override void useSpecial ()
	{
		eraseKeypad ();
	}

}
