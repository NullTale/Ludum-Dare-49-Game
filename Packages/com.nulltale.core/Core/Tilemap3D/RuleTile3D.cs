using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Tilemap3D
{
    [CreateAssetMenu(fileName = nameof(RuleTile3D), menuName = k_Tilemap3DTileMenu + nameof(RuleTile3D))]
    public class RuleTile3D : Tile3DBase
    {
        public  Material   m_Material;
        private Mesh       m_MeshResult;
        private Quaternion m_RotationResult;

        [Tooltip("tile mesh")]
        public Mesh m_Mesh;
        [Tooltip("left corner border mesh")]
        public Mesh m_MeshB;
        [Tooltip("left down side border mesh")]
        public Mesh m_MeshC;
        [Tooltip("down side border mesh")]
        public Mesh m_MeshS;

        public override Material   Material => m_Material;
        public override Mesh       Mesh     => m_MeshResult;
        public override Quaternion Rotation => m_RotationResult;
        public override Vector3    Scale    => Vector3.one;

        //////////////////////////////////////////////////////////////////////////
        public override void Setup(in Vector2Int index, Tilemap3D map, List<CombineInstance> chunkData)
        {
            setup(in index);

            // add combine instance
            _addCombineInstance(in index, chunkData);

            void setup(in Vector2Int index)
            {
                var left  = ReferenceEquals(map.GetTile(index.x - 1, index.y), this);
                var right = ReferenceEquals(map.GetTile(index.x + 1, index.y), this);
                var up    = ReferenceEquals(map.GetTile(index.x, index.y + 1), this);
                var down  = ReferenceEquals(map.GetTile(index.x, index.y - 1), this);

                var leftUp    = ReferenceEquals(map.GetTile(index.x - 1, index.y + 1), this);
                var leftDown  = ReferenceEquals(map.GetTile(index.x - 1, index.y - 1), this);
                var rightUp   = ReferenceEquals(map.GetTile(index.x + 1, index.y + 1), this);
                var rightDown = ReferenceEquals(map.GetTile(index.x + 1, index.y - 1), this);

                // block check
                if (left && down && !leftDown)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation0;
                    return;
                }
                if (down && right && !rightDown)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation90;
                    return;
                }
                if (right && up && !rightUp)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation180;
                    return;
                }
                if (left && up && !leftUp)
                {
                    m_MeshResult     = m_MeshB;
                    m_RotationResult = k_Rotation270;
                    return;
                }

                // corner check
                if (!left && !down && !leftDown)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation0;
                    return;
                }
                if (!down && !right && !rightDown)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation90;
                    return;
                }
                if (!right && !up && !rightUp)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation180;
                    return;
                }
                if (!left && !up && !leftUp)
                {
                    m_MeshResult     = m_MeshC;
                    m_RotationResult = k_Rotation270;
                    return;
                }

                // side check
                if (!down)
                {
                    m_MeshResult     = m_MeshS;
                    m_RotationResult = k_Rotation0;
                    return;
                }
                if (!right)
                {
                    m_MeshResult     = m_MeshS;
                    m_RotationResult = k_Rotation90;
                    return;
                }
                if (!up)
                {
                    m_MeshResult     = m_MeshS;
                    m_RotationResult = k_Rotation180;
                    return;
                }
                if (!left)
                {
                    m_MeshResult     = m_MeshS;
                    m_RotationResult = k_Rotation270;
                    return;
                }

                // set idle by default
                m_MeshResult     = m_Mesh;
                m_RotationResult = k_Rotation0;
            }
        }

    }
}