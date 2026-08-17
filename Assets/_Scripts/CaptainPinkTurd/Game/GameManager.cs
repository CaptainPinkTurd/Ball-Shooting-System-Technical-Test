using System;
using CaptainPinkTurd.Core;
using UnityEngine;
using CaptainPinkTurd.Core.DesignPattern.Singleton;
using CaptainPinkTurd.Core.DesignPattern.SOAP.Events;
using CaptainPinkTurd.DataPersistence;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CaptainPinkTurd.Game
{
    public class GameManager : Singleton<GameManager>, IDataPersistence
    {
        [Header("GameManager Properties")]
        [SerializeField] private float gameFastForwardSpeed = 2;
        [SerializeField] private VoidEvent onGameOver;
        [SerializeField] private VoidEvent onGameOverWin;

        internal bool hasDoneTutorial;
        
        public GameEvent OnGameOver = new GameEvent();

        //public readonly GameFlowController Flow = new GameFlowController();
        private float oldTimeScale;

        public string Name => name;

        private void OnEnable()
        {
            OnGameOver.Subscribe(OnGameOverEvents);
        }
        private void OnDisable()
        {
            OnGameOver.Unsubscribe(OnGameOverEvents);
        }

        private void OnGameOverEvents()
        {
            onGameOver.Raise();
        }
        public void SceneReset()
        {
            Debug.Log($"Reset scene: {SceneManager.GetActiveScene().name}");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SetTimeScale(1); //in case Timescale got set to something else when scene reset
        }

        public void SetTimeScale(float timeScale)
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale = timeScale;
        }
        public void ResetTimeScale()
        {
            Time.timeScale = oldTimeScale;
        }
        
        public void LoadData(object data)
        {
            hasDoneTutorial = (bool)data;
        }
        public object SaveData()
        {
            return hasDoneTutorial;
        }
        public void QuitGame()
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.ExitPlaymode();
                return;
            }
            #endif
            
            Application.Quit();
        }

        #region GAME SPECIFIC REGION

        //[Header("Game Specific Variables")]
        public void OnLevelSceneLoaded()
        {
            
        }
        public void OnGameRestartEvents()
        {
            
        }

        #endregion
    }
}