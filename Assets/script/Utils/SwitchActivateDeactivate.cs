using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchActivateDeactivate : MonoBehaviour {

    public GameObject obj1;
	public AudioClip helpOpenSnd;

//    public GameObject obj2;

    public void action()
    {
		GetComponent<AudioSource> ().PlayOneShot (helpOpenSnd);
        obj1.SetActive(!obj1.activeInHierarchy);
//        obj2.SetActive(!obj2.activeInHierarchy);
    }
}
