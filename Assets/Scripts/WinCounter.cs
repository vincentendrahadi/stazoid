using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinCounter : MonoBehaviour {

	private Image[] ballImages;
	private int winCount;

	void Start () {
		ballImages = GetComponentsInChildren <Image> ();
		winCount = 0;
	}

	public void add () {
		++winCount;
		ballImages [winCount - 1].color = new Color (0, 1, 1);
	}

	public int getWinCount () {
		return winCount;
	}
}
