using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace CoreLib
{
    public static class ScriptableObjectCreator
    {
        private static Texture2D s_ScriptableObjectIcon = (EditorGUIUtility.IconContent("ScriptableObject Icon").image as Texture2D);

        // =======================================================================
        private class DoCreateFile : EndNameEditAction
        {
            public Type ObjectType;
            
            // =======================================================================
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                _create(pathName);
            }

            public override void Cancelled(int instanceId, string pathName, string resourceFile)
            {
                _create(pathName);
            }

            // =======================================================================
            private void _create(string pathName)
            {
                var so = ScriptableObject.CreateInstance(ObjectType);

                AssetDatabase.CreateAsset(so, Path.ChangeExtension(pathName, ".asset"));
                ProjectWindowUtil.ShowCreatedAsset(so);
            }
        }

        // =======================================================================
        [MenuItem("Assets/Create/Scriptable Object", false, -1000)]
        private static void CreateScriptableObject(MenuCommand menuCommand)
        {
            var mainAssambly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(n => n.GetName().Name == "Assembly-CSharp");
            var onlyMain     = !EditorPrefs.GetBool(CreateScriptableObjectSettingsProvider.k_AllAssemblies);

            var types = TypeCache.GetTypesDerivedFrom<ScriptableObject>()
                                 .Where(type =>
                                 {
                                     if (type.IsAbstract || type.IsGenericTypeDefinition)
                                         return false;

                                     var vibilityAttribute = type.GetCustomAttribute<CreateScriptableObjectAttribute>();

                                     if (vibilityAttribute != null)
                                         return vibilityAttribute.Visible;

                                     if (onlyMain && type.Assembly != mainAssambly)
                                         return false;

                                     return true;
                                 })
                                 .ToList();

            ObjectPickerWindow.Show(picked =>
            {
                var pickedType = (Type)picked;
                var doCreateFile = ScriptableObject.CreateInstance<DoCreateFile>();
                doCreateFile.ObjectType = pickedType;

                ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                    0,
                    doCreateFile,
                    pickedType.Name,
                    s_ScriptableObjectIcon,
                    string.Empty);
            }, null, types, 0, s => new GUIContent(s.FullName), "ScriptableObject Type", true);
        }

        // =======================================================================
        private class CreateScriptableObjectSettingsProvider : SettingsProvider
        {
            public const string k_AllAssemblies = nameof(CreateScriptableObjectSettingsProvider) + ".AllAssemblies";

            // =======================================================================
            private CreateScriptableObjectSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
                : base(path, scope) { }

            public override void OnGUI(string searchContext)
            {
                //EditorGUILayout.ObjectField(null, typeof(AssemblyDefinitionAsset), false);
                EditorPrefs.SetBool("Only main assembly", !EditorGUILayout.Toggle(k_AllAssemblies, !EditorPrefs.GetBool(k_AllAssemblies)));
            }

            [SettingsProvider]
            public static SettingsProvider CreateMyCustomSettingsProvider()
            {
                var provider = new CreateScriptableObjectSettingsProvider("Preferences/ScriptableObject Creator", SettingsScope.User);

                // Automatically extract all keywords from the Styles.
                //provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
                return provider;
            }
        }
    }
}