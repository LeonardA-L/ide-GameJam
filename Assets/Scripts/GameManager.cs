using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace MarsFrenzy
{
    public class GameManager : MonoBehaviour
    {
        protected static readonly GameManager instance = new GameManager();
        public GameDataModel data;

        // Use this for initialization
        void Start()
        {
            Debug.Log("Init GameManager");
            data = LoadGameData("gamedata.json");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }

        private GameDataModel LoadGameData(string path)
        {
            // Path.Combine combines strings into a file path
            // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
            string filePath = Path.Combine(Application.streamingAssetsPath, path);

            if (File.Exists(filePath))
            {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath);
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                GameDataModel loadedData = JsonUtility.FromJson<GameDataModel>(dataAsJson);

                return loadedData;
            }
            else
            {
                Debug.LogError("Cannot load game data!");
                throw new System.Exception("Cannot load data");
            }
        }
    }

}