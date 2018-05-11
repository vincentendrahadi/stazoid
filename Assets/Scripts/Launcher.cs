using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : Photon.PunBehaviour {

	private const string GAME_VERSION = "1";
	private const string PLAY_SCENE_NAME = "PlayLandscape"; 
	private const string SINGLE_PLAYER_SCENE_NAME = "SinglePlayer";

	[SerializeField]
	private GameObject launcher;
	[SerializeField]
	private AudioSource audioSource;
	[SerializeField]
	private Button[] menuButtons;

	[SerializeField]
	private PhotonLogLevel LOG_LEVEL = PhotonLogLevel.Informational;
	[SerializeField]
	private byte MAX_PLAYER_PER_ROOM = 2;

	[SerializeField]
	private Button playButton;
	[SerializeField]
	private GameObject statusText;
	[SerializeField]
	private Button selectedCharacterButton;
	[SerializeField]
	private Button[] characterList;
	[SerializeField]
	private GameObject cancelButton;
	[SerializeField]
	private Text characterNameDisplay;

	[SerializeField]
	private AudioSource tappingButtons;
	[SerializeField]
	private AudioSource bgmSound;

	[SerializeField]
	private GameObject canvasCaracterSelection;
	[SerializeField]
	private GameObject canvasWaiting;

	private bool isPlayingOnline;
	private bool isConnecting;
	private Button currentCharacter;
	private Vector3 STANDARD_CHARACTER_SCALE = new Vector3(0.3f, 0.3f, 1);
	private Vector3 ZOOMED_CHARACTER_SCALE = new Vector3(0.5f, 0.5f, 1);

	void Start() {
		foreach (Button button in menuButtons) {
			button.onClick.AddListener (delegate {
				audioSource.PlayOneShot(GameSFX.TAP_MENU);
			});
		}

		foreach (Button character in characterList) {
			character.onClick.AddListener (delegate {
				chooseCharacter(character);
				if (isPlayingOnline) {
					Connect();
				} else {
					PlayWithNPC();
				}
			});
		}
		if (PlayerPrefs.GetFloat ("bgmVolume") == null && PlayerPrefs.GetFloat ("sfxVolume") == null) {
			bgmSound.volume = 50;
			tappingButtons.volume = 50;
			PlayerPrefs.SetFloat ("bgmVolume", bgmSound.volume);
			PlayerPrefs.SetFloat ("sfxVolume", tappingButtons.volume);
			PlayerPrefs.SetInt ("isMute", 0);
		} else {
			bgmSound.volume = PlayerPrefs.GetFloat ("bgmVolume");
			tappingButtons.volume = PlayerPrefs.GetFloat ("sfxVolume");
			if (PlayerPrefs.GetInt ("isMute") == 1) {
				bgmSound.volume = 0;
				tappingButtons.volume = 0;
			}
		}

	}

	void Awake () {
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.logLevel = LOG_LEVEL;
		for (int i = 0; i < characterList.Length; i++) {
			characterList [i].interactable = false;
		}
		currentCharacter = characterList [0];

		SnapScroller.idxObjNearestToCenter = 0;
	}

	public string getNPCCharacterName() {
		int opponentCharacterNameIndex = Random.Range (0, characterList.Length);
		return characterList [opponentCharacterNameIndex].transform.GetChild (0).GetComponent <Text> ().text;
	}

	public void PlayWithNPC() {
		CharacterHolder.Instance.OwnCharacterName = selectedCharacterButton.transform.GetChild (0).GetComponent <Text> ().text;
		CharacterHolder.Instance.NpcCharacterName = getNPCCharacterName ();
		SceneManager.LoadScene (GameScene.SINGLE_PLAYER);
	}

	public void Connect () {
		isConnecting = true;
		canvasWaiting.SetActive(true);
		canvasCaracterSelection.SetActive(false);

		if (PhotonNetwork.connected) {
			PhotonNetwork.JoinRandomRoom ();
		} else {
			PhotonNetwork.ConnectUsingSettings (GAME_VERSION);
		}
	}

	public void Disconnect () {
		PhotonNetwork.Disconnect ();
	}

	public override void OnConnectedToMaster () {
		Debug.Log ("Stazoid/Launcher: OnConnectedToMaster() was called by PUN");
		if (isConnecting) {
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	public override void OnDisconnectedFromPhoton () {
		Debug.Log ("Stazoid/Launcher: OnDisconnectedFromPhoton() was called by PUN");

		isConnecting = false;
		canvasWaiting.SetActive(false);
		canvasCaracterSelection.SetActive(true);
	}

	public override void OnPhotonRandomJoinFailed (object[] codeAndMsg) {
		Debug.Log ("Stazoid/Launcher: OnPhotonRandomJoinFailed() was called by PUN");
		PhotonNetwork.CreateRoom (null, new RoomOptions () { MaxPlayers = MAX_PLAYER_PER_ROOM }, null);
	}

	public override void OnJoinedRoom () {
		Debug.Log ("Stazoid/Launcher: OnJoinedRoom() called by PUN");

		CharacterHolder.Instance.OwnCharacterName = selectedCharacterButton.transform.GetChild (0).GetComponent <Text> ().text;
	}

	void EnterGame () {
		if (!PhotonNetwork.isMasterClient) {
			Debug.LogError ("PhotontNetwork : Trying to load level but we are not master Client");
		}
		Debug.Log ("PhotonNetwork : Loading Level");
		PhotonNetwork.LoadLevel (PLAY_SCENE_NAME);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer other) {
		Debug.Log ("OnPhotonPlayerConnected()");

		if (PhotonNetwork.isMasterClient) {
			Debug.Log ("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);

			EnterGame ();
		}
	}

	public void choosePlayingOnline() {
		isPlayingOnline = true;
		canvasCaracterSelection.SetActive (true);
		changeCharacterInteractable ();
	}

	public void choosePlayingOffline() {
		isPlayingOnline = false;
		canvasCaracterSelection.SetActive (true);
		changeCharacterInteractable ();
	}

	public void chooseCharacter (Button button) {
		audioSource.PlayOneShot (GameSFX.CHOSE_CHAR);
		selectedCharacterButton = button;
	}

	public void changeCharacterInteractable() {
		currentCharacter.interactable = false;
		currentCharacter.transform.localScale = STANDARD_CHARACTER_SCALE;
		currentCharacter = characterList [SnapScroller.idxObjNearestToCenter];
		currentCharacter.interactable = true;
		currentCharacter.transform.localScale = ZOOMED_CHARACTER_SCALE;
		characterNameDisplay.text = (currentCharacter.transform.GetChild (0).GetComponent <Text> ().text).ToUpper();
	}
}
