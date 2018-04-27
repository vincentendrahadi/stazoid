using UnityEngine;
using System.Collections;

public class ScrollRectSnap : MonoBehaviour {
	public RectTransform listHolder;
	public GameObject[] scrolledObjList;
	public RectTransform center;

	private float[] objVerticalDistanceToCenter;
	private float[] objHorizontalDistanceToCenter;
	private float nearestDistanceToCenter;

	private bool dragging = false;
	private bool isScrollVertical;

	private int verticalDistanceBetweenObj;
	private int horizontalDistanceBetweenObj;
	private int idxObjNearestToCenter;
	private int listLength;

	// Use this for initialization
	void Start () {
		listLength = scrolledObjList.Length;
		objVerticalDistanceToCenter = new float[listLength];
		objHorizontalDistanceToCenter = new float[listLength];

		verticalDistanceBetweenObj = (int)Mathf.Abs (scrolledObjList [1].GetComponent<RectTransform> ().anchoredPosition.y 
			- scrolledObjList [0].GetComponent<RectTransform> ().anchoredPosition.y);
		horizontalDistanceBetweenObj = (int)Mathf.Abs (scrolledObjList [1].GetComponent<RectTransform> ().anchoredPosition.x 
			- scrolledObjList [0].GetComponent<RectTransform> ().anchoredPosition.x);

		checkScrollBehaviour ();

		resetNearestDistance ();
	}
	
	// Update is called once per frame
	void Update () {		
		for (int i = 0; i < listLength; i++) {
			objVerticalDistanceToCenter[i] = Mathf.Abs(center.transform.position.y - scrolledObjList [i].transform.position.y);
			if (objVerticalDistanceToCenter [i] < nearestDistanceToCenter) {
				nearestDistanceToCenter = objVerticalDistanceToCenter [i];
				idxObjNearestToCenter = i;
			}
		}

		resetNearestDistance ();

		if (!dragging) {
			if (isScrollVertical) {
				lerpToObj (idxObjNearestToCenter * verticalDistanceBetweenObj);
			} else {
				lerpToObj (idxObjNearestToCenter * -horizontalDistanceBetweenObj);
			}
		}
	}

	private void checkScrollBehaviour() {
		isScrollVertical = verticalDistanceBetweenObj != 0;
	}

	private void resetNearestDistance() {
		nearestDistanceToCenter = (isScrollVertical) ? verticalDistanceBetweenObj : horizontalDistanceBetweenObj;
	}

	void lerpToObj(int position) {
		Vector2 newCoordinate;

		if (isScrollVertical) {
			newCoordinate = new Vector2 (listHolder.anchoredPosition.x, (Mathf.Lerp (listHolder.anchoredPosition.y, position, Time.deltaTime * 10f)));
		} else {
			newCoordinate = new Vector2 ((Mathf.Lerp (listHolder.anchoredPosition.x, position, Time.deltaTime * 10f)), listHolder.anchoredPosition.y);
		}

		listHolder.anchoredPosition = newCoordinate;
	}

	public void setDragging(bool value) {
		dragging = value;
	}
}

