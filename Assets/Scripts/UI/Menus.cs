using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI, DeathMenuUI, SettingsMenuUI, MainMenuUI;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Play() {
        SceneManager.LoadScene("TestScene");
    }

    public void RetryLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Pause() {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume() {
        // PauseMenuUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenu() {
        SceneManager.LoadScene("Main Menu");
    }

    public void SettingsMenu() {
        MainMenuUI.SetActive(false);
        SettingsMenuUI.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
