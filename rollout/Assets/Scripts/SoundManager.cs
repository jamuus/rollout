using UnityEngine;
using System;

public class SoundManager : MonoBehaviour {

	private AudioSource[] sources;
	public AudioClip shoot;
	public AudioClip slowDown;
	public AudioClip collideProjectile;
	public AudioClip collidePlayer;
	public AudioClip collideDamageField;
	public AudioClip collideHealthField;
	public AudioClip collideObstacle;
	public AudioClip pickPowerUp;
	public AudioClip grenadeShoot;
	public AudioClip stun;
	private AudioSource mains;
	private GameObject player;

	public float GetPosition(){
		float objectPosX =  player.transform.position.x;
		return objectPosX;
	}
	public void SetSteroPan(AudioSource source){
		float gridX = GetPosition ();
		float posX = (gridX)/16.5f;
		source.panStereo = posX;
	}

	public void PickPowerUp(){
		mains.PlayOneShot (pickPowerUp);
	}

	public void Shoot(){
		SetSteroPan (mains);
		mains.PlayOneShot (shoot);
	}
	public void SlowDown(){
		SetSteroPan (mains);
		mains.PlayOneShot (slowDown);
	}
	public void Stun(){
		mains.volume= (0.4f);
		SetSteroPan (mains);
		mains.PlayOneShot (stun);
	}
	public void CollideProjectile(Vector3 impact){
		SetSteroPan (mains);
		mains.PlayOneShot (collideProjectile);
	}

	public void CollidePlayer(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		SetSteroPan (mains);
		mains.PlayOneShot (collidePlayer);
	}

	public void CollideDamageField(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		SetSteroPan (mains);
		mains.PlayOneShot (collideDamageField);
	}

	public void CollideHealthField(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		SetSteroPan (mains);
		mains.PlayOneShot (collideHealthField);

	}
	public void CollideObstacle(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		SetSteroPan (mains);
		mains.PlayOneShot (collideObstacle);

	}
	public void GrenadeShoot(){
		SetSteroPan (mains);
		mains.PlayOneShot (grenadeShoot);
	}

	void Start () {
		mains = GetComponent<AudioSource>();
		player = this.transform.parent.gameObject;
	}
}
