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

    internal bool autoFire;
    internal Vector2 lastClickWorldPos;
    Plane groundPlane = new Plane(Vector3.down, Vector3.zero);

    public bool UseTouchControls {
        get {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#elif UNITY_WEBGL
            return WebGLMobileCheck.IsMobile;
#else
            return false;
#endif
        }
    }

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
		state = GameState.BeforeStart;

		bool canvasManagerFound  = FindObjectOfType<CanvasController>() != null; //TODO other assign that works on disabled object
		if(!canvasManagerFound)
			Instantiate(canvasManagerPrefab).GetComponent<CanvasController>();

		player = FindObjectOfType<Player>();
		bool playerFound = player != null; 
		if(!playerFound)
			player = Instantiate(playerPrefab).GetComponent<Player>();

        lastClickWorldPos = Scenario.I.playerStartPos;
        if (UseTouchControls)
            autoFire = true;

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
		state = GameState.Ocurring;
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
					if(destructible.team == Team.Ally)
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

	public void BackToMainMenu() {
        // Save time regarding to ADS exhibition.
        if (AdsUtil.Supported) {
            int adsGameplayTime = PlayerPrefs.GetInt(MainMenuManager.ADS_GAMEPLAY_TIME_KEY, 0);
            PlayerPrefs.SetInt(MainMenuManager.ADS_GAMEPLAY_TIME_KEY, adsGameplayTime + SecondsInt);
        }
        LoadScene("MainMenu");
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
		state = GameState.End;
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
        BackToMainMenu();
    }

	/// <summary>
	/// Loads other scene. When name is null or empty, load self.
	/// </summary>
	public void LoadScene(string name=null, float delay = 0f){
		state = GameState.End; // To make sure that some things like pause aren't called.
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
		if(state==GameState.End)
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
    
    public void RefreshCursorPosition() {
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition != Vector3.zero) {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition); // Only checks first
            float rayDistance;
            if (groundPlane.Raycast(clickRay, out rayDistance))
                lastClickWorldPos = clickRay.GetPoint(rayDistance).XZToV2();
        }
    }

#region Pause
    public void TryToTogglePause(){
		if(state != GameState.Ocurring)
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
		if(state == GameState.Ocurring)
			CanvasController.I.TogglePauseMenu();
	}
	protected void Unpause(){
		Time.timeScale = currentTimeScale;
		if(state == GameState.Ocurring)
			CanvasController.I.TogglePauseMenu();
	}
#endregion
}