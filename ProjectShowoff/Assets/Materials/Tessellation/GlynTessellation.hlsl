// Tessellation programs based on this article by Catlike Coding:
// https://catlikecoding.com/unity/tutorials/advanced-rendering/tessellation/

// added by niuage
#if defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_VULKAN) || defined(SHADER_API_METAL) || defined(SHADER_API_PSSL)
#define UNITY_CAN_COMPILE_TESSELLATION 1
#define UNITY_domain                 domain
#define UNITY_partitioning           partitioning
#define UNITY_outputtopology         outputtopology
#define UNITY_patchconstantfunc      patchconstantfunc
#define UNITY_outputcontrolpoints    outputcontrolpoints
#endif

float _TessellationUniform;

struct TessellationControlPoint
{
    float4 vertex : INTERNALTESSPOS;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv : TEXCOORD0;
    float2 uv1 : TEXCOORD1;
};

struct TessellationFactors
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

Varyings TessellationVertexProgram(Varyings input)
{    
    float3 positionWS = input.positionWSAndFogFactor.xyz;
    half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

    float3 toThis = SafeNormalize(positionWS - _WorldCenter.xyz);

    float snowy = clamp(pow(abs(dot(toThis, input.normalWS)), _SnowThreshold), 0.0, 1.0);
    
    Varyings output;

    output.uv = input.uv;
    output.uvLM = input.uvLM;
    
    output.positionWSAndFogFactor = input.positionWSAndFogFactor;

    output.normalWS = input.normalWS;
    
#ifdef _MAIN_LIGHT_SHADOWS
    output.shadowCoord = input.shadowCoord;
#endif

    output.positionCS = input.positionCS;
    
    return output;
}

TessellationFactors patchConstantFunction(InputPatch<Varyings, 3> patch)
{
    TessellationFactors f;
	f.edge[0] = _TessellationUniform;
	f.edge[1] = _TessellationUniform;
	f.edge[2] = _TessellationUniform;
	f.inside = _TessellationUniform;
	return f;
}

[UNITY_domain("tri")]
[UNITY_outputcontrolpoints(3)]
[UNITY_outputtopology("triangle_cw")]
[UNITY_partitioning("integer")]
[UNITY_patchconstantfunc("patchConstantFunction")]
Varyings hull(InputPatch<Varyings, 3> patch, uint id : SV_OutputControlPointID)
{
    return patch[id];
}

[UNITY_domain("tri")]
Varyings domain(TessellationFactors factors, OutputPatch<Varyings, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
{
    Varyings data;

#define MY_DOMAIN_PROGRAM_INTERPOLATE(fieldName) data.fieldName = patch[0].fieldName * barycentricCoordinates.x + patch[1].fieldName * barycentricCoordinates.y + patch[2].fieldName * barycentricCoordinates.z;

	MY_DOMAIN_PROGRAM_INTERPOLATE(positionWSAndFogFactor)
	MY_DOMAIN_PROGRAM_INTERPOLATE(normalWS)
#ifdef _MAIN_LIGHT_SHADOWS
	MY_DOMAIN_PROGRAM_INTERPOLATE(shadowCoord)
#endif
	MY_DOMAIN_PROGRAM_INTERPOLATE(positionCS)
	MY_DOMAIN_PROGRAM_INTERPOLATE(uv)
	MY_DOMAIN_PROGRAM_INTERPOLATE(uvLM)

    return TessellationVertexProgram(data);
}