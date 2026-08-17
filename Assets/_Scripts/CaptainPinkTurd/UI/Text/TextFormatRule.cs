using System;
using UnityEngine;

namespace CaptainPinkTurd.UI.TextUI
{
    [CreateAssetMenu(fileName = "TextFormatRule", menuName = "Scriptable Objects/UI/TextFormatRule")]
    public class TextFormatRule : ScriptableObject
    {
        [Tooltip("Format string using placeholders. Example: {0}/{1} or Level {0}")]
        [SerializeField] private string format = "{0}";
        
        public string Format(params object[] values)
        {
            try
            {
                return string.Format(format, values);
            }
            catch
            {
                throw new FormatException("There's a problem with the provided format, " +
                      "please check if it's either in the code or in the way the format data was written.");
            }
        }
    }
}