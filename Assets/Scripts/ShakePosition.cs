using Cinemachine;
using CoreLib;
using UnityEngine;

public class ShakePosition : MonoBehaviour
{
    public NoiseSettings    m_Noise;

    private float m_Time;

    private Vector3    m_PositionImpact;
    public float       m_FrequencyGain = 1f;
    public float       m_AmplitudeGain = 1f;

    public Vector3     m_Scale = Vector3.one;

    public bool m_UnscaledTime;
    [Range(0, 1)]
    public float m_Weight;

    // =======================================================================
    private void Update()
    {
        if (m_AmplitudeGain == 0f)
            return;
        
        m_Time += (m_UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * m_FrequencyGain * m_Weight;
        m_Noise.GetSignal(m_Time, out var pos, out var rot);

        var impact = pos.HadamardMul(m_Scale) * (m_AmplitudeGain * m_Weight);
        transform.localPosition += impact - m_PositionImpact;
        m_PositionImpact = impact;
    }

    private void OnDisable()
    {
        transform.localPosition -= m_PositionImpact;
        m_PositionImpact   =  Vector3.zero;
    }
}