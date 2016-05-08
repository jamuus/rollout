using UnityEngine;
using System.Collections;

public class RegionHeal : Region
{
	private GameObject music;


	public int magnitude = 0;

	// Use this for initialization
	public override void ApplyEffect(PlayerControl player)
	{
		if (Time.time > lastEffectCall + Rate)
		{
			player.GetComponent<UniversalHealth>().healPlayer(magnitude);
			lastEffectCall = Time.time;
			music = player.gameObject.transform.Find("sound").gameObject;
			SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
			manager.Stun ();
		}
	}

	public override void OnPlayerEnter(PlayerControl player)
	{
		Debug.LogFormat("PLAYER {0} ENTER REGION", player.name);
	}

	public override void OnPlayerLeave(PlayerControl player)
	{
		Debug.LogFormat("PLAYER {0} LEFT REGION", player.name);
	}

}
