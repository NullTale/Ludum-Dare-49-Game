using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class Comment : MonoBehaviour
    {
        [ResizableTextArea] [Tooltip("Exists in editor only")]
        public string m_Note;
    }
}