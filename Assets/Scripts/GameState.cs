﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace MarsFrenzy
{
    [System.Serializable]
    public class GameState
    {
        // IdleWorks
        public IdleWorks.Clock m_clock = new IdleWorks.Clock();
        public IdleWorks.StorageManager m_storageManager = new IdleWorks.StorageManager();
        public IdleWorks.GeneratorManager m_generatorManager = new IdleWorks.GeneratorManager();
        public IdleWorks.MultiplierManager m_multiplierManager = new IdleWorks.MultiplierManager();

        // MarsFrenzy
        public FactsManager m_factsManager = new FactsManager();

        public float playerHungerStart = 100.0f;
        public float playerThirstStart = 100.0f;
        public float starvationDecay = 10.0f;
        public float playerRegen = 1.0f;

        public float stormDamage = 0.0f;
        public int stormDuration = 10;

        public bool newGame = true;


        // ------------------------------

        public IdleWorks.Clock GetClock()
        {
            return m_clock;
        }

        public void Init()
        {
            // IdleWorks
            m_storageManager.Init();
            m_generatorManager.Init();
            m_multiplierManager.Init();
            m_clock.Init();

            // MarsFrenzy
            m_factsManager.Init();
        }

        public void Save(string _path = Constants.SAVE_PATH)
        {
            string savePath = Application.persistentDataPath + _path;
            newGame = false;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(savePath);

            bf.Serialize(file, this);
            file.Close();

            Debug.Log("State saved");
        }

        public static GameState Load(string _path, bool _failOnError = false)
        {
            string savePath = Application.persistentDataPath + _path;
            Debug.Log(savePath);
            if (!File.Exists(savePath))
            {
                Debug.LogError("Savegame doesn't exist");
                if (_failOnError)
                    throw new FileNotFoundException();
                GameState newGameState = new GameState();
                newGameState.Init();
                return newGameState;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);

            GameState save = (GameState)bf.Deserialize(file);
            file.Close();

            if (save == null)
            {
                Debug.LogError("Savegame doesn't exist");
                if (_failOnError)
                    throw new FormatException();
                GameState newGameState = new GameState();
                newGameState.Init();
                return newGameState;
            }

            save.Init();
            return save;
        }

        public static void Save(GameState _state, string _path)
        {
            _state.Save(_path);
        }
    }
}