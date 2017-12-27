using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager> {
	[SerializeField] GameObject canvasManagerPrefab;
	[SerializeField] GameObject playerPrefab;

	internal Player player;
	internal GameState state;
	internal int score{get; private set;} // Score is the point amount, counting the values spent.
	internal int points{get; private set;}

	protected float startTime;
	protected float endTime;
	float currentTimeScale;

	public bool Paused{
		get{
			return Time.timeScale == 0f;
		}
	}

	public int SecondsInt{
		get{
			return Mathf.RoundToInt(SecondsFloat);
		}
	}
	public float SecondsFloat{
		get{
			return (endTime!=0f ? endTime : Time.timeSinceLevelLoad) - startTime;
		}
	}

	void Awake () {
		state = GameState.BEFORE_START;

		bool canvasManagerFound  = FindObjectOfType<CanvasController>() != null; //TODO other assign that works on disabled object
		if(!canvasManagerFound)
			Instantiate(canvasManagerPrefab).GetComponent<CanvasController>();

		player = FindObjectOfType<Player>();
		bool playerFound = player != null; 
		if(!playerFound)
			player = Instantiate(playerPrefab).GetComponent<Player>();

		currentTimeScale = Time.timeScale;
	}

	void Start (){
		// Initialize Player
		player.transform.SetParent(Scenario.I.actorArea);
		player.transform.position = Scenario.I.playerStartPos.YToZ();

		// Initialize Camera
		Camera.main.transform.SetParent(player.cameraContainer, false);
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;

		StartGame();
	}

	protected virtual void StartGame(){
		startTime = Time.timeSinceLevelLoad;
		state = GameState.OCURRING;
		HiveSpawnManager.I.Begin();
		HunterSpawnManager.I.Begin();
	}

	void Update () {
		if(Debug.isDebugBuild){
			// Turbo button
			if (Input.GetKeyDown (KeyCode.F)){
				float fastTimeScale = currentTimeScale*8f;
				Time.timeScale = Mathf.Approximately(Time.timeScale, fastTimeScale) ? currentTimeScale : fastTimeScale;
			}
			// Damage all
			if (Input.GetKey (KeyCode.E)){
				const float radius = 16f;
				float sqrRadius = radius*radius;
				List<Destructible> destructibleToDamage = new List<Destructible>();
				foreach(Destructible destructible in FindObjectsOfType<Destructible>()){
					if(destructible.team == Team.ALLY)
						continue;

					Vector3 difference = destructible.transform.position - player.transform.position;
					bool onRange = difference.XZToV2().sqrMagnitude < sqrRadius;
					if(onRange)
						destructibleToDamage.Add(destructible);
				}
				foreach(Destructible destructible in destructibleToDamage)
					destructible.TakeDamage(500);
			}
			// Restart
			if (Input.GetKeyDown (KeyCode.R)){
				LoadScene();
			}
			// Force Game Over
			if(Input.GetKeyDown (KeyCode.G)){
				player.Explode();
			}
		}
		if (Input.GetButtonDown ("Jump")){
			TryToTogglePause();
		}
	}

	protected virtual void OnEscapePress(){
		// LoadScene(LobbyMenu.lobbyMenuSceneName); //TODO
	}

	/// <summary>
	/// Ends the game with a Game Over.
	/// </summary>
	public void EndGame (){
		StartCoroutine(EndGameRoutine());
	}

	/// <summary>
	/// Ends the game with a Game Over.
	/// </summary>
	IEnumerator EndGameRoutine (){
		state = GameState.END;
		endTime = Time.timeSinceLevelLoad;

		if(Debug.isDebugBuild)
			PrintDebugData();

		int timeToSave = SecondsInt;
		ScoreListTimed scoreList = new ScoreListTimed();
		scoreList.Load();
		bool newRecord = scoreList.AddScore(timeToSave);
		if(newRecord)
			scoreList.Save();
		ScoreListTimedDrawer.lastScore = timeToSave;

		yield return new WaitForSeconds(2f);
		CanvasController.I.DisplayGameOverMenu();
		yield return new WaitForSeconds(3.5f);
		LoadScene("MainMenu");
	}

	/// <summary>
	/// Loads other scene. When name is null or empty, load self.
	/// </summary>
	public void LoadScene(string name=null, float delay = 0f){
		state = GameState.END; // To make sure that some things like pause aren't called.
		if(delay != 0f){
			this.Invoke(new WaitForSecondsRealtime(delay), () => LoadScene(name));
			return;
		}
		if(Time.timeScale != currentTimeScale)
			Time.timeScale = currentTimeScale;
		if(string.IsNullOrEmpty(name))
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		else
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
	}

	public void PrintDebugData(){
		int totalSeconds = SecondsInt;
		string timeString = string.Format("{0}:{1:00}",totalSeconds / 60, totalSeconds % 60);
		string debugMessage = string.Format("Time: {0}. Points: {1}/{2}", timeString, points, score);
		if(state==GameState.END)
			debugMessage = "End game! " + debugMessage;
		Debug.LogFormat("[GameManager.PrintDebugData] {0}", debugMessage);
	}

	/// <summary>
	/// Add a value into score. Also increase points.
	/// </summary>
	public void AddScore (int gain) {
		score+=gain;
		points+=gain;

		bool initializePauseAlert = !CanvasController.I.pauseAlert.initialized && points >= player.levelController.CostForNextLevel(1);
		if(initializePauseAlert)
			CanvasController.I.pauseAlert.Initialize();
	}

	public void SpendPoint (int point) {
		points-=point;
	}

	#region Pause
	public void TryToTogglePause(){
		if(state != GameState.OCURRING)
			return;
		TogglePause();
	}
	protected void TogglePause(){
		if(Paused)
			Unpause();
		else
			Pause();
	}
	protected void Pause(){
		Time.timeScale = 0;
		if(state == GameState.OCURRING)
			CanvasController.I.TogglePauseMenu();
	}
	protected void Unpause(){
		Time.timeScale = currentTimeScale;
		if(state == GameState.OCURRING)
			CanvasController.I.TogglePauseMenu();
	}
	#endregion
}