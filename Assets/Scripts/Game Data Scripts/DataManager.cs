using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public string scoreData = "/ScoreData.dat";

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public int GetScore()
    {
        string dataPath = Application.persistentDataPath + scoreData;
        int highscore = 0;

        if (File.Exists(dataPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(dataPath, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;

            highscore = gameData.highscore;
            stream.Close();
        }
        else
        {
            SetScore(0);
        }

        return highscore;
    }

    public void SetScore(int score)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string dataPath = Application.persistentDataPath + scoreData;

        FileStream stream = new FileStream(dataPath, FileMode.Create);

        GameData gameData = new GameData();
        gameData.highscore = score;

        formatter.Serialize(stream, gameData);
        stream.Close();
    }

} // class
