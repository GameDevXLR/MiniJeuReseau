using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GingerPowerAI : MonoBehaviour 
{


	public CityV2 lastCityTaken;


	// Use this for initialization
	void Start () 
	{
		
	}

	void OnEnable()
	{
		Debug.Log ("initialisation de l'incroyable GingerPower AI ! ! !");
		NetworkGameManager.instance.ActivateTheAmazingGingerAI ();
		GameManager.instance.player2Name.text = "[AI]Ginger Power";
	}
	
	 public void PlayOneTurn()
	{

		//particular case : the first turn. Let's make it kindda random. Better take a city.
		if (lastCityTaken == null) 
		{
			for (int i = Random.Range(0,GameManager.instance.cities.Length-2 ); i < GameManager.instance.cities.Length; i++) 
			{
				if (!GameManager.instance.cities [i].isTaken) 
				{
					GameManager.instance.cities [i].CaptureThisCity ();
					lastCityTaken = GameManager.instance.cities [i];
					return;
				}
				
			}
		}

		for (int i = 0; i < GameManager.instance.citiesPlayer1.Count; i++) 
		{
			//			CityNeighborhood CN = GameManager.instance.citiesPlayer2 [i].neighboors;
			GameManager.instance.citiesPlayer1 [i].neighboors.CheckTheNeighboorhood ();
			Debug.Log ("checking city team1: " + i);

		}

		for (int i = 0; i < GameManager.instance.citiesPlayer2.Count; i++) 
		{
//			CityNeighborhood CN = GameManager.instance.citiesPlayer2 [i].neighboors;
			GameManager.instance.citiesPlayer2 [i].neighboors.CheckTheNeighboorhood ();
			Debug.Log ("checking city " + i);

			//make a defensive move if there is danger...
		}
		for (int i = 0; i < GameManager.instance.citiesPlayer2.Count; i++) 
		{
			if (GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForDefenseMove) 
			{
				if (GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForFriendlyCityLinkMove) 
				{
					Debug.Log("making a defensive move: road to friendly city "+i);
					GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForFriendlyCityLinkMove.MakeRoad();
					return;

				}
				GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForDefenseMove.MakeBarrage ();
				Debug.Log ("Barrage construit sur la route menant a l'ennemi " + i);
				return;
			}
		}
		for (int i = 0; i < GameManager.instance.citiesPlayer2.Count; i++) 
		{
			if (GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForAttackMove) 
			{
				GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForAttackMove.MakeRoad();
				Debug.Log ("Route construite, menant a une prise en " + i);
				return;
			}
		}
		if (GameManager.instance.citiesPlayer2.Count + GameManager.instance.citiesPlayer1.Count < GameManager.instance.cities.Length) 
		{
			FindAndCaptureACity ();
			return;
		}
		for (int i = 0; i < GameManager.instance.citiesPlayer2.Count; i++) 
		{
			if (GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForFriendlyCityLinkMove) 
			{
				GameManager.instance.citiesPlayer2 [i].neighboors.currentTargetForFriendlyCityLinkMove.MakeRoad();
				Debug.Log ("Route construite, menant a une ville allié en " + i);
				return;
			}
		}
		for (int i = 0; i < GameManager.instance.lines.Length; i++) 
		{
			if (!GameManager.instance.lines [i].isModified) 
			{
				GameManager.instance.lines [i].MakeBarrage ();
				return;
			}
			
		}

		Debug.Log ("nothing to do!");


		//make an offensive move if there is absolutly no danger, and no more cities to be taken!
	}

	public void FindAndCaptureACity()
	{
		Debug.Log ("trying to capture a free city");
		int i = Random.Range (0, GameManager.instance.cities.Length);
		if (!GameManager.instance.cities [i].isTaken) 
		{
			GameManager.instance.cities [i].CaptureThisCity ();
			lastCityTaken = GameManager.instance.cities [i];

			Debug.Log ("capturing city nbr: " + i);
			return;
		}
		FindAndCaptureACity ();
	}
}
