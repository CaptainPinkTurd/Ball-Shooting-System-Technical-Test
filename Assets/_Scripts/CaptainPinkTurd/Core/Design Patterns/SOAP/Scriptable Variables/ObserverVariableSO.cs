using CaptainPinkTurd.Core.SO;
using UnityEngine;
using UnityEngine.Events;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Variables
{
    public abstract class ObserverVariableSO<T> : RuntimeScriptableObject
    {
        [SerializeField] private T initialValue;
        [SerializeField] private T value;
        
        public event UnityAction<T> OnValueChanged = delegate { }; 
        
        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;
                
                this.value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        protected override void OnReset()
        {
            OnValueChanged = delegate { };
            value = initialValue;
        }
    }
}