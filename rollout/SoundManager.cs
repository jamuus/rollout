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
	private AudioSource _shoot;
	private AudioSource _collideProjectile;
	private AudioSource _collidePlayer;
	private AudioSource _collideDamageField;
	private AudioSource _collideHealthField;
	private AudioSource _collideObstacle;
	private AudioSource _pickPowerUp;


	public void PickPowerUp(){
		_pickPowerUp.Play ();
	}

	public void Shoot(){
		_shoot.Play();
	}

	public void CollideProjectile(){
		_collideProjectile.Play();

	}

	public void CollidePlayer(){
		_collidePlayer.Play();
	}

	public void CollideDamageField(){
		_collideDamageField.Play();
	}

	public void CollideHealthField(){
		_collideHealthField.Play();
	}
	public void CollideObstacle(){
		_collideObstacle.Play();
	}


	void Start () {
		sources = GetComponents<AudioSource>();
		sources[0].clip = collideObstacle;
		sources[1].clip = collidePlayer;
		sources[2].clip = collideDamageField;
		sources[3].clip = collideProjectile;
		sources[4].clip = collideHealthField;
		sources[5].clip = shoot;
		sources[6].clip = pickPowerUp;
		_collideObstacle = sources[0];
		_collidePlayer = sources[1];
		_collideDamageField = sources[2];
		_collideProjectile = sources[3];
		_collideHealthField = sources[4];
		_shoot = sources[5];
		_pickPowerUp = sources[6];

	}
}
