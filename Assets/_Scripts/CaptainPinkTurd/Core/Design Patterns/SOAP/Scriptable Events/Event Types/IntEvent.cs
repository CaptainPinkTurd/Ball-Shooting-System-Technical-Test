using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    [CreateAssetMenu(fileName = "IntEvent", menuName = "Scriptable Objects/SOAP/GameEventSO/Int Event", order = 1)]
    public class IntEvent : GameEventSO<int> { }
}