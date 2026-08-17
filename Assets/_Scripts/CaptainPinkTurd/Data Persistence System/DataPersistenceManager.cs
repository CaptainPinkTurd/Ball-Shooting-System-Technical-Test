using System.Collections.Generic;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.DataPersistence.DataHandlers;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZLinq;

#if  UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace CaptainPinkTurd.DataPersistence
{
    public class DataPersistenceManager : Singleton<DataPersistenceManager>
    {
        [Header("File Storage Config")] 
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;

        [Header("Debug")] 
        [SerializeField] private bool initializeDataIfNull;
        
        private List<IDataPersistence> dataPersistenceObjects;
        private DataHandler dataHandler;
        
        private string selectedProfileId = "Default"; //for multiple save slots, haven't implemented yet

        protected override void Awake()
        {
            base.Awake();

#if UNITY_WEBGL && !UNITY_EDITOR
            dataHandler = new WebGLDataHandler(fileName, useEncryption);
#else
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
#endif
        }
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            var dataPersistenceObjects =
                FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    .AsValueEnumerable().OfType<IDataPersistence>().ToList();

            dataPersistenceObjects.AddRange(
                Resources.FindObjectsOfTypeAll<ScriptableObject>()
                    .AsValueEnumerable().OfType<IDataPersistence>().ToList());

            return dataPersistenceObjects;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
        
        public void NewGame()
        {
            dataHandler.ClearDataDictionary();
            dataHandler.Save(new List<IDataPersistence>(), selectedProfileId);
        }
        public void LoadGame()
        {
            dataHandler.Load(dataPersistenceObjects, selectedProfileId);
        }
        public void SaveGame()
        {
            //save that data to a file using the data handler
            dataHandler.Save(dataPersistenceObjects, selectedProfileId);
        }

        public void ChangeSelectedProfileId(string newProfileId)
        {
            selectedProfileId = newProfileId;
            
            //load the game, which will use that profile, updating our game data accordingly 
            LoadGame();
        }
        // public Dictionary<string, GameData> GetAllProfilesGameData()
        // {
        //     return dataHandler.LoadAllProfiles();
        // }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
        
#if UNITY_EDITOR
        [Button("Open Save File Folder Location")]
        private void OpenSaveFileFolderLocation()
        {
            // Opens the folder or highlights the specified item native to the OS
            EditorUtility.RevealInFinder(Path.Combine(Application.persistentDataPath)); 
        }
#endif
    }
}