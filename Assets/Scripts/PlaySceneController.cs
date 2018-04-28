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

	const int BGM_VOLUME = 1;
	const int SFX_VOLUME = 2;
	const int MUTE_TOGGLE = 3;

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

	public void saveState(int state) {
		switch (state) {
			case 1:
				PlayerPrefs.SetFloat ("bgmVolume", bgmSlider.value);
				break;
			case 2:	
				PlayerPrefs.SetFloat ("sfxVolume", sfxSlider.value);
				break;
			case 3:
				PlayerPrefs.SetInt ("isMute", isMute == true ? 1 : 0);
				break;
		}
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
		saveState (MUTE_TOGGLE);
	}

	public void bgmVolume() {
		if (!isMute) {
			bgmSource.volume = bgmSlider.value;	
		}
		saveState (BGM_VOLUME);
	}

	public void sfxVolume() {
		if (!isMute) {
			sfxSource [0].volume = sfxSlider.value;
			sfxSource [1].volume = sfxSlider.value;
		}
		saveState (SFX_VOLUME);
	}
}

