using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private SceneReference gameContentScene;
        [SerializeField] private AudioClip menuMusic;
        
        private void Start()
        {
            MusicManager.Instance.Play(menuMusic, loop: true);
        }

        public void StartSession()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, gameContentScene.SceneName, true)
                .Unload(SceneDatabase.Slots.Menu)
                .WithOverlay()
                .Perform();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}