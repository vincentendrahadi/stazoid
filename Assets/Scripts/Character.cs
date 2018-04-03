using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : Photon.PunBehaviour, IPunObservable {
	
	#region IPunObservable implementation
	void IPunObservable.OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		
	}
	#endregion

	protected int maxHp;
	protected float comboTimer;
	protected int[] damage = new int[3];
	protected float[] specialBarIncrease = new float[3];
	protected int easyDamage;
	protected int mediumDamage;
	protected int hardDamage;

	public abstract KeyValuePair<string, int> generateProblem (int difficulty);

	public int getMaxHp () {
		return maxHp;
	}
		
	public float getComboTimer () {
		return comboTimer;
	}

	public int[] getDamage () {
		return damage;
	}

	public float[] getSpecialBarIncrease () {
		return specialBarIncrease;
	}
		
	abstract public void useSpecial();

		
}
