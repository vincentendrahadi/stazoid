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

	public void showPauseCanvas() {
		pauseCanvas.gameObject.SetActive (true);
	}

	public void hidePauseCanvas() {
		pauseCanvas.gameObject.SetActive (false);
	}

	public void mute() {
		bgmSource.volume = 0f;
		sfxSource [0].volume = 0f;
		sfxSource [1].volume = 0f;
	}

	public void unmute() {
		// isi sesuai nilai slider * MAX masing-masing (SoundInfo.MAX_something)
	}
		
}

