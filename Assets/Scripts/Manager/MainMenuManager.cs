using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : SingletonMonoBehaviour<MainMenuManager> {
    [SerializeField] Text versionLabel;

	MainMenuPanelType currentPanelOption;
	MainMenuPanel[] panelArray;
	Timer clickCooldownTimer;

    string Version {
        get {
            return "Version 1.0.1";
        }
    }

	void Start () {
		clickCooldownTimer = new Timer(0.75f);

		panelArray = GetComponentsInChildren<MainMenuPanel>(true);

		bool hasScore = ScoreListTimedDrawer.lastScore!=null;

		MainMenuPanelType newPanelOption = hasScore ? MainMenuPanelType.HIGH_SCORES : MainMenuPanelType.TITLE;
		EnablePanel(newPanelOption);

        versionLabel.text = Version;

        System.GC.Collect(); // Clear game no more used memory.
    }

	public void EnablePanel(MainMenuPanelType panelOption){
		currentPanelOption = panelOption;
		foreach(MainMenuPanel panel in panelArray){
			panel.gameObject.SetActive(panel.panelType==panelOption);
		}
	}

#region Buttons
	public void Play(){
		ScoreListTimedDrawer.lastScore=null;
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}

	public void Info(){
		EnablePanel(MainMenuPanelType.INFO);
	}

	public void Controls(){
		EnablePanel(MainMenuPanelType.CONTROLS);
	}

	public void HighScores(){
		EnablePanel(MainMenuPanelType.HIGH_SCORES);
	}

	public void Exit(){
		Application.Quit();
	}
#endregion

	void Update(){
		bool backToTitle = (
			Input.GetButtonDown("Fire1") && 
			currentPanelOption!=MainMenuPanelType.TITLE && 
			currentPanelOption!=MainMenuPanelType.INFO && 
			clickCooldownTimer.CheckAndUpdate()
		);
		if(backToTitle){
			EnablePanel(MainMenuPanelType.TITLE);
		}
	}
}
