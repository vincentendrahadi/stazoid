using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : Photon.PunBehaviour {

	private const string GAME_VERSION = "1";
	private const string PLAY_SCENE_NAME = "PlayLandscape"; 

	[SerializeField]
	private PhotonLogLevel LOG_LEVEL = PhotonLogLevel.Informational;
	[SerializeField]
	private byte MAX_PLAYER_PER_ROOM = 2;

	[SerializeField]
	private Button playButton;
	[SerializeField]
	private GameObject statusText;

	private bool isConnecting;

	void Awake () {
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.logLevel = LOG_LEVEL;

		statusText.SetActive (false);
	}

	public void Connect () {
		isConnecting = true;
		statusText.SetActive (true);
		playButton.interactable = false;

		PhotonNetwork.ConnectUsingSettings (GAME_VERSION);
	}

	public override void OnConnectedToMaster () {
		Debug.Log ("Stazoid/Launcher: OnConnectedToMaster() was called by PUN");
		if (isConnecting) {
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	public override void OnDisconnectedFromPhoton () {
		Debug.Log ("Stazoid/Launcher: OnDisconnectedFromPhoton() was called by PUN");

		statusText.SetActive (false);
		playButton.interactable = true;
	}

	public override void OnPhotonRandomJoinFailed (object[] codeAndMsg) {
		Debug.Log ("Stazoid/Launcher: OnPhotonRandomJoinFailed() was called by PUN");
		PhotonNetwork.CreateRoom (null, new RoomOptions () { MaxPlayers = MAX_PLAYER_PER_ROOM }, null);
	}

	public override void OnJoinedRoom () {
		Debug.Log ("Stazoid/Launcher: OnJoinedRoom() called by PUN");
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

}
