using UnityEngine;
using System.Collections;

public class GameSFX : MonoBehaviour {
	private const string SFX_PATH = "SFX/";

	public static AudioClip TAP_NUMBER = Resources.Load (SFX_PATH + "TapNumber") as AudioClip;
	public static AudioClip TAP_BACKSPACE = Resources.Load (SFX_PATH + "TapBackspace") as AudioClip;
	public static AudioClip TAP_DIFFICULTY = Resources.Load (SFX_PATH + "TapDifficulty") as AudioClip;
	public static AudioClip ANSWER_CORRECT = Resources.Load (SFX_PATH + "AnswerCorrect") as AudioClip;
	public static AudioClip ANSWER_FALSE = Resources.Load (SFX_PATH + "AnswerFalse") as AudioClip;
	public static AudioClip WIN = Resources.Load (SFX_PATH + "GameWin") as AudioClip;
	public static AudioClip LOSE = Resources.Load (SFX_PATH + "GameLose") as AudioClip;
	public static AudioClip DRAW = Resources.Load (SFX_PATH + "GameDraw") as AudioClip;
	public static AudioClip SPECIAL_FULL = Resources.Load (SFX_PATH + "LockTarget") as AudioClip;
	public static AudioClip SPECIAL_LAUNCH = Resources.Load (SFX_PATH + "LaunchSpecial") as AudioClip;
}

