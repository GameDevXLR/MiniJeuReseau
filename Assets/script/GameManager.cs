using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    static public GameManager instance;

	public bool isPlayer1Turn = true;
    
	public Player player1;
    public Player player2;

	public Text MainTextInfoDisplay;
	public Text positionsLeftCount;
    public Text textScoreP1;
    public Text textScoreP2;
	public Text player1Name;
	public Text player2Name;

	public GameObject localPlayerObj;
	public GameObject backToMenuEndGameButton;

    public int pointsP1 = 0;
	public int pointsP2 = 0;

    public Material road;
    public Material barrage;

    public CityV2[] cities;
    
	public List<CityV2> citiesPlayer1;
    public List<CityV2> citiesPlayer2;

	public string[] AINames;

    public LineController[] lines;

    public int positionPossible = 0;

	public MeshRenderer PlateauMeshR;

	public Texture2D cursorNormal;
	public Texture2D cursorOver;

	public Slider timeLeftSliderP1;
	public Slider timeLeftSliderP2;

	public Button ActivateGingerPowerAIBtn;
	public Animator citiesAnimator;
	public GingerPowerAI GPAI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            positionPossible += cities.Length + lines.Length;
			positionsLeftCount.text = positionPossible.ToString ();
//            if (SettingPlayer.instance.isSolo)
//            {
//                NetworkGameManager.instance.BeginTheGame();
//            }
        }
        else
            Destroy(gameObject);
    }

	public void StartAnimation()
	{
		if (SettingPlayer.instance.isSolo) 
		{
			player2Name.text = "You/A friend";
		}
		citiesAnimator.enabled = true;
		Invoke ("ShowTheLines", 2f);
	}


	public void ShowTheLines()
	{
		foreach (var l in lines) 
		{
			l.GetComponent<LineRenderer> ().enabled = true;
			
		}
	}

	public void PlayAgainstGPAI()
	{
		GPAI.enabled = true;
		ActivateGingerPowerAIBtn.gameObject.SetActive (false);
	}

	public void ChangeCursor(bool isOver)
	{
		if (isOver) 
		{
			Cursor.SetCursor (cursorOver, new Vector2 (8f, 8f), CursorMode.Auto);
		} else 
		{
			Cursor.SetCursor (cursorNormal, new Vector2 (8f, 8f), CursorMode.Auto);

		}
	}

	public IEnumerator ShowInfo(string info, float displayTime)
	{
		MainTextInfoDisplay.enabled = true;
		MainTextInfoDisplay.text = info;
		yield return new WaitForSecondsRealtime (displayTime);
		MainTextInfoDisplay.enabled = false;

	} 

    public void addCity(CityV2 city)
    {
        if (isPlayer1Turn)
        {
            if (!citiesPlayer1.Contains(city))
            {
                citiesPlayer1.Add(city);
//                setPoint(1);
                textScoreP1.text = pointsP1.ToString();
            }
            else
            {
                Debug.Log("Player : Try to add a City already add");
            }
        }
        else
        {
            if (!citiesPlayer2.Contains(city))
            {
                citiesPlayer2.Add(city);
//                setPoint(1);
            }
            else
            {
                Debug.Log("Player : Try to add a City already add");
            }
        }
    }

    public void removeCity(CityV2 city)
    {
        if (isPlayer1Turn)
        {
            if (citiesPlayer1.Contains(city))
            {
                citiesPlayer1.Remove(city);
//                setPoint(-1);
            }
            else
            {
                Debug.Log("Player : Try to remove a City not here");
            }
        }
        else
        {
            if (citiesPlayer2.Contains(city))
            {
                citiesPlayer2.Remove(city);
//                setPoint(-1);
            }
            else
            {
                Debug.Log("Player : Try to remove a City not here");
            }
        }
    }

    public void printCities()
    {
        string result = "players 1 cities => ";
        foreach (CityV2 city in citiesPlayer1)
        {
            result += city.gameObject.name;
        }
        Debug.Log(result);

        result = "players 2 cities => ";
        foreach (CityV2 city in citiesPlayer2)
        {
            result += city.gameObject.name;
        }
        Debug.Log(result);
    }

	public void AddPointP1(bool wasOwned)
	{
		if (wasOwned) 
		{
			pointsP2--;
			textScoreP2.text = pointsP2.ToString();

		}
		pointsP1++;
		textScoreP1.text = pointsP1.ToString();
	}

	public void AddPointP2(bool wasOwned)
	{
		if (wasOwned) 
		{
			pointsP1--;
			textScoreP1.text = pointsP1.ToString();

		}
		pointsP2++;
		textScoreP2.text = pointsP2.ToString();
	}

    public void setPoint(int point)
    {
        if (isPlayer1Turn)
        {
            pointsP1 += point;
            textScoreP1.text = pointsP1.ToString();
        }
        else
        {
            pointsP2 += point;
            textScoreP2.text = pointsP2.ToString();
        }
    }

    public void setPoint(int point, bool isP1)
    {
        if (isP1)
        {
            pointsP1 += point;
            textScoreP1.text = pointsP1.ToString();
        }
        else
        {
            pointsP2 += point;
            textScoreP2.text = pointsP2.ToString();
        }
    }

	public void ChangeTurn()
	{
		isPlayer1Turn = !isPlayer1Turn;
		if (isPlayer1Turn) 
		{
			StartCoroutine(ShowInfo("YOUR TURN!",1.5f));
		}
		ChangePositionPossible (-1);
	}

	public void ChangePositionPossible(int i)
	{
		positionPossible += i;
		positionsLeftCount.text = positionPossible.ToString ();
		if (positionPossible == 0) 
		{
			FinishTheGame ();
		}
	}

	public void FinishTheGame()
	{
		StartCoroutine (EndOfGame ());
	}

	IEnumerator EndOfGame()
	{

		PlayerNetworkManager[] players = GameObject.FindObjectsOfType <PlayerNetworkManager> ()as PlayerNetworkManager[];
		foreach (var player in players) 
		{
			player.needDeleteOnLoad = false;
			
		}
		if (citiesPlayer1.Count > citiesPlayer2.Count) {
			if (localPlayerObj.GetComponent<PlayerNetworkManager> ().isServer) {
				//si t'es le serveur et que t'as plus de villes : t'as gagné!
				int i = PlayerPrefs.GetInt ("WINS");
				PlayerPrefs.SetInt ("WINS", i + 1);
			} else {
				//t'as perdu :(
				int i = PlayerPrefs.GetInt ("LOSSES");
				PlayerPrefs.SetInt ("LOSSES", i + 1);
			}
		} else 
		{
			if (!localPlayerObj.GetComponent<PlayerNetworkManager> ().isServer) {
				//si t'es le serveur et que t'as plus de villes : t'as gagné!
				int i = PlayerPrefs.GetInt ("WINS");
				PlayerPrefs.SetInt ("WINS", i + 1);
			} else {
				//t'as perdu :(
				int i = PlayerPrefs.GetInt ("LOSSES");
				PlayerPrefs.SetInt ("LOSSES", i + 1);
			}
		}
		backToMenuEndGameButton.SetActive (true);
		StartCoroutine( ShowInfo ("The Game will restart in 10 seconds...", 10f));
		yield return new WaitForSecondsRealtime (10f);
		NATTraversal.NetworkManager.singleton.ServerChangeScene ("Plateau1");	}

	public void GoBackToMenu()
	{
		//on check quand tu quittes si il reste des points a prnedre: si oui c'est abandon donc loose.
		//faudrait aussi checker voir si t'es solo...si t'es solo ca compte ptete pas ? mais je laisse la place en attendant
		//une possible IA.
		if (positionPossible > 0) {
			int i = PlayerPrefs.GetInt ("LOSSES");
			PlayerPrefs.SetInt ("LOSSES", i + 1);
		} else 
		{
		}
		StopCoroutine ("EndOfGame");

		NATTraversal.NetworkManager.singleton.StopHost();
		Destroy (NATTraversal.NetworkManager.singleton.gameObject);

	}

	IEnumerator ActivateTimer ()
	{
		while (true) {
			if (NetworkGameManager.instance.isPlayer1Turn) {
				timeLeftSliderP1.value -= .5f;
				yield return new WaitForSecondsRealtime (.5f);
				if (timeLeftSliderP1.value <= 0) {
					NetworkGameManager.instance.ChangePlayerTurn ();
					yield break;
				}
			} else {
				timeLeftSliderP2.value -= .5f;
				yield return new WaitForSecondsRealtime (.5f);
				if (timeLeftSliderP2.value <= 0) {
					NetworkGameManager.instance.ChangePlayerTurn ();
					yield break;
				}
			}
		}
	}
}
