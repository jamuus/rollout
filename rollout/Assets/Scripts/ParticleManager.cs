using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour
{
    public ParticleSystem boost;
    public ParticleSystem damage;
    public ParticleSystem stun;
    public ParticleSystem slow;
    public ParticleSystem regen;


    public void PlayParticle(int ID)
    {
        switch(ID)
        {
            case 0:
                boost.Play();
                break;

            case 1:
                damage.Play();
                break;

            case 2:
                stun.Play();
                break;

            case 3:
                slow.Play();
                break;

            case 4:
                regen.Play();
                break;
        }
    }

    public void StopParticle(int ID)
    {
        switch (ID)
        {
            case 0:
                boost.Stop();
                break;

            case 1:
                damage.Stop();
                break;

            case 2:
                stun.Stop();
                break;

            case 3:
                slow.Stop();
                break;

            case 4:
                regen.Stop();
                break;
        }
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
