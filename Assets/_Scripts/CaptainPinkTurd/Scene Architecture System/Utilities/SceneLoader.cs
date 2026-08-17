using CaptainPinkTurd.Core.CustomDataStructure;
using UnityEngine;

namespace CaptainPinkTurd.Scene.Utilities
{
    //Simple utility class for loading scene
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene Loader Configs")]
        [SerializeField] private SceneReference sceneToLoad;
        
        public void LoadScene()
        {
            SceneController.Instance
                .NewTransition()
                .Load(SceneDatabase.Slots.SessionContent, sceneToLoad.SceneName, true)
                .WithOverlay()
                .Perform();
        }
    }
}