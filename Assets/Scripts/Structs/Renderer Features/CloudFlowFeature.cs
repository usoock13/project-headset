using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CloudFlowFeature : ScriptableRendererFeature {
    [System.Serializable]
    public class CloudFlowSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        public Material blitMaterial = null;
    }
    
    public CloudFlowSettings settings;
    private CloudFlowPass _pass;

    public override void Create() {
        _pass = new CloudFlowPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(_pass);
        _pass._destination = renderer.cameraColorTarget;
    }
    
    public class CloudFlowPass : ScriptableRenderPass {
        private Material m_blitMaterial;
        private ProfilingSampler m_ProfilingSampler;
        private RenderStateBlock m_RenderStateBlock;
        private List<ShaderTagId> m_ShaderTagIds = new List<ShaderTagId>();
        private FilteringSettings m_FilteringSettings;
        private float m_pixelDensity;

        static int pixelTexId = Shader.PropertyToID("_PixelTexture");
        static int pixelDepthId = Shader.PropertyToID("_DepthTex");
        static int cameraColorId = Shader.PropertyToID("_CameraColorTexture");

        public RenderTargetIdentifier _destination;


        public CloudFlowPass(CloudFlowSettings settings) {
            m_ProfilingSampler = new ProfilingSampler("__Pixel Render Feature__");
            renderPassEvent = settings.renderPassEvent;
            m_blitMaterial = settings.blitMaterial;
            m_blitMaterial.SetFloat("_PixelDensity", this.m_pixelDensity);

            m_ShaderTagIds.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIds.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
            
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIds, ref renderingData, sortingCriteria);
            CommandBuffer cmd = CommandBufferPool.Get("CloudFlowRenderFeature");
            using(new ProfilingScope(cmd, m_ProfilingSampler)) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
                
                RenderTargetHandle temp = new RenderTargetHandle();
                temp.Init("Temp");

                cmd.GetTemporaryRT(temp.id, renderingData.cameraData.cameraTargetDescriptor, FilterMode.Point);

                cmd.Blit(_destination, temp.Identifier(), m_blitMaterial);
                cmd.Blit(temp.Identifier(), _destination);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
        }
    }
}
