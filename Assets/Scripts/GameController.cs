using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	private static GameController _instance;

	public static GameController Instance {
		get {
			if (_instance == null) {
				_instance = (GameController)FindObjectOfType (typeof(GameController));
			}
			return _instance;
		}
	}

	private static float SPECIAL_BAR_MODIFIER = 0.01f;

	[SerializeField]
	private Text problemText;
	[SerializeField]
	private Text answerText;
	[SerializeField]
	private Button backspaceButton;
	[SerializeField]
	private Button[] numberButtons;
	[SerializeField]
	private Button[] difficultyButtons;
	[SerializeField]
	private Text comboText;
	[SerializeField]
	private Slider comboTimerSlider;
	[SerializeField]
	private Button specialButton;
	[SerializeField]
	private Image specialBarImage;

	private KeyValuePair<string, int> problemSet;
	private int solution;
	private int difficulty;
	private int combo;
	private float comboTimer;
	private float specialBar;

	public Character playerCharacter;

	void Start () {
		// Force portrain orientation
		Screen.orientation = ScreenOrientation.Portrait;

		// Add onClick listener to all number buttons
		foreach (Button button in numberButtons) {
			button.onClick.AddListener (delegate {
				addNumberToAnswer (button.name [9]);
			});
		}

		// Add onClick listener to all difficulty buttons
		difficultyButtons [Difficulty.EASY].onClick.AddListener (delegate {
			changeDifficulty (Difficulty.EASY);
		});
		difficultyButtons [Difficulty.MEDIUM].onClick.AddListener (delegate {
			changeDifficulty (Difficulty.MEDIUM);
		});
		difficultyButtons [Difficulty.HARD].onClick.AddListener (delegate {
			changeDifficulty (Difficulty.HARD);
		});

		// Initialize combo & comboTimer
		combo = 0;
		comboTimerSlider.maxValue = playerCharacter.getComboTimer ();

		// Initialize special bar
		specialBar = 0;

		// Generate problem
		difficulty = Difficulty.MEDIUM;
		difficultyButtons [difficulty].interactable = false;
		problemSet = playerCharacter.generateProblem (difficulty);
		problemText.text = problemSet.Key;
	}

	void Update () {
		// Update combo timer
		if (comboTimer > 0) {
			comboTimer -= Time.deltaTime;
			comboTimerSlider.value = comboTimer;
		} else {
			resetCombo ();
		}

		// Animate special bar
		if (specialBarImage.fillAmount < specialBar && specialBarImage.fillAmount + SPECIAL_BAR_MODIFIER <= specialBar) {
			specialBarImage.fillAmount += SPECIAL_BAR_MODIFIER;
		} else if (specialBarImage.fillAmount > specialBar && specialBarImage.fillAmount - SPECIAL_BAR_MODIFIER >= specialBar) {
			specialBarImage.fillAmount -= SPECIAL_BAR_MODIFIER;
		} else {
			specialBarImage.fillAmount = specialBar;
		}
	}

	void addNumberToAnswer (char number) {
		if (answerText.text != "0") {
			answerText.text += number;
		} else {
			answerText.text = "" + number;
		}
	}

	public void backspace () {
		answerText.text = answerText.text.Substring (0, answerText.text.Length - 1);
		if (answerText.text == "") {
			answerText.text = "0";
		}
	}

	void generateNewProblem () {
		problemSet = playerCharacter.generateProblem (difficulty);
		problemText.text = problemSet.Key;
	}

	void changeDifficulty(int difficulty) {
		difficultyButtons [this.difficulty].interactable = true;
		this.difficulty = difficulty;
		generateNewProblem ();
		difficultyButtons [difficulty].interactable = false;
	}

	void resetCombo () {
		combo = 0;
		comboText.text = "";
		comboTimer = 0;
		comboTimerSlider.value = 0;
	}

	public void judgeAnswer() {
		if (int.Parse (answerText.text) == problemSet.Value) {
			generateNewProblem ();
			++combo;
			comboText.text = "" + combo;
			comboTimer = playerCharacter.getComboTimer ();
			specialBar += playerCharacter.getSpecialBarIncrease () [difficulty];
			if (specialBar >= 1) {
				specialBar = 1;
				specialButton.interactable = true;
			}
		} else {
			resetCombo ();
		}
		answerText.text = "0";
	}

	public void useSpecial () {
		specialBar = 0;
		specialButton.interactable = false;
	}
}
