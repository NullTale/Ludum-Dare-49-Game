using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace CoreLib.Tilemap3D
{
    public class Tilemap3D : MonoBehaviour, ITicked
    {
        private const  int     k_BeshChunkSize    = 10;
        private static Chunk[] s_UpdateArrayCache = new Chunk[64];

        [SerializeField] [HideIf(nameof(m_AutoChunkSize))]
        private int m_ChunkSize = k_BeshChunkSize;
        [SerializeField]
        private bool m_AutoChunkSize;

        private Tile3DBase[,]               m_Cells;
        private Dictionary<Material, Layer> m_LayersData;
        private RectInt                     m_Bounds;
        private RectInt                     m_ChunkBounds;
        private Vector2Int                  m_Size;

        private HashSet<Chunk>      m_DirtyChunks = new HashSet<Chunk>();
        public  TickedQueue         m_Queue;
        private bool                m_InQueue;
        private HashSet<Vector2Int> m_ChunkCoords = new HashSet<Vector2Int>();

        public float TickLength => m_UpdateInterval;

        [SerializeField]
        private int m_ChunksPerUpdateLimit = 1;
        [SerializeField]
        private float m_MaxUpdateDuration = (1000.0f / 60.0f) / 1000.0f;
        [SerializeField]
        private float m_UpdateInterval = 0.1f;

        [SerializeField]
        private LayerSetup[] m_Layers;

        public RectInt Bounds => m_Bounds;

        public int ChunkSize => m_ChunkSize;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public class LayerSetup
        {
            public Material Material;
            public bool     CastShadows;
            public bool     ReceiveShadows;
        }

        public class Layer
        {
            public  Tilemap3D m_Tilemap;
            public  Material  m_Material;
            private Chunk[,]  m_Chunks;
            private bool      m_CastShadows;
            private bool      m_ReceiveShadows;

            //////////////////////////////////////////////////////////////////////////
            public void Setup(Tilemap3D tilemap, Material material, bool castShadows, bool receiveShadows)
            {
                m_Tilemap  = tilemap;
                m_Material = material;

                m_CastShadows = castShadows;
                m_ReceiveShadows = receiveShadows;

                // create chunks
                var chunkSize = m_Tilemap.m_ChunkSize;
                m_Chunks = new Chunk[tilemap.m_ChunkBounds.width, tilemap.m_ChunkBounds.height];
                m_Chunks.Initialize(((x, y) =>
                {
                    var go = new GameObject($"Chunk_{x}_{y} {m_Material.name}");
                    go.transform.SetParent(tilemap.transform, false);

                    var chunk = go.AddComponent<Chunk>();
                    chunk.Setup(this, new RectInt(x * chunkSize, y * chunkSize, chunkSize, chunkSize), m_CastShadows, m_ReceiveShadows);

                    return chunk;
                }));

                m_Tilemap.m_DirtyChunks.UnionWith(m_Chunks.ToEnumerable());
            }

            public void DirtyChunk(in Vector2Int chunkIndex)
            {
                m_Tilemap.m_DirtyChunks.Add(m_Chunks[chunkIndex.x, chunkIndex.y]);
            }
        }

        public class Chunk : MonoBehaviour
        {
            private static List<CombineInstance> s_CombineInstances = new List<CombineInstance>();

            public Layer      m_Layer;
            public MeshFilter m_Mesh;
            public RectInt    m_Rect;

            //////////////////////////////////////////////////////////////////////////
            public void Setup(Layer layer, RectInt rectInt, bool castShadows, bool receiveShadows)
            {
                m_Mesh      = gameObject.AddComponent<MeshFilter>();
                m_Mesh.mesh = new Mesh();

                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.material = layer.m_Material;
                meshRenderer.receiveShadows = receiveShadows;
                meshRenderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;

                m_Layer = layer;
                m_Rect  = rectInt;
            }


            public void Rebuild()
            {
                Profiler.BeginSample("Collect data");

                var cells = m_Layer.m_Tilemap.m_Cells;
                foreach (var index in m_Rect.allPositionsWithin)
                {
                    var tile = cells[index.x, index.y];
                    if (ReferenceEquals(tile,  null) == false && ReferenceEquals(tile.Material, m_Layer.m_Material))
                    {
                        Profiler.BeginSample("Tile3D setup");
                        tile.Setup(in index, m_Layer.m_Tilemap, s_CombineInstances);
                        Profiler.EndSample();
                    }
                }

                Profiler.EndSample();

                if (s_CombineInstances.Count != 0)
                {
                    Profiler.BeginSample("Combine meshes");

                    var result = s_CombineInstances.ToArray();
                    s_CombineInstances.Clear();
                    Destroy(m_Mesh.mesh);
                    m_Mesh.mesh = new Mesh();
                    //m_Mesh.mesh.Clear();
                    m_Mesh.mesh.CombineMeshes(result, true, true, false);
                    //m_Mesh.mesh.OptimizeIndexBuffers();
                    //m_Mesh.mesh.OptimizeReorderVertexBuffer();
                    m_Mesh.mesh.UploadMeshData(true);

                    Profiler.EndSample();
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public void Init(Vector2Int size, Tile3DBase initial)
        {
            if (m_AutoChunkSize)
            {
                m_ChunkSize = k_BeshChunkSize;
                while (size.x % m_ChunkSize != 0 || size.y % m_ChunkSize != 0)
                    m_ChunkSize --;
            }

            if (size.x % m_ChunkSize != 0 || size.y % m_ChunkSize != 0)
                throw new ArgumentException($"Tilemap3D size must be a multiple of the chunk size ({size}, {m_ChunkSize}).");

            m_DirtyChunks.Clear();
            m_InQueue = false;
            gameObject.DestroyChildren();

            m_Size        = size;
            m_Bounds      = new RectInt(0, 0, size.x, size.y);
            m_ChunkBounds = new RectInt(0, 0, size.x / m_ChunkSize, size.y / m_ChunkSize);

            m_Cells = new Tile3DBase[size.x, size.y];
            m_Cells.Initialize(((x, y) => initial));


            m_LayersData = new Dictionary<Material, Layer>(m_Layers.Length);
            foreach (var layerSetup in m_Layers)
            {
                var layer = new Layer();
                layer.Setup(this, layerSetup.Material, layerSetup.CastShadows, layerSetup.ReceiveShadows);
                m_LayersData[layerSetup.Material] = layer;
            }
        
            _inQueue();
        }

        private void _inQueue()
        {
            if (m_InQueue == false && m_DirtyChunks.Count != 0)
            {
                m_InQueue = true;
                m_Queue.Add(this);
            }
        }

        public Tile3DBase GetTile(in Vector2Int pos)
        {
            if (m_Bounds.Contains(pos) == false)
                return null;

            return m_Cells[pos.x, pos.y];
        }

        public Tile3DBase GetTile(int x, int y)
        {
            if (x < 0 || x >= m_Size.x || y < 0 || y >= m_Size.y)
                return null;

            return m_Cells[x, y];
        }

        public void SetTile(in Vector2Int pos, Tile3DBase tile)
        {
            Profiler.BeginSample("Tilemap3D SetTile", this);

            if (m_Bounds.Contains(pos) == false)
                return;

            var tilePrev = m_Cells[pos.x, pos.y];
            if (ReferenceEquals(tilePrev, tile))
                return;

            m_Cells[pos.x, pos.y] = tile;

            // update neighbours
            var xCenter = pos.x / m_ChunkSize;
            var yCenter = pos.y / m_ChunkSize;


            var xRight = (pos.x + 1) / m_ChunkSize;
            var xLeft  = (pos.x - 1) / m_ChunkSize;
            var yUp    = (pos.y + 1) / m_ChunkSize;
            var yDown  = (pos.y - 1) / m_ChunkSize;

            m_ChunkCoords.Add(new Vector2Int(xCenter, yCenter));

            m_ChunkCoords.Add(new Vector2Int(xRight, yCenter));
            m_ChunkCoords.Add(new Vector2Int(xLeft, yCenter));
            m_ChunkCoords.Add(new Vector2Int(xCenter, yUp));
            m_ChunkCoords.Add(new Vector2Int(xCenter, yDown));
        
            m_ChunkCoords.Add(new Vector2Int(xRight, yUp));
            m_ChunkCoords.Add(new Vector2Int(xRight, yDown));
            m_ChunkCoords.Add(new Vector2Int(xLeft, yUp));
            m_ChunkCoords.Add(new Vector2Int(xLeft, yDown));

            var tileLayer = m_LayersData[tile.Material];

            // if layer was changed, update both layers
            if (ReferenceEquals(tilePrev.Material, tile.Material) == false)
            {
                var tilePrevLayer = m_LayersData[tilePrev.Material];
                foreach (var chunkIndex in m_ChunkCoords)
                {
                    if (m_ChunkBounds.Contains(chunkIndex))
                    {
                        tileLayer.DirtyChunk(in chunkIndex);
                        tilePrevLayer.DirtyChunk(in chunkIndex);
                    }
                }
            }
            else
                foreach (var chunkIndex in m_ChunkCoords)
                {
                    if (m_ChunkBounds.Contains(chunkIndex))
                        tileLayer.DirtyChunk(in chunkIndex);
                }

        
            m_ChunkCoords.Clear();

            _inQueue();

            Profiler.EndSample();
        }

        public void OnTicked()
        {
            Profiler.BeginSample("Dirty chunks");
        
            var startTime = Time.realtimeSinceStartup;

            var length = m_DirtyChunks.Count > 64 ? 64 : m_DirtyChunks.Count;
            m_DirtyChunks.CopyTo(s_UpdateArrayCache, 0, length);

            var updated = 0;
            for (var n = 0; n < length; n++)
            {
                var dirtyChunk = s_UpdateArrayCache[n];
                dirtyChunk.Rebuild();
                m_DirtyChunks.Remove(dirtyChunk);
                if (Time.realtimeSinceStartup - startTime > m_MaxUpdateDuration)
                    break;
                if (++updated >= m_ChunksPerUpdateLimit)
                    break;
            }


            if (m_DirtyChunks.Count == 0)
            {
                m_InQueue = false;
                m_Queue.Remove(this);
            }

            Profiler.EndSample();
        }
    }
}