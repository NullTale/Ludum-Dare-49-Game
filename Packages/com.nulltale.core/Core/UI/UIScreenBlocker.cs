using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    public class UIScreenBlocker : MonoBehaviour
    {
        public const int				k_CanvasSortingOrder = 29999;
     
        private static GameObject s_Blocker;
        private static int        s_Activated;

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            s_Activated ++;
            if (s_Blocker == null)
            {
                s_Blocker = UIScreenBlocker.Create();
                DontDestroyOnLoad(s_Blocker);
                s_Blocker.hideFlags = HideFlags.HideAndDontSave;
            }

            s_Blocker.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            s_Activated --;

            if (s_Activated == 0)
                s_Blocker.gameObject.SetActive(false);
        }

        //////////////////////////////////////////////////////////////////////////
        public static void Create(Action onClick, GameObject caller, int canvasSortingOrder = k_CanvasSortingOrder)
        {
            // recreate blocker in code
            var result = new GameObject("blocker", typeof(RectTransform), typeof(Button), typeof(Canvas), typeof(CanvasRenderer), typeof(GraphicRaycaster), typeof(Image));
		
            var rectTransform = result.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.SetParent(_topComponent<Canvas>(caller).transform, false);

            var canvas = result.GetComponent<Canvas>();
            //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.overrideSorting = true;
            canvas.sortingOrder = canvasSortingOrder;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var image = result.GetComponent<Image>();
            image.color = Color.clear;

            var button = result.GetComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation(){mode = Navigation.Mode.None};
            button.onClick.AddListener(() => onClick?.Invoke());

            T _topComponent<T>(GameObject caller) where T : Component
            {	// gets top most component of the type

                // current go can contain component to
                var result = caller.GetComponent<T>();
                var current = caller;

                // go at the top of the hierarchy
                while (true)
                {
                    var comp = current.transform.parent?.GetComponentInParent<T>();
                    if (comp != null)
                    {
                        result = comp;
                        current = comp.gameObject;
                    }
                    else
                        break;
                }

                return result;
            }
        }

        public static GameObject Create()
        {
            var result = new GameObject("ScreenBlocker", typeof(RectTransform), typeof(Canvas), typeof(CanvasRenderer), typeof(GraphicRaycaster), typeof(Image));
		
            var canvas = result.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.overrideSorting = true;
            canvas.sortingOrder = k_CanvasSortingOrder;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

            var image = result.GetComponent<Image>();
            image.color = Color.clear;
            return result;
        }
    }
}