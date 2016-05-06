using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	public enum State { Pause = 0, Play = 1, Prev = 2, Next = 3}

	private AudioSource source;
	public AudioClip bs12;
	public AudioClip bs2;
	public AudioClip bb1;
	public AudioClip bb2;
	public AudioClip bass;
	public AudioClip rumble;
	public AudioClip tension;
	private int[] playlist;
	private int current;
	private int eventFlag;
	public class Track
	{
		public int id;
		public string name;
		public bool loop;
		public AudioClip clip;
		public float volume;

		public Track(int newId, string newName, float newVolume, bool newLoop, AudioClip newClip){
			volume = newVolume;
			id = newId;
			name = newName;
			loop = newLoop;
			clip = newClip;
		}
	}


	private Track[] tracks = new Track[5];
	private Track[] events = new Track[3];

	private void init()
	{
		source = this.GetComponent<AudioSource>();
		tracks [0] = new Track (0, "bs12", 1.0f, true,bs12);
		tracks [1] = new Track (1, "bs2", 1.0f, true,bs2);
		tracks [2] = new Track (2, "bass",  1.0f, true,bass);
		tracks [3] = new Track (3, "bb1", 1.0f, true,bb1);
		tracks [4] = new Track (4, "bb2", 1.0f, true,bb2);

		events [0] = new Track (0, "rumble",  1.0f, true,rumble);
		events [1] = new Track (1, "tension", 1.0f, true,tension);
		events [2] = new Track (1, "tension", 1.0f, true,tension);

	}

	void Start () {
		init ();
		current = 0;
		int[] list = { 2, 3, 4, 1, 0 };
		playlist = list;
		source.clip = tracks[list[current]].clip;
		source.volume = 0.6f;
		source.Play();
		eventFlag = -1;
	}
		
	private void Update()
	{        
		if (!source.isPlaying)
		{
			if (eventFlag == -1) {
				current += 1;
				if (current == playlist.Length)
					current = 0;
				source.clip = tracks [playlist [current]].clip;
				source.Play ();
			} else {
				source.clip = events [eventFlag].clip;
				source.Play ();
			}
		}

	}
	public void lava()
	{
		eventFlag = 0;
	}
	public void earthquake()
	{
		eventFlag = 1;
	}
	public void enemy()
	{
		eventFlag = 2;
	}
	public void reset()
	{
		eventFlag = -1;
	}
}