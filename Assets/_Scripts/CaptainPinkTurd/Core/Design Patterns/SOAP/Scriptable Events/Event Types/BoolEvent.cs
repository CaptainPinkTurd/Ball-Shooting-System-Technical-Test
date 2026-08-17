using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    [CreateAssetMenu(fileName = "BoolEvent", menuName = "Scriptable Objects/SOAP/GameEventSO/Bool Event", order = 3)]
    public class BoolEvent : GameEventSO<bool> { }
}