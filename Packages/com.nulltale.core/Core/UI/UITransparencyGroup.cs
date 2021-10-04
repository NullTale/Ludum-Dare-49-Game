using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    [DefaultExecutionOrder(2)]
    public class UITransparencyGroup : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)]
        private float               m_Transparency = 1.0f;
        private float               m_TransparencyLast;
        [SerializeField]
        private Influence           m_Influence = Influence.All;
        private List<LinkAdapter>   m_Links = new List<LinkAdapter>();

        //////////////////////////////////////////////////////////////////////////
        [Serializable] [Flags]
        public enum Influence
        {
            None = 0,
            Image = 1,
            TextMeshPro = 1 << 1,

            All = Image | TextMeshPro
        }

        public abstract class LinkAdapter
        {
            protected float m_Base;

            public abstract void Set(float transparecy);
        }

        private class LinkImage : LinkAdapter
        {
            private Image m_Image;

            public override void Set(float transparecy)
            {
                if (m_Image == null)
                    return;

                m_Image.color = m_Image.color.WithA(m_Base * transparecy);
            }

            public LinkImage(Image image)
            {
                m_Image = image;
                m_Base = image.color.a;
            }
        }
        
        private class LinkTextMeshPro : LinkAdapter
        {
            private TMP_Text m_Text;

            public override void Set(float transparecy)
            {
                if (m_Text == null)
                    return;
                m_Text.color = m_Text.color.WithA(m_Base * transparecy);
            }

            public LinkTextMeshPro(TMP_Text text)
            {
                m_Text = text;
                m_Base = text.color.a;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private void Start()
        {
            Link();
        }

        private void Update()
        {
            if (m_Transparency != m_TransparencyLast)
            {
                foreach (var link in m_Links)
                    link.Set(m_Transparency);

                m_TransparencyLast = m_Transparency;
            }

        }

        [Button]
        public void Link()
        {
            m_Links.Clear();

            if (m_Influence.HasFlag(Influence.Image))
                m_Links.AddRange(GetComponentsInChildren<Image>().Select(n => new LinkImage(n)));
            
            if (m_Influence.HasFlag(Influence.TextMeshPro))
                m_Links.AddRange(GetComponentsInChildren<TMP_Text>().Select(n => new LinkTextMeshPro(n)));
        }
    }
}