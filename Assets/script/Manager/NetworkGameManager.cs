using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class NetworkGameManager : NetworkBehaviour {

	//le serveur est toujours "player1"
	//on utilise donc isServer pour savoir si on a affaire au joueur1 ou 2.
	//on utilise "isplayer1Turn" et isServer pour savoir si c'est ton tour ou pas.
	//une commande sur PlayerNetworkManager permet de changer le isplayer1Turn (sur le playerObj trouvable dans Gamemanager)
	public static NetworkGameManager instance;
	[SyncVar]public bool GameHasBegun;
	[SyncVar(hook = "ActuPlayerTurn")] public bool isPlayer1Turn;
	public Material[] matPlayerTurn;
	public Material[] matOpponentTurn;
	public bool gingerPowerAI;
    public IAInterface IAPlayer1;
    public IAInterface IAPlayer2;


    public void ActivateTheAmazingGingerAI()
	{
		gingerPowerAI = true;
		BeginTheGame ();
		NATTraversal.NetworkManager.singleton.StopMatchMaker ();

	}

	//référencement de la fonction static.
	void Awake()
	{
		if (instance != null) 
		{
			Destroy (this);
		}
		instance = this;
	}
	void Start()
	{
		if (SettingPlayer.instance.isSolo)
		{
			BeginTheGame();
		}
		
	}
	//arrive qu'une fois que chez le serveur.
	public override void OnStartServer ()
	{
		base.OnStartServer ();

		//on initialise les ID des points d'interets qui sont des syncvar.
		for (int i = 0; i < GameManager.instance.cities.Length; i++) 
		{
			GameManager.instance.cities [i].cityID = i;

		}
		for (int j = 100; j < GameManager.instance.lines.Length+100; j++) 
		{
			int k = j - 100;
			GameManager.instance.lines [k].lineID = j;

		}


    }
		
	//hook. Pas besoin de run sur le serveur.
	public void ActuPlayerTurn(bool isP1Turn)
	{
		if (!isServer) 
		{
			ChangePlayerTurn ();
		}
	}

	public void BeginTheGame()
	{
		StartCoroutine(GameManager.instance.ShowInfo("You play first!", 1f));
		RpcGameHasBegun ();
	}
	[ClientRpc]
	public void RpcGameHasBegun()
	{
		GameManager.instance.ActivateGingerPowerAIBtn.gameObject.SetActive (false);

		GameManager.instance.StartAnimation ();
		GameHasBegun = true;
		GameManager.instance.PlateauMeshR.materials = matPlayerTurn;
		if (!gingerPowerAI) {
			
			GameManager.instance.timeLeftSliderP1.gameObject.SetActive (true);
			GameManager.instance.StartCoroutine ("ActivateTimer");
		}
		
	}

	//appeler chez le serveur pour changer le tour.
	//appeler chez le client qui est pas serveur via hook callback.
	public void ChangePlayerTurn()
	{
		isPlayer1Turn = !isPlayer1Turn;
		GameManager.instance.isPlayer1Turn = !isPlayer1Turn;

		if (!gingerPowerAI) {
		    if (isPlayer1Turn) {
			
			    GameManager.instance.timeLeftSliderP1.gameObject.SetActive (true);
			    GameManager.instance.timeLeftSliderP2.gameObject.SetActive (false);

			    GameManager.instance.StopCoroutine ("ActivateTimer");
			    GameManager.instance.timeLeftSliderP2.value = GameManager.instance.timeLeftSliderP2.maxValue;
			    GameManager.instance.StartCoroutine ("ActivateTimer");
                

		    }
            else
            {
				GameManager.instance.timeLeftSliderP1.gameObject.SetActive (false);
				GameManager.instance.timeLeftSliderP2.gameObject.SetActive (true);

				GameManager.instance.StopCoroutine ("ActivateTimer");
				GameManager.instance.timeLeftSliderP1.value = GameManager.instance.timeLeftSliderP1.maxValue;
				GameManager.instance.StartCoroutine ("ActivateTimer");
            }
		}

		if (isServer) 
		{
			if (isPlayer1Turn) //si c'est ton tour et t'es serveur
			{
				StartCoroutine( GameManager.instance.ShowInfo ("Your Turn", 1f));
				GameManager.instance.PlateauMeshR.materials = matPlayerTurn;
                if (IAPlayer1 != null)
                {
                    DelayAITurn(IAPlayer1);
                }
            } else 
			{
				GameManager.instance.PlateauMeshR.materials= matOpponentTurn;
				if (gingerPowerAI) 
				{
					DelayAITurn ();
                }
                if (IAPlayer2 != null)
                {
                    DelayAITurn(IAPlayer2);
                }

            }
		} else 
		{
			if (!isPlayer1Turn) //si c'est ton tour que t'es pas serveur.
			{
				StartCoroutine( GameManager.instance.ShowInfo ("Your Turn", 1f));
				GameManager.instance.PlateauMeshR.materials = matOpponentTurn;

			} else 
			{
				GameManager.instance.PlateauMeshR.materials = matPlayerTurn;

			}
		}
	}

    public void DelayAITurn()
    {
        StartCoroutine(DelayAITurnProcedure());
    }
    public void DelayAITurn(IAInterface ia)
    {
        StartCoroutine(DelayAITurnProcedure(ia));
    }

    IEnumerator DelayAITurnProcedure()
    {
        yield return new WaitForSecondsRealtime(Random.Range(1f, 3f));
        GameManager.instance.GetComponent<GingerPowerAI>().PlayOneTurn();

    }
    IEnumerator DelayAITurnProcedure(IAInterface ia)
    {
        yield return new WaitForSecondsRealtime(Random.Range(1f, 3f));
        ia.launchTurn();

    }

    public void EndMyTurn()
	{
		if (isServer) 
		{
			ChangePlayerTurn ();
		}
	}
}
