using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFrenzy
{
    public class AudioManager : MonoBehaviour
    {
        protected static AudioManager instance;
        // General sounds
        public AudioSource characterWalking;
        public AudioSource events;
        public AudioSource communication;
        public AudioSource alarm;
        public AudioSource win;
        public AudioSource gameOver;

        // Module specific
        public AudioSource moduleFull;
        public AudioSource moduleRepairs;
        public AudioSource moduleStopProduction;
        public AudioSource moduleUpdate;
        public AudioSource modulePotato;
        public AudioSource moduleWater;
        public AudioSource moduleElectricity;

        private Dictionary<string, AudioSource> sounds;

        // Use this for initialization
        void Start()
        {
            instance = this;
            sounds = new Dictionary<string, AudioSource>();

            sounds.Add("characterWalking", characterWalking);
            sounds.Add("events", events);
            sounds.Add("communication", communication);
            sounds.Add("alarm", alarm);
            sounds.Add("win", win);
            sounds.Add("gameOver", gameOver);

            // Module specific
            sounds.Add("moduleFull", moduleFull);
            sounds.Add("moduleRepairs", moduleRepairs);
            sounds.Add("moduleStopProduction", moduleStopProduction);
            sounds.Add("moduleUpdate", moduleUpdate);
            sounds.Add("modulePotato", modulePotato);
            sounds.Add("moduleWater", moduleWater);
            sounds.Add("moduleElectricity", moduleElectricity);
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
            }
        }

        public void StopSound(string _name)
        {

        }
    }

}