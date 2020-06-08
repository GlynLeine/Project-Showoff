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

//struct Attributes
//{
//    float4 positionOS : POSITION;
//    float3 normalOS : NORMAL;
//    float2 uv : TEXCOORD0;
//    float2 uvLM : TEXCOORD1;
//				UNITY_VERTEX_INPUT_INSTANCE_ID
//};

Varyings TessellationVertexProgram(Varyings input, OutputPatch<Varyings, 3> patch, float3 barycentricCoordinates)
{
    float3 positionWS = input.positionWSAndFogFactor.xyz;

    float3 toThis = SafeNormalize(positionWS - _WorldCenter.xyz);

    float3 normalWS = SafeNormalize(input.normalWS);
    
    float snowFactor = clamp(snoise(float2(dot(normalWS, float3(0.0, 1.0, 0.0)), dot(normalWS, float3(1.0, 0.0, 0.0))) * _NoiseScale), 0.0, 1.0);
    snowFactor = clamp(snowFactor + lerp(1.0, -1.0, _Pollution), 0.0, 1.0);
    
    float snowy = clamp(pow(clamp(dot(toThis, input.normalWS), 0, 1), _SnowThreshold), 0.0, 1.0);
    snowy = lerp(0.0, snowy, snowFactor);
    
    float displacement = _Displacement * snowy * 0.001 * smoothstep(2.0 / 3.0, 1.0, _SeasonTime);
    float edgeDistance = min(min(barycentricCoordinates.x, barycentricCoordinates.y), barycentricCoordinates.z);
    if (edgeDistance > 0.0)
        positionWS += normalWS * displacement;
    
    if (_RecalculateNormals >= 0.5)
    {
        float3 bcca = float3(barycentricCoordinates.x + 0.001, barycentricCoordinates.yz);
        float3 bccb = float3(barycentricCoordinates.x, barycentricCoordinates.y + 0.001, barycentricCoordinates.z);
        float3 bccc = float3(barycentricCoordinates.xy, barycentricCoordinates.z + 0.001);
    
        float3 a = patch[0].positionWSAndFogFactor.xyz * bcca.x + patch[1].positionWSAndFogFactor.xyz * bcca.y + patch[2].positionWSAndFogFactor.xyz * bcca.z;
        float3 b = patch[0].positionWSAndFogFactor.xyz * bccb.x + patch[1].positionWSAndFogFactor.xyz * bccb.y + patch[2].positionWSAndFogFactor.xyz * bccb.z;
        float3 c = patch[0].positionWSAndFogFactor.xyz * bccc.x + patch[1].positionWSAndFogFactor.xyz * bccc.y + patch[2].positionWSAndFogFactor.xyz * bccc.z;
    
        a += normalWS * displacement * (min(min(bcca.x, bcca.y), bcca.z) + 0.1);
        b += normalWS * displacement * (min(min(bccb.x, bccb.y), bccb.z) + 0.1);
        c += normalWS * displacement * (min(min(bccc.x, bccc.y), bccc.z) + 0.1);
    
        normalWS = normalize(cross(b - a, c - a));
    }
    
    Varyings output;

    VertexPositionInputs vertexInput = GetVertexPositionInputs(TransformWorldToObject(positionWS));

    VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(TransformWorldToObjectNormal(normalWS));

    float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    output.uv = input.uv;
    output.uvLM = input.uvLM;

    output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
    output.normalWS = vertexNormalInput.normalWS;

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
	output.shadowCoord = GetShadowCoord(vertexInput);
#endif
    output.positionCS = vertexInput.positionCS;

    toThis = SafeNormalize(vertexInput.positionWS - _WorldCenter.xyz);

    snowy = clamp(pow(clamp(dot(toThis, vertexNormalInput.normalWS), 0, 1), _SnowThreshold), 0.0, 1.0) * smoothstep(2.0 / 3.0, 1.0, _SeasonTime);

    output.snowy = lerp(0.0, snowy, snowFactor);
    
    return output;
}

TessellationFactors patchConstantFunction(InputPatch<Varyings, 3> patch)
{
#define Average(field) ((patch[0].field + patch[1].field + patch[2].field)/3.0)
    
    float3 positionWS = Average(positionWSAndFogFactor).xyz;

    float3 toThis = SafeNormalize(positionWS - _WorldCenter.xyz);

    float snowy = clamp(pow(clamp(dot(toThis, Average(normalWS)), 0, 1), _SnowThreshold), 0.0, _SeasonTime > (2.0 / 3.0) ? 1.0 : 0.0);
    
    float tessellationFactor = max(_TessellationUniform * snowy, 1.0);
    
    TessellationFactors f;
    f.edge[0] = tessellationFactor;
    f.edge[1] = tessellationFactor;
    f.edge[2] = tessellationFactor;
    f.inside = tessellationFactor;
    return f;
}

[UNITY_domain("tri")]
[UNITY_outputcontrolpoints(3)]
[UNITY_outputtopology("triangle_cw")]
[UNITY_partitioning("fractional_odd")]
[UNITY_patchconstantfunc("patchConstantFunction")]
Varyings hull(InputPatch<Varyings, 3> patch, uint id : SV_OutputControlPointID)
{
    return patch[id];
}

[UNITY_domain("tri")]
Varyings domain(TessellationFactors factors, OutputPatch<Varyings, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
{
    Varyings data;
#define Interpolate(field) patch[0].field*barycentricCoordinates.x + patch[1].field*barycentricCoordinates.y + patch[2].field*barycentricCoordinates.z

    data.positionCS = Interpolate(positionCS); // POSITION;
    data.normalWS = Interpolate(normalWS); // NORMAL;
    data.positionWSAndFogFactor = Interpolate(positionWSAndFogFactor); // TANGENT;
    data.uv = Interpolate(uv); // TEXCOORD0;
    data.uvLM = Interpolate(uvLM); // TEXCOORD1;
    
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    data.shadowCoord = Interpolate(shadowCoord);
#endif
    
    return TessellationVertexProgram(data, patch, barycentricCoordinates);
}