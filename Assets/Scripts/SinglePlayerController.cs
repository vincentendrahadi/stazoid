using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SinglePlayerController : MonoBehaviour {

	private static GameController _instance;

	public static GameController Instance {
		get {
			if (_instance == null) {
				_instance = (GameController)FindObjectOfType (typeof(GameController));
			}
			return _instance;
		}
	}

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
	private Text comboText;
	[SerializeField]
	private Slider comboTimerSlider;
	[SerializeField]
	private GameObject specialButton;

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
	private GameObject resultPanel;
	[SerializeField]
	private Text resultText;

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
		Debug.Log ("Single Player");
		ownCharacter = (Character)ownCharacterObject.AddComponent (System.Type.GetType (CharacterHolder.Instance.OwnCharacterName));
		opponentCharacter = (Character)opponentCharacterObject.AddComponent (System.Type.GetType (CharacterHolder.Instance.NpcCharacterName));

		// Force landscape orientation
		// Screen.orientation = ScreenOrientation.Landscape;

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

		// Animate bars
		AnimateSlider (ownSpecialBarSlider, ownSpecialGauge, SPECIAL_BAR_MODIFIER);
		AnimateSlider (opponentSpecialBarSlider, opponentSpecialGauge, SPECIAL_BAR_MODIFIER);
		AnimateSlider (ownHealthBarSlider, ownHealthGauge, HEALTH_BAR_MODIFIER);
		AnimateSlider (opponentHealthBarSlider, opponentHealthGauge, HEALTH_BAR_MODIFIER);


//		//NPC use special
		if(opponentSpecialGauge >= 1) {
			npcUseSpecial ();
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
		float damage = opponentCharacter.getDamage () [npcDifficulty] * (1 + combo * COMBO_MULTIPLIER);
		ownHealthGauge -= damage;
		if (ownHealthGauge <= 0) {
			resultPanel.SetActive (true);
			resultText.text = "LOSE";
		}

		// Increase opponent's special gauge
		ownSpecialGauge += damage / DAMAGE_TO_SPECIAL_DIVISOR;
		if (ownSpecialGauge >= 1) {
			ownSpecialGauge = 1;
		}

		// Call RPC
		modifyOpponentSpecialGauge(opponentSpecialGauge);
		modifyOpponentHealthGauge (opponentHealthGauge);
		modifyOwnHealthGauge (ownHealthGauge);
		modifyOwnSpecialGauge (ownSpecialGauge);
	}

	public void judgeAnswer() {
		if (int.Parse (answerText.text) == problemSet.Value) {
			generateNewProblem ();

			// Add combo
			++combo;
			comboText.text = "" + combo;
			comboTimer = ownCharacter.getComboTimer ();

			// Increase own special gauge
			ownSpecialGauge += ownCharacter.getSpecialBarIncrease () [difficulty];
			if (ownSpecialGauge >= 1) {
				ownSpecialGauge = 1;
				specialButton.SetActive (true);
			}

			// Decrease opponent's health
			float damage = ownCharacter.getDamage () [difficulty] * (1 + combo * COMBO_MULTIPLIER);
			opponentHealthGauge -= damage;
			if (opponentHealthGauge <= 0) {
				resultPanel.SetActive (true);
				resultText.text = "WIN";
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
//			this.photonView.RPC ("modifyOpponentSpecialGauge", PhotonTargets.Others, ownSpecialGauge);
//			this.photonView.RPC ("modifyOwnHealthGauge", PhotonTargets.Others, opponentHealthGauge);
//			this.photonView.RPC ("modifyOwnSpecialGauge", PhotonTargets.Others, opponentSpecialGauge);
		} else {
			resetCombo ();
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
			specialButton.SetActive (true);
		}
	}
		
	void modifyOwnHealthGauge (float healthGauge) {
		ownHealthGauge = healthGauge;
		if (ownHealthGauge <= 0) {
			resultPanel.SetActive (true);
			resultText.text = "LOSE";
		}
	}

	public void npcUseSpecial() {
		opponentSpecialGauge = 0;
		opponentCharacter.npcUseSpecial ();
	}
					
	public void useSpecial () {
		ownSpecialGauge = 0;
		specialButton.SetActive (false);
		npcAttackTimeMultiplierTime = 5.0f;
		npcAttackTimeMultiplier = 1.15f;
//		this.photonView.RPC ("modifyOpponentSpecialGauge", PhotonTargets.Others, ownSpecialGauge);
//		opponentCharacter.photonView.RPC ("useSpecial", PhotonTargets.Others);
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

		
}
