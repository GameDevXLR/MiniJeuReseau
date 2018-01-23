using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacBoom : MonoBehaviour {

	public float timeBeforeExplo;
	float time;


	// Use this for initialization
	void Start () 
	{
//		time = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		time += Time.deltaTime;
		if ( time > timeBeforeExplo) 
		{
			Destroy (gameObject);
		}
		
	}
}
