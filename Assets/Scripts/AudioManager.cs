using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class AudioManager : MonoBehaviour
    {
        protected static AudioManager instance;
        private Dictionary<string, AudioSource> sounds;

        // Use this for initialization
        void Start()
        {
            instance = this;
            sounds = new Dictionary<string, AudioSource>();

            GameObject[] soundObjects = GameObject.FindGameObjectsWithTag("Sound");

            foreach (GameObject sound in soundObjects)
            {
                AudioSource source = sound.GetComponent<AudioSource>();
                if(source != null)
                {
                    sounds.Add(sound.name, source);
                }
            }
        }

        public static AudioManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void PlaySound(string _name)
        {
            AudioSource source = sounds[_name];
            if(source)
            {
                source.Play();
            } else
            {
                throw new System.Exception("Audio Source not found");
            }
        }

        public void StopSound(string _name)
        {
            AudioSource source = sounds[_name];
            if (source)
            {
                source.Stop();
            }
            else
            {
                throw new System.Exception("Audio Source not found");
            }
        }
    }

}