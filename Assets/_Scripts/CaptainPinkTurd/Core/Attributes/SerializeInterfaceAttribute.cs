using UnityEngine;

namespace CaptainPinkTurd.Core.Attributes
{
    public class SerializeInterfaceAttribute : PropertyAttribute
    {
        public System.Type TargetType;
    
        public SerializeInterfaceAttribute(System.Type targetType)
        {
            TargetType = targetType;
        }
    }
}
