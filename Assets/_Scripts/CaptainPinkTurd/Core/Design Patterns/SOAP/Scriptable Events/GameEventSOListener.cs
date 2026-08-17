using UnityEngine;
using UnityEngine.Events;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    public class GameEventSOListener<T> : MonoBehaviour, IGameEventSOListener<T>
    {
        [SerializeField] private GameEventSO<T> gameEvent;
        [SerializeField] private UnityEvent<T> response;

        private void OnEnable() => gameEvent.Subscribe(this);
        private void OnDisable() => gameEvent.Unsubscribe(this);

        public void OnEventRaised(T data) => response.Invoke(data);
    }
}