using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenController : MonoBehaviour {

	void Awake() {
		StopCoroutine (closeSplashScreen ());
		StartCoroutine (closeSplashScreen ());
	}

	IEnumerator closeSplashScreen() {
		yield return new WaitForSeconds (1.5f);
		loadLobby ();
	}

	public void loadLobby() {
		UnityEngine.SceneManagement.SceneManager.LoadScene (GameScene.LOBBY);
	}
}
