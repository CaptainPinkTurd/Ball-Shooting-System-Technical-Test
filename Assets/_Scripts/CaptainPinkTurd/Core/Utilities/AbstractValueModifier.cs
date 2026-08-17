using System;
using UnityEngine;
using System.Reflection;
using CaptainPinkTurd.Core.Attributes;
using CaptainPinkTurd.Core.Exceptions;
using CaptainPinkTurd.Core.Interfaces;

namespace CaptainPinkTurd.Core.Utilities
{
    public abstract class AbstractValueModifier<TTarget> : IModifiable<TTarget>
    {
        [Header("Abstract Value Modifier Configuration")]
        public bool RequireAttributePath = true;
        
        [ShowIf(nameof(RequireAttributePath))]
        public string AttributeName;
        
        public abstract void Modify(TTarget targetType);
        public abstract void Modify(object target);
        
        protected TFieldType GetAttribute<TFieldType>(TTarget targetType,
            out object targetObject, out FieldInfo field)
        {
            //Using AttributeName = "shootConfig/spread" (paths = { "shootConfig", "spread" }) for example
            string[] paths = AttributeName.Split('/');
            string attribute = paths[paths.Length - 1]; //in case we're more than 1 level deep
            
            Type type = targetType.GetType();
            object target = targetType;
            
            for (int i = 0; i < paths.Length - 1; i++)
            {
                //look for the field inside the given type. For example, shootConfig is a field inside GunInfo"
                FieldInfo fieldInfo = type.GetField(paths[i], BindingFlags.Public 
                          | BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo == null)
                {
                    Debug.LogError($"Unable to apply modifier to attribute {AttributeName}" +
                        $" because it does not exist on {targetType}");
                    
                    throw new InvalidPathSpecifiedException(AttributeName);
                }

                //fieldInfo.GetValue(target) means:
                //“From this object (target), give me whatever is stored in the field I’ve just found.”
                //So after each iteration, the (target) value will go from gun --> gun.shootConfig --> gun.shootConfig.spread
                target = fieldInfo.GetValue(target);
                type = target.GetType(); //tracks the type of the current target.
            }
            
            FieldInfo attributeField = type.GetField(attribute, BindingFlags.Public 
                        | BindingFlags.NonPublic | BindingFlags.Instance);
            if (attributeField == null)
            {
                Debug.LogError($"Unable to apply modifier to attribute {AttributeName}" +
                               $" because it does not exist on {targetType}");
                    
                throw new InvalidPathSpecifiedException(AttributeName);
            }
            field = attributeField;
            targetObject = target;
            
            return (TFieldType) attributeField.GetValue(target);
        }
    }
}