using UnityEngine;
using System;

public class SoundManager : MonoBehaviour {

	public AudioSource[] sources;
	public AudioClip shoot;
	public AudioClip collideProjectile;
	public AudioClip collidePlayer;
	public AudioClip collideDamageField;
	public AudioClip collideHealthField;
	public AudioClip collideObstacle;
	public AudioClip pickPowerUp;
	private AudioSource mains;



	public void PickPowerUp(){
		mains.PlayOneShot (pickPowerUp);
	}

	public void Shoot(){
		mains.PlayOneShot (shoot);
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


	void Start () {
		mains = GetComponent<AudioSource>();
	}
}
