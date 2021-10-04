using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = nameof(_), menuName = k_Tilemap3DTileMenu + nameof(_))]
namespace CoreLib.Tilemap3D
{
    public abstract class Tile3DBase : ScriptableObject
    {
        internal const string k_Tilemap3DTileMenu = "Tile3D/";

        protected static Quaternion k_Rotation0   = Quaternion.AngleAxis(0.0f, Vector3.up);
        protected static Quaternion k_Rotation90  = Quaternion.AngleAxis(-90.0f, Vector3.up);
        protected static Quaternion k_Rotation180 = Quaternion.AngleAxis(-180.0f, Vector3.up);
        protected static Quaternion k_Rotation270 = Quaternion.AngleAxis(-270.0f, Vector3.up);

        protected static Vector3 k_Normal   = new Vector3(1.0f, 1.0f, 1.0f);
        protected static Vector3 k_InvertX  = new Vector3(-1.0f, 1.0f, 1.0f);
        protected static Vector3 k_InvertY  = new Vector3(1.0f, 1.0f, -1.0f);
        protected static Vector3 k_InvertXY = new Vector3(-1.0f, 1.0f, -1.0f);

        public abstract Material   Material { get; }
        public abstract Mesh       Mesh     { get; }
        public abstract Quaternion Rotation { get; }
        public abstract Vector3    Scale    { get; }

        public abstract void Setup(in Vector2Int index, Tilemap3D map, List<CombineInstance> chunkData);
    
        protected void _addCombineInstance(in Vector2Int index, List<CombineInstance> chunkData)
        {
            if (Mesh == null)
                return;

            chunkData.Add(new CombineInstance()
            {
                mesh      = Mesh,
                transform = Matrix4x4.TRS(new Vector3(index.x + 0.5f, 0.0f, index.y + 0.5f), Rotation, Scale)
            });
        }
    }
}