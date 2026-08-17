using CaptainPinkTurd.AudioSystem;
using CaptainPinkTurd.Core.Utils;
using UnityEngine;
using CaptainPinkTurd.Core.CustomDataStructure;

namespace CaptainPinkTurd.Scene.Manager
{
    public class CoreManager : MonoBehaviour
    {
        [SerializeField] private SceneReference sceneToLoad;
        [SerializeField] private CanvasGroup loadingOverlayCanvasGroup;

        private void Awake()
        {
            loadingOverlayCanvasGroup.alpha = 0;
        }

        private void Start()
        {
            //Core Setup for the game
            //Load everything like SoundManager, Save System,...
            StartCoroutine(CoroutineUtils.WaitForCondition(() => SoundManager.Instance.didAwake,
                () =>
                {
                    SceneController.Instance
                        .NewTransition()
                        .Load(SceneDatabase.Slots.SessionContent, sceneToLoad.SceneName, true)
                        .WithOverlay()
                        .Perform();
                }));
        }
    }
}