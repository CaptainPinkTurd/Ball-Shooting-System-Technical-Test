using System;

namespace CaptainPinkTurd.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public readonly string Label;
    
        public ButtonAttribute(string label = null)
        {
            Label = label;
        }
    }
}