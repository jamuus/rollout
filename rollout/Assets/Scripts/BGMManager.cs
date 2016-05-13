using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {

	private AudioSource source;
	private AudioClip bs121;
	private AudioClip bs122;
	private AudioClip bs123;
	private AudioClip bs124;

	private AudioClip bs21;
	private AudioClip bs22;
	private AudioClip bs23;
	private AudioClip bs24;

	private AudioClip bb11;
	private AudioClip bb12;
	private AudioClip bb13;
	private AudioClip bb14;

	private AudioClip bb21;
	private AudioClip bb22;
	private AudioClip bb23;
	private AudioClip bb24;

	private AudioClip bass1;
	private AudioClip bass2;
	private AudioClip bass3;
	private AudioClip bass4;

	private AudioClip rumble1;
	private AudioClip rumble2;
	private AudioClip rumble3;
	private AudioClip rumble4;

	private AudioClip tension1;
	private AudioClip tension2;
	private AudioClip tension3;
	private AudioClip tension4;

	private int[] playlist;
	private int current;
	private int eventFlag;
	private int eventTrack;

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


	private Track[] tracks = new Track[20];
	private Track[] rumbles = new Track[4];
	private Track[] tensions = new Track[4];

	private void  locate(){
		bs121 = (AudioClip) Resources.Load("SFX/bgm/bs121");
		bs122 = (AudioClip) Resources.Load("SFX/bgm/bs121");
		bs123 = (AudioClip) Resources.Load("SFX/bgm/bs121");
		bs124 = (AudioClip) Resources.Load("SFX/bgm/bs121");

		bs21 = (AudioClip) Resources.Load("SFX/bgm/bs21");
		bs22 = (AudioClip) Resources.Load("SFX/bgm/bs22");
		bs23 = (AudioClip) Resources.Load("SFX/bgm/bs23");
		bs24 = (AudioClip) Resources.Load("SFX/bgm/bs24");

		bb11 = (AudioClip) Resources.Load("SFX/bgm/bb11");
		bb12 = (AudioClip) Resources.Load("SFX/bgm/bb12");
		bb13 = (AudioClip) Resources.Load("SFX/bgm/bb13");
		bb14 = (AudioClip) Resources.Load("SFX/bgm/bb14");

		bb21 = (AudioClip) Resources.Load("SFX/bgm/bb21");
		bb22 = (AudioClip) Resources.Load("SFX/bgm/bb22");
		bb23 = (AudioClip) Resources.Load("SFX/bgm/bb23");
		bb24 = (AudioClip) Resources.Load("SFX/bgm/bb24");

		bass1 = (AudioClip) Resources.Load("SFX/bgm/bass1");
		bass2 = (AudioClip) Resources.Load("SFX/bgm/bass2");
		bass3 = (AudioClip) Resources.Load("SFX/bgm/bass3");
		bass4 = (AudioClip) Resources.Load("SFX/bgm/bass4");

		rumble1 = (AudioClip) Resources.Load("SFX/bgm/rumble1");
		rumble2 = (AudioClip) Resources.Load("SFX/bgm/rumble2");
		rumble3 = (AudioClip) Resources.Load("SFX/bgm/rumble3");
		rumble4 = (AudioClip) Resources.Load("SFX/bgm/rumble4");

		tension1 = (AudioClip) Resources.Load("SFX/bgm/tension1");
		tension2 = (AudioClip) Resources.Load("SFX/bgm/tension2");
		tension3 = (AudioClip) Resources.Load("SFX/bgm/tension3");
		tension4 = (AudioClip) Resources.Load("SFX/bgm/tension4");
	}
	private void init()
	{
		source = this.GetComponent<AudioSource>();
		tracks [0] = new Track (0, "bs121", 1.0f, true,bs121);
		tracks [1] = new Track (1, "bs122", 1.0f, true,bs122);
		tracks [2] = new Track (2, "bs123", 1.0f, true,bs123);
		tracks [3] = new Track (3, "bs124", 1.0f, true,bs124);

		tracks [4] = new Track (4, "bs21", 1.0f, true,bs21);
		tracks [5] = new Track (5, "bs22", 1.0f, true,bs22);
		tracks [6] = new Track (6, "bs23", 1.0f, true,bs23);
		tracks [7] = new Track (7, "bs24", 1.0f, true,bs24);

		tracks [8] = new Track (8, "bass1",  1.0f, true,bass1);
		tracks [9] = new Track (9, "bass2",  1.0f, true,bass2);
		tracks [10] = new Track (10, "bass3",  1.0f, true,bass3);
		tracks [11] = new Track (11, "bass4",  1.0f, true,bass4);

		tracks [12] = new Track (12, "bb11", 1.0f, true,bb11);
		tracks [13] = new Track (13, "bb12", 1.0f, true,bb12);
		tracks [14] = new Track (14, "bb13", 1.0f, true,bb13);
		tracks [15] = new Track (15, "bb14", 1.0f, true,bb14);

		tracks [16] = new Track (16, "bb21", 1.0f, true,bb21);
		tracks [17] = new Track (17, "bb22", 1.0f, true,bb22);
		tracks [18] = new Track (18, "bb23", 1.0f, true,bb23);
		tracks [19] = new Track (19, "bb24", 1.0f, true,bb24);

		rumbles [0] = new Track (0, "rumble",  1.0f, true,rumble1);
		rumbles [1] = new Track (1, "rumble", 1.0f, true,rumble2);
		rumbles [2] = new Track (2, "rumble", 1.0f, true,rumble3);
		rumbles [3] = new Track (3, "rumble", 1.0f, true,rumble4);

		tensions [0] = new Track (0, "tension1",  1.0f, true,tension1);
		tensions [1] = new Track (1, "tension2", 1.0f, true,tension2);
		tensions [2] = new Track (2, "tension3", 1.0f, true,tension3);
		tensions [3] = new Track (3, "tension4", 1.0f, true,tension4);

	}

	void Start () {
		locate ();
		init ();
		current = 0;
		int[] list = { 8,9,10,11, 12,13,14,15, 16,17,18,19, 4,5,6,7, 0,1,2,3 };
		playlist = list;
		source.clip = tracks[list[current]].clip;

		source.volume = 0.3f;
		eventFlag = -1;
		eventTrack = 0;

		source.Play ();

	}
		
	private void Update()
	{        
		if (!source.isPlaying)
		{
			if (eventFlag == -1) {
				eventTrack = 0;
				current += 1;
				if (current == playlist.Length)
					current = 0;
				source.clip = tracks [playlist [current]].clip;
				source.Play ();
			} else{
				switch (eventFlag)
				{
				case 0:
					source.clip = tensions [eventTrack].clip;
					source.Play ();
					break;
				case 1:
					source.clip = rumbles [eventTrack].clip;
					source.Play ();
					break;
				}
				eventTrack += 1;
				if (eventTrack == 4) eventTrack = 0;
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
		eventFlag = 0;
	}
	public void reset()
	{
		eventFlag = -1;
	}
}