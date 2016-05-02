using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	public enum State { Pause = 0, Play = 1, FadingOut = 2,  FadingIn = 3, Stop = 4, None = 5}

	private AudioSource[] sources;
	public AudioClip basetrack;
	public AudioClip earthquake;
	public Track[] tracks = new Track[2];

	public class Track : MonoBehaviour
	{
		public int id;
		public string name;
		public State State;
		public float fadeSpeed;
		public bool loop;
		public AudioSource output;
		public AudioClip clip;
		public float volume;
		public float nextStateVolume;

		public Track(int newId, string newName, State newState, float newFadeSpeed, bool newLoop, AudioSource newOutput, AudioClip newClip,
					float newVolume, float newNextStateVolume ){
			newOutput.clip=newClip;
			newOutput.loop = newLoop;
			newOutput.volume = newVolume;
			id = newId;
			name = newName;
			State = newState;
			fadeSpeed = newFadeSpeed;
			loop = newLoop;
			output = newOutput;
			clip = newClip;
			nextStateVolume = newNextStateVolume;

		}
	}

	private void init()
	{
		sources = this.GetComponents<AudioSource>();
		tracks [0] = new Track (0, "basetrack", State.Play, 1.0f, true, sources [0], basetrack, 1.0f, 0.0f);
		tracks [1] = new Track (1, "earthquake", State.Play, 1.0f, true, sources [1], earthquake, 0.0f, 1.0f);
	}

	void Start () {
		init ();
	}

	public void FadeToNextClip(int fromTrackID, int toTrackID)
	{
		FadeOut (fromTrackID);
		FadeIn (toTrackID);
	}

	private void FadeOut(int TrackID)
	{
		tracks [TrackID].State = State.FadingOut;
	}
	private void FadeIn(int TrackID)
	{
		tracks [TrackID].State = State.FadingIn;
	}

//
//		
	private void Update()
	{
		foreach (Track x in tracks) 
		{
			switch (x.State) 
			{

			case State.None:
				break;
			
			case State.Pause:
				x.output.Pause();
				x.State = State.None;
				break;


			case State.Play:
				x.outputmain.Play();
				x.State = State.None;
				break;

			case State.FadingIn:
				if (x.output.volume < x.nextStateVolume) {
					x.output.volume += x.fadeSpeed * Time.deltaTime;
				} else {
					x.State = State.None;
				}
				break;

			case State.FadingOut:
				if (x.output.volume > x.nextStateVolume) {
					x.output.volume -= x.fadeSpeed * Time.deltaTime;
				} else {
					if (x.nextStateVolume == 0.0f) {
						x.State = State.Pause;
					} else {
						x.State = State.None;
					}
				}
				break;

			case State.Stop:
				x.output.Stop();
				break;

			}
		}
	}
}