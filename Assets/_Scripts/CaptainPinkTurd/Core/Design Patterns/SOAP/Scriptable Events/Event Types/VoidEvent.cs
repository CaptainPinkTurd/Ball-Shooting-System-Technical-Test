using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    //For parameterless events, use Unit as the type
    [CreateAssetMenu(fileName = "VoidEvent", menuName = "Scriptable Objects/SOAP/GameEventSO/Void Event", order = 0)]
    public class VoidEvent : GameEventSO<Unit> 
    {
        public void Raise() => Raise(Unit.Default);
    }
}