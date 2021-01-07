using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start() {
        Play("BGM");
    }

   public void Play (string name) {
       foreach (Sound s in sounds) {
           if (s.name == name) {
               s.source.Play();
               return;
           }
       }
   } 

   public void Stop(string name) {
       foreach (Sound s in sounds) {
           if (s.name == name) {
               s.source.Stop();
               return;
           }
       }
   }
}
