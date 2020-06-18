Shader "Universal Render Pipeline/Custom/MasterWebGL"
{
	Properties
	{
		_Spring("Spring", 2D) = "white" {}
		_Summer("Summer", 2D) = "white" {}
		_Fall("Fall", 2D) = "white" {}
		_Winter("Winter", 2D) = "white" {}

		_SeasonTime("Time", Range(0.0, 1.0)) = 0.0

		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.0

		_Pollution("Pollution", Range(0.0, 1.0)) = 0.0
		_NoiseScale("Snow Noise Scale", Range(0.0, 10.0)) = 1.0

		_WorldCenter("World Center", Vector) = (0.0, 0.0, 0.0, 0.0)
		_SnowThreshold("Snow Threshold", Range(0.0, 10.0)) = 0.5

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
				Name "MasterWebGL"
				Tags{"LightMode" = "UniversalForward"}

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]
				Cull[_Cull]

				HLSLPROGRAM

				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0

				#pragma vertex LitPassVertex
				#pragma fragment LitPassFragment

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

				CBUFFER_START(UnityPerMaterial)
				sampler _Spring;
				sampler	_Summer;
				sampler _Fall;
				sampler _Winter;

				float _SeasonTime;
				float _Smoothness;
				float _Pollution;
				float _NoiseScale;

				float4 _WorldCenter;
				float _SnowThreshold;

				float _SrcBlend;
				float _DstBlend;
				float _ZWrite;
				float _Cull;
				CBUFFER_END

				struct Attributes
				{
					float4 positionOS   : POSITION;
					float3 normalOS     : NORMAL;
					float2 uv           : TEXCOORD0;
					float2 uvLM         : TEXCOORD1;
				};

				struct Varyings
				{
					float2 uv                       : TEXCOORD0;
					float2 uvLM                     : TEXCOORD1;
					float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
					half3  normalWS                 : TEXCOORD3;

	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					float4 shadowCoord : TEXCOORD7;
	#endif
					float4 positionCS               : SV_POSITION;
					float snowy : TEXCOORD8;
				};

#include "SimplexNoise.hlsl"

				Varyings LitPassVertex(Attributes input)
				{
					Varyings output;

					// VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
					// Our compiler will strip all unused references (say you don't use view space).
					// Therefore there is more flexibility at no additional cost with this struct.
					VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

					// Similar to VertexPositionInputs, VertexNormalInputs will contain normal, tangent and bitangent
					// in world space. If not used it will be stripped.
					VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS);

					// Computes fog factor per-vertex.
					float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

					// TRANSFORM_TEX is the same as the old shader library.
					output.uv = input.uv;
					output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

					output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
					output.normalWS = vertexNormalInput.normalWS;

	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					output.shadowCoord = GetShadowCoord(vertexInput);
	#endif

					// We just use the homogeneous clip position from the vertex input
					output.positionCS = vertexInput.positionCS;

					float3 toThis = SafeNormalize(vertexInput.positionWS - _WorldCenter.xyz);

					output.snowy = pow(clamp(dot(toThis, vertexNormalInput.normalWS), 0, 1), _SnowThreshold) * smoothstep(2.0 / 3.0, 1.0, _SeasonTime);

					float snowFactor = clamp(snoise(float2(dot(output.normalWS, float3(0.0, 1.0, 0.0)), dot(output.normalWS, float3(1.0, 0.0, 0.0))) * _NoiseScale), 0.0, 1.0);
					snowFactor = clamp(snowFactor + lerp(1.0, -1.0, _Pollution), 0.0, 1.0);

					output.snowy = lerp(0.0, output.snowy, snowFactor);

					return output;
				}


				half4 LitPassFragment(Varyings input) : SV_Target
				{


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
					float3 spring = tex2D(_Spring, input.uv).rgb;
					float3 summer = tex2D(_Summer, input.uv).rgb;
					float3 fall = tex2D(_Fall, input.uv).rgb;
					float3 winter = tex2D(_Winter, input.uv).rgb;

					float3 ss = lerp(spring, summer, smoothstep(0.0, 1.0 / 3.0, _SeasonTime));
					float3 fw = lerp(fall, winter, smoothstep(2.0 / 3.0, 1.0, _SeasonTime));
					float3 albedo = lerp(ss, fw, smoothstep(1.0 / 3.0, 2.0 / 3.0, _SeasonTime));

					float3 positionWS = input.positionWSAndFogFactor.xyz;
					half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

					float3 inputColor = lerp(albedo, float3(1.0, 1.0, 1.0), input.snowy);

					BRDFData brdfData;
					InitializeBRDFData(inputColor, 0, 0, _Smoothness, 1.0, brdfData);

	#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					Light mainLight = GetMainLight(input.shadowCoord);
	#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					Light mainLight = GetMainLight(TransformWorldToShadowCoord(positionWS));
	#else
					Light mainLight = GetMainLight();
	#endif

					half3 color = GlobalIllumination(brdfData, bakedGI, 1.0, normalWS, viewDirectionWS);

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
					float fogFactor = input.positionWSAndFogFactor.w;

					color = MixFog(color, fogFactor);
					return half4(color, 1.0);
				}
				ENDHLSL
			}

			UsePass "Universal Render Pipeline/Lit/ShadowCaster"
			UsePass "Universal Render Pipeline/Lit/DepthOnly"
			UsePass "Universal Render Pipeline/Lit/Meta"

		}
}
