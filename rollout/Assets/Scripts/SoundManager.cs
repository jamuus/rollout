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

	public float GetPosition(GameObject player){
		float objectPos =  player.transform.position.x;
		return objectPos;
	}

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
		mains.volume= (0.4f);
		mains.PlayOneShot (stun);
	}
	public void CollideProjectile(Vector3 impact){
		mains.PlayOneShot (collideProjectile);
	}

	public void CollidePlayer(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		mains.PlayOneShot (collidePlayer);
	}

	public void CollideDamageField(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		mains.PlayOneShot (collideDamageField);
	}

	public void CollideHealthField(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		mains.PlayOneShot (collideHealthField);

	}
	public void CollideObstacle(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		mains.PlayOneShot (collideObstacle);

	}
	public void GrenadeShoot(){
		mains.PlayOneShot (grenadeShoot);
	}

	void Start () {
		mains = GetComponent<AudioSource>();
	}
}
