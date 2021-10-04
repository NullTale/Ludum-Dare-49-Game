using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    public class BlitPass : ScriptableRenderPass
    {
        public  Material               m_BlitMaterial;
        public  int                    m_BlitShaderPassIndex;
        public  FilterMode             m_FilterMode          = FilterMode.Point;
        private RenderTargetIdentifier m_Source;
        private RenderTargetHandle     m_Destination;

        private RenderTargetHandle m_TemporaryColorTexture;
        private string             m_ProfilerTag;

        // =======================================================================
        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }

        // =======================================================================
        public BlitPass(RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag)
        {
            this.renderPassEvent  = renderPassEvent;
            m_BlitMaterial        = blitMaterial;
            m_BlitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag         = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");
        }

        public void Setup(ref RenderTargetIdentifier source, ref RenderTargetHandle destination)
        {
            m_Source      = source;
            m_Destination = destination;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(m_ProfilerTag);

            var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            // can't read and write to same color target, create a temp render target to blit. 
            if (m_Destination == RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, m_FilterMode);
                Blit(cmd, m_Source, m_TemporaryColorTexture.Identifier(), m_BlitMaterial, m_BlitShaderPassIndex);
                Blit(cmd, m_TemporaryColorTexture.Identifier(), m_Source);
            }
            else
            {
                Blit(cmd, m_Source, m_Destination.Identifier(), m_BlitMaterial, m_BlitShaderPassIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (m_Destination == RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
        }
    }
}