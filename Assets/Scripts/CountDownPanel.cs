using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownPanel : MonoBehaviour {

	private const float COUNT_DOWN_LENGTH = 2.0f;
	private const float TEXT_CHANGE_TIME = 1.0f;
	private const int FINAL_ROUND = 5;

	[SerializeField]
	private Text countDownText;

	private float countDown;
	private int round;

	void Start () {
		round = 1;
	}

	void OnEnable () {
		countDown = COUNT_DOWN_LENGTH;
		if (round == FINAL_ROUND) { 
			countDownText.text = "FINAL ROUND";
		} else {
			countDownText.text = "ROUND 1";
		}
	}

	void Update () {
		countDown -= Time.deltaTime;
		if (countDown <= 0) {
			++round;
			gameObject.SetActive (false);
		} else if (countDown < TEXT_CHANGE_TIME) {
			countDownText.text = "FIGHT!";
		}
	}
}
