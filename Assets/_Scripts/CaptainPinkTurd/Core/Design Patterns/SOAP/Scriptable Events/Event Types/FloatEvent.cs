using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    [CreateAssetMenu(fileName = "FloatEvent", menuName = "Scriptable Objects/SOAP/GameEventSO/Float Event", order = 2)]
    public class FloatEvent : GameEventSO<float> { }
}