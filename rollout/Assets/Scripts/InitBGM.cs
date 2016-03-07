using UnityEngine;
using System.Collections;

public class InitBGM : MonoBehaviour {
	public enum FadeState { None = 0, FadingOut = 1, FadingIn = 2}
	private AudioSource[] sources;
	public AudioClip basetrack;
	public AudioClip earthquake;

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
	public Track[] tracks;
	void Start () {
		tracks = new Track[2];
		sources = this.GetComponents<AudioSource>();
//		sources [0].clip = basetrack;
//		sources [1].clip = earthquake;
		tracks [0] = new Track (0, "basetrack", FadeState.None, 0.05f, false, sources [0],basetrack);
		tracks [1] = new Track (1, "earthquake", FadeState.None, 0.05f, false, sources [1],earthquake);
		tracks [0].output.Play ();
//		print(tracks[0].output.name);
	}
}