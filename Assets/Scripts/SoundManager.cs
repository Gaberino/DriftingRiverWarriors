using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public float percentageDrownedMusicShift = 75f;
	public AudioClip regularMusic;
	public AudioClip nearDeathMusic;

	private AudioSource[] sources;

	// Use this for initialization
	void Start () {
		sources = new AudioSource[2];

		//instantiate sources onto the main camera
		GameObject AudioSourceObject = new GameObject("AudioSourceObject");

		sources [0] = AudioSourceObject.AddComponent<AudioSource> ();
		sources [1] = AudioSourceObject.AddComponent<AudioSource> ();
		sources[0].volume = .7f;
		sources[1].volume = .7f;
		sources [0].playOnAwake = false;
		sources [1].playOnAwake = false;

		sources [0].clip = regularMusic;
		sources [1].clip = nearDeathMusic;
	}
	
	// Update is called once per frame
	void Update () {
		if (LakeGameManager.instance.gameState == 1) {
			if (!sources [0].isPlaying || !sources [1].isPlaying) { //if neither is playing we just started
				sources [0].Play ();
				sources [1].Play ();
				sources [1].mute = true;
			} else { //check for swapping between tracks
				if (Player.instance.wetness / Player.instance.myBrain.timeToDrown < percentageDrownedMusicShift / 100) {//normal music
					if (sources [0].mute) {
						sources [0].mute = false;
						sources [1].mute = true;
					}
				} else {
					if (sources [1].mute) {
						sources [0].mute = true;
						sources [1].mute = false;
					}
				}
			}
		} else {
			//stop the music
			for (int i = 0; i < sources.Length; i++) {
				if (sources [i].isPlaying)
					sources [i].Stop ();
			}
		}
	}
}
