using System.Collections.Generic;
using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    public abstract class GameEventSO<T> : ScriptableObject
    {
        private readonly List<IGameEventSOListener<T>> listeners = new();

        public void Raise(T data)
        {
            //We're going backward in the loop in case one of the reactions of those listeners is to unsubscribe or destroy itself
            for(int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(data);
            }
        }
        
        public void Subscribe(IGameEventSOListener<T> listener) => listeners.Add(listener);
        public void Unsubscribe(IGameEventSOListener<T> listener) => listeners.Remove(listener);
    }
}