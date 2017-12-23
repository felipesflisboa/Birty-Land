using UnityEngine;
using System.Collections;

public class InitialMenuManager : SingletonMonoBehaviour<InitialMenuManager> {
	public enum Option{
		NONE=0, TITLE, INFO, CONTROLS, HIGH_SCORES
	}
	Option currentPanelOption;
	InitialMenuPanel[] panelArray;
	Timer clickCooldownTimer;

	void Start () {
		clickCooldownTimer = new Timer(0.75f);

		panelArray = GetComponentsInChildren<InitialMenuPanel>(true);

		bool hasScore = ScoreListTimedDrawer.lastScore!=null;

		Option newPanelOption = hasScore ? Option.HIGH_SCORES : Option.TITLE;
		EnablePanel(newPanelOption);
	}

	public void EnablePanel(Option panelOption){
		currentPanelOption = panelOption;
		foreach(InitialMenuPanel panel in panelArray){
			panel.gameObject.SetActive(panel.panelType==panelOption);
		}
	}

#region Buttons
	public void Play(){
		ScoreListTimedDrawer.lastScore=null;
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}

	public void Info(){
		EnablePanel(Option.INFO);
	}

	public void Controls(){
		EnablePanel(Option.CONTROLS);
	}

	public void HighScores(){
		EnablePanel(Option.HIGH_SCORES);
	}

	public void Exit(){
		Application.Quit();
	}
#endregion

	void Update(){
		bool backToTitle = (
			Input.GetButtonDown("Fire1") && 
			currentPanelOption!=Option.TITLE && 
			currentPanelOption!=Option.INFO && 
			clickCooldownTimer.CheckAndUpdate()
		);
		if(backToTitle){
			EnablePanel(Option.TITLE);
		}
	}
}
