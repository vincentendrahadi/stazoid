using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlaySceneController : MonoBehaviour {
	[SerializeField]
	private Canvas pauseCanvas;

	[SerializeField]
	private AudioSource bgmSource;
	[SerializeField]
	private AudioSource[] sfxSource;

	[SerializeField]
	private Toggle bgmToggle;
	[SerializeField]
	private Toggle sfxToggle;

	void Start() {
		pauseCanvas.gameObject.SetActive (false);
	}

	public void showPauseCanvas() {
		pauseCanvas.gameObject.SetActive (true);
	}

	public void hidePauseCanvas() {
		pauseCanvas.gameObject.SetActive (false);
	}

	public void toggleBGM() {
		if (bgmToggle.isOn) {
			bgmSource.volume = 1f;
		} else {
			bgmSource.volume = 0f;
		}
	}

	public void toggleSFX() {
		if (sfxToggle.isOn) {
			sfxSource[0].volume = 0.3f;
			sfxSource[1].volume = 1f;
		} else {
			sfxSource[0].volume = 0f;
			sfxSource[1].volume = 0f;
		}
	}
}

