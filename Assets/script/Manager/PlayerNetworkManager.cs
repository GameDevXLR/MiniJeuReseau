using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class PlayerNetworkManager : NetworkBehaviour  
{
	[SyncVar] string playerName;
	public bool needDeleteOnLoad = true;


	// Use this for initialization
	void Start () 
	{

		if (isLocalPlayer && !isServer) 
		{
			Invoke( "ActuNameOnClient",1f);
		}
		if (!isLocalPlayer && !isServer) 
		{
			GameManager.instance.player1Name.text = playerName;
		}
		if (!isLocalPlayer && isServer) 
		{
			NetworkGameManager.instance.BeginTheGame();

		}
	}


	void OnDisable()
	{
		if (needDeleteOnLoad) {
			if (!isLocalPlayer) {
				NATTraversal.NetworkManager.singleton.StopHost ();
				if (NATTraversal.NetworkManager.singleton) {
					Destroy (NATTraversal.NetworkManager.singleton.gameObject);
				}
			}
		}
	}

	public override void OnStartServer ()
	{
		base.OnStartServer ();
//		Debug.Log ("1");
//		if (!isLocalPlayer) 
//		{
//			Debug.Log ("done");
//			NetworkGameManager.instance.GameHasBegun = true;
//		}

	}



	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		GameManager.instance.localPlayerObj = gameObject;
		if (!isServer) {
//			playerName = PlayerPrefs.GetString ("PLAYER_NAME");
//			GameManager.instance.player2Name.text = playerName;
			GameManager.instance.MainTextInfoDisplay.enabled = false;
			CmdChangeMyName (PlayerPrefs.GetString ("PLAYER_NAME"));
		} else 
		{
			playerName = PlayerPrefs.GetString ("PLAYER_NAME");

			GameManager.instance.player1Name.text = PlayerPrefs.GetString ("PLAYER_NAME");
			
		}

	}

	public void CaptureCity(int cityID, bool isP1)
	{
		CmdCaptureCity (cityID, isP1);


	}
	public void CaptureLineMakeRoad(int line)
	{
		CmdCaptureLineMakeRoad (line);

	}
	public void CaptureLineMakeBarrage(int line)
	{
		CmdCaptureLineMakeBarrage (line);

	}

	[Command]
	public void CmdCaptureCity (int cityid, bool isP1T)
	{
		foreach (CityV2 c in GameManager.instance.cities) 
		{
			if (c.cityID == cityid) 
			{
				c.RpcCaptureCity (isP1T);
			} 
		}
		NetworkGameManager.instance.ChangePlayerTurn ();
	}

	[Command]
	public void CmdCaptureLineMakeRoad (int lineid)
	{
		NetworkGameManager.instance.ChangePlayerTurn ();
		foreach (LineController lineC in GameManager.instance.lines) {
			if (lineC.lineID == lineid) {	
				lineC.RpcChangeTheLineToRoad ();
				break;
			}
		}
	}
	[Command]
	public void CmdCaptureLineMakeBarrage (int lineid)
	{
		NetworkGameManager.instance.ChangePlayerTurn ();
		foreach (LineController lineC in GameManager.instance.lines) {
			if (lineC.lineID == lineid) {	
				lineC.RpcChangeTheLineToBarrage ();
				break;
			}
		}
	}

	[Command]
	public void CmdChangeMyName(string myName)
	{
		playerName = myName;
		GameManager.instance.player2Name.text = myName;
	}
	[ClientRpc]
	public void RpcActuMyName()
	{
		
	}

	public void ActuNameOnClient()
	{
		GameManager.instance.player2Name.text = playerName;
	}
}
