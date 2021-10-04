using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Tilemap3D
{
    [CreateAssetMenu(fileName = nameof(StampTile3D), menuName = k_Tilemap3DTileMenu + nameof(StampTile3D))]
    public class StampTile3D : Tile3DBase
    {
        public Mesh     m_Mesh;
        public Material m_Material;

        public override Mesh       Mesh     => m_Mesh;
        public override Material   Material => m_Material;
        public override Quaternion Rotation { get; } = Quaternion.identity;
        public override Vector3    Scale    { get; } = Vector3.one;

        public override void Setup(in Vector2Int index, Tilemap3D map, List<CombineInstance> chunkData)
        {
            _addCombineInstance(in index, chunkData);
        }
    }
}