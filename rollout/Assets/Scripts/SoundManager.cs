using UnityEngine;
using System;

public class SoundManager : MonoBehaviour {

	private AudioSource[] sources;
	public AudioClip shoot;
	public AudioClip slowDown;
	public AudioClip collideProjectile;
	public AudioClip collidePlayer;
	public AudioClip collideObstacle;
	public AudioClip pickPowerUp;
	public AudioClip shootBurst;
	public AudioClip heal;
	public AudioClip stun;
	private AudioSource mains;
	private GameObject player;

	public float GetPosition(){
		float objectPosX =  player.transform.position.x;
		return objectPosX;
	}
	public void SetSteroPan(AudioSource source){
		float gridX = GetPosition ();
		float posX = (gridX)/18.0f;
		source.panStereo = posX;
		float vol = (float) System.Math.Abs (posX) + 0.25f;
		if (vol > 1.0f) vol = 1.0f;
		else if (vol < 0.25f) vol = 0.25f;
		source.volume = vol;
	}

	public void PickPowerUp(){
		mains.PlayOneShot (pickPowerUp);
	}

	public void Shoot(){
		SetSteroPan (mains);
		mains.PlayOneShot (shoot);
	}
	public void ShootBurst(){
		SetSteroPan (mains);
		mains.PlayOneShot (shootBurst);
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
		
	public void Heal(){
		SetSteroPan (mains);
		mains.PlayOneShot (heal);
	}
		
	public void CollideObstacle(Vector3 impact){
		float volume = (impact.magnitude / 8f);
		mains.volume = volume;
		print (volume);
		SetSteroPan (mains);
		mains.PlayOneShot (collideObstacle);

	}
	void Start () {
		mains = GetComponent<AudioSource>();
		player = this.transform.parent.gameObject;
	}
}
