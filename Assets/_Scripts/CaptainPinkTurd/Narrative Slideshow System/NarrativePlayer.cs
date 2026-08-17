using CaptainPinkTurd.Core.Utilities;
using CaptainPinkTurd.Input;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CaptainPinkTurd.NarrativeSlideshow
{
    public class NarrativePlayer : MonoBehaviour
    {
        [SerializeField] private Image displayImage;
        [SerializeField] private TypewriterText  typewriterText;
        [SerializeField] private StorySequence sequence;
        
        private InputSystemActions playerInput;
        private int currentIndex = 0;
        private bool isFading;
        private bool slideCurrentImage;
        private bool isSliding;

        private void Awake()
        {
            playerInput = new InputSystemActions();
        }

        private void OnEnable()
        {
            playerInput.Player.Enable();
            playerInput.Player.Confirm.performed += Next;
        }

        private void OnDisable()
        {
            playerInput.Player.Confirm.performed -= Next;
            playerInput.Player.Disable();
        }

        private void OnDestroy()
        {
            playerInput.Dispose();
        }

        private void Start()
        {
            Play(sequence);
        }

        public void Play(StorySequence seq)
        {
            sequence = seq;
            currentIndex = 0;
            ShowPanel();
        }

        private void ShowPanel()
        {
            var panel = sequence.panels[currentIndex];
            displayImage.sprite = panel.image;
            displayImage.SetNativeSize();
            slideCurrentImage = panel.slidingImage;
            displayImage.transform.localPosition = panel.slidingImage ? panel.slideFromLocalPos : Vector3.zero;
            
            //if sprite is null then show a black screen
            isFading = true;
            displayImage.DOFade(displayImage.sprite ? 1 : 0, panel.fadeInTime).OnComplete(() =>
            {
                isFading = false;
            });
            typewriterText.StartTyping("Default", panel.text, panel.alignment);
        }

        public void Next(InputAction.CallbackContext ctx)
        {
            if (isFading || currentIndex >= sequence.panels.Count) return;
            
            var panel = sequence.panels[currentIndex];
            if(typewriterText.IsTyping)
            {
                typewriterText.SkipTyping();
            }
            else if (panel.slidingImage && slideCurrentImage)
            {
                if (!isSliding)
                {
                    isSliding = true;
                    displayImage.transform.DOLocalMove(panel.slideToLocalPos, panel.slideDuration).OnComplete(() =>
                    {
                        isSliding = false;
                        slideCurrentImage = false;
                    });
                }
                else
                {
                    DOTween.Kill(displayImage.transform, true);
                }
            }
            else 
            {
                typewriterText.SetTextUIActive(!panel.disableTextOnFadeOut);
                
                isFading = true;
                displayImage.DOFade(0, panel.fadeOutTime).OnComplete(() =>
                {
                    isFading = false;
                    currentIndex++;

                    if (currentIndex >= sequence.panels.Count)
                    {
                        EndSequenceEvents();
                        return;
                    }
                    
                    ShowPanel();
                });
            }
        }

        private void EndSequenceEvents()
        {
            //To be implemented later
        }
    }
}