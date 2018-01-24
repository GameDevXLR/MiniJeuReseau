using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour {

	bool firstMusicActivation = true;
	bool firstSoundActivation = true;
	bool musicOn;
	bool soundsOn;

	public AudioMixer audioMix;

	public Sprite soundsOffImg;
	public Sprite musicOffImg;
	public Sprite soundsOnImg;
	public Sprite musicOnImg;

	public Image soundBtnImg;
	public Image musicBtnImg;
	public AudioClip ClicSnd;
	public AudioClip clic2Snd;


	// Use this for initialization
	void Start () 
	{
		if (PlayerPrefs.GetInt ("MUSIC", 1) == 1) 
		{
			musicOn = true;
		}
		if (PlayerPrefs.GetInt ("SOUNDS", 1) == 1) 
		{
			soundsOn = true;
		}
		MusicActivator ();
		SoundsActivator ();
	}


	public void MusicActivator()
	{
		if (musicOn) 
		{
			audioMix.SetFloat("Music",1);
			if (!firstMusicActivation) 
			{
				GetComponent<AudioSource> ().PlayOneShot (ClicSnd);
			}
			PlayerPrefs.SetInt ("MUSIC", 1);
			musicBtnImg.sprite = musicOnImg;
		} else 
		{
			audioMix.SetFloat("Music",-80);

			if (!firstMusicActivation) {
				
				GetComponent<AudioSource> ().PlayOneShot (clic2Snd);
			}
			musicBtnImg.sprite = musicOffImg;

			PlayerPrefs.SetInt ("MUSIC", 0);

		}
		musicOn = !musicOn;
		firstMusicActivation = false;
		
	}
	public void SoundsActivator()
	{
		if (soundsOn) 
		{
			if (!firstSoundActivation) {
				GetComponent<AudioSource> ().PlayOneShot (clic2Snd);
			}
			soundBtnImg.sprite = soundsOffImg;
			audioMix.SetFloat ("AudioSFX", -80);
			PlayerPrefs.SetInt ("SOUNDS", 0);
		} else 
		{
			
			audioMix.SetFloat ("AudioSFX", 0);
			if (!firstSoundActivation) {
				
				GetComponent<AudioSource> ().PlayOneShot (ClicSnd);
			}
			soundBtnImg.sprite = soundsOnImg;
			PlayerPrefs.SetInt ("SOUNDS", 1);

		}
		soundsOn = !soundsOn;
		firstSoundActivation = false;
	}

	public void ShowHideOptionPanel()
	{
		GetComponent<Canvas> ().enabled = !GetComponent<Canvas> ().enabled;
		if (GetComponent<Canvas> ().enabled) {
			GetComponent<AudioSource> ().PlayOneShot (ClicSnd);
		} else {
			GetComponent<AudioSource> ().PlayOneShot (clic2Snd);

		}
	}
}
