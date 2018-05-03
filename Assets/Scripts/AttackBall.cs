using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBall : MonoBehaviour {
	[SerializeField]
	private float SPEED = 20.0f;

	private float damage;
	private bool isMultiplayer;
	private bool isOwn;

	void Start () {
		isMultiplayer = GameController.Instance != null;
	}

	void FixedUpdate () {
		transform.position += Vector3.right * SPEED * Time.deltaTime * (isOwn ? 1 : -1);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.name == "Own Character") {
			if (isMultiplayer) {
				GameController.Instance.hitOwn (damage);
			} else {
				SinglePlayerController.Instance.hitOwn (damage);
			}
			transform.GetComponent<Animator>().SetTrigger (AnimationCommand.HIT);
			Destroy (gameObject);
		} else if (other.gameObject.name == "Opponent's Character") {
			if (isMultiplayer) {
				GameController.Instance.hitOpponent (damage);
			} else {
				SinglePlayerController.Instance.hitOpponent (damage);
			}
			transform.GetComponent<Animator>().SetTrigger (AnimationCommand.HIT);
			Destroy (gameObject);
		}
	}
		
	public void setDamage (float damage) {
		this.damage = damage;
	}

	public void setOwn (bool isOwn) {
		this.isOwn = isOwn;
	}

}
