using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

public static class SaveLoad
{
    public static void SaveData(GameData gameData, string filePath)
    {
        // Debug.Log(JsonUtility.ToJson(gameData));
        try
        {
            File.WriteAllText(filePath, JsonUtility.ToJson(gameData));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void LoadData(out GameData gameData, string filePath)
    {
        gameData = null;


        if (filePath == string.Empty)
        {
            Guid g = Guid.NewGuid();
            filePath = Path.Combine(Application.persistentDataPath, $"savegame{g}.thelight");

            GameManager.Instance.SetPath(filePath);

            gameData = MakeNewSave(new GameData(), filePath);

            return;
        }

        try
        {
            gameData = JsonUtility.FromJson<GameData>(File.ReadAllText(filePath));

            GameManager.Instance.SetPath(filePath);
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to load {filePath} with: {e.Message}");
        }
    }


    public static void DeleteFile([CanBeNull] string filePath)
    {
        if (File.Exists(@filePath) && filePath != null)
        {
            File.Delete(@filePath);
        }
    }

    public static GameData MakeNewSave(GameData playerData, string filePath)
    {
        try
        {
            File.WriteAllText(filePath, JsonUtility.ToJson(playerData));
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return playerData;
    }

    public static Dictionary<string, DateTime> GetSaveGamesFromDirectory()
    {
        IEnumerable<string> lightFiles = new List<string>();

        Dictionary<string, DateTime> fileDictionary = new Dictionary<string, DateTime>();
        try
        {
            string sourceDirectory = @Application.persistentDataPath;

            lightFiles =
                Directory.EnumerateFiles(
                    sourceDirectory,
                    "*.thelight"
                );

            foreach (string tx in lightFiles)
            {
                FileStream f = new FileStream(tx, FileMode.Open);
                fileDictionary.Add(tx, GetInFo(tx));
                f.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not get files: {e.Message}");
        }

        return fileDictionary;
    }

    public static DateTime GetInFo(string file)
    {
        DateTime writeTime = new DateTime();

        if (File.Exists(file))
        {
            writeTime = File.GetLastWriteTime(file);
        }
        else
        {
            Console.WriteLine(
                "Could not find the file: {0}.", file);
        }

        return writeTime;
    }
}