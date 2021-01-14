using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    public event Action turnOnSpawner;
    public void OnButtonInteracted() {
        if (turnOnSpawner != null) turnOnSpawner();
    }

    public event Action turnOffSpawner;
    public void OffButtonInteracted() {
        if (turnOffSpawner != null) turnOffSpawner();
    }

    
}
