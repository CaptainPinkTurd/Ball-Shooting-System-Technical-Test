using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    [CreateAssetMenu(fileName = "StringEvent", menuName = "Scriptable Objects/SOAP/GameEventSO/String Event", order = 4)]
    public class StringEvent : GameEventSO<string> { }
}