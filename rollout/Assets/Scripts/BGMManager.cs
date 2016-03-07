using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {
	public enum FadeState { None = 0, FadingOut = 1, FadingIn = 2}
	private float _fadeSpeed = 0.05f;
	private float _inVolume = 1.00f;
	private float _outVolume = 0.00f;
	private bool _loop = false;
	public AudioClip basetrack;
	public AudioClip earthquake;
	public AudioClip lava;
	public AudioClip enemy;
	public AudioSource[] sources;


	public struct Track
	{
		public int id;
		public string name;
		public FadeState fadeState;
		public float fadeSpeed;
		public bool loop;

	}
//	public Track[] tracks = new Track[3];
//
//	private void AddClip(int track)
//	{
//		tracks[track].fadeState = FadeState.FadingIn;
//		tracks [track].fadeSpeed = _fadeSpeed;
//		tracks [track].loop = _loop;
//
//	}
//
//	private void RemoveClip(int track)
//	{
//		tracks[track].fadeState = FadeState.FadingOut;
//	}
//
////	private void FadeToNextClip()
////	{
//////		this._audioSource.loop = this._nextClipLoop;
//////		this._fadeState = FadeState.FadingIn;
//////		this._audioSource.Play();
////	}
////		
//
//	void Start () {
//		sources = this.GetComponents<AudioSource>();
//		
//	}
//		
//	private void Update()
//	{
//		if (this._fadeState == FadeState.FadingOut)
//		{
//			if (this._audioSource.volume > this.outVolume)
//			{
//				this._audioSource.volume -= this.FadeSpeed * Time.deltaTime;
//			}
//			else
//			{
////				this.FadeToNextClip();
//			}
//		}
//		else if (this._fadeState == FadeState.FadingIn)
//		{
//			if (this._audioSource.volume < this.inVolume)
//			{
//				this._audioSource.volume += this.FadeSpeed * Time.deltaTime;
//			}
//			else
//			{
//				this._fadeState = FadeState.None;
//				print ("transition ended");
//			}
//		}
//
//	}
}