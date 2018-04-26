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

	[SerializeField]
	private Slider bgmSlider;

	[SerializeField]
	private Slider sfxSlider;

	public void showPauseCanvas() {
		pauseCanvas.gameObject.SetActive (true);
	}

	public void hidePauseCanvas() {
		pauseCanvas.gameObject.SetActive (false);
	}

	public void toggleMute() {
		if (bgmSource.volume > 0) {
			bgmSource.volume = 0f;
			sfxSource [0].volume = 0f;
			sfxSource [1].volume = 0f;
		} else {
			bgmSource.volume =  bgmSlider.value;
			sfxSource [0].volume = sfxSlider.value;
			sfxSource [1].volume = sfxSlider.value;
		}

	}

	public void bgmVolume() {
		bgmSource.volume = bgmSlider.value;
	}

	public void sfxVolume() {
		sfxSource [0].volume = sfxSlider.value;
		sfxSource [1].volume = sfxSlider.value;
	}
}

