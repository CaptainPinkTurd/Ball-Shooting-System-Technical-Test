using CaptainPinkTurd.Core.Enum;
using UnityEngine;

namespace CaptainPinkTurd.Core.DesignPattern.SOAP.Events
{
    [CreateAssetMenu(fileName = "Enum Color Event", menuName = "Scriptable Objects/SOAP/GameEventSO/Enum Color Event", order = 10)]
    public class EnumColorEvent : GameEventSO<EColor> { }
}