using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NaughtyAttributes;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace CoreLib.PlayerData
{
    [CreateAssetMenu(fileName = nameof(PlayerData), menuName = Core.k_CoreModuleMenu + nameof(PlayerData))]
    public class PlayerData : Core.Module<PlayerData>
    {
        private const string k_FileExtention = ".json";
        private const string k_ZipFileExtention = ".bin";
        private const string k_PlayerDataFileName  = "data";

        [SerializeField] [Expandable]
        private PlayerPrefsValue m_PlayerNameValue;
        [SerializeField]
        private bool m_ForceDefault;
        [SerializeField]
        private Optional<string> m_ForcePlayerName = new Optional<string>("Player", false);
        [SerializeField]
        private Optional<CompressionLevel> m_Zip = new Optional<CompressionLevel>(CompressionLevel.Optimal, false);
        [SerializeField]
        private Optional<string> m_CustomFileExtention = new Optional<string>(".save", false);

        private string m_PlayerName;

        public  string              PlayerFolder   => $"{Path.GetFullPath(Path.Combine(Application.persistentDataPath, m_PlayerName))}{Path.DirectorySeparatorChar}";
        private string              PlayerDataFile => $"{PlayerFolder}{k_PlayerDataFileName}{(m_CustomFileExtention.Enabled ? m_CustomFileExtention.Value : (m_Zip.Enabled ? k_ZipFileExtention : k_FileExtention))}";
        public  IEnumerable<string> Players        => Directory.GetDirectories(Application.persistentDataPath);

        [SerializeField]
        private ScriptableObjectCollection<DataHandle> m_Data;

        // =======================================================================
        public abstract class DataHandle : ScriptableObject
        {
            public abstract void InitDefault();
            public abstract void Init(string data);
            public abstract string Serialize();
        }

        [Serializable]
        private class FileData
        {
            [Serializable]
            public struct Record
            {
                public string Key;
                public string Data;
            }

            public Record[] Data;
        }

        // =======================================================================
        public override void Init()
        {
            if (m_PlayerNameValue != null)
                m_PlayerName = m_PlayerNameValue.GetValue<string>();

            if (m_ForcePlayerName)
                m_PlayerName = m_ForcePlayerName.Value;

            Load();
        }
        
        [Button]
        public void Save()
        {
            // player data file
            var path = PlayerDataFile;

            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(PlayerFolder);

            // save to the key value array
            var fileData = new FileData() { Data = m_Data.Values.Select(n => new FileData.Record() { Key = n.name, Data = n.Serialize() }).ToArray() };
            var json     = JsonUtility.ToJson(fileData, true);

            using (var file = File.Open(path, FileMode.Create))
            {
                var data = m_Zip.Enabled ? json.Zip(m_Zip.Value) : Encoding.UTF8.GetBytes(json);
                file.Write(data, 0, data.Length);
            }

            Extensions.JsSyncFiles();

            Debug.Log($"<color=yellow>PlayerData has saved at:</color> {path}");
        }
        
        [Button]
        public void Load()
        {
            var path = PlayerDataFile;

            try
            {
                if (File.Exists(path) && m_ForceDefault == false)
                {
                    var data     = File.ReadAllBytes(path);
                    var fileData = JsonUtility.FromJson<FileData>(m_Zip.Enabled ? data.UnzipString() : Encoding.UTF8.GetString(data));

                    // unpack player records
                    foreach (var record in fileData.Data)
                    {
                        if (m_Data.TryGetValue(record.Key, out var dataHandle))
                            dataHandle.Init(record.Data);
                    }

                    Debug.Log($"<color=yellow>PlayerData has loaded from:</color> {path}");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log($"<color=yellow>PlayerData init as default</color>");
            // init default if error was occur
            foreach (var dataHandle in m_Data)
                dataHandle.InitDefault();
        }

        [Button]
        public void Delete()
        {
            var path = PlayerFolder;

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Debug.Log($"<color=red>Player data at path</color> {path}<color=red> was deleted</color>");
            }
        }
    }
}