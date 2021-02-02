using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonController : MonoBehaviour
{
    public GameObject spawner;
    public bool turnOn;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.current.turnOnSpawner += turnOnSpawner;
        GameEvents.current.turnOffSpawner += turnOffSpawner;

    }

    private void turnOnSpawner() {
        spawner.SetActive(true);
    }

    private void turnOffSpawner() {
        spawner.SetActive(false);
        spawner.GetComponent<Spawner>().removeAllEnemies();
    }
    
    

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag != "PlayerProjectile") return;

        if (turnOn) GameEvents.current.OnButtonInteracted(); 
        else GameEvents.current.OffButtonInteracted();
    }
}
