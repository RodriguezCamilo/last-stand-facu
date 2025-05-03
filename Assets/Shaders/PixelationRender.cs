using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelationRendererFeature : ScriptableRendererFeature
{
    class PixelationPass : ScriptableRenderPass
    {
        readonly Material material;
        RTHandle          tempTex;

        public PixelationPass(Material mat)
        {
            material        = mat;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void Execute(ScriptableRenderContext ctx, ref RenderingData data)
        {
            if (data.cameraData.cameraType != CameraType.Game)
                return;

            if (material == null)
            {
                Debug.LogError("Pixelation material is null."); 
                return;
            }

            RTHandle src = data.cameraData.renderer.cameraColorTargetHandle;
            if (src == null || src.rt == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("PixelationPass");
            RenderingUtils.ReAllocateIfNeeded(ref tempTex, src.rt.descriptor);

            Blit(cmd, src, tempTex);
            cmd.SetGlobalTexture("_BlitTexture", tempTex);
            Blit(cmd, tempTex, src, material);

            ctx.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (tempTex != null) { tempTex.Release(); tempTex = null; }
        }
    }
    [System.Serializable] public class Settings { public Material pixelationMaterial; }
    public Settings settings = new Settings();

    PixelationPass pass;

    public override void Create()
    {
        if (settings.pixelationMaterial == null)
        {
            Debug.LogError("PixelationRendererFeature: asigna un material.");
            return;
        }
        pass = new PixelationPass(settings.pixelationMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (pass != null) renderer.EnqueuePass(pass);
    }
}
