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
		mains.Play ();
	}

	public void Shoot(){
		mains.Play();
	}

	public void CollideProjectile(){
		mains.Play();

	}

	public void CollidePlayer(){
		mains.Play();
	}

	public void CollideDamageField(){
		mains.Play();
	}

	public void CollideHealthField(){
		mains.Play();
	}
	public void CollideObstacle(){
		mains.Play();
	}


	void Start () {
		mains = GetComponent<AudioSource>();
	}
}
