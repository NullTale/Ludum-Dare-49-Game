using System.Collections;
using CoreLib;
using CoreLib.SceneManagement;
using UnityEngine;

namespace Game
{
    public class SceneInitializer : SceneInitializerNull
    {
        public static SceneInitializer Instance;

        public Pig                        m_PigPrefab;
        public ParticleSystem.MinMaxCurve m_SpawnInterval;

        public Transform                  m_SpawnZone;
        public float                      m_SpawnDeviation;
        public GamePreferences.Difficulty m_Difficulty;

        // =======================================================================
        public override void Init(SceneArgsNull args)
        {
            Instance = this;
            m_Difficulty = GamePreferences.Instance.GetDifficulty();
            StartCoroutine(_spawner());

            // -----------------------------------------------------------------------
            IEnumerator _spawner()
            {
                for (var n = 0; n < m_Difficulty.Pigs; n++)
                {
                    yield return new WaitForSeconds(m_SpawnInterval.Evaluate());

                    Instantiate(m_PigPrefab, m_SpawnZone.position + m_SpawnDeviation.Amplitude().ToVector2().To3DXZ(), Quaternion.LookRotation(UnityRandom.Normal3D()));
                }
            }
        }
    }
}