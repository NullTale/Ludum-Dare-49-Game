using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using CoreLib.SpriteAnimation;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[CustomEditor(typeof(SpriteAnimationAsset))]
public class SpriteAnimationEditor : Editor
{
    private const HideFlags k_HideClip      = HideFlags.HideInInspector | HideFlags.HideInHierarchy | HideFlags.DontSave | HideFlags.NotEditable;
    private const HideFlags k_OpenClip      = HideFlags.None;
    private const string    k_SpritePostfix = "_sprite";
    private const string    k_ImagePostfix  = "_image";

    private static bool     s_Pause;

    public bool Pause
    {
        get => m_Pause;
        set
        {
            m_Pause = value;
            if (m_Pause == false)
                m_Time = EditorApplication.timeSinceStartup - _getFrameTime();

            s_Pause = m_Pause;

            // ===================================
            float _getFrameTime()
            {
                var spriteAnimation = (SpriteAnimationAsset)target;
                var frameTime = 0f;
                foreach (var keyFrame in spriteAnimation.m_KeyFrames)
                {
                    if (keyFrame == m_Keyframe)
                        return frameTime;

                    frameTime += keyFrame.Duration * (1f / spriteAnimation.m_FrameRate);
                }

                return 0f;
            }
        }
    }

    private EditorCoroutine m_Coroutine;
    private Sprite          m_Sprite;
    private Color           m_Color;
    private bool            m_Pause;
    private double          m_Time;

    private ReorderableList m_ReorderableList; 

    // =======================================================================
    public override void OnInspectorGUI()
    {
        var spriteAnimation = (SpriteAnimationAsset)target;

        var keyFrames = serializedObject.FindProperty("m_KeyFrames");
        var frameRate = serializedObject.FindProperty("m_FrameRate");
        var createSprite = serializedObject.FindProperty("m_CreateSpriteAnimation");
        var createImage = serializedObject.FindProperty("m_CreateImageAnimation");
        var color = serializedObject.FindProperty("m_Color");
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(frameRate);
        EditorGUILayout.PropertyField(createSprite);
        EditorGUILayout.PropertyField(createImage);
        EditorGUILayout.PropertyField(color);

        var duration = spriteAnimation.m_KeyFrames.Sum(n => n.Duration) / spriteAnimation.m_FrameRate;
        if (duration != spriteAnimation.m_Duration)
        {
            spriteAnimation.m_Duration = duration;
            EditorUtility.SetDirty(spriteAnimation);
        }

        // i'm shitcode, refactor me in better future
        m_ReorderableList.DoLayoutList();
        
		var e = Event.current;
		switch (e.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    var spriteList = new List<Sprite>();
                    foreach (var source in DragAndDrop.objectReferences)
                    {
                        // get nested sprites from texture or from atlas
                        if (source is SpriteAtlas atlas)
                        {
                            var sprites = new Sprite[atlas.spriteCount];
                            atlas.GetSprites(sprites);
                            var spritesIDs = new HashSet<GUID>(sprites.Select(n => n.GetSpriteID()).ToArray());

                            var assets = sprites
                                 .SelectMany(n => AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(n.texture)).OfType<Sprite>())
                                 .Where(n => spritesIDs.Contains(n.GetSpriteID()));
                            spriteList.AddRange(assets);
                        }

                        spriteList.AddRange(AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(source)).OfType<Sprite>());
                    }

                    foreach (var sprite in spriteList.OrderBy(n => n.name))
                    {
                        keyFrames.InsertArrayElementAtIndex((keyFrames.arraySize - 1).ClampMin(0));
                        var item = keyFrames.GetArrayElementAtIndex(keyFrames.arraySize - 1);
                        item.FindPropertyRelative(nameof(SpriteAnimationAsset.KeyFrame.Sprite)).objectReferenceValue = sprite;
                        item.FindPropertyRelative(nameof(SpriteAnimationAsset.KeyFrame.Duration)).floatValue = 1f;
                        item.FindPropertyRelative(nameof(SpriteAnimationAsset.KeyFrame.Color)).colorValue = Color.white;
                    }

                    keyFrames.serializedObject.ApplyModifiedProperties();
                }

                e.Use();
            } break;

            case EventType.KeyDown when e.keyCode == KeyCode.Space:
            {
                Pause = !Pause;
            } break;
		}

        if (EditorGUI.EndChangeCheck())
        {
            _getAnimationClips(out var sprite, out var image);

            _setupClip<SpriteRenderer>(sprite, serializedObject.FindProperty("m_SpriteAnimationClip"));
            _setupClip<Image>(image, serializedObject.FindProperty("m_ImageAnimationClip"));

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        var prev= AssetPreview.GetAssetPreview(m_Sprite);
        
        if (spriteAnimation.m_Color)
            GUI.color = m_Color;
        if (GUILayout.Button(new GUIContent(prev, m_Sprite != null ? m_Sprite.name : ""), GUI.skin.label, GUILayout.MinWidth(64), GUILayout.MinHeight(64)))
            Pause = !Pause;
        GUI.color = Color.white;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // ===================================
        void _getAnimationClips(out AnimationClip sprite, out AnimationClip image)
        {
            sprite = null;
            image = null;
            var updateDatabase = false;

            foreach (var animationClip in serializedObject.GetNestedAssets().OfType<AnimationClip>().ToArray())
            {
                if (sprite == null && animationClip.name.EndsWith(k_SpritePostfix))
                {
                    _setClip(ref sprite, k_SpritePostfix);
                    continue;
                }
                
                if (image == null && animationClip.name.EndsWith(k_ImagePostfix))
                {
                    _setClip(ref image, k_ImagePostfix);
                    continue;
                }

                AssetDatabase.RemoveObjectFromAsset(animationClip);
                updateDatabase = true;

                // ===================================
                void _setClip(ref AnimationClip target, string postfix)
                {
                    target = animationClip;
                    if (target.name != $"{serializedObject.targetObject.name}{postfix}")
                    {
                        target.name = $"{serializedObject.targetObject.name}{postfix}";
                        updateDatabase = true;
                    }
                }
            }

            _validateClip(createSprite.boolValue, ref sprite, k_SpritePostfix);
            _validateClip(createImage.boolValue, ref image, k_ImagePostfix);

            if (updateDatabase)
                AssetDatabase.SaveAssets();
            
            // ===================================
            void _validateClip(bool created, ref AnimationClip clip, string postfix)
            {
                if (created)
                {
                    if (clip == null)
                        _createClip(out clip, postfix);
                    else if (clip.hideFlags != k_OpenClip)
                    {
                        clip.hideFlags = k_OpenClip;
                        updateDatabase  = true;
                    }
                }
                else if (clip != null)
                {
                    if (clip.hideFlags != k_HideClip)
                    {
                        clip.hideFlags = k_HideClip;
                        updateDatabase  = true;
                    }
                }
            }
        }

        IEnumerable<ObjectReferenceKeyframe> _getSpriteKeyframes()
        {
            for (var n = 0; n < keyFrames.arraySize; n++)
                yield return new ObjectReferenceKeyframe() { time = n * 1f / frameRate.floatValue, value = keyFrames.GetArrayElementAtIndex(n).FindPropertyRelative(nameof(SpriteAnimationAsset.KeyFrame.Sprite)).objectReferenceValue };
        }

        void _createClip(out AnimationClip clip, string postfix)
        {
            clip = new AnimationClip();
            clip.name = $"{serializedObject.targetObject.name}{postfix}";
            clip.hideFlags = HideFlags.None;
            serializedObject.CreateNestedAsset(clip);
        }

        void _setupClip<T>(AnimationClip clip, SerializedProperty prop)
        {
            if (clip == null)
                return;

            clip.ClearCurves();

            prop.objectReferenceValue = clip;
            AnimationUtility.SetObjectReferenceCurve(clip, EditorCurveBinding.DiscreteCurve(string.Empty, typeof(T), "m_Sprite"), _getSpriteKeyframes().ToArray());

            if (spriteAnimation.m_Color)
            {
                var curveR   = new AnimationCurve();
                var curveG   = new AnimationCurve();
                var curveB   = new AnimationCurve();
                var curveA   = new AnimationCurve();
                var time     = 0f;
                var discreet = true;

                for (var n = 0; n < keyFrames.arraySize; n++)
                {
                    var keyframe = keyFrames.GetArrayElementAtIndex(n);
                    var color    = keyframe.FindPropertyRelative(nameof(SpriteAnimationAsset.KeyFrame.Color)).colorValue;
                    var duration = keyframe.FindPropertyRelative(nameof(SpriteAnimationAsset.KeyFrame.Duration)).floatValue;
                    if (duration != 1f)
                        discreet = false;

                    _addKeyframe(curveR, time, color.r);
                    _addKeyframe(curveG, time, color.g);
                    _addKeyframe(curveB, time, color.b);
                    _addKeyframe(curveA, time, color.a);

                    time += duration / frameRate.floatValue;

                    // ===================================
                    static void _addKeyframe(AnimationCurve curve, float time, float value)
                    {
                        curve.AddKey(new Keyframe(time, value));
                        AnimationUtility.SetKeyLeftTangentMode(curve, curve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(curve, curve.keys.Length - 1, AnimationUtility.TangentMode.Constant);
                    }
                }

                clip.frameRate = discreet ? frameRate.floatValue : 60f;

                if (curveR.keys.Any(n => n.value != 1f) || curveG.keys.Any(n => n.value != 1f) || curveB.keys.Any(n => n.value != 1f))
                {
                    clip.SetCurve(string.Empty, typeof(T), "m_Color.r", curveR);
                    clip.SetCurve(string.Empty, typeof(T), "m_Color.g", curveG);
                    clip.SetCurve(string.Empty, typeof(T), "m_Color.b", curveB);
                }

                if (curveA.keys.Any(n => n.value != 1f))
                    clip.SetCurve(string.Empty, typeof(T), "m_Color.a", curveA);
            }
            EditorUtility.SetDirty(clip);
        }
    }
    private int m_SelectedLast = -1;
    private void OnEnable()
    {
        var keyFrames = serializedObject.FindProperty(nameof(SpriteAnimationAsset.m_KeyFrames));
        m_ReorderableList = new ReorderableList(serializedObject, keyFrames);
        m_ReorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
        m_ReorderableList.drawElementCallback = (pos, index, active, focused) =>
        {
            var sprite = keyFrames.GetArrayElementAtIndex(index).FindPropertyRelative("Sprite");
            var color = keyFrames.GetArrayElementAtIndex(index).FindPropertyRelative("Color");

            var durationRect = pos.WithWidth(EditorGUIUtility.labelWidth);
            EditorGUI.Slider(durationRect, keyFrames.GetArrayElementAtIndex(index).FindPropertyRelative("Duration"), 0f, 6f, GUIContent.none);

            if (((SpriteAnimationAsset)target).m_Color)
            {
                var colorRect = pos.Field().WithWidth(60);
                color.colorValue = EditorGUI.ColorField(colorRect, GUIContent.none, color.colorValue);
                var spriteRect = pos.IncWidth(-(EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing + (60 + EditorGUIUtility.standardVerticalSpacing) + EditorGUIUtility.standardVerticalSpacing))
                                    .IncX(EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing + (60 + EditorGUIUtility.standardVerticalSpacing) + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.ObjectField(spriteRect, sprite, GUIContent.none);
            }
            else
            {
                EditorGUI.ObjectField(pos.Field(), sprite, GUIContent.none);
            }
        };
        m_ReorderableList.drawHeaderCallback = pos =>
        {
            EditorGUI.LabelField(pos, new GUIContent("Frames"));
            if (Event.current.type == EventType.MouseDown 
                && Event.current.button == 1
                && pos.Contains(Event.current.mousePosition))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Sort"), false, () =>
                {
                    var index = 0;
                    foreach (var item in keyFrames.GetList().Select(n => n.FindPropertyRelative("Sprite").objectReferenceValue).OrderBy(n => (n as Sprite)?.name).ToArray())
                    {
                        var objects   = keyFrames.GetList().Select(n => n.FindPropertyRelative("Sprite").objectReferenceValue).ToList();
                        var itemIndex = objects.IndexOf(item, index);
                        keyFrames.MoveArrayElement(itemIndex, index);
                        index ++;
                    }
                    keyFrames.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Reset"), false, () =>
                {
                    foreach (var serializedProperty in keyFrames.GetList())
                    {
                        serializedProperty.FindPropertyRelative("Duration").floatValue = 1f;
                        serializedProperty.FindPropertyRelative("Color").colorValue = Color.white;
                    }

                    keyFrames.serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Clear"), false, () =>
                {
                    keyFrames.ClearArray();
                    keyFrames.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }
        };
        m_ReorderableList.onSelectCallback = list =>
        {
            if (m_SelectedLast == list.index && Pause)
            {
                m_SelectedLast = -1;
                Pause = false;
                return;
            }

            Pause = true;
            m_SelectedLast = list.index;
            _setKeyframe(((SpriteAnimationAsset)target).m_KeyFrames[list.index]);
        };

        m_Coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(_update());

        // -----------------------------------------------------------------------
        IEnumerator _update()
        {
            var spriteAnimation = (SpriteAnimationAsset)target;
            Pause = s_Pause;
            _setKeyframe(spriteAnimation.m_KeyFrames.FirstOrDefault());

            while (true)
            {
                yield return null;

                if (spriteAnimation == null)
                    continue;

                if (Pause)
                    continue;

                var keyFrame = spriteAnimation.m_KeyFrames?.Count > 0 ? spriteAnimation.KeyFrameAt((float)(m_Time - EditorApplication.timeSinceStartup), true) : null;
                _setKeyframe(keyFrame);
            }
        }
    }

    private SpriteAnimationAsset.KeyFrame   m_Keyframe;

    private void _setKeyframe(SpriteAnimationAsset.KeyFrame keyFrame)
    {
        if (keyFrame == null)
            return;

        m_Keyframe = keyFrame;

        if (keyFrame.Sprite != m_Sprite || keyFrame.Color != m_Color)
        {
            m_Sprite = keyFrame.Sprite;
            m_Color  = keyFrame.Color;
            Repaint();
        }
    }

    private void OnDisable()
    {
        if (m_Coroutine != null)
        {
            EditorCoroutineUtility.StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }
}