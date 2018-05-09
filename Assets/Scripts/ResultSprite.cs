using UnityEngine;
using System.Collections;

public class ResultSprite : MonoBehaviour
{
	private const string SPRITE_PATH = "Text in Game/";

	public static Sprite WIN = Resources.Load<Sprite>(SPRITE_PATH + "Win");
	public static Sprite DRAW = Resources.Load<Sprite>(SPRITE_PATH + "Draw");
	public static Sprite LOSE = Resources.Load<Sprite>(SPRITE_PATH + "Lose");
	public static Sprite KO = Resources.Load<Sprite>(SPRITE_PATH + "KO");
	public static Sprite DOUBLE_KO = Resources.Load<Sprite>(SPRITE_PATH + "Double KO");
	public static Sprite GREAT = Resources.Load<Sprite>(SPRITE_PATH + "Great");
	public static Sprite PERFECT = Resources.Load<Sprite>(SPRITE_PATH + "Perfect");
}

