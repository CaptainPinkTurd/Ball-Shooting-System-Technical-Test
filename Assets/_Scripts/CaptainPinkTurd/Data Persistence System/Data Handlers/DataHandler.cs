using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace CaptainPinkTurd.DataPersistence.DataHandlers
{
    /// <summary>
    /// Base class for all data handlers (file-based, WebGL/PlayerPrefs-based, cloud, etc).
    /// DataPersistenceManager only ever talks to this abstraction, so swapping platforms
    /// is just a matter of instantiating a different subclass.
    /// </summary>
    public abstract class DataHandler
    {
        protected string dataDirPath;
        protected string dataFileName;
        protected bool useEncryption;
        protected readonly string encryptionCodeWord = "Super Secret Encrypted Code Word";
        
        //global dictionary that contains the data for EVERY object in the game
        //even the one that's NOT currently loaded, will load up all of its values initially in the Load method at the start of the game
        private Dictionary<string, string> dataDict = new();
        private bool initialLoad = false;

        protected DataHandler(string dataFileName, bool useEncryption)
        {
            this.dataFileName = dataFileName;
            this.useEncryption = useEncryption;
        }

        public void ClearDataDictionary() => dataDict.Clear();
        public void Save(List<IDataPersistence> dataPersistenceObjects, string profileId)
        {
            //use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            try
            {
                // create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? "Null Directory Name");

                foreach (var persistenceObject in dataPersistenceObjects)
                {
                    // Serialize the C# game data object into Json
                    string jsonData = JsonConvert.SerializeObject(persistenceObject.SaveData());
                    //Debug.Log($"Save {persistenceObject.Name} save data: {persistenceObject.SaveData()} to {jsonData}");
                    dataDict[persistenceObject.Name] = jsonData;
                }

                SaveWrapper wrapper = new SaveWrapper { saveDataDictionary = dataDict };
                string dataToStore = JsonConvert.SerializeObject
                (
                    wrapper,
                    Formatting.Indented
                );
                
                if (useEncryption)
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                // write the serialized data to the file
                // use using to ensure the connection to that file is close when we're done reading or writing to it
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
        public void Load(List<IDataPersistence> dataPersistenceObjects, string profileId)
        {
            //use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (File.Exists(fullPath))
            {
                try
                {
                    //load the serialized data from the file
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    if (useEncryption)
                    {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }

                    //deserialize the data from Json back into the C# object
                    SaveWrapper saveWrapper = JsonConvert.DeserializeObject<SaveWrapper>(dataToLoad);
                    if(!initialLoad)
                    {
                        initialLoad = true;
                        dataDict = new Dictionary<string, string>(saveWrapper.saveDataDictionary);
                        //Debug.Log("Data Dict now loaded with " + dataDict.Count + " elements");
                    }

                    foreach (var persistenceObject in dataPersistenceObjects)
                    {
                        if (saveWrapper.saveDataDictionary == null) return;
                        if (!saveWrapper.saveDataDictionary.TryGetValue(persistenceObject.Name, out var jsonData)) return;
                        
                        persistenceObject.LoadData(JsonConvert.DeserializeObject(
                            jsonData, persistenceObject.SaveData().GetType()));
                    }
                }
                catch (Exception e)
                {
                    //persistenceObject.SaveData() MIGHT be null if encounter null reference exception here
                    Debug.LogError("Error occured when trying to load data to file: " + fullPath + "\n" + e);
                }
            }
            else
            {
                Debug.LogWarning("No save file found.");
            }
        }

        // public Dictionary<string, GameData> LoadAllProfiles()
        // {
        //     var profileDict = new Dictionary<string, GameData>();
        //
        //     IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        //     foreach (var dirInfo in dirInfos)
        //     {
        //         string profileId = dirInfo.Name;
        //         string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        //         if (!File.Exists(fullPath))
        //         {
        //             Debug.LogWarning("Save profile does not exist at ID: " + profileId + "\nSkip instead");
        //             continue;
        //         }
        //
        //         GameData profileData = Load(profileId);
        //         if (profileData != null)
        //         {
        //             profileDict.Add(profileId, profileData);
        //         }
        //         else
        //         {
        //             Debug.LogError("Tried to load profile but something went wrong. ProfileID " + profileId);
        //         }
        //     }
        //
        //     return profileDict;
        // }

        /// <summary>
        /// Simple XOR encryption, shared by every handler implementation.
        /// </summary>
        protected string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }
            return modifiedData;
        }
    }
}