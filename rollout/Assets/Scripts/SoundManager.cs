using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	private AudioSource sPlayer1;
	private AudioSource sPlayer2;
	AudioSource sBGM;
	private GameObject player1;
	private GameObject player2;
	public AudioClip shoot;
	public AudioClip collideProjectile;
	public AudioClip collidePlayer;
	public AudioClip collideDamageField;
	public AudioClip collideHealthField;
	public AudioClip collideObstacle;
	public AudioClip pickPowerUp;



	public AudioClip BGM;

	//	public AudioClip collideIntoObstacle;
//	public AudioClip collideIntoPlayer;
//	public AudioClip homingMissle;
//	public AudioClip restoreHealth;
//	public AudioClip damageOverTime;


	void PlaySFX(){
	
	}

	void PlayBGM(){
			
	}

	public void PickPowerUp(GameObject player){
		sPlayer1.clip = pickPowerUp;
		sPlayer2.clip = pickPowerUp;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	
	}

	public void Shoot(GameObject player){
		sPlayer1.clip = shoot;
		sPlayer2.clip = shoot;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	}

	public void CollideProjectile(GameObject player){
		sPlayer1.clip = collideProjectile;
		sPlayer2.clip = collideProjectile;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	}

	public void CollidePlayer(GameObject player){
		sPlayer1.clip = collidePlayer;
		sPlayer2.clip = collidePlayer;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	}

	public void CollideDamageField(GameObject player){
		sPlayer1.clip = collideDamageField;
		sPlayer2.clip = collideDamageField;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	}

	public void CollideHealthField(GameObject player){
		sPlayer1.clip = collideDamageField;
		sPlayer2.clip = collideDamageField;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	}
	public void CollideObstacle(GameObject player){
		sPlayer1.clip = collideObstacle;
		sPlayer2.clip = collideObstacle;
		if (player.name == "player1") {
			sPlayer1.Play ();
			print(player.name + " plays ");

		} else if (player.name == "player2") {
			sPlayer2.Play ();
			print(player.name + " plays ") ;
		}
	}

	public void Collide(GameObject player, string obj){
		if (obj == "obstacle") {
			sPlayer1.clip = collideObstacle;
			sPlayer2.clip = collideObstacle;
		} else if (obj == "player") {
			sPlayer1.clip = collidePlayer;
			sPlayer2.clip = collidePlayer;
		}
		else if (obj == "projectile"){
			sPlayer1.clip = collideProjectile;
			sPlayer2.clip = collideProjectile;
		}
		if (player.name == "player1")sPlayer1.Play ();
		else sPlayer2.Play ();
	}

	void Start () {
		player1 = GameObject.Find("player1");
		player2 = GameObject.Find("player2");
		sPlayer1 = player1.GetComponent<AudioSource> ();
		sPlayer2 = player2.GetComponent<AudioSource> ();
		sBGM = GetComponent<AudioSource>();
		sBGM.clip = BGM;
		sBGM.Play ();
		print("BGM SHOULD PLAY OK !!!!!!!!!!!") ;
//		shoot = Resources.Load<AudioClip> ("SFX/zap");
//		pickupPowerup = Resources.Load<AudioClip> ("SFX/zap");
//		collideIntoPlayer = Resources.Load<AudioClip> ("SFX/zap");
//		collideIntoObstacle = Resources.Load<AudioClip> ("SFX/zap");
//		homingMissle = Resources.Load<AudioClip> ("SFX/zap");
	}
}
