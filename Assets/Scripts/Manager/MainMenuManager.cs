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
            return "Version 1.2";
        }
    }
    
    /// Gameplay time needed for showing ADS.
    public const string ADS_GAMEPLAY_TIME_KEY = "ADSGameplayTime";
    const int ADS_SHOW_GAMEPLAY_TIME = 180;

    void Start () {
		clickCooldownTimer = new Timer(0.75f);

		panelArray = GetComponentsInChildren<MainMenuPanel>(true);

		bool hasScore = ScoreListTimedDrawer.lastScore!=null;

		MainMenuPanelType newPanelOption = hasScore ? MainMenuPanelType.HighScores : MainMenuPanelType.Title;
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
        EnablePanel(MainMenuPanelType.Title);
    }

    public void Play() {
        EnablePanel(MainMenuPanelType.Loading);
        ScoreListTimedDrawer.lastScore = null;
        this.Invoke(new WaitForEndOfFrame(), () => {
            if (AdsUtil.Supported) {
                int adsGameplayTime = PlayerPrefs.GetInt(ADS_GAMEPLAY_TIME_KEY, 0);

                if (adsGameplayTime >= ADS_SHOW_GAMEPLAY_TIME && AdsUtil.IsReady) {
                    PlayerPrefs.SetInt(ADS_GAMEPLAY_TIME_KEY, 0);
                    AdsUtil.Show(GoIntoGameScene);
                } else {
                    GoIntoGameScene();
                }
            } else {
                GoIntoGameScene();
            }
        });
	}

    public void Info(){
		EnablePanel(MainMenuPanelType.Info);
	}

	public void Resolution(){
		EnablePanel(MainMenuPanelType.Resolution);
	}

	public void HighScores(){
		EnablePanel(MainMenuPanelType.HighScores);
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
        if (currentPanelOption == MainMenuPanelType.Loading)
            return;

        bool backToTitle = (
			Input.GetButtonDown("Fire1") && 
			currentPanelOption==MainMenuPanelType.HighScores &&
			clickCooldownTimer.CheckAndUpdate()
		);
		if(backToTitle){
			EnablePanel(MainMenuPanelType.Title);
		}
        if(Input.GetKey(KeyCode.Escape)){
            if (currentPanelOption == MainMenuPanelType.Title) {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL && !UNITY_IOS			
			    Application.Quit();
#endif
            } else {
                EnablePanel(MainMenuPanelType.Title);
            }
        }
	}
}
