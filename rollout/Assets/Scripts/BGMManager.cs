using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	public enum FadeState { None = 0, FadingOut = 1, FadingIn = 2, NextTrack = 3}

	private AudioSource[] sources;
	public AudioClip basetrack;
	public AudioClip earthquake;
	public Track[] tracks = new Track[2];

	public struct Track
	{
		public int id;
		public string name;
		public FadeState fadeState;
		public float fadeSpeed;
		public bool loop;
		public AudioSource output;
		public AudioClip clip;

		public Track(int newId, string newName, FadeState newFadeState, float newFadeSpeed, bool newLoop, AudioSource newOutput, AudioClip newClip){
			newOutput.clip=newClip;
			newOutput.loop = newLoop;
			id = newId;
			name = newName;
			fadeState = newFadeState;
			fadeSpeed = newFadeSpeed;
			loop = newLoop;
			output = newOutput;
			clip = newClip;
		}
	}

	private void init()
	{
		sources = this.GetComponents<AudioSource>();
		sources [0].clip = basetrack;
		sources [1].clip = earthquake;
		tracks [0] = new Track (0, "basetrack", FadeState.None, 0.05f, false, sources [0],basetrack);
		tracks [1] = new Track (1, "earthquake", FadeState.None, 0.05f, false, sources [1],earthquake);
	}

	void Start () {
		init ();
		tracks [0].output.Play ();
	}

	private void RemoveClip(int track)
	{
		tracks[track].fadeState = FadeState.FadingOut;
	}

	private void FadeToNextClip()
	{
		this._audioSource.loop = this._nextClipLoop;
		this._fadeState = FadeState.FadingIn;
		this._audioSource.Play();
	}
		

	private void RemoveClip(int track)
	{
		tracks[track].fadeState = FadeState.FadingOut;
	}

		
	private void Update()
	{
		if (this._fadeState == FadeState.FadingOut)
		{
			if (this._audioSource.volume > this.outVolume)
			{
				this._audioSource.volume -= this.FadeSpeed * Time.deltaTime;
			}
			else
			{
//				this.FadeToNextClip();
			}
		}
		else if (this._fadeState == FadeState.FadingIn)
		{
			if (this._audioSource.volume < this.inVolume)
			{
				this._audioSource.volume += this.FadeSpeed * Time.deltaTime;
			}
			else
			{
				this._fadeState = FadeState.None;
				print ("transition ended");
			}
		}

	}
}