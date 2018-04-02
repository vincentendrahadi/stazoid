using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A : Character {

	private static int MAX_HP = 200;
	private static float COMBO_TIMER = 5;
	private static int EASY_DAMAGE = 10;
	private static int MEDIUM_DAMAGE = 15;
	private static int HARD_DAMAGE = 25;
	private static float EASY_INCREASE = 0.1f;
	private static float MEDIUM_INCREASE = 0.15f;
	private static float HARD_INCREASE = 0.25f;

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

}
