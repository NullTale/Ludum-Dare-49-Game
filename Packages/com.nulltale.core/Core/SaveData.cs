using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class SaveData
    {
        public SaveData SetupFrom(object source)
        {
            foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var sourceField = source.GetType().GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                try
                {
                    field.SetValue(this, sourceField.GetValue(source));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return this;
        }
        
        public void InjectTo(object source)
        {
            foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var sourceField = source.GetType().GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                try
                {
                    sourceField.SetValue(source, field.GetValue(this));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}