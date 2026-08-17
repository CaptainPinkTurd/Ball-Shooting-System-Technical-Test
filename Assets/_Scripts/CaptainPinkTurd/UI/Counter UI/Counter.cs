using TMPro;
using UnityEngine;

namespace CaptainPinkTurd.UI
{
    public class Counter : MonoBehaviour
    {
        private TMP_Text counterText;
        private int counterValue;

        public int CounterValue => counterValue;

        private void Awake()
        {
            counterText = GetComponentInChildren<TMP_Text>();
            
            ResetCounterValue();
        }
        internal void AddCounterValue(int amount)
        {
            counterValue += amount;
            counterText.text = counterValue.ToString();
        }
        internal void SetCounterValue(int value)
        {
            counterValue = value;
            counterText.text = counterValue.ToString();
        }
        internal void ResetCounterValue() => SetCounterValue(0);
    }
}
