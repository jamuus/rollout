using UnityEngine;

public class DamageSlowRegion : Region
{
    public int Damage;
    public float DragMultiplier;
	private GameObject music;
    private float outOfRegionClamp;
    private const float inRegionClamp = 0.05f;

    public override void ApplyEffect(PlayerControl player)
    {
        if (Time.time > lastEffectCall + Rate)
        {
            player.GetComponent<UniversalHealth>().damagePlayer(Damage);
            lastEffectCall = Time.time;
			Debug.LogFormat ("damage here");
			if (!music || music.transform.parent.gameObject != player.gameObject) music = player.gameObject.transform.Find("sound").gameObject;
			SoundManager manager = (SoundManager) music.GetComponent(typeof(SoundManager));
			manager.Stun ();
        }
    }

    public override void OnPlayerEnter(PlayerControl player)
    {
        Debug.LogFormat("PLAYER {0} ENTER REGION", player.name);

		if (player.sphero != null) {
			outOfRegionClamp = player.sphero.PhysicalForceClamp;
			player.sphero.PhysicalForceClamp = inRegionClamp;
		}
        #if SOFTWARE_MODE
        player.GetComponent<Rigidbody>().drag *= DragMultiplier;
        #endif
    }

    public override void OnPlayerLeave(PlayerControl player)
    {

		if (player.sphero != null)
			player.sphero.PhysicalForceClamp = outOfRegionClamp;

        Debug.LogFormat("PLAYER {0} LEFT REGION", player.name);

        //player.sphero.PhysicalForceClamp = outOfRegionClamp;

        #if SOFTWARE_MODE
        player.GetComponent<Rigidbody>().drag /= DragMultiplier;
        #endif
    }

}