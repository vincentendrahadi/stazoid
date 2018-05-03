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

	void OnCollisionEnter (Collision collision) {
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
		transform.GetComponent<Animator>().SetTrigger (AnimationCommand.HIT);
		Destroy (gameObject);
	}
		
	public void setDamage (float damage) {
		this.damage = damage;
	}

	public void setOwn (bool isOwn) {
		this.isOwn = isOwn;
	}

}
