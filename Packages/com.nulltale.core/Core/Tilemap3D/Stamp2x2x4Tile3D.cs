using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Tilemap3D
{
    [CreateAssetMenu(fileName = nameof(Stamp2x2x4Tile3D), menuName = k_Tilemap3DTileMenu + nameof(Stamp2x2x4Tile3D))]
    public class Stamp2x2x4Tile3D : Tile3DBase
    {
        public  Material m_Material;
        private Mesh     m_MeshResult;
        
        [Tooltip("left bottom")]
        public Mesh m_MeshLB;
        [Tooltip("left top")]
        public Mesh m_MeshLT;
        [Tooltip("right down")]
        public Mesh m_MeshRB;
        [Tooltip("right top")]
        public Mesh m_MeshRT;

        public override Material   Material => m_Material;
        public override Mesh       Mesh     => m_MeshResult;
        public override Quaternion Rotation => Quaternion.identity;
        public override Vector3    Scale    => Vector3.one;

        //////////////////////////////////////////////////////////////////////////
        public override void Setup(in Vector2Int index, Tilemap3D map, List<CombineInstance> chunkData)
        {
            var evenX = index.x % 2 == 0;
            var evenY = index.y % 2 == 0;

            m_MeshResult = evenX ? evenY ? m_MeshLB : m_MeshLT : evenY ? m_MeshRB : m_MeshRT;

            _addCombineInstance(in index, chunkData);
        }
    }
}