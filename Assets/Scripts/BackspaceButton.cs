using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BackspaceButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	[SerializeField]
	private float DELAY_TIMER;
	[SerializeField]
	private float REPEAT_TIMER;

	private bool isPressed;
	private float delayTimer;
	private float repeatTimer;

	void Start () {
		isPressed = false;
	}

	void Update () {
		if (isPressed) {
			if (delayTimer > 0) {
				delayTimer -= Time.deltaTime;
			} else {
				if (repeatTimer > 0) {
					repeatTimer -= Time.deltaTime;
				} else {
					Debug.Log ("hold");
					GameController.Instance.backspace ();
					repeatTimer = REPEAT_TIMER;
				}
			}
		}
	}

	public void OnPointerDown (PointerEventData eventData) {
		GameController.Instance.backspace ();
		isPressed = true;
		delayTimer = DELAY_TIMER;
		repeatTimer = REPEAT_TIMER;
	}

	public void OnPointerUp (PointerEventData eventData) {
		isPressed = false;
	}
		
}
