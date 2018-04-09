using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NumberButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	private Image image;

	void Start () {
		image = GetComponent <Image> ();
	}

	public void OnPointerDown (PointerEventData eventData) {
		image.SetNativeSize ();
	}

	public void OnPointerUp (PointerEventData eventData) {
		image.SetNativeSize ();
	}

}
