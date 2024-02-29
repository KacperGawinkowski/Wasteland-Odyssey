Shader "Hidden/Outline"
{
    Properties
    {
        _OutlineSize("Outline size", Integer) = 1
        _Threshold("Threshold", Integer) = 1
    }

    HLSLINCLUDE
    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    float _Threshold;
    int _OutlineSize;

    #define v2 1.41421
    #define c45 0.707107
    #define c225 0.9238795
    #define s225 0.3826834

    #define MAXSAMPLES 8
    // Neighbour pixel positions
    static float2 samplingPositions[MAXSAMPLES] =
    {
        float2(1, 1),
        float2(0, 1),
        float2(-1, 1),
        float2(-1, 0),
        float2(-1, -1),
        float2(0, -1),
        float2(1, -1),
        float2(1, 0),
    };

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);

        float depth = LoadCameraDepth(varyings.positionCS.xy);
        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP,
                                                   UNITY_MATRIX_V);
        float4 color = float4(0.0, 0.0, 0.0, 0.0);
        float luminanceThreshold = max(0.000001, _Threshold * 0.01);

        // Load the camera color buffer at the mip 0 if we're not at the before rendering injection point
        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING)
            color = float4(CustomPassSampleCameraColor(posInput.positionNDC.xy, 0), 1);

        // When sampling RTHandle texture, always use _RTHandleScale.xy to scale your UVs first.
        // float2 uv = posInput.positionNDC.xy * _RTHandleScale.xy;
        // float4 outline = SAMPLE_TEXTURE2D_X_LOD(_OutlineBuffer, s_linear_clamp_sampler, uv, 0);
        float4 outline = CustomPassLoadCustomColor(varyings.positionCS.xy);
        float outlineDepth = CustomPassLoadCustomDepth(varyings.positionCS.xy);
        // outline.a = 0;

        // If this sample is below the threshold

        // return float4(outline.a, 0, 0, 1);
        // if (any(outline.rgb != 0))
        // {
        //     return outline;
        // }
        // else
        // {
        //     return float4(0, 0, 0, 1);
        // }

        // if (outline.a == 0)
        if (all(outline.rgb == 0) || outlineDepth < depth)
        {
            for (int q = 1; q <= _OutlineSize; ++q)
            {
                // Search neighbors
                for (int i = 0; i < MAXSAMPLES; ++i)
                {
                    // float2 uvN = uv + _ScreenSize.zw * _RTHandleScale.xy * samplingPositions[i];
                    float4 neighbour = CustomPassLoadCustomColor(varyings.positionCS.xy + samplingPositions[i] * q);
                    float neighbourDepth = CustomPassLoadCustomDepth(varyings.positionCS.xy + samplingPositions[i] * q);
                    float neighbourCameraDepth = LoadCameraDepth(varyings.positionCS.xy + samplingPositions[i] * q);

                    if (any(neighbour.rgb != 0) && neighbourDepth == neighbourCameraDepth)
                    {
                        // outline.rgb = neighbour.rgb;
                        // outline.a = 1;
                        return neighbour;
                    }
                }
            }
        }

        return 0;
    }
    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Custom Pass 0"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
            #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}