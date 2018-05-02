using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioController : MonoBehaviour {
	[SerializeField]
	private AudioSource bgmSource;
	[SerializeField]
	private AudioSource[] sfxSource;

	[SerializeField]
	private Toggle muteToggle;

	[SerializeField]
	private Slider bgmSlider;

	[SerializeField]
	private Slider sfxSlider;

	private static bool isMute;

	const int BGM_VOLUME = 1;
	const int SFX_VOLUME = 2;
	const int MUTE_TOGGLE = 3;

	class MaxSound : MonoBehaviour {
		public const float BGM = 1f;
		public const float TAPPING = 0.3f;
		public const float SOUND = 1f;
	}

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
			bgmSource.volume =  bgmSlider.value * MaxSound.BGM;
			sfxSource [0].volume = sfxSlider.value * MaxSound.TAPPING;
			sfxSource [1].volume = sfxSlider.value * MaxSound.SOUND;
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

	public void toggleMute() {
		if (muteToggle.isOn) {
			bgmSource.volume = 0f;
			sfxSource [0].volume = 0f;
			sfxSource [1].volume = 0f;
			isMute = true;
		} else {
			bgmSource.volume =  bgmSlider.value * MaxSound.BGM;
			sfxSource [0].volume = sfxSlider.value * MaxSound.TAPPING;
			sfxSource [1].volume = sfxSlider.value * MaxSound.SOUND;
			isMute = false;
		}
		saveState (MUTE_TOGGLE);
	}

	public void bgmVolume() {
		if (!isMute) {
			bgmSource.volume = bgmSlider.value * MaxSound.BGM;	
		}
		saveState (BGM_VOLUME);
	}

	public void sfxVolume() {
		if (!isMute) {
			sfxSource [0].volume = sfxSlider.value * MaxSound.TAPPING;
			sfxSource [1].volume = sfxSlider.value * MaxSound.SOUND;
		}
		saveState (SFX_VOLUME);
	}
}

