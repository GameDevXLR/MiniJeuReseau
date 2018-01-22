using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CityV2 : NetworkBehaviour {

	public CityNeighborhood neighboors;

	public AudioClip hoverSnd;
	public AudioClip CapturedSnd;
	public AudioSource audioS;
    public List<CityV2> linkCities;
    public bool isTaken = false;
    public bool isP1 = false;
	public MeshRenderer meshR;
	[SyncVar]public int cityID;
	public Material matNeutral;
	public Material matNeutralHover;
	Material tmpMat; //utiliser pour faire clignoter
	public int defenseStr;
	public int attackStr;
    bool isP1Turn;


    private void OnMouseDown()
	{
		if (NetworkGameManager.instance.GameHasBegun) {
			if (!isTaken) {
				if (NetworkGameManager.instance.isPlayer1Turn && isServer || !NetworkGameManager.instance.isPlayer1Turn && !isServer || SettingPlayer.instance.isSolo) 
				{
					isP1Turn = NetworkGameManager.instance.isPlayer1Turn;
					GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureCity (cityID, isP1Turn);
					isTaken = true;

				}

			}
        
		}
	}

	public void CaptureThisCity()
	{
		isP1Turn = NetworkGameManager.instance.isPlayer1Turn;
		GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureCity (cityID, isP1Turn);
		isTaken = true;
	}

	[ClientRpc]
	public void RpcCaptureCity(bool wasP1Turn)
	{
		GameManager.instance.ChangePositionPossible (-1);
//		Debug.Log (gameObject.name);
		isTaken = true;
		if (wasP1Turn) 
		{
			meshR.material =GameManager.instance.player1.material;
			isP1 = true;
			GameManager.instance.addCity(this);
			GameManager.instance.AddPointP1 (false);
		} 
		else 
		{
			meshR.material = GameManager.instance.player2.material;
			isP1 = false;
			GameManager.instance.addCity(this);
			GameManager.instance.AddPointP2 (false);

		}
		StartCoroutine (AfterCaptureProcedure ());
	}

	void OnMouseEnter()
	{
		if (!isTaken) {
			GameManager.instance.ChangeCursor (true);
			meshR.material = matNeutralHover;
			audioS.PlayOneShot (hoverSnd);

		}
	}

	void OnMouseExit()
	{
		if (!isTaken) {
			audioS.Stop ();

			GameManager.instance.ChangeCursor (false);
			meshR.material = matNeutral;
		}
	}

    public void checkAppartenance()
    {
        if (isTaken)
        {

            int cityP1 = (isP1) ? 1 : 0;
            int cityP2 = (isP1) ? 0 : 1;
            foreach (CityV2 city in linkCities)
            {
                if (city.isP1)
                {
                    cityP1++;
                }
                else
                {
                    cityP2++;
                } 
            }
            if(cityP1>cityP2 && !isP1)
            {
				meshR.material = GameManager.instance.player1.material;
                GameManager.instance.addCity(this);
				GameManager.instance.AddPointP1 (true);
				StartCoroutine (AfterCaptureProcedure ());

//                GameManager.instance.setPoint(+1, true);
//                GameManager.instance.setPoint(-1, false);
                isP1 = true;

                foreach (CityV2 city in linkCities)
                {
                    city.checkAppartenance();
                }
            }
            else if (cityP1 < cityP2 && isP1)
            {
				meshR.material = GameManager.instance.player2.material;
                GameManager.instance.addCity(this);
				GameManager.instance.AddPointP2 (true);
				StartCoroutine (AfterCaptureProcedure ());

//                GameManager.instance.setPoint(-1, true);
//                GameManager.instance.setPoint(+1, false);
                isP1 = false;
                foreach (CityV2 city in linkCities)
                {
                    city.checkAppartenance();
                }
            }
        }

    }

	IEnumerator AfterCaptureProcedure()
	{
		audioS.PlayOneShot (CapturedSnd);

		tmpMat = meshR.material;
		yield return new WaitForSecondsRealtime (.3f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.3f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.3f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.2f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.2f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.1f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.1f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.1f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = matNeutralHover;
		yield return new WaitForSecondsRealtime (.05f);
		meshR.material = tmpMat;
	}
}
