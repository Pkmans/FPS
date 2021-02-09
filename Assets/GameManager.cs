using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
   
    public GameObject deathMenuUI;
    private bool GameOver = false;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
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
