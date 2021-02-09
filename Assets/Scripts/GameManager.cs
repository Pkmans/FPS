using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
   
    //menu UI
    public GameObject deathMenuUI;
    private bool GameOver = false;

    //UI damage feedback
    public GameObject dmgCrosshair, dethCrosshair;

    //settings carried over from main menu
    public float sensitivity;

    void Awake() {
        if (instance != null) {
            GameObject.Destroy(instance);
            GameObject.Destroy(instance.gameObject);
        }
        else
            instance = this;

        DontDestroyOnLoad(this);
    }

    public IEnumerator damageCrosshair() {
        print("func called");
        dmgCrosshair.SetActive(true);
        print("crosshair active is : " + dmgCrosshair.activeSelf);
        yield return new WaitForSeconds(0.2f);
        dmgCrosshair.SetActive(false);
    }

    public IEnumerator deathCrosshair() {
        dethCrosshair.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        dethCrosshair.SetActive(false);
    }

    public void PlayerDeath() {
        if (GameOver) return;

        deathMenuUI.SetActive(true);
        FindObjectOfType<FPS_Camera>().enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameOver = true;
    }
}
