using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogicUI : MonoBehaviour
{

    public Main main;

    public GameObject gui_mainmenu;
    public GameObject gui_game;
    public GameObject gui_upgrades;

    public GameObject panel_pause;
    public GameObject panel_summary;
    public GameObject panel_gameover;
    public GameObject panel_options;

    public GameObject text_wave_number;
    public HPBar hpbar_fort;

    public GameObject text_game_gold;
    public GameObject text_upgrades_gold;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void onClick_MainMenu()
    {
        //SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        pause();
        closePause();
        closeSummary();
        closeGameOver();
        closeGUI_Game();
        closeGUI_Upgrades();
        openGUI_MainMenu();
    }





    //MAIN MENU GUI
    public void openGUI_MainMenu()
    {
        gui_mainmenu.SetActive(true);
    }

    public void closeGUI_MainMenu()
    {
        gui_mainmenu.SetActive(false);
    }

    public void onClick_Play()
    {
        //SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
        unpause();
        closeGUI_MainMenu();
        closeGUI_Upgrades();
        openGUI_Game(); 
        main.onNewGame();
    }

    public void onClick_Options()
    {
        Debug.Log("Clicked Options");
        panel_options.SetActive(true);
    }

    public void onClick_Reset()
    {
        Debug.Log("Clicked Reset");
    }


    public void onClick_Back()
    {
        panel_options.SetActive(false);
    }









    //GAME GUI
    public void openGUI_Game()
    {
        gui_game.SetActive(true);
    }

    public void closeGUI_Game()
    {
        gui_game.SetActive(false);
    }

    public void onClick_Pause()
    {
        if(main.isPaused){
            closePause();
            unpause(); 
        }
        else{
            openPause();
            pause();
        }
    }

    public void pause()
    {
        main.isPaused = true;
        Time.timeScale = 0;          
        //GameObject.SetActive(false);
        //TODO: Disable other scripts
    }

    public void unpause()
    {
        main.isPaused = false; 
        Time.timeScale = 1;  
        //GameObject.SetActive(true);
        //TODO: Enable other scripts
    }

    public void openPause()
    {
        panel_pause.SetActive(true);
    }

    public void closePause()
    {
        panel_pause.SetActive(false);
    }


    public void openSummary()
    {
        this.panel_summary.SetActive(true);
    }

    public void closeSummary()
    {
        this.panel_summary.SetActive(false);
    }

    public void onClick_NextWave()
    {
        main.wave_number++;
        main.resetPlayer();
        main.generateWave();
        closeSummary();
        closeGUI_MainMenu();
        closeGUI_Upgrades();
        openGUI_Game();
        unpause();
    }

    public void onClick_Upgrade()
    {
        closeSummary();
        closeGUI_Game();
        openGUI_Upgrades();
    }


    public void openGameOver()
    {
        this.panel_gameover.SetActive(true);
    }

    public void closeGameOver()
    {
        this.panel_gameover.SetActive(false);
    }

    public void onClick_AdContinue()
    {
        //TODO: ad video, reward with revival at current wave, continue to upgrades before starting wave

    }

    public void onClick_RestartNew()
    {
        main.clearGame();
        closeGameOver();
        unpause();
        main.onNewGame();
    }





    //UPGRADES GUI
    public void openGUI_Upgrades()
    {
        gui_upgrades.SetActive(true);
    }

    public void closeGUI_Upgrades()
    {
        gui_upgrades.SetActive(false);
    }

    public void onClick_WatchAd()
    {

    }

    public void onClick_UpgradeItem()
    {
        GameObject clicker = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        UpgradeItem upitem = clicker.transform.parent.GetComponent<UpgradeItem>();
        main.upgrade(upitem);
    }

    
}
