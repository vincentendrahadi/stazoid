using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBallMovement : MonoBehaviour {
	[SerializeField]
	private float SPEED = 20.0f;

	private float startTime;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (transform.position.x + (SPEED * (Time.time - startTime)), transform.position.y, transform.position.z);
	}
}
