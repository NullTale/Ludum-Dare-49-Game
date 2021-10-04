using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Tilemap3D
{
    [CreateAssetMenu(fileName = nameof(Stamp2x2Tile3D), menuName = k_Tilemap3DTileMenu + nameof(Stamp2x2Tile3D))]
    public class Stamp2x2Tile3D : Tile3DBase
    {
        public  Material m_Material;
        private Quaternion m_RotationResult;

        [Tooltip("left down side border mesh")]
        public Mesh m_MeshC;

        public override Material   Material => m_Material;
        public override Mesh       Mesh     => m_MeshC;
        public override Quaternion Rotation => m_RotationResult;
        public override Vector3    Scale    => Vector3.one;

        //////////////////////////////////////////////////////////////////////////
        public override void Setup(in Vector2Int index, Tilemap3D map, List<CombineInstance> chunkData)
        {
            var evenX = index.x % 2 == 0;
            var evenY = index.y % 2 == 0;
            m_RotationResult = evenX ? evenY ? k_Rotation0 : k_Rotation270 : evenY ? k_Rotation90 : k_Rotation180;

            _addCombineInstance(in index, chunkData);
        }
    }
}