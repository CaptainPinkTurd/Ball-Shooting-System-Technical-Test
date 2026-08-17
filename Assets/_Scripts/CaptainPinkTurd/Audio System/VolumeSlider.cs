using UnityEngine;
using UnityEngine.UI;

namespace CaptainPinkTurd.AudioSystem
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private EVolumeType volumeType;
        
        private Slider slider;
        
        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.wholeNumbers = false;
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
        }
        private void OnEnable()
        {
            slider.onValueChanged.AddListener(OnValueChanged);
        }
        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }
        private void Start()
        {
            switch (volumeType)
            {
                case EVolumeType.Master:
                    slider.value = AudioManager.Instance.GetMasterVolume();
                    break;
                case EVolumeType.Music:
                    slider.value = AudioManager.Instance.GetMusicVolume();
                    break;
                case EVolumeType.SFX:
                    slider.value = AudioManager.Instance.GetSFXVolume();
                    break;
            }
        }

        private void OnValueChanged(float value)
        {
            AudioManager.Instance.SetVolume(volumeType, value);
        }
    }
}