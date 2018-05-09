using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : Photon.PunBehaviour, IPunObservable {
	#region IPunObservable implementation
	void IPunObservable.OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		
	}
	#endregion

	private static GameController _instance;

	public static GameController Instance {
		get {
			if (_instance == null) {
				_instance = (GameController)FindObjectOfType (typeof(GameController));
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
	private const float BURN_TIME = 1.5f;

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
	private GameObject characterPictHolder;
	[SerializeField]
	private Image ownPicture;
	[SerializeField]
	private Image opponentPicture;
	[SerializeField]
	private Text ownNameText;
	[SerializeField]
	private Text opponentNameText;

	[SerializeField]
	private GameObject countDownPanel;

	[SerializeField]
	private GameObject resultPanel;
	[SerializeField]
	private Image resultImage;
	[SerializeField]
	private GameObject panelClickToQuit;

	[SerializeField]
	private WinCounter ownWinCounter;
	[SerializeField]
	private WinCounter opponentWinCounter;

	[SerializeField]
	private AudioSource backgroundMusic;
	[SerializeField]
	private AudioSource sound;
	[SerializeField]
	private AudioSource tappingSound;

	[SerializeField]
	private GameObject ownAttackBallPrefab;
	[SerializeField]
	private Vector3 ownAttackBallSpawnPosition;
	[SerializeField]
	private GameObject opponentAttackBallPrefab;
	[SerializeField]
	private Vector3 opponentAttackBallSpawnPosition;
	[SerializeField]
	private GameObject explosionPrefab;

	private KeyValuePair<string, int> problemSet;
	private int solution;
	private int difficulty;

	private int combo;
	private float comboTimer;

	private float ownSpecialGauge;
	private float ownHealthGauge;

	private float opponentSpecialGauge;
	private float opponentHealthGauge;

	private float ownBurntTimer;
	private float opponentBurntTimer;

	private Character ownCharacter;
	private Character opponentCharacter;

	private Animator ownCharacterAnimator;
	private Animator opponentCharacterAnimator;

	private List <Vector3> numberButtonDefaultPositions;

	private bool resultReceived;

	enum CharacterName {
		Pencil,
		Eraser
	}

	#region Gameplay related

	void Start () {
		ownCharacter = (Character)ownCharacterObject.AddComponent (System.Type.GetType (CharacterHolder.Instance.OwnCharacterName));
		ownCharacterAnimator = ownCharacterObject.GetComponent<Animator> ();
		ownCharacterAnimator.runtimeAnimatorController = Resources.Load (ownCharacter.getControllerPath()) as RuntimeAnimatorController;
		this.photonView.RPC ("assignOpponentCharacter", PhotonTargets.Others, CharacterHolder.Instance.OwnCharacterName);

		// Add onClick listener to all number buttons and get default position of all number buttons
		numberButtonDefaultPositions = new List <Vector3> ();
		foreach (Button button in numberButtons) {
			button.onClick.AddListener (delegate {
				addNumberToAnswer (button.name [9]);
				tappingSound.PlayOneShot(GameSFX.TAP_NUMBER);
			});
			numberButtonDefaultPositions.Add (button.transform.position);
		}

		// Add onClick listener to all difficulty buttons
		difficultyButtons [Difficulty.EASY].onClick.AddListener (delegate {
			changeDifficulty (Difficulty.EASY);
			tappingSound.PlayOneShot(GameSFX.TAP_DIFFICULTY);
		});
		difficultyButtons [Difficulty.MEDIUM].onClick.AddListener (delegate {
			changeDifficulty (Difficulty.MEDIUM);
			tappingSound.PlayOneShot(GameSFX.TAP_DIFFICULTY);
		});
		difficultyButtons [Difficulty.HARD].onClick.AddListener (delegate {
			changeDifficulty (Difficulty.HARD);
			tappingSound.PlayOneShot(GameSFX.TAP_DIFFICULTY);
		});

		// Initialize combo & comboTimer
		combo = 0;
		comboTimerSlider.maxValue = ownCharacter.getComboTimer ();

		// Initialize own special & health
		ownSpecialGauge = 0;
		ownHealthGauge = ownCharacter.getMaxHp ();
		ownHealthBarSlider.maxValue = ownHealthGauge;
		ownHealthBarSlider.value = ownHealthGauge;
		specialButton.SetActive (false);

		// Initialize difficulty
		difficulty = Difficulty.MEDIUM;

		// Generate problem
		problemSet = ownCharacter.generateProblem (difficulty);
		problemText.text = problemSet.Key;

		// Initialize result received
		resultReceived = false;

		// Set character picture
		ownNameText.text = CharacterHolder.Instance.OwnCharacterName;
		int ownPictIndex = (int) System.Convert.ToUInt32(System.Enum.Parse(typeof(CharacterName), ownNameText.text));
		ownPicture.sprite = characterPictHolder.GetComponentsInChildren<Image>()[ownPictIndex].sprite;
	}

	void Update () {
		// Update combo timer
		if (comboTimer > 0) {
			comboTimer -= Time.deltaTime;
			comboTimerSlider.value = comboTimer;
		} else {
			resetCombo ();
		}

		// check burn state
		if (ownBurntTimer > 0) {
			ownBurntTimer -= Time.deltaTime;
		} else {
			unburnCharacter (ownCharacterObject);
		}
		if (opponentBurntTimer > 0) {
			opponentBurntTimer -= Time.deltaTime;
		} else {
			unburnCharacter (opponentCharacterObject);
		}

		// Animate bars
		AnimateSlider (ownSpecialBarSlider, ownSpecialGauge, SPECIAL_BAR_MODIFIER);
		AnimateSlider (opponentSpecialBarSlider, opponentSpecialGauge, SPECIAL_BAR_MODIFIER);
		AnimateSlider (ownHealthBarSlider, ownHealthGauge, HEALTH_BAR_MODIFIER);
		AnimateSlider (opponentHealthBarSlider, opponentHealthGauge, HEALTH_BAR_MODIFIER);
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
		deleteAnswer ();
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

			ownCharacterAnimator.SetTrigger (AnimationCommand.ATTACK);

			// Play sound effects
			tappingSound.PlayOneShot (GameSFX.ANSWER_CORRECT);

			// Add combo
			++combo;
			comboText.text = "" + combo;
			comboTimer = ownCharacter.getComboTimer ();

			// Increase own special gauge
			ownSpecialGauge += ownCharacter.getSpecialBarIncrease () [difficulty];
			if (ownSpecialGauge >= 1) {
				ownSpecialGauge = 1;
				if (!specialButton.activeSelf) {
					sound.PlayOneShot (GameSFX.SPECIAL_FULL);
				}
				specialButton.SetActive (true);
			}

			float damage = ownCharacter.getDamage () [difficulty] * (1 + combo * COMBO_MULTIPLIER);
			AttackBall ownAttackBall = Instantiate (ownAttackBallPrefab, opponentAttackBallSpawnPosition, Quaternion.identity).GetComponent <AttackBall> ();
			ownAttackBall.setDamage (damage);
			ownAttackBall.setOwn (true);

			// Call RPC
			this.photonView.RPC ("opponentAttack", PhotonTargets.Others, damage);
			this.photonView.RPC ("modifyOpponentSpecialGauge", PhotonTargets.Others, ownSpecialGauge);
		} else {
			resetCombo ();

			// Play sound effects
			tappingSound.PlayOneShot (GameSFX.ANSWER_FALSE);
		}
		deleteAnswer ();
	}

	[PunRPC]
	void opponentAttack (float damage) {
		AttackBall opponentAttackBall = Instantiate (opponentAttackBallPrefab, opponentAttackBallSpawnPosition, Quaternion.identity).GetComponent <AttackBall> ();
		opponentAttackBall.transform.Rotate (new Vector3 (0f, 180f));
		opponentAttackBall.setDamage (damage);
		opponentAttackBall.setOwn (false);
		opponentCharacterAnimator.SetTrigger (AnimationCommand.ATTACK);
	}

	public void hitOwn (float damage) {
		// Decrease own health
		ownHealthGauge -= damage;
		ownCharacterAnimator.SetTrigger (AnimationCommand.ATTACKED);
		if (ownHealthGauge <= 0) {
			resultPanel.SetActive (true);
			this.photonView.RPC ("setResult", PhotonTargets.Others, Result.WIN);
		}

		// Increace own special gauge
		ownSpecialGauge += damage / DAMAGE_TO_SPECIAL_DIVISOR;
		if (ownSpecialGauge >= 1) {
			if (!specialButton.activeSelf) {
				sound.PlayOneShot (GameSFX.SPECIAL_FULL);
			}
			specialButton.SetActive (true);
		}
	}

	public void hitOpponent (float damage) {
		// Decrease opponent's health
		opponentHealthGauge -= damage;
		opponentCharacterAnimator.SetTrigger (AnimationCommand.ATTACKED);
		if (opponentHealthGauge <= 0) {
			resultPanel.SetActive (true);
			this.photonView.RPC ("setResult", PhotonTargets.Others, Result.LOSE);
		}

		// Increase opponent's special gauge
		opponentSpecialGauge += damage / DAMAGE_TO_SPECIAL_DIVISOR;
		if (opponentSpecialGauge >= 1) {
			opponentSpecialGauge = 1;
		}
	}

	[PunRPC]
	void modifyOpponentSpecialGauge (float opponentSpecialGauge) {
		this.opponentSpecialGauge = opponentSpecialGauge;
	}

	 Sprite getResultSprite (float healthPercentage) {
		if (healthPercentage > 0.99f) {
			return ResultSprite.PERFECT;
		} else if (healthPercentage < 0.1f) {
			return ResultSprite.GREAT;
		} else {
			return ResultSprite.KO;
		}
	}

	[PunRPC]
	void setResult (int result) {
		if (result == Result.LOSE) {
			opponentWinCounter.add ();
		} else {
			ownWinCounter.add ();
		}
		if (!resultReceived) {
			resultReceived = true;
			if (result == Result.LOSE) {
				resultImage.sprite = getResultSprite (opponentHealthGauge / opponentCharacter.getMaxHp ());
			} else {
				resultImage.sprite = getResultSprite (ownHealthGauge / ownCharacter.getMaxHp ());
			}
		} else {
			resultImage.sprite = ResultSprite.DOUBLE_KO;
		}
			
		if (ownWinCounter.getWinCount () < WIN_NEEDED && opponentWinCounter.getWinCount () < WIN_NEEDED) {
			StopCoroutine (newRound ());
			StartCoroutine (newRound ());
		} else {
			StopCoroutine (announceWinner ());
			StartCoroutine (announceWinner ());
		}
	}
					
	public void useSpecial () {
		sound.PlayOneShot(GameSFX.SPECIAL_LAUNCH);
		ownSpecialGauge = 0;
		specialButton.SetActive (false);
		ownCharacterAnimator.SetTrigger (AnimationCommand.SPECIAL);
		Instantiate (explosionPrefab, explosionPrefab.transform.position, Quaternion.identity);
		burnCharacter (opponentCharacterObject);
		opponentBurntTimer = BURN_TIME;
		this.photonView.RPC ("modifyOpponentSpecialGauge", PhotonTargets.Others, ownSpecialGauge);
		this.photonView.RPC ("opponentUseSpecial", PhotonTargets.Others);
	}

	[PunRPC]
	public void opponentUseSpecial () {
		sound.PlayOneShot(GameSFX.SPECIAL_LAUNCH);
		opponentCharacterAnimator.SetTrigger (AnimationCommand.SPECIAL);
		Vector3 ownExplosionPosition = explosionPrefab.transform.position;
		ownExplosionPosition.x *= -1;
		Instantiate (explosionPrefab, ownExplosionPosition, Quaternion.identity);
		burnCharacter (ownCharacterObject);
		ownBurntTimer = BURN_TIME;
		opponentCharacter.useSpecial ();
	}

	private void burnCharacter (GameObject characterObj) {
		characterObj.GetComponent<SpriteRenderer> ().color = new Color (0f, 0f, 0f);
	}

	private void unburnCharacter (GameObject characterObj) {
		characterObj.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f);
	}

	public Button[] getNumberButtons() {
		return numberButtons;
	}

	public Vector3[] getNumberButtonDeffaultPositions() {
		return numberButtonDefaultPositions.ToArray ();
	}

	[PunRPC]
	public void assignOpponentCharacter (string characterName) {
		opponentCharacter = (Character)opponentCharacterObject.AddComponent (System.Type.GetType (characterName));
		opponentCharacterAnimator = opponentCharacterObject.GetComponent<Animator> ();
		opponentCharacterAnimator.runtimeAnimatorController = Resources.Load (opponentCharacter.getControllerPath()) as RuntimeAnimatorController;
		opponentSpecialGauge = 0;
		opponentHealthGauge = opponentCharacter.getMaxHp ();
		opponentHealthBarSlider.maxValue = opponentHealthGauge;
		opponentHealthBarSlider.value = opponentHealthGauge;

		opponentNameText.text = characterName;
		int opponentPictIndex = (int) System.Convert.ToUInt32(System.Enum.Parse(typeof(CharacterName), opponentNameText.text));
		opponentPicture.sprite = characterPictHolder.GetComponentsInChildren<Image>()[opponentPictIndex].sprite;

		blockingPanel.SetActive (false);
		countDownPanel.SetActive (true);
	}
		
	IEnumerator newRound () {
		yield return new WaitForSeconds (ANNOUNCEMENT_DELAY);

		ownHealthGauge = ownCharacter.getMaxHp ();
		combo = 0;
		comboTimer = 0;

		opponentHealthGauge = opponentCharacter.getMaxHp ();

		resultReceived = false;

		resultImage.sprite = null;
		resultPanel.SetActive (false);

		generateNewProblem ();

		countDownPanel.SetActive (true);
	}

	IEnumerator announceWinner () {
		yield return new WaitForSeconds (ANNOUNCEMENT_DELAY);
		backgroundMusic.volume = 0.5f;
		if (ownWinCounter.getWinCount () == WIN_NEEDED) {
			if (opponentWinCounter.getWinCount () < WIN_NEEDED) {
				resultImage.sprite = ResultSprite.WIN;
				sound.PlayOneShot (GameSFX.WIN);
				ownCharacterAnimator.SetTrigger (AnimationCommand.WIN);
				opponentCharacterAnimator.SetTrigger (AnimationCommand.LOSE);
			} else {
				resultImage.sprite = ResultSprite.DRAW;
				sound.PlayOneShot (GameSFX.DRAW);
				ownCharacterAnimator.SetTrigger (AnimationCommand.DRAW);
				opponentCharacterAnimator.SetTrigger (AnimationCommand.DRAW);
			}
		} else {
			resultImage.sprite = ResultSprite.LOSE;
			sound.PlayOneShot (GameSFX.LOSE);
			ownCharacterAnimator.SetTrigger (AnimationCommand.LOSE);
			opponentCharacterAnimator.SetTrigger (AnimationCommand.WIN);
		}
		panelClickToQuit.SetActive (true);
	}
		
	#endregion

	#region others

	public override void OnLeftRoom () {
		SceneManager.LoadScene (GameScene.LOBBY);
	}

	public void leaveRoom () {
		PhotonNetwork.LeaveRoom ();
	}

	public override void OnPhotonPlayerDisconnected (PhotonPlayer other) {
		Debug.Log ("OnPhotonPlayerDisconnected()");
		leaveRoom ();
	}
		
	#endregion

}
