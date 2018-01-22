using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CityNeighborhood : MonoBehaviour 
{
	CityV2 motherCity;
	int tries;
	public LineController currentTargetForDefenseMove;
	public LineController currentTargetForAttackMove;
	public LineController currentTargetForFriendlyCityLinkMove;

	public LineController[] allConnections; // a compléter manuellement pour le moment. Utiliser un sphere cast ou autre plus tard.
    public LineController[] listLinkGameObj = new LineController[8];
    public Dictionary<CityV2,LineController> connectedCities;
	public Dictionary<CityV2,int> dangereousNeighB;
	public bool isInDanger;
	int dangereousRoads;
    public int nbrLink;

	// Use this for initialization
	void Awake () 
	{
        motherCity = GetComponent<CityV2>();
        dangereousNeighB = new Dictionary<CityV2, int>();
        connectedCities = new Dictionary<CityV2, LineController>();
        GameStatisticsLogger.instance.allCitiesNeighborhood.Add(this);
        if (!GameManager.instance.isgenerate)
        {
            procedureStart();
        }
	}

    public void procedureStart()
    {
        foreach (var lineC in allConnections)
        {
            addCityInConnectedCities(lineC);
        }
    }

    public void addCityInConnectedCities(LineController lineC)
    {
        if (lineC.cities[0] != motherCity)
        {
            connectedCities.Add(lineC.cities[0], lineC);
        }
        if (lineC.cities[1] != motherCity)
        {

            connectedCities.Add(lineC.cities[1], lineC);
        }
    }

    public void addCityLink(LineController cityLink, int indexLink)
    {
        listLinkGameObj[indexLink] = cityLink;

        addCityInConnectedCities(cityLink);
        nbrLink++;
    }

    public void addCityLinkInverse(LineController city, int indexLink)
    {
        if (indexLink > 3)
        {
            indexLink -= 4;
        }
        else
        {
            indexLink += 4;
        }
        listLinkGameObj[indexLink] = city;
        nbrLink++;
    }

    public int TellMeMyDefStrenght()
	{
		int j = 1;
		if (!motherCity.isP1) {
			foreach (var C in connectedCities) {
				if (C.Value.isModified && !C.Value.isBarrage && !C.Key.isP1) {
					j++;
				}
			
			}
			return j;
		} else {
			foreach (var C in connectedCities) {
				if (C.Value.isModified && !C.Value.isBarrage && C.Key.isP1) {
					j++;
				}

			}
			return j;
		}
	}

	public int TellMeAttackStr()
	{
		int k = 0;
		if (!motherCity.isP1) {
			foreach (var C in connectedCities) {
				if (C.Value.isModified && !C.Value.isBarrage && C.Key.isP1) {
					k++;
				}
			}

			return k;
		} else 
		{
			foreach (var C in connectedCities) {
				if (C.Value.isModified && !C.Value.isBarrage && !C.Key.isP1) {
					k++;
				}
			}

			return k;
		}
	}

	public void CheckTheNeighboorhood()
	{
//		Debug.Log ("checking neightb");

		motherCity.defenseStr = TellMeMyDefStrenght ();
		motherCity.attackStr = TellMeAttackStr ();

		currentTargetForDefenseMove = null;
		currentTargetForAttackMove = null;
		currentTargetForFriendlyCityLinkMove = null;

		tries = 0;
		dangereousRoads = 0;
		dangereousNeighB.Clear ();

		foreach (var city in connectedCities) 
		{
			if (!city.Value.isBarrage) {
				if (city.Key.isTaken && city.Key.isP1) {
					dangereousNeighB.Add (city.Key, city.Key.defenseStr);
//					Debug.Log ("adding  dangereous neighB");
					if (city.Value.isModified) 
					{
						dangereousRoads++;
//						Debug.Log ("a road exist between this city and the dangereous neighB");
					} else 
					{
						//ya une route, non prise, et elle mène a une ville qui serait prise si jmet une route:
						if (city.Key.defenseStr - city.Key.attackStr == 0) {
							if (currentTargetForAttackMove == null) {
								currentTargetForAttackMove = city.Value;
							}
						}
					}
				}
				//si c'est une ville amie et que la route est pas construite
				if (city.Key.isTaken && !city.Key.isP1 && !city.Value.isModified) 
				{
					if (currentTargetForFriendlyCityLinkMove == null) 
					{
						currentTargetForFriendlyCityLinkMove = city.Value;
					}
				}
			}
		}
		if (CheckIfDanger ()) 
		{

			FindAnEmptyLineToMakeBarrage ();
		}
		//jpourrais remettre ca plus tard si besoin de ranger les villes par ordre de danger : 
		//peut servir pour une IA plus maligne en attaque.
		//code a vérifier of course.
		//		dangereousNeighB.OrderBy((KeyValuePair<CityV2, int> arg) => 0);
	}


	//cette ville est-elle en danger ? y a t-il plus d'ennemi que d'allié autour d'elle ?
	public bool CheckIfDanger()
	{
		
		if (dangereousNeighB.Count-motherCity.defenseStr >0) 
		{
//			Debug.Log ("more enemies than this city can handle! care!!!");
			if (dangereousRoads == motherCity.defenseStr) 
			{
//				Debug.Log ("Alerte!On est a un tour de se faire baisé!");
				return true;

			}
		} 
		return false;

	}


	//Défini le "currentTargetForDefensiveMove" soit un LineController qui
	//n'est pas déja occuper et est connecté a un ennemi.
	public void FindAnEmptyLineToMakeBarrage()
	{
//		Debug.Log ("trying to find a nice and soft empty line");
		connectedCities.TryGetValue (dangereousNeighB.ElementAt(tries).Key, out currentTargetForDefenseMove);
		if (currentTargetForDefenseMove.isModified) 
		{
			tries++;
			if (tries == dangereousNeighB.Count) 
			{
//				Debug.Log ("aucun des ennemis a proximité n'a de route vide menant a lui");
				return;
			}
			FindAnEmptyLineToMakeBarrage();
		}
	}


    public int buildALink()
    {
        LineController line;
        int indexCity = Random.Range(0, listLinkGameObj.Length - 1);
        while (listLinkGameObj[indexCity] != null)
        {
            indexCity++;
            if (indexCity == listLinkGameObj.Length)
                indexCity = 0;
        }
        return indexCity;
    }


}
