using CaptainPinkTurd.Core.DesignPattern.Singleton;
using UnityEngine;

namespace CaptainPinkTurd.UI
{
    public class UiManager : Singleton<UiManager>
    {
        [SerializeField] private Counter killCounter;

        public void AddKillCount()
        {
            killCounter?.AddCounterValue(1);
        }

        public int GetKillCount() => killCounter.CounterValue;
    }
}