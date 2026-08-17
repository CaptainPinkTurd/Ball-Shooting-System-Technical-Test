using UnityEngine;

namespace CaptainPinkTurd.Core.Attributes
{
    public class InlineScriptableObjectAttribute : PropertyAttribute
    {
        public bool AllowCreate;

        public InlineScriptableObjectAttribute(bool allowCreate = true)
        {
            AllowCreate = allowCreate;
        }
    }
}
