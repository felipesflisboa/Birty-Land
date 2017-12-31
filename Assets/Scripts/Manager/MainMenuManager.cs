using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : SingletonMonoBehaviour<MainMenuManager> {
    [SerializeField] Text versionLabel;
    [SerializeField] CanvasGroup mainGroup;
    [SerializeField] CanvasGroup webGLGroup;

    MainMenuPanelType currentPanelOption;
	MainMenuPanel[] panelArray;
	Timer clickCooldownTimer;

    string Version {
        get {
            return "Version 1.1";
        }
    }

	void Start () {
		clickCooldownTimer = new Timer(0.75f);

		panelArray = GetComponentsInChildren<MainMenuPanel>(true);

		bool hasScore = ScoreListTimedDrawer.lastScore!=null;

		MainMenuPanelType newPanelOption = hasScore ? MainMenuPanelType.HIGH_SCORES : MainMenuPanelType.TITLE;
		EnablePanel(newPanelOption);

        versionLabel.text = Version;

#if UNITY_WEBGL
        mainGroup.gameObject.SetActive(false);
        webGLGroup.gameObject.SetActive(true);
#else
        mainGroup.gameObject.SetActive(true);
        webGLGroup.gameObject.SetActive(false);
#endif

        System.GC.Collect(); // Clear game no more used memory.
    }

	public void EnablePanel(MainMenuPanelType panelOption){
		currentPanelOption = panelOption;
		foreach(MainMenuPanel panel in panelArray){
			panel.gameObject.SetActive(panel.panelType==panelOption);
		}
	}

#region Buttons
    public void Title() {
        EnablePanel(MainMenuPanelType.TITLE);
    }

    public void Play(){
		ScoreListTimedDrawer.lastScore=null;
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}

	public void Info(){
		EnablePanel(MainMenuPanelType.INFO);
	}

	public void Resolution(){
		EnablePanel(MainMenuPanelType.RESOLUTION);
	}

	public void HighScores(){
		EnablePanel(MainMenuPanelType.HIGH_SCORES);
	}

	public void Exit(){
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
#endregion

	void Update(){
		bool backToTitle = (
			Input.GetButtonDown("Fire1") && 
			currentPanelOption==MainMenuPanelType.HIGH_SCORES &&
			clickCooldownTimer.CheckAndUpdate()
		);
		if(backToTitle){
			EnablePanel(MainMenuPanelType.TITLE);
		}
	}
}
