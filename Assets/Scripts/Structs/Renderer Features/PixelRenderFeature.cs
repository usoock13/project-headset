using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelRenderFeature : ScriptableRendererFeature {
    [System.Serializable]
    public class PixelRenderSettings {
        public LayerMask layerMask = 0;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingSkybox;
        public Material blitMaterial = null;
        [Range(1f, 15f)]
        public float pixelDensity = 1f;
    }
    
    public PixelRenderSettings settings;
    private PixelRenderPass _pass;

    public override void Create() {
        _pass = new PixelRenderPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        // _pass.Setup(renderer, targetLayerMask);
        renderer.EnqueuePass(_pass);
    }
    
    public class PixelRenderPass : ScriptableRenderPass {
        
        private Material m_blitMaterial;
        private ProfilingSampler m_ProfilingSampler;
        private RenderStateBlock m_RenderStateBlock;
        private List<ShaderTagId> m_ShaderTagIds = new List<ShaderTagId>();
        private FilteringSettings m_FilteringSettings;
        private float m_pixelDensity;

        static int pixelTexId = Shader.PropertyToID("_PixelTexture");
        static int pixelDepthId = Shader.PropertyToID("_DepthTex");
        static int cameraColorId = Shader.PropertyToID("_CameraColorTexture");

        public PixelRenderPass(PixelRenderSettings settings) {
            m_ProfilingSampler = new ProfilingSampler("__Pixel Render Feature__");
            renderPassEvent = settings.renderPassEvent;
            m_blitMaterial = settings.blitMaterial;
            m_pixelDensity = settings.pixelDensity;
            m_blitMaterial.SetFloat("_PixelDensity", this.m_pixelDensity);

            m_FilteringSettings = new FilteringSettings(RenderQueueRange.all, settings.layerMask);

            m_ShaderTagIds.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIds.Add(new ShaderTagId("LightweightForward"));
            m_ShaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            SortingCriteria sortingCriteria = SortingCriteria.CommonTransparent;
            
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIds, ref renderingData, sortingCriteria);
            ref CameraData CameraData = ref renderingData.cameraData;
            Camera camera = CameraData.camera;
            Rect pixelRect = camera.pixelRect;
            int width = (int) (camera.pixelWidth / m_pixelDensity);
            int height = (int) (camera.pixelHeight / m_pixelDensity);
            CommandBuffer cmd = CommandBufferPool.Get("PixelRenderFeature");
            using(new ProfilingScope(cmd, m_ProfilingSampler)) {
                cmd.GetTemporaryRT(pixelTexId, width, height, 0, FilterMode.Point);
                cmd.GetTemporaryRT(pixelDepthId, width, height, 24, FilterMode.Point, RenderTextureFormat.Depth);
                cmd.SetRenderTarget(pixelTexId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store,
                                    pixelDepthId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                cmd.ClearRenderTarget(true, true, Color.clear);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
                cmd.SetRenderTarget(cameraColorId, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);

                cmd.Blit(new RenderTargetIdentifier(pixelTexId), BuiltinRenderTextureType.CurrentActive, m_blitMaterial);

                cmd.ReleaseTemporaryRT(pixelTexId);
                cmd.ReleaseTemporaryRT(pixelDepthId);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }
        }
    }
}
