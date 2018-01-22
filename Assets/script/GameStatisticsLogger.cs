using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatisticsLogger : MonoBehaviour {

	public static GameStatisticsLogger instance;
	public List<CityNeighborhood> allCitiesNeighborhood;


	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
