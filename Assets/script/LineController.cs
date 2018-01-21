using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LineController : NetworkBehaviour {

	public AudioSource audioS;
	public AudioClip hoverSnd;
	public AudioClip barrageSnd;
	public AudioClip roadSnd;

    public CityV2[] cities;
	[SyncVar]public int lineID;
    public bool isModified = false;
	public bool isBarrage;
	public Material matNormal;
	public Material matHover;
	public Material matHoverBarrage;
	LineRenderer lineR;
	Material tmpMat; //utiliser pour faire clignoter.

	float doubleClicTimer;
	bool waitingForDoubleClick;

	void Start()
	{
		lineR = GetComponent<LineRenderer> ();
	}
    private void OnMouseDown()
	{
		if (NetworkGameManager.instance.GameHasBegun) {
			
			if (!isModified) {
				if (NetworkGameManager.instance.isPlayer1Turn && isServer || !NetworkGameManager.instance.isPlayer1Turn && !isServer || SettingPlayer.instance.isSolo) 
				{
					isModified = true;
					StartCoroutine(WaitForDoubleClick());
//					if (Input.GetKey (KeyCode.LeftControl)) {
//						GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeBarrage (lineID);
//						isModified = true;
//						return;
//
//					} else {	
//						GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeRoad (lineID);
//						isModified = true;
//					}
				}
			}
		}
	}

	IEnumerator WaitForDoubleClick()
	{
		waitingForDoubleClick = true;
		doubleClicTimer = 0f;
		yield return new WaitForEndOfFrame ();
		while (waitingForDoubleClick) 
		{

			doubleClicTimer += .1f;
			if (doubleClicTimer > .7f) {
				if (Input.GetMouseButton (0)) {
					GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeBarrage (lineID);
					
				} else {
					GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeRoad (lineID);
				}
				waitingForDoubleClick = false;
			}
			if (!Input.GetMouseButton(0)) 
			{
				GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeRoad (lineID);
				waitingForDoubleClick = false;
			}
			yield return new WaitForSecondsRealtime (.1f);			
		}
	}
	void OnMouseEnter()
	{
		if (!isModified) {
			audioS.PlayOneShot (hoverSnd);
			if (Input.GetKey (KeyCode.LeftControl)) 
			{
				lineR.material = matHoverBarrage;

				return;
			}
			GameManager.instance.ChangeCursor (true);
			lineR.material = matHover;
		}
	}

	void OnMouseExit()
	{
		if (!isModified) {
			
			GameManager.instance.ChangeCursor (false);
			lineR.material = matNormal;

		}
	}

	public void MakeBarrage()
	{
		GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeBarrage (lineID);
		isModified = true;
	}

	public void MakeRoad()
	{
		GameManager.instance.localPlayerObj.GetComponent<PlayerNetworkManager> ().CaptureLineMakeRoad (lineID);
		isModified = true;
	}

	[ClientRpc]
	public void RpcChangeTheLineToRoad()
	{
		audioS.PlayOneShot (roadSnd);

		isModified = true;
		lineR.material = GameManager.instance.road;
		cities[0].linkCities.Add(cities[1]);
		cities[1].linkCities.Add(cities[0]);
		cities[0].checkAppartenance();
		cities[1].checkAppartenance();
		StartCoroutine (AfterCaptureProcedure ());

	}

	[ClientRpc]
	public void RpcChangeTheLineToBarrage()
	{
		audioS.PlayOneShot (barrageSnd);
		isBarrage = true;
		isModified = true;
		lineR.material = GameManager.instance.barrage;
		StartCoroutine (AfterCaptureProcedure ());

	}
	IEnumerator AfterCaptureProcedure()
	{
		tmpMat = lineR.material;
		yield return new WaitForSecondsRealtime (.3f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.3f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.3f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.2f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.2f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.1f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.1f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.1f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = tmpMat;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = matNormal;
		yield return new WaitForSecondsRealtime (.05f);
		lineR.material = tmpMat;
	}
}
