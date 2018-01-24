using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPlayer : MonoBehaviour {

    public static SettingPlayer instance;

    public bool isSolo = false;
	public bool startMusic;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

	//this will happen only once! ever!
	void Start()
	{
		Invoke( "StartPlayingMusicTheme",.5f);

	}

	void StartPlayingMusicTheme()
	{
		if (!startMusic) 
		{
			ExampleNetworkManager.singleton.transform.GetChild(0).GetComponent<AudioSource> ().enabled = true;
			startMusic = true;
		}
	}
}
