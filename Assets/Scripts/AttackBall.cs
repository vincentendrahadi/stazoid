using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBall : MonoBehaviour {
	[SerializeField]
	private float SPEED = 20.0f;

	private float damage;
	private bool isMultiplayer;
	private bool isOwn;
	private Animator attackBallAnimator;
	private Vector3 movementTrajectory;
	void Start () {
		isMultiplayer = GameController.Instance != null;
		attackBallAnimator = transform.GetComponent<Animator> ();
		movementTrajectory = Vector3.right * SPEED * Time.deltaTime * (isOwn ? 1 : -1);
	}

	void FixedUpdate () {
		transform.position += movementTrajectory;
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.name == "Own Character") {
			if (isMultiplayer) {
				GameController.Instance.hitOwn (damage);
			} else {
				SinglePlayerController.Instance.hitOwn (damage);
			}
			attackBallAnimator.SetTrigger (AnimationCommand.HIT);
			Destroy (gameObject);
		} else if (other.gameObject.name == "Opponent's Character") {
			if (isMultiplayer) {
				GameController.Instance.hitOpponent (damage);
			} else {
				SinglePlayerController.Instance.hitOpponent (damage);
			}
			attackBallAnimator.SetTrigger (AnimationCommand.HIT);
			Destroy (gameObject);
		}
	}
		
	public void setDamage (float damage) {
		this.damage = damage;
	}

	public void setOwn (bool isOwn) {
		this.isOwn = isOwn;
	}

	public void pauseAttack() {
		attackBallAnimator.enabled = false;
		movementTrajectory = Vector3.zero;
	}

	public void resumeAttack() {
		movementTrajectory = Vector3.right * SPEED * Time.deltaTime * (isOwn ? 1 : -1);
	}
}
