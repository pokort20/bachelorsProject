using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveLoadSystem
{
    public static bool SaveGame(SceneTransitionData sceneData, string saveName)
    {
        bool success;
        try
        {
            SaveData saveData = new SaveData(sceneData);
            BinaryFormatter bf = new BinaryFormatter();
            string path = Application.dataPath + "/" + saveName + ".sejv";
            FileStream fs = new FileStream(path, FileMode.Create);
            bf.Serialize(fs, saveData);
            fs.Close();
            success = true;
        }
        catch(Exception e)
        {
            Debug.LogError("Saving the game FAILED! " + e.Message);
            success = false;
        }
        return success;
    }
    public static SceneTransitionData LoadGame(string saveName)
    {
        SceneTransitionData sceneData;
        SaveData saveData;
        try
        {

            string path = Application.dataPath + "/" + saveName + ".sejv";
            if(File.Exists(path))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(path, FileMode.Open);
                saveData = bf.Deserialize(fs) as SaveData;
                fs.Close();
            }
            else
            {
                Debug.LogError("Loading the game FAILED, save file not found!");
                return null;
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Loading the game FAILED! " + e.Message);
            return null;
        }
        if(saveData != null)
        {
            GameObject sceneDataObject = new GameObject("SceneData");
            sceneDataObject.AddComponent<SceneTransitionData>();
            sceneData = sceneDataObject.GetComponent<SceneTransitionData>();
            sceneData.health = saveData.health;
            sceneData.stamina = saveData.stamina;
            sceneData.batteryLevel = saveData.batteryLevel;
            sceneData.flashlightEnabled = saveData.flashlightEnabled;
            sceneData.inventoryItems = new List<Item>();
            foreach(string s in saveData.items)
            {
                GameObject gameObject = new GameObject(s);
                Debug.LogWarning("ITEM: " + s);
                switch(s)
                {
                    case "flashlight":
                        gameObject.AddComponent<FlashlightItem>();
                        sceneData.inventoryItems.Add(gameObject.GetComponent<FlashlightItem>());
                        break;
                    case "batteries":
                        gameObject.AddComponent<BatteryItem>();
                        sceneData.inventoryItems.Add(gameObject.GetComponent<BatteryItem>());
                        break;
                    case "medkit":
                        gameObject.AddComponent<MedkitItem>();
                        sceneData.inventoryItems.Add(gameObject.GetComponent<MedkitItem>());
                        break;
                    case "staminaShot":
                        gameObject.AddComponent<StaminaShotItem>();
                        sceneData.inventoryItems.Add(gameObject.GetComponent<StaminaShotItem>());
                        break;
                    case "key":
                        gameObject.AddComponent<KeyItem>();
                        sceneData.inventoryItems.Add(gameObject.GetComponent<KeyItem>());
                        break;
                    case "securityCard":
                        gameObject.AddComponent<SecurityCardItem>();
                        sceneData.inventoryItems.Add(gameObject.GetComponent<SecurityCardItem>());
                        break;
                    default:
                        Debug.LogError("wrong item name while loading from save");
                        break;
                }
            }
        }
        else
        {
            return null;
        }

        return sceneData;
    }
}
