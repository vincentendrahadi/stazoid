using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBall : MonoBehaviour {
	[SerializeField]
	private float SPEED = 20.0f;

	private float damage;
	bool isMultiplayer;

	void Start () {
		isMultiplayer = GameController.Instance != null;
	}

	void FixedUpdate () {
		transform.position += Vector3.right * SPEED * Time.deltaTime;
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name == "Own Character") {
			if (isMultiplayer) {
				GameController.Instance.hitOwn (damage);
			} else {
				// TODO: isi d sini VE
			}
		} else {
			if (isMultiplayer) {
				GameController.Instance.hitOpponent (damage);
			} else {
				// Sama kayak di atas
			}
		}
	}
		
	public void launch (float damage) {
		this.damage = damage;
		this.gameObject.SetActive (true);
	}

}
