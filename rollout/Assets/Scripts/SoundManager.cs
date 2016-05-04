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



	public void PickPowerUp(){
		mains.PlayOneShot (pickPowerUp);
	}

	public void Shoot(){
		mains.PlayOneShot (shoot);
	}
	public void SlowDown(){
		mains.PlayOneShot (slowDown);
	}
	public void Stun(){
		mains.PlayOneShot (stun);
	}
	public void CollideProjectile(){
		mains.PlayOneShot (collideProjectile);
	}

	public void CollidePlayer(){
		mains.PlayOneShot (collidePlayer);
	}

	public void CollideDamageField(){
		mains.PlayOneShot (collideDamageField);
	}

	public void CollideHealthField(){
		mains.PlayOneShot (collideHealthField);
	}
	public void CollideObstacle(){
		mains.PlayOneShot (collideObstacle);
	}
	public void GrenadeShoot(){
		mains.PlayOneShot (grenadeShoot);
	}

	void Start () {
		mains = GetComponent<AudioSource>();
	}
}
