using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	private Toggle muteToggle;
	[SerializeField]
	private Toggle sfxToggle;

	[SerializeField]
	private Slider bgmSlider;

	[SerializeField]
	private Slider sfxSlider;

	private static bool isMute;

	void Awake() {
		bgmSlider.value = PlayerPrefs.GetFloat("bgmVolume");
		sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
		if (PlayerPrefs.GetInt("isMute") == 1) {
			muteToggle.isOn = true;
			bgmSource.volume = 0f;
			sfxSource [0].volume = 0f;
			sfxSource [1].volume = 0f;
		} else {
			muteToggle.isOn = false;
			bgmSource.volume =  bgmSlider.value;
			sfxSource [0].volume = sfxSlider.value;
			sfxSource [1].volume = sfxSlider.value;
		}

	}

	public void saveState() {
		PlayerPrefs.SetInt ("isMute",isMute == true ? 1 : 0);
		PlayerPrefs.SetFloat("bgmVolume", bgmSlider.value);
		PlayerPrefs.SetFloat("sfxVolume", sfxSlider.value);
	}

	public void showPauseCanvas() {
		pauseCanvas.gameObject.SetActive (true);
	}

	public void hidePauseCanvas() {
		pauseCanvas.gameObject.SetActive (false);
	}

	public void toggleMute() {
		if (muteToggle.isOn) {
			bgmSource.volume = 0f;
			sfxSource [0].volume = 0f;
			sfxSource [1].volume = 0f;
			isMute = true;
		} else {
			bgmSource.volume =  bgmSlider.value;
			sfxSource [0].volume = sfxSlider.value;
			sfxSource [1].volume = sfxSlider.value;
			isMute = false;
		}
		saveState ();
	}

	public void bgmVolume() {
		if (!isMute) {
			bgmSource.volume = bgmSlider.value;	
		}
		saveState ();
	}

	public void sfxVolume() {
		if (!isMute) {
			sfxSource [0].volume = sfxSlider.value;
			sfxSource [1].volume = sfxSlider.value;
		}
		saveState ();
	}
}

