using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Tilemap3D
{
    [CreateAssetMenu(fileName = nameof(Rule2x2Tile3D), menuName = k_Tilemap3DTileMenu + nameof(Rule2x2Tile3D))]
    public class Rule2x2Tile3D : Tile3DBase
    {
        public  Material   m_Material;
        private Mesh       m_MeshResult;
        private Quaternion m_RotationResult;
        private Vector3    m_ScaleResult;

        [Tooltip("tile mesh")]
        public Mesh m_Mesh;
        [Tooltip("left corner border mesh")]
        public Mesh m_MeshB;
        [Tooltip("left down side border mesh")]
        public Mesh m_MeshC;
        [Tooltip("down side border mesh even")]
        public Mesh m_MeshSE;
        [Tooltip("down side border mesh odd")]
        public Mesh m_MeshSO;

        public override Material   Material => m_Material;
        public override Mesh       Mesh     => m_MeshResult;
        public override Quaternion Rotation => m_RotationResult;
        public override Vector3    Scale    => m_ScaleResult;

        /*
        protected static bool m_left
        protected static bool m_right
        protected static bool m_up
        protected static bool m_down
        */

        //////////////////////////////////////////////////////////////////////////
        public override void Setup(in Vector2Int index, Tilemap3D map, List<CombineInstance> chunkData)
        {
            setup(in index);
        
            _addCombineInstance(in index, chunkData);

            void setup(in Vector2Int index)
            {
                m_ScaleResult = k_Normal;

                var left  = ReferenceEquals(map.GetTile(index.x - 1, index.y), this);
                var right = ReferenceEquals(map.GetTile(index.x + 1, index.y), this);
                var up    = ReferenceEquals(map.GetTile(index.x, index.y + 1), this);
                var down  = ReferenceEquals(map.GetTile(index.x, index.y - 1), this);

                // corner check
                if (!left && !down)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation0;
                    return;
                }
                if (!down && !right)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation90;
                    return;
                }
                if (!right && !up)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation180;
                    return;
                }
                if (!left && !up)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation270;
                    return;
                }

                var evenX = index.x % 2 == 0;
                var evenY = index.y % 2 == 0;

                // side check
                if (!down)
                {
                    m_MeshResult     = evenX ? m_MeshSE : m_MeshSO;
                    m_RotationResult = k_Rotation0;
                    return;
                }
                if (!right)
                {
                    m_MeshResult     = evenY ? m_MeshSE : m_MeshSO;
                    m_RotationResult = k_Rotation90;
                    return;
                }
                if (!up)
                {
                    m_MeshResult     = !evenX ? m_MeshSE : m_MeshSO;
                    m_RotationResult = k_Rotation180;
                    return;
                }
                if (!left)
                {
                    m_MeshResult     = !evenY ? m_MeshSE : m_MeshSO;;
                    m_RotationResult = k_Rotation270;
                    return;
                }

                var leftUp    = ReferenceEquals(map.GetTile(index.x - 1, index.y + 1), this);
                var leftDown  = ReferenceEquals(map.GetTile(index.x - 1, index.y - 1), this);
                var rightUp   = ReferenceEquals(map.GetTile(index.x + 1, index.y + 1), this);
                var rightDown = ReferenceEquals(map.GetTile(index.x + 1, index.y - 1), this);

                // block check
                if (!leftDown)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation0;
                    return;
                }
                if (!rightDown)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation90;
                    return;
                }
                if (!rightUp)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation180;
                    return;
                }
                if (!leftUp)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation270;
                    return;
                }

                // set idle by default
                m_MeshResult     = m_Mesh;
                m_RotationResult = k_Rotation0;
            
                m_ScaleResult = evenX ? evenY ? k_Normal : k_InvertY : evenY ? k_InvertX : k_InvertXY;
            }
        }
    }
}