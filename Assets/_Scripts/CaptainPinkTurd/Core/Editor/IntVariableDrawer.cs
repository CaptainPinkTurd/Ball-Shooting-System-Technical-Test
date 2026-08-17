#if UNITY_EDITOR
using CaptainPinkTurd.Core.DesignPattern.SOAP.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CaptainPinkTurd.Core.Editor
{
    [CustomPropertyDrawer(typeof(IntVariableSO))]
    public class IntVariableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
        
            var objectField = new ObjectField(property.displayName) 
            {
                objectType = typeof(IntVariableSO)
            };
            objectField.BindProperty(property);
        
            var valueLabel = new Label();
            valueLabel.style.paddingLeft = 20;
        
            container.Add(objectField);
            container.Add(valueLabel);
        
            objectField.RegisterValueChangedCallback
            (
                evt => 
                {
                    var variable = evt.newValue as IntVariableSO;
                    if (variable) 
                    {
                        valueLabel.text = $"Current Value: {variable.Value}";
                        variable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
                    }
                    else 
                    {
                        valueLabel.text = string.Empty;
                    }
                }
            );
        
            var currentVariable = property.objectReferenceValue as IntVariableSO;
            if (currentVariable) 
            {
                valueLabel.text = $"Current Value: {currentVariable.Value}";
                currentVariable.OnValueChanged += newValue => valueLabel.text = $"Current Value: {newValue}";
            }
        
            return container;
        }
    }
}
#endif