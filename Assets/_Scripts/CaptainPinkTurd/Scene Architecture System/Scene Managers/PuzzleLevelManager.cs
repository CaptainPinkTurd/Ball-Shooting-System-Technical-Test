using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Base;
using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class PuzzleLevelManager : GameObjectBase
    {
        [Header("Level Configs")]
        [SerializeField] private SceneReference mainMenuScene;
        [SerializeField] private SceneReference zoomOutScene;
        
        public void PlayLevelTheme(AudioClip levelTheme)
        {
            MusicManager.Instance.Play(levelTheme, loop: true);
        }

        public void ZoomOut()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, zoomOutScene.SceneName, true)
                .WithClearUnusedAssets()
                .WithOverlay()
                .Perform();
        }
        
        public void EndSession()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.Menu, mainMenuScene.SceneName, true)
                .Unload(SceneDatabase.Slots.Session)
                .Unload(SceneDatabase.Slots.SessionContent)
                .WithClearUnusedAssets()
                .WithOverlay()
                .Perform();
        }
    }
}