using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SinglePlayerController : MonoBehaviour {

	private static SinglePlayerController _instance;

	public static SinglePlayerController Instance {
		get {
			if (_instance == null) {
				_instance = (SinglePlayerController)FindObjectOfType (typeof(SinglePlayerController));
			}
			return _instance;
		}
	}


	private class Result {
		public const int LOSE = -1;
		public const int NEUTRAL = 0;
		public const int WIN = 1;
	}

	private const int WIN_NEEDED = 3;

	private const float ANNOUNCEMENT_DELAY = 3.0f;
	private const float GAME_OVER_DELAY = 5.0f;

	[SerializeField]
	private float COMBO_MULTIPLIER;

	[SerializeField]
	private const float SPECIAL_BAR_MODIFIER = 0.01f;
	[SerializeField]
	private const float HEALTH_BAR_MODIFIER = 1f;
	[SerializeField]
	private const float DAMAGE_TO_SPECIAL_DIVISOR = 100f;

	[SerializeField]
	private GameObject blockingPanel;

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
	private GameObject specialButton;

	[SerializeField]
	private Text comboText;
	[SerializeField]
	private Slider comboTimerSlider;

	[SerializeField]
	private Slider ownSpecialBarSlider;
	[SerializeField]
	private Slider ownHealthBarSlider;

	[SerializeField]
	private Slider opponentSpecialBarSlider;
	[SerializeField]
	private Slider opponentHealthBarSlider;

	[SerializeField]
	private GameObject ownCharacterObject;
	[SerializeField]
	private GameObject opponentCharacterObject;

	[SerializeField]
	private GameObject countDownPanel;

	[SerializeField]
	private WinCounter ownWinCounter;
	[SerializeField]
	private WinCounter opponentWinCounter;

	[SerializeField]
	private GameObject resultPanel;
	[SerializeField]
	private Text resultText;

	[SerializeField]
	private AudioSource backgroundMusic;
	[SerializeField]
	private AudioSource audioSource;
	[SerializeField]
	private AudioClip[] audioClips;

	private bool isBlocked = true;
	private bool isHealthGaugeZero = false;
	private bool ownWin = false;
	private bool npcWin = false;

	private KeyValuePair<string, int> problemSet;

	private int solution;
	private int difficulty;

	private int combo;
	private float comboTimer;

	private int npcComboCount;
	private float npcComboTimer;

	private float npcAttackTime;
	private float npcAttackTimeMultiplier;
	private float npcAttackTimeMultiplierTime;

	private float ownSpecialGauge;
	private float ownHealthGauge;

	private float opponentSpecialGauge;
	private float opponentHealthGauge;

	private Character ownCharacter;
	private Character opponentCharacter;

	private List<string> characterList;

	private List <Vector3> numberButtonDefaultPositions;


	void Start () {
		ownCharacter = (Character)ownCharacterObject.AddComponent (System.Type.GetType (CharacterHolder.Instance.OwnCharacterName));
		opponentCharacter = (Character)opponentCharacterObject.AddComponent (System.Type.GetType (CharacterHolder.Instance.NpcCharacterName));

		blockingPanel.SetActive (false);
		countDownPanel.SetActive (true);
		isBlocked = true;

		// Add onClick listener to all number buttons and get default position of all number buttons
		numberButtonDefaultPositions = new List <Vector3> ();
		foreach (Button button in numberButtons) {
			button.onClick.AddListener (delegate {
				addNumberToAnswer (button.name [9]);
			});
			numberButtonDefaultPositions.Add (button.transform.position);
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
		comboTimerSlider.maxValue = ownCharacter.getComboTimer ();

		//Initialize npc combo timer
		npcComboTimer = opponentCharacter.getComboTimer ();

		// Initialize own special & health
		ownSpecialGauge = 0;
		ownHealthGauge = ownCharacter.getMaxHp ();
		ownHealthBarSlider.maxValue = ownHealthGauge;
		ownHealthBarSlider.value = ownHealthGauge;

		//Initialize NPC special & health
		opponentSpecialGauge = 0;
		opponentHealthGauge = opponentCharacter.getMaxHp ();
		opponentHealthBarSlider.maxValue = opponentHealthGauge;
		opponentHealthBarSlider.value = opponentHealthGauge;

		// Generate problem
		difficulty = Difficulty.MEDIUM;
		difficultyButtons [difficulty].interactable = false;
		problemSet = ownCharacter.generateProblem (difficulty);
		problemText.text = problemSet.Key;

		//Set first NPC attack time
		npcAttackTimeMultiplier = 1.0f;
		npcAttackTimeMultiplierTime = 0;
		npcAttackTime = Random.Range (3, npcComboTimer + 1);
	}

	void Update () {

		// Animate bars
		AnimateSlider (ownSpecialBarSlider, ownSpecialGauge, SPECIAL_BAR_MODIFIER);
		AnimateSlider (opponentSpecialBarSlider, opponentSpecialGauge, SPECIAL_BAR_MODIFIER);
		AnimateSlider (ownHealthBarSlider, ownHealthGauge, HEALTH_BAR_MODIFIER);
		AnimateSlider (opponentHealthBarSlider, opponentHealthGauge, HEALTH_BAR_MODIFIER);

		if (countDownPanel.activeInHierarchy || blockingPanel.activeInHierarchy || resultPanel.activeInHierarchy) {
			isBlocked = true;
		} else {
			isBlocked = false;
		}

		if (!isBlocked) {
			// Update combo timer
			if (comboTimer > 0) {
				comboTimer -= Time.deltaTime;
				comboTimerSlider.value = comboTimer;
			} else {
				resetCombo ();
			}

			//Update npc combo timer
			if (npcComboTimer > 0) {
				npcComboTimer -= Time.deltaTime;
			} else {
				npcComboCount = 0;
			}
			
			//Update npc attack time
			if (npcAttackTime > 0) {
				npcAttackTime -= Time.deltaTime;
			} else {
				npcAttack ();
			}

			//Update npc attack time multiplier because of special
			if (npcAttackTimeMultiplierTime > 0) {
				npcAttackTimeMultiplierTime -= Time.deltaTime;
			} else {
				npcAttackTimeMultiplier = 1.0f;
			}

			//NPC use special
			if (opponentSpecialGauge >= 1) {
				npcUseSpecial ();
			}
				
		}

	}
		
	void AnimateSlider (Slider slider, float gauge, float modifier) {
		if (slider.value < gauge && slider.value + modifier <= gauge) {
			slider.value += modifier;
		} else if (slider.value > gauge && slider.value - modifier >= gauge) {
			slider.value -= modifier;
		} else {
			slider.value = gauge;
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

	public void deleteAnswer () {
		answerText.text = "0";
	}

	void generateNewProblem () {
		problemSet = ownCharacter.generateProblem (difficulty);
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

	void npcAttack() {
		++npcComboCount;

		int npcDifficulty = Random.Range (0, 3);

		//Set npc attack time
		switch (npcDifficulty) {
			case 0:
				npcAttackTime = Random.Range (1, 4);
				break;
			case 1: 
				npcAttackTime = Random.Range (2, opponentCharacter.getComboTimer () + 1);
				break;
			case 2:
				npcAttackTime = Random.Range (3, opponentCharacter.getComboTimer () + 2);
				break;
		};

		npcComboTimer = opponentCharacter.getComboTimer ();


		// Increase own special gauge
		opponentSpecialGauge += opponentCharacter.getSpecialBarIncrease () [npcDifficulty];
		if (opponentSpecialGauge >= 1) {
			opponentSpecialGauge = 1;
		}

		// Decrease opponent's health
		float damage = opponentCharacter.getDamage () [npcDifficulty] * (1 + npcComboCount * COMBO_MULTIPLIER);
		ownHealthGauge -= damage;
		if (ownHealthGauge <= 0) {
			npcWin = true;
			isBlocked = true;
			setResult (Result.LOSE);
		}
		// Increase opponent's special gauge
		ownSpecialGauge += damage / DAMAGE_TO_SPECIAL_DIVISOR;
		if (ownSpecialGauge >= 1) {
			ownSpecialGauge = 1;
		}

		// Modify Slider
		modifyOpponentSpecialGauge(opponentSpecialGauge);
		modifyOpponentHealthGauge (opponentHealthGauge);
		modifyOwnHealthGauge (ownHealthGauge);
		modifyOwnSpecialGauge (ownSpecialGauge);
	}

	public void judgeAnswer() {
		if (int.Parse (answerText.text) == problemSet.Value) {
			generateNewProblem ();

			// Play sound effects
			audioSource.PlayOneShot (audioClips[GameController.AudioSourceIndex.CORRECT]);

			// Add combo
			++combo;
			comboText.text = "" + combo;
			comboTimer = ownCharacter.getComboTimer ();

			// Increase own special gauge
			ownSpecialGauge += ownCharacter.getSpecialBarIncrease () [difficulty];
			if (ownSpecialGauge >= 1) {
				ownSpecialGauge = 1;
				if (!specialButton.activeSelf) {
					audioSource.PlayOneShot (audioClips [GameController.AudioSourceIndex.SPECIAL_FULL]);
				}
				specialButton.SetActive (true);
			}

			// Decrease opponent's health
			float damage = ownCharacter.getDamage () [difficulty] * (1 + combo * COMBO_MULTIPLIER)*10;
			opponentHealthGauge -= damage;
			if (opponentHealthGauge <= 0) {
				ownWin = true;
				isBlocked = true;			
				setResult (Result.WIN);
			}

			// Increase opponent's special gauge
			opponentSpecialGauge += damage / DAMAGE_TO_SPECIAL_DIVISOR;
			if (opponentSpecialGauge >= 1) {
				opponentSpecialGauge = 1;
			}

			// Call RPC
			modifyOpponentSpecialGauge(opponentSpecialGauge);
			modifyOpponentHealthGauge (opponentHealthGauge);
			modifyOwnHealthGauge (ownHealthGauge);
			modifyOwnSpecialGauge (ownSpecialGauge);
		} else {
			resetCombo ();

			// Play sound effects
			audioSource.PlayOneShot (audioClips[GameController.AudioSourceIndex.FALSE]);
		}
		answerText.text = "0";
	}
		
	void modifyOpponentSpecialGauge (float specialGauge) {
		opponentSpecialGauge = specialGauge;
	}

	void modifyOpponentHealthGauge (float specialGauge) {
		opponentHealthGauge = specialGauge;
	}

	void modifyOwnSpecialGauge (float specialGauge) {
		ownSpecialGauge = specialGauge;
		if (ownSpecialGauge >= 1) {
			if (!specialButton.activeSelf) {
				audioSource.PlayOneShot (audioClips [GameController.AudioSourceIndex.SPECIAL_FULL]);
			}
			specialButton.SetActive (true);
		}
	}
		
	void modifyOwnHealthGauge (float healthGauge) {
		ownHealthGauge = healthGauge;
	}

	public void npcUseSpecial() {
		opponentSpecialGauge = 0;
		opponentCharacter.npcUseSpecial ();
	}
					
	public void useSpecial () {
		audioSource.PlayOneShot (audioClips [GameController.AudioSourceIndex.SPECIAL_USE]);
		ownSpecialGauge = 0;
		specialButton.SetActive (false);
		npcAttackTimeMultiplierTime = 5.0f;
		npcAttackTimeMultiplier = 1.15f;
	}

	public Button[] getNumberButtons() {
		return numberButtons;
	}

	public Vector3[] getNumberButtonDeffaultPositions() {
		return numberButtonDefaultPositions.ToArray ();
	}

	public void quitRoom() {
		SceneManager.LoadScene ("Lobby");
	}
	void setResult (int result) {
		if (result == Result.LOSE) {
			opponentWinCounter.add ();
		} else {
			ownWinCounter.add ();
		}
		resultPanel.SetActive (true);
		if (!(npcWin && ownWin)) {
			if (result == Result.LOSE) {
				resultText.text = getResultText (opponentHealthGauge / opponentCharacter.getMaxHp ());
			} else {
				resultText.text = getResultText (ownHealthGauge / ownCharacter.getMaxHp ());
			}
		} else {
			resultText.text = "DOUBLE K.O";
		}

		if (ownWinCounter.getWinCount () < WIN_NEEDED && opponentWinCounter.getWinCount () < WIN_NEEDED) {
			StopCoroutine (newRound ());
			StartCoroutine (newRound ());
		} else {
			StopCoroutine (announceWinner ());
			StartCoroutine (announceWinner ());
		}
		npcWin = false;
		ownWin = false;
	}

	string getResultText (float healthPercentage) {
		if (healthPercentage > 0.99f) {
			return "PERFECT";
		} else if (healthPercentage < 0.1f) {
			return "GREAT";
		} else {
			return "K.O";
		}
	}

	IEnumerator newRound () {
		yield return new WaitForSeconds (ANNOUNCEMENT_DELAY);

		ownHealthGauge = ownCharacter.getMaxHp ();
		combo = 0;
		comboTimer = 0;

		opponentHealthGauge = opponentCharacter.getMaxHp ();

		resultText.text = "";
		resultPanel.SetActive (false);

		generateNewProblem ();

		countDownPanel.SetActive (true);
	}

	IEnumerator announceWinner () {
		yield return new WaitForSeconds (ANNOUNCEMENT_DELAY);
		backgroundMusic.volume = 0.5f;
		if (ownWinCounter.getWinCount () == WIN_NEEDED) {
			if (opponentWinCounter.getWinCount () < WIN_NEEDED) {
				resultText.text = "WIN";
				audioSource.PlayOneShot (audioClips [GameController.AudioSourceIndex.WIN]);
			} else {
				resultText.text = "DRAW";
			}
		} else {
			resultText.text = "LOSE";
			audioSource.PlayOneShot (audioClips [GameController.AudioSourceIndex.LOSE]);
		}
		yield return new WaitForSeconds (GAME_OVER_DELAY);
		quitRoom ();
	}
		
}
