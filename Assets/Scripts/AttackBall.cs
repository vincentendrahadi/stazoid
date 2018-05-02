using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBall : MonoBehaviour {
	[SerializeField]
	private float SPEED = 20.0f;
	[SerializeField]
	private Vector3 INITIAL_POSITION;

	private float damage;
	bool isMultiplayer;

	void Start () {
		isMultiplayer = GameController.Instance != null;
	}

	void FixedUpdate () {
		transform.position += Vector3.right * SPEED * Time.deltaTime;
	}

	void OnCollisionEnter(Collision collision) {
		gameObject.SetActive (false);
		if (collision.gameObject.name == "Own Character") {
			if (isMultiplayer) {
				GameController.Instance.hitOwn (damage);
			} else {
				SinglePlayerController.Instance.hitOwn (damage);
			}
		} else {
			if (isMultiplayer) {
				GameController.Instance.hitOpponent (damage);
			} else {
				SinglePlayerController.Instance.hitOpponent (damage);
			}
		}
	}
		
	public void setDamage(float damage) {
		this.damage = damage;
	}

}
