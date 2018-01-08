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
            return "Version 1.1.1";
        }
    }
    
    /// Gameplay time needed for showing ADS.
    public const string ADS_GAMEPLAY_TIME_KEY = "ADSGameplayTime";
    const int ADS_SHOW_GAMEPLAY_TIME = 180;

    void Start () {
		clickCooldownTimer = new Timer(0.75f);

		panelArray = GetComponentsInChildren<MainMenuPanel>(true);

		bool hasScore = ScoreListTimedDrawer.lastScore!=null;

		MainMenuPanelType newPanelOption = hasScore ? MainMenuPanelType.HIGH_SCORES : MainMenuPanelType.TITLE;
		EnablePanel(newPanelOption);

        versionLabel.text = Version;

        System.GC.Collect(); // Clear game no more used memory.
    }

    public void EnablePanel(MainMenuPanelType panelOption) {
        currentPanelOption = panelOption;
        foreach (MainMenuPanel panel in panelArray) {
            panel.gameObject.SetActive(panel.panelType == panelOption);
        }
    }
    
    void GoIntoGameScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

#region Buttons
    public void Title() {
        EnablePanel(MainMenuPanelType.TITLE);
    }

    public void Play() {
        EnablePanel(MainMenuPanelType.LOADING);
        ScoreListTimedDrawer.lastScore = null;
        this.Invoke(new WaitForEndOfFrame(), () => {
            if (ADSUtil.Supported) {
                int adsGameplayTime = PlayerPrefs.GetInt(ADS_GAMEPLAY_TIME_KEY, 0);

                if (adsGameplayTime >= ADS_SHOW_GAMEPLAY_TIME && ADSUtil.IsReady) {
                    PlayerPrefs.SetInt(ADS_GAMEPLAY_TIME_KEY, 0);
                    ADSUtil.Show(GoIntoGameScene);
                } else {
                    GoIntoGameScene();
                }
            } else {
                GoIntoGameScene();
            }
        });
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

    void Update() {
        if (currentPanelOption == MainMenuPanelType.LOADING)
            return;

        bool backToTitle = (
			Input.GetButtonDown("Fire1") && 
			currentPanelOption==MainMenuPanelType.HIGH_SCORES &&
			clickCooldownTimer.CheckAndUpdate()
		);
		if(backToTitle){
			EnablePanel(MainMenuPanelType.TITLE);
		}
        if(Input.GetKey(KeyCode.Escape)){
            if (currentPanelOption == MainMenuPanelType.TITLE) {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL && !UNITY_IOS			
			    Application.Quit();
#endif
            } else {
                EnablePanel(MainMenuPanelType.TITLE);
            }
        }
	}
}
