using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour {

	public void showCanvas(Canvas controlledCanvas) {
		controlledCanvas.gameObject.SetActive (true);
	}

	public void hideCanvas(Canvas controlledCanvas) {
		controlledCanvas.gameObject.SetActive (false);
	}

}

