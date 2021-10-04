using System;
using System.Globalization;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class PlayerPrefsValue : ScriptableObject
    {
        [SerializeField]
        private Optional<string> m_Default;

        public bool HasValue => PlayerPrefs.HasKey(name);

        // =======================================================================
        internal void WriteDefault()
        {
            if (HasValue == false && m_Default.Enabled)
                PlayerPrefs.SetString(name, m_Default.Value);
        }

        public TType GetValue<TType>()
        {
            var tType = typeof(TType);

            // set default if not set
            if (HasValue == false && m_Default.Enabled)
                SetValue<string>(m_Default.Value);

            // try get data
            if (HasValue)
                try
                {
                    var data = PlayerPrefs.GetString(name);

                    if (tType == typeof(bool))
                        return (TType)(object)bool.Parse(data);
                    if (tType == typeof(string))
                        return (TType)(object)data;
                    if (tType == typeof(int))
                        return (TType)(object)int.Parse(data);
                    if (tType == typeof(float))
                        return (TType)(object)float.Parse(data, CultureInfo.InvariantCulture);

                    return JsonUtility.FromJson<TType>(data);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            // bad result
            return default;
        }

        public void SetValue<T>(T value)
        {
            var tType = typeof(T);

            if (tType == typeof(bool))
                PlayerPrefs.SetString(name, ((bool)(object)value).ToString());
            else
            if (tType == typeof(string))
                PlayerPrefs.SetString(name, (string)(object)value);
            else
            if (tType == typeof(int))
                PlayerPrefs.SetString(name, ((int)(object)value).ToString());
            else
            if (tType == typeof(float))
                PlayerPrefs.SetString(name, ((float)(object)value).ToString(CultureInfo.InvariantCulture));
            else
                PlayerPrefs.SetString(name, JsonUtility.ToJson(value));

            SaveChanges();
        }

        [Button]
        public void Delete()
        {
            PlayerPrefs.DeleteKey(name);
            SaveChanges();
        }

        [Button]
        private void Log()
        {
            Debug.Log($"{(HasValue ? PlayerPrefs.GetString(name) : "Not set")}");
        }

        // =======================================================================
        private void _writeDefault()
        {
            PlayerPrefs.SetString(name, m_Default.Value);
            SaveChanges();
        }

        // =======================================================================
        public static void SaveChanges()
        {
            PlayerPrefs.Save();
            Extensions.JsSyncFiles();
        }
    }
}