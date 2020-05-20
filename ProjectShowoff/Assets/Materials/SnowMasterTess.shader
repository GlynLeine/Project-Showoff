Shader "Universal Render Pipeline/Custom/SnowMasterTess"
{
    Properties
    {
		[MainColor] _BaseColor("Color", Color) = (0.5,0.5,0.5,1)
		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
		
		_WorldCenter("World Center", Vector) = (0.0, 0.0, 0.0, 0.0)
		_SnowThreshold("Snow Threshold", Range(1.0, 5.0)) = 0.5
		_Displacement("Displacement", Float) = 0.0
		_TessellationUniform("Tessellation", Range(1,32)) = 4

		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 0.0)) = 0.0
		[HideInInspector] _Metallic("Metallic", Range(0.0, 0.0)) = 0.0
		[HideInInspector] _Smoothness("Smoothness", Range(0.0, 0.0)) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Opaque"
			"IgnoreProjector" = "True"
		}
		LOD 200

		pass
		{
			Name "SnowMasterTess"
			Tags{"LightMode" = "UniversalForward"}

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
			Cull[_Cull]

			HLSLPROGRAM

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 4.5

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment
			#pragma hull hull
			#pragma domain domain

			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

			float4 _WorldCenter;
			float _SnowThreshold;

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normalOS     : NORMAL;
				float4 tangentOS    : TANGENT;
				float2 uv           : TEXCOORD0;
				float2 uvLM         : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float2 uv                       : TEXCOORD0;
				float2 uvLM                     : TEXCOORD1;
				float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
				half3  normalWS                 : TEXCOORD3;

#ifdef _MAIN_LIGHT_SHADOWS
				float4 shadowCoord              : TEXCOORD6; // compute shadow coord per-vertex for the main light
#endif
				float4 positionCS               : SV_POSITION;
			};

			Varyings LitPassVertex(Attributes input)
			{
				Varyings output;

				// VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
				// Our compiler will strip all unused references (say you don't use view space).
				// Therefore there is more flexibility at no additional cost with this struct.
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

				// Similar to VertexPositionInputs, VertexNormalInputs will contain normal, tangent and bitangent
				// in world space. If not used it will be stripped.
				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

				// Computes fog factor per-vertex.
				float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

				// TRANSFORM_TEX is the same as the old shader library.
				output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
				output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

				output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
				output.normalWS = vertexNormalInput.normalWS;

#ifdef _MAIN_LIGHT_SHADOWS
				// shadow coord for the main light is computed in vertex.
				// If cascades are enabled, LWRP will resolve shadows in screen space
				// and this coord will be the uv coord of the screen space shadow texture.
				// Otherwise LWRP will resolve shadows in light space (no depth pre-pass and shadow collect pass)
				// In this case shadowCoord will be the position in light space.
				output.shadowCoord = GetShadowCoord(vertexInput);
#endif
				// We just use the homogeneous clip position from the vertex input
				output.positionCS = vertexInput.positionCS;
				return output;
			}

#include "tessellation/GlynTessellation.hlsl"

			half4 LitPassFragment(Varyings input) : SV_Target
			{
				// Surface data contains albedo, metallic, specular, smoothness, occlusion, emission and alpha
				// InitializeStandarLitSurfaceData initializes based on the rules for standard shader.
				// You can write your own function to initialize the surface data of your shader.
				SurfaceData surfaceData;
				InitializeStandardLitSurfaceData(input.uv, surfaceData);

				half3 normalWS = input.normalWS;
				normalWS = normalize(normalWS);

#ifdef LIGHTMAP_ON
				// Normal is required in case Directional lightmaps are baked
				half3 bakedGI = SampleLightmap(input.uvLM, normalWS);
#else
				// Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
				// are also defined in case you want to sample some terms per-vertex.
				half3 bakedGI = SampleSH(normalWS);
#endif

				float3 positionWS = input.positionWSAndFogFactor.xyz;
				half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

				float3 toThis = SafeNormalize(positionWS - _WorldCenter.xyz);

				float snowy = clamp(pow(abs(dot(toThis, normalWS)), _SnowThreshold), 0.0, 1.0);

				float3 inputColor = lerp(surfaceData.albedo, float3(1.0, 1.0, 1.0), snowy);

				BRDFData brdfData;
				InitializeBRDFData(inputColor, 0, 0, 0, surfaceData.alpha, brdfData);

#ifdef _MAIN_LIGHT_SHADOWS
				// Main light is the brightest directional light.
				// It is shaded outside the light loop and it has a specific set of variables and shading path
				// so we can be as fast as possible in the case when there's only a single directional light
				// You can pass optionally a shadowCoord (computed per-vertex). If so, shadowAttenuation will be
				// computed.
				Light mainLight = GetMainLight(input.shadowCoord);
#else
				Light mainLight = GetMainLight();
#endif

				half3 color = GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);

				color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);

				// Additional lights loop
#ifdef _ADDITIONAL_LIGHTS

				// Returns the amount of lights affecting the object being renderer.
				// These lights are culled per-object in the forward renderer
				int additionalLightsCount = GetAdditionalLightsCount();
				for (int i = 0; i < additionalLightsCount; ++i)
				{
					// Similar to GetMainLight, but it takes a for-loop index. This figures out the
					// per-object light index and samples the light buffer accordingly to initialized the
					// Light struct. If _ADDITIONAL_LIGHT_SHADOWS is defined it will also compute shadows.
					Light light = GetAdditionalLight(i, positionWS);

					// Same functions used to shade the main light.
					color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
				}
#endif
				// Emission
				color += surfaceData.emission;

				float fogFactor = input.positionWSAndFogFactor.w;

				color = MixFog(color, fogFactor);
				return half4(color, surfaceData.alpha);
			}
			ENDHLSL
		}

		UsePass "Universal Render Pipeline/Lit/ShadowCaster"
		UsePass "Universal Render Pipeline/Lit/DepthOnly"
		UsePass "Universal Render Pipeline/Lit/Meta"

    }
}
