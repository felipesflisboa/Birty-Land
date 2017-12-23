using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// T2
public class GameManager : SingletonMonoBehaviour<GameManager> {
	[SerializeField] GameObject canvasManagerPrefab;
	[SerializeField] GameObject playerPrefab;

	internal Player player;
	internal GameState state;
	internal int score{get; private set;}
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

		bool canvasManagerFound  = FindObjectOfType<CanvasManager>() != null; //TODO other assign that works on disabled object
		if(!canvasManagerFound)
			Instantiate(canvasManagerPrefab).GetComponent<CanvasManager>();

		player = FindObjectOfType<Player>();
		bool playerFound = player != null; 
		if(!playerFound)
			player = Instantiate(playerPrefab).GetComponent<Player>();

		currentTimeScale = Time.timeScale;
	}

	void Start (){
		// Initialize Player
		player.transform.position = Vector3.zero;
		player.transform.SetParent(Scenario.I.actorArea);
		player.Initialize();
		//TODO Start Position

		// Initialize Camera
		Camera.main.transform.SetParent(player.cameraContainer, false);
		Camera.main.transform.localPosition = Vector3.zero;
		Camera.main.transform.localRotation = Quaternion.identity;

		StartGame();
	}

	protected virtual void StartGame(){
		startTime = Time.timeSinceLevelLoad;
		state = GameState.OCURRING;
		SpawnManager.I.Begin();
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
				//TODO
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
		if(Debug.isDebugBuild)
			PrintDebugData();

		state = GameState.END;
		endTime = Time.timeSinceLevelLoad;

		int timeToSave = SecondsInt;
		ScoreListTimed scoreList = new ScoreListTimed();
		scoreList.Load();
		bool newRecord = scoreList.AddScore(timeToSave);
		if(newRecord)
			scoreList.Save();
		ScoreListTimedDrawer.lastScore = timeToSave;

		yield return new WaitForSeconds(2f);
		CanvasManager.I.DisplayGameOverMenu();
		yield return new WaitForSeconds(3.5f);
		LoadScene("MainMenu");
	}

	/// <summary>
	/// Loads other scene. When name is null or empty, load self.
	/// </summary>
	public void LoadScene(string name=null, float delay = 0f){
		state = GameState.END; // To make sure that some things like pause aren't called.
		if(delay != 0f){ //TODO realtime
			this.Invoke(new WaitForSeconds(delay), () => LoadScene(name));
			return;
		}
		if(Time.timeScale != currentTimeScale)
			Time.timeScale = currentTimeScale;
		if(string.IsNullOrEmpty(name))
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		else
			UnityEngine.SceneManagement.SceneManager.LoadScene(name);
	}

	void PrintDebugData(){
		int totalSeconds = SecondsInt;
		string timeString = string.Format("{0}:{1:00}",totalSeconds / 60, totalSeconds % 60);
		// string poolDebugString = PoolManager.I==null ? "" : PoolManager.I.DebugString();
		// Debug.LogFormat("[GameManager.End] Time: {0}, TotalScore {1} {2}", timeString, TotalScore, poolDebugString);
		//TODO spawn
	}

	/// <summary>
	/// Add a value into score. Also increase points.
	/// </summary>
	public void AddScore (int gain) {
		score+=gain;
		points+=gain;

		bool initializePauseAlert = !CanvasManager.I.pauseAlertInitialized && points >= player.levelSystem.CostForNextLevel(1);
		if(initializePauseAlert)
			CanvasManager.I.InitializePauseAlert();
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
			CanvasManager.I.TogglePauseMenu();
	}
	protected void Unpause(){
		Time.timeScale = currentTimeScale;
		//TODO
		if(state == GameState.OCURRING)
			CanvasManager.I.TogglePauseMenu();
	}
	#endregion
}