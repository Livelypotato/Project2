using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CRTRendererFeature : ScriptableRendererFeature
{
    public enum Presets
    {
        none,
        subtle,
        retro,
        strong,
        oldCrt,
        arcade,
        custom
    }

    public Shader shader;
    public Presets preset;

    [Range(0f, 640f)] public float pixelResolutionX = 320;
    [Range(0f, 640f)] public float pixelResolutionY = 200;

    [Range(0f, 10f)] public float screenBend = 4f;
    [Range(0f, 10f)] public float screenOverscan = 1f;
    [Range(0f, 10f)] public float blur = 0;
    [Range(0f, 50f)] public float bleed = 0;
    [Range(0f, 50f)] public float smidge = 0;
    [Range(0f, 10f)] public float scanlinesStrength = 3;
    [Range(0f, 10f)] public float apertureStrength = 3;
    [Range(0f, 50f)] public float shadowlines = 8;
    [Range(-20f, 20f)] public float shadowlinesSpeed = -2;
    [Range(0f, 1f)] public float shadowlinesAlpha = 0.05f;
    [Range(0f, 50f)] public float noiseSize = 75f;
    [Range(0f, 10f)] public float noiseSpeed = 0.02f;
    [Range(0f, 1f)] public float noiseAlpha = 0.05f;
    [Range(0f, 10f)] public float vignetteSize = 5.3f;
    [Range(0f, 20f)] public float vignetteSmooth = 2;
    [Range(2f, 50f)] public float vignetteRound = 25f;
    [Range(-2f, 2f)] public float brightness = 0;
    [Range(-3f, 3f)] public float contrast = 1;
    [Range(-3f, 3f)] public float gamma = 1;
    [Range(0f, 2f)] public float red = 1;
    [Range(0f, 2f)] public float green = 1;
    [Range(0f, 2f)] public float blue = 1;
    [Range(-10f, 10f)] public float chromaticAberration = 1;

    public Vector2 redOffset = new Vector2(0.1f, -0.1f);
    public Vector2 blueOffset = new Vector2(0, 0.1f);
    public Vector2 greenOffset = new Vector2(-0.1f, 0f);

    private CRTRenderPass crtRenderPass;
    private Material material;

    #region Settings presets

    public void OnValidate()
    {
        switch (preset)
        {
            case Presets.none:
                screenBend = 0;
                screenOverscan = 0;
                blur = 0;
                bleed = 0;
                smidge = 0;
                scanlinesStrength = 0;
                apertureStrength = 0;
                shadowlines = 0;
                shadowlinesSpeed = 0;
                shadowlinesAlpha = 0;
                vignetteSize = 0;
                vignetteSmooth = 0;
                vignetteRound = 25;
                noiseSize = 0;
                noiseAlpha = 0;
                noiseSpeed = 0;
                brightness = 0;
                contrast = 1;
                gamma = 1;
                red = 1;
                green = 1;
                blue = 1;
                chromaticAberration = 0;
                redOffset = Vector2.zero;
                blueOffset = Vector2.zero;
                greenOffset = Vector2.zero;
                break;
            case Presets.subtle:
                screenBend = 0.51f;
                screenOverscan = 0;
                blur = 0.5f;
                bleed = 0;
                smidge = 0;
                scanlinesStrength = 1;
                apertureStrength = 0.1f;
                shadowlines = 0;
                shadowlinesSpeed = 0;
                shadowlinesAlpha = 0;
                vignetteSize = 5.65f;
                vignetteSmooth = 2;
                vignetteRound = 37;
                noiseSize = 0;
                noiseAlpha = 0;
                noiseSpeed = 0;
                chromaticAberration = 0;
                break;
            case Presets.retro:
                screenBend = 0;
                screenOverscan = 0;
                blur = 0.5f;
                bleed = 1.1f;
                smidge = 14;
                scanlinesStrength = 9;
                apertureStrength = 1;
                shadowlines = 0;
                shadowlinesSpeed = 0;
                shadowlinesAlpha = 0;
                vignetteSize = 5.7f;
                vignetteSmooth = 4.3f;
                vignetteRound = 50;
                noiseSize = 0;
                noiseAlpha = 0;
                noiseSpeed = 0;
                chromaticAberration = 0;
                break;
            case Presets.strong:
                screenBend = 6.5f;
                screenOverscan = 0.5f;
                blur = 0.8f;
                bleed = 0;
                smidge = 0;
                scanlinesStrength = 2.8f;
                apertureStrength = 1;
                shadowlines = 3.5f;
                shadowlinesSpeed = 0.5f;
                shadowlinesAlpha = 0.1f;
                vignetteSize = 5.7f;
                vignetteSmooth = 2.8f;
                vignetteRound = 30;
                noiseSize = 0;
                noiseAlpha = 0;
                noiseSpeed = 0;
                chromaticAberration = 0.5f;
                break;
            case Presets.oldCrt:
                screenBend = 8.3f;
                screenOverscan = 1.5f;
                blur = 1;
                bleed = 0.1f;
                smidge = 0;
                scanlinesStrength = 9;
                apertureStrength = 4;
                shadowlines = 3.5f;
                shadowlinesSpeed = 1.5f;
                shadowlinesAlpha = 0.2f;
                vignetteSize = 5.7f;
                vignetteSmooth = 2;
                vignetteRound = 13;
                noiseSize = 26;
                noiseAlpha = 0.25f;
                noiseSpeed = 7.2f;
                chromaticAberration = 1.5f;
                break;
            case Presets.arcade:
                screenBend = 7.2f;
                screenOverscan = 0.5f;
                blur = 0;
                bleed = 3;
                smidge = 15;
                scanlinesStrength = 9;
                apertureStrength = 4;
                shadowlines = 0;
                shadowlinesSpeed = 0;
                shadowlinesAlpha = 0;
                vignetteSize = 5.7f;
                vignetteSmooth = 1;
                vignetteRound = 15;
                noiseSize = 0;
                noiseAlpha = 0;
                noiseSpeed = 0;
                chromaticAberration = 1;
                break;
            case Presets.custom:
            default:
                break;
        }

        if (chromaticAberration != 0)
        {
            redOffset = new Vector2(chromaticAberration / 10, chromaticAberration / 10);
            blueOffset = new Vector2(0, -(chromaticAberration / 10) * 1.4f);
            greenOffset = new Vector2(-chromaticAberration / 10, chromaticAberration / 10);
        }
    }

    #endregion

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null || crtRenderPass == null)
            return;

        material.SetFloat("m_screenBend", screenBend == 0 ? 1000 : 13 - screenBend);
        material.SetFloat("m_screenOverscan", screenOverscan * 0.025f);
        material.SetFloat("m_blur", blur / 1000);
        material.SetFloat("m_smidge", smidge / 50);
        material.SetFloat("m_bleedr", bleed);
        material.SetFloat("m_bleedg", bleed > 0 ? 1 : 0);
        material.SetFloat("m_bleedb", bleed > 0 ? 1 : 0);
        material.SetFloat("m_resX", pixelResolutionX);
        material.SetFloat("m_resY", pixelResolutionY);
        material.SetFloat("m_scanlinesStrength", scanlinesStrength / 10);
        material.SetFloat("m_apertureStrength", apertureStrength / 10);
        material.SetFloat("m_shadowlines", shadowlines);
        material.SetFloat("m_shadowlinesSpeed", shadowlinesSpeed);
        material.SetFloat("m_shadowlinesAlpha", shadowlinesAlpha * 0.2f);
        material.SetFloat("m_vignetteSize", vignetteSize * 0.35f);
        material.SetFloat("m_vignetteSmooth", vignetteSmooth * 0.1f);
        material.SetFloat("m_vignetteRound", vignetteRound);
        material.SetFloat("m_noiseSize", noiseSize * 20);
        material.SetFloat("m_noiseAlpha", noiseAlpha * 0.2f);
        material.SetFloat("m_noiseSpeed", noiseSpeed * 0.0001f);
        material.SetFloat("m_brightness", brightness);
        material.SetFloat("m_contrast", contrast);
        material.SetFloat("m_gamma", gamma);
        material.SetFloat("m_red", red);
        material.SetFloat("m_green", green);
        material.SetFloat("m_blue", blue);
        material.SetVector("m_redOffset", redOffset / 100);
        material.SetVector("m_greenOffset", greenOffset / 100);
        material.SetVector("m_blueOffset", blueOffset / 100);

        crtRenderPass.Init(renderer, material);
        renderer.EnqueuePass(crtRenderPass);
    }

    public override void Create()
    {
        Debug.Log("Create crtRenderPass"+crtRenderPass);
        if (material == null)
            material = new Material(shader);

        if (crtRenderPass == null)
            crtRenderPass = new CRTRenderPass();
    }

    protected override void Dispose(bool disposing)
    {
        if (crtRenderPass != null)
            crtRenderPass = null;
        if (material != null)
        {
            CoreUtils.Destroy(material);
            material = null;
        }
    }

    /// <summary>
    /// 执行渲染通道之前调用
    /// 创建临时渲染目标纹理
    /// 为Excute提前准备需要的RenderTexture或其他变量
    /// </summary>
    class CRTRenderPass : ScriptableRenderPass
    {
        private const string PROFTAG = "CRTFilter";
        private Material copyMaterial;
        private ScriptableRenderer renderer;
        private Material material;
        private RenderTargetIdentifier cameraRT;
        private RenderTargetIdentifier tempRT;

        // 定义渲染的位置
        public CRTRenderPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            Debug.Log("CRTRenderPass构造函数");
        }

        public void Init(ScriptableRenderer renderer, Material material)
        {
            this.renderer = renderer;
            this.material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Debug.Log("Execute CRTRenderPass");
            if (material == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(PROFTAG);
            try
            {
                RenderTextureDescriptor cameraTextureDesc = renderingData.cameraData.cameraTargetDescriptor;
                cameraTextureDesc.depthBufferBits = 0;
        
                // 获取临时渲染纹理
                int tempTextureId = Shader.PropertyToID("_CRTFilterTexture");
                cmd.GetTemporaryRT(tempTextureId, cameraTextureDesc);
        
                // 设置材质属性
                material.SetFloat("m_time", Time.time);
        
                // 应用 CRT 效果到临时纹理
                //Blitter.BlitTexture(cmd, renderer.cameraColorTargetHandle, new Vector4(1, 1, 0, 0), material, 0);
                cmd.Blit(renderer.cameraColorTargetHandle, tempTextureId, material, 0);
                cmd.Blit(tempTextureId, renderer.cameraColorTargetHandle);
                // 将结果复制回相机目标
                //Blitter.BlitTexture(cmd, tempTextureId, renderer.cameraColorTargetHandle);
        
                // 释放临时纹理
                cmd.ReleaseTemporaryRT(tempTextureId);
        
                context.ExecuteCommandBuffer(cmd);
            }
            finally
            {
                CommandBufferPool.Release(cmd);
            }
        }

        // 新增：适配 Render Graph 的方法
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer contextContainer)
        {
            //base.RecordRenderGraph(renderGraph, contextContainer);
                    Debug.Log("material:"+material);
            
            if (material == null)
                return;

            // 获取渲染数据
            var resourceData = contextContainer.Get<UniversalResourceData>();
            var cameraData = contextContainer.Get<UniversalCameraData>();
            //var rendererData = contextContainer.Get<UniversalRendererData>();

            // 获取相机颜色纹理
            TextureHandle cameraColor = resourceData.activeColorTexture;

            // 创建临时纹理
            RenderTextureDescriptor tempTextureDesc = cameraData.cameraTargetDescriptor;
            tempTextureDesc.depthBufferBits = 0;
            tempTextureDesc.msaaSamples = 1;

            TextureHandle tempTexture = renderGraph.CreateTexture(
                new TextureDesc(tempTextureDesc.width, tempTextureDesc.height)
                {
                    colorFormat = tempTextureDesc.graphicsFormat,
                    name = "_CRTFilterTexture",
                    enableRandomWrite = false
                });

            // 添加CRT效果渲染通道
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CRT Effect", out var passData))
            {
                // 设置材质属性
                material.SetFloat("m_time", Time.time);

                // 设置输入输出纹理
                passData.material = material;
                passData.source = cameraColor;

                // 声明纹理使用方式
                builder.UseTexture(cameraColor, AccessFlags.Read);
                builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);

                // 设置渲染函数 - 使用Blitter而不是DrawProcedural
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Debug.Log("SetRenderFunc");
                    // 使用Blitter进行全屏绘制，这是Render Graph推荐的方式
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // 将处理后的纹理复制回相机颜色纹理
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("CRT Copy Back", out var passData))
            {
                // 设置输入输出纹理
                passData.source = tempTexture;
                passData.destination = cameraColor;

                // 声明纹理使用方式
                builder.UseTexture(tempTexture, AccessFlags.Read);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                // 设置渲染函数
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // 直接复制纹理，不使用材质
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }
    }

    // RenderGraph使用的数据结构
    private class PassData
    {
        public TextureHandle source;
        public TextureHandle destination;
        public Material material;
    }

    // 复制操作使用的数据结构
    // private class CopyPassData
    // {
    //     public TextureHandle source;
    //     public TextureHandle destination;
    // }
}