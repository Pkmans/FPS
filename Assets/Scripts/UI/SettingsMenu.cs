﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject MainMenuUI, SettingsMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetSensitivity(float sensitivity) {
        GameManager.instance.sensitivity = sensitivity;
    }

    public void Back() {
        SettingsMenuUI.SetActive(false);
        MainMenuUI.SetActive(true);
    }

    
}
