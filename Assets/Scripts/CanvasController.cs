using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour {
	[SerializeField]
	private Canvas controlledCanvas;

	public void showCanvas() {
		controlledCanvas.gameObject.SetActive (true);
	}

	public void hideCanvas() {
		controlledCanvas.gameObject.SetActive (false);
	}

}

