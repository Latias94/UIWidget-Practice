#ifndef MTOON_CORE_INCLUDED
#define MTOON_CORE_INCLUDED

#include "Lighting.cginc"
#include "AutoLight.cginc"

half _Cutoff;
fixed4 _Color;
fixed4 _ShadeColor;
sampler2D _MainTex; float4 _MainTex_ST;
sampler2D _ShadeTexture; float4 _ShadeTexture_ST;
half _BumpScale;
sampler2D _BumpMap; float4 _BumpMap_ST;
sampler2D _ReceiveShadowTexture; float4 _ReceiveShadowTexture_ST;
half _ReceiveShadowRate;
sampler2D _ShadingGradeTexture; float4 _ShadingGradeTexture_ST;
half _ShadingGradeRate;
half _ShadeShift;
half _ShadeToony;
half _LightColorAttenuation;
half _IndirectLightIntensity;
sampler2D _SphereAdd;
fixed4 _EmissionColor;
sampler2D _EmissionMap; float4 _EmissionMap_ST;
sampler2D _OutlineWidthTexture; float4 _OutlineWidthTexture_ST;
half _OutlineWidth;
half _OutlineUseSoftNormal;
half _OutlineScaledMaxDistance;
fixed4 _OutlineColor;
half _OutlineLightingMix;

//UNITY_INSTANCING_BUFFER_START(Props)
//UNITY_INSTANCING_BUFFER_END(Props)

struct v2f
{
    float4 pos : SV_POSITION;
    float4 posWorld : TEXCOORD0;
    half3 tspace0 : TEXCOORD1;
    half3 tspace1 : TEXCOORD2;
    half3 tspace2 : TEXCOORD3;
    float2 uv0 : TEXCOORD4;
    float isOutline : TEXCOORD5;
    fixed4 color : TEXCOORD6;
    LIGHTING_COORDS(7,8)
    UNITY_FOG_COORDS(9)
    //UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if any instanced properties are going to be accessed in the fragment Shader.
};

inline v2f InitializeV2F(appdata_full v, float4 projectedVertex, float isOutline)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    //UNITY_TRANSFER_INSTANCE_ID(v, o);
    
    o.pos = projectedVertex;
    o.posWorld = mul(unity_ObjectToWorld, v.vertex);
    o.uv0 = v.texcoord;
    half3 worldNormal = UnityObjectToWorldNormal(v.normal);
    half3 worldTangent = UnityObjectToWorldDir(v.tangent);
    half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
    half3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
    o.tspace0 = half3(worldTangent.x, worldBitangent.x, worldNormal.x);
    o.tspace1 = half3(worldTangent.y, worldBitangent.y, worldNormal.y);
    o.tspace2 = half3(worldTangent.z, worldBitangent.z, worldNormal.z);
    o.isOutline = isOutline;
    o.color = v.color;
    TRANSFER_VERTEX_TO_FRAGMENT(o);
    UNITY_TRANSFER_FOG(o, o.pos);
    return o;
}

inline float4 CalculateOutlineVertexClipPosition(appdata_full v)
{
    float4 nearUpperRight = mul(unity_CameraInvProjection, float4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));
    float aspect = abs(nearUpperRight.y / nearUpperRight.x);
    
    float outlineTex = tex2Dlod(_OutlineWidthTexture, float4(TRANSFORM_TEX(v.texcoord, _OutlineWidthTexture), 0, 0)).r;
   
 #if defined(MTOON_OUTLINE_WIDTH_WORLD)
    float3 outlineOffset = 0.01 * _OutlineWidth * outlineTex * v.normal;
    float4 vertex = UnityObjectToClipPos(v.vertex + outlineOffset);
 #elif defined(MTOON_OUTLINE_WIDTH_SCREEN)
    float4 vertex = UnityObjectToClipPos(v.vertex);
    float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal.xyz);
    float3 clipNormal = TransformViewToProjection(viewNormal.xyz);
    float2 projectedNormal = normalize(clipNormal.xy);
    projectedNormal *= min(vertex.w, _OutlineScaledMaxDistance);
    projectedNormal.x *= aspect;
    vertex.xy += 0.01 * _OutlineWidth * outlineTex * projectedNormal.xy;
#elif defined(MTOON_OUTLINE_WIDTH_WORLD_VERTEX_COLOR)
	float3 normal = lerp(v.normal, v.tangent, _OutlineUseSoftNormal);
	float3 outlineOffset = 0.01 * v.color.x *_OutlineWidth * outlineTex * normal;
	float4 vertex = UnityObjectToClipPos(v.vertex + outlineOffset);
#elif defined(MTOON_OUTLINE_WIDTH_SCREEN_VERTEX_COLOR)
	float3 normal = lerp(v.normal, v.tangent, _OutlineUseSoftNormal);
	float4 vertex = UnityObjectToClipPos(v.vertex);
	float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, normal.xyz);
	float3 clipNormal = TransformViewToProjection(viewNormal.xyz);
	float2 projectedNormal = normalize(clipNormal.xy);
	projectedNormal *= min(vertex.w, _OutlineScaledMaxDistance);
	projectedNormal.x *= aspect;
	vertex.xy += 0.01 * v.color.x  * _OutlineWidth * outlineTex * projectedNormal.xy;
#else
    float4 vertex = UnityObjectToClipPos(v.vertex);
 #endif
    return vertex;
}

float4 frag_forward(v2f i, fixed facing : VFACE) : SV_TARGET
{
#ifdef MTOON_CLIP_IF_OUTLINE_IS_NONE
    #ifdef MTOON_OUTLINE_WIDTH_WORLD
    #elif MTOON_OUTLINE_WIDTH_SCREEN
	#elif MTOON_OUTLINE_WIDTH_WORLD_VERTEX_COLOR
	#elif MTOON_OUTLINE_WIDTH_SCREEN_VERTEX_COLOR
	#else
        clip(-1);
    #endif
#endif

    //UNITY_TRANSFER_INSTANCE_ID(v, o);
    
    // main tex
    half4 mainTex = tex2D(_MainTex, TRANSFORM_TEX(i.uv0, _MainTex));
    
    // alpha
    half alpha = 1;
#ifdef _ALPHATEST_ON
    alpha = _Color.a * mainTex.a;
    clip(alpha - _Cutoff);
#endif
#ifdef _ALPHABLEND_ON
    alpha = _Color.a * mainTex.a;
#endif
    
    // normal
#ifdef _NORMALMAP
    half3 tangentNormal = UnpackScaleNormal(tex2D(_BumpMap, TRANSFORM_TEX(i.uv0, _BumpMap)), _BumpScale);
    half3 worldNormal;
    worldNormal.x = dot(i.tspace0, tangentNormal);
    worldNormal.y = dot(i.tspace1, tangentNormal);
    worldNormal.z = dot(i.tspace2, tangentNormal);
#else
    half3 worldNormal = half3(i.tspace0.z, i.tspace1.z, i.tspace2.z);
#endif
    worldNormal *= facing;
    worldNormal *= lerp(+1.0, -1.0, i.isOutline);
    worldNormal = normalize(worldNormal);

    // lighting intensity
    half3 lightDir = lerp(_WorldSpaceLightPos0.xyz, normalize(_WorldSpaceLightPos0.xyz - i.posWorld.xyz), _WorldSpaceLightPos0.w);
    half receiveShadow = _ReceiveShadowRate * tex2D(_ReceiveShadowTexture, TRANSFORM_TEX(i.uv0, _ReceiveShadowTexture)).a;
    half shadingGrade = 1.0 - _ShadingGradeRate * (1.0 - tex2D(_ShadingGradeTexture, TRANSFORM_TEX(i.uv0, _ShadingGradeTexture)).r);
    UNITY_LIGHT_ATTENUATION(atten, i, i.posWorld.xyz);
    half lightIntensity = dot(lightDir, worldNormal);
    lightIntensity = lightIntensity * 0.5 + 0.5; // from [-1, +1] to [0, 1]
    lightIntensity = lightIntensity * (1.0 - receiveShadow * (1.0 - (atten * 0.5 + 0.5))); // receive shadow
    lightIntensity = lightIntensity * shadingGrade; // darker
    lightIntensity = lightIntensity * 2.0 - 1.0; // from [0, 1] to [-1, +1]
    lightIntensity = smoothstep(_ShadeShift, _ShadeShift + (1.0 - _ShadeToony), lightIntensity); // shade & tooned

    // lighting with color
    half3 directLighting = lightIntensity * _LightColor0.rgb; // direct
    half3 indirectLighting = _IndirectLightIntensity * ShadeSH9(half4(worldNormal, 1)); // ambient
    half3 lighting = directLighting + indirectLighting;
    lighting = lerp(lighting, max(0.001, max(lighting.x, max(lighting.y, lighting.z))), _LightColorAttenuation); // color atten
    
    // color lerp
    half4 shade = _ShadeColor * tex2D(_ShadeTexture, TRANSFORM_TEX(i.uv0, _ShadeTexture));
    half4 lit = _Color * mainTex;
#ifdef MTOON_FORWARD_ADD
    half3 col = lerp(half3(0,0,0), saturate(lit.rgb - shade.rgb), lighting);
#else
    half3 col = lerp(shade.rgb, lit.rgb, lighting);
#endif

    // additive matcap
#ifdef MTOON_FORWARD_ADD
#else
    half3 worldCameraUp = normalize(UNITY_MATRIX_V[1].xyz);
    half3 worldView = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
    half3 worldViewUp = normalize(worldCameraUp - worldView * dot(worldView, worldCameraUp));
    half3 worldViewRight = normalize(cross(worldView, worldViewUp));
    half2 rimUv = half2(dot(worldViewRight, worldNormal), dot(worldViewUp, worldNormal)) * 0.5 + 0.5;
    half3 rimLighting = tex2D(_SphereAdd, rimUv);
    col += lerp(rimLighting, half3(0, 0, 0), i.isOutline);
#endif

    // Emission
#ifdef MTOON_FORWARD_ADD
#else
    half3 emission = tex2D(_EmissionMap, TRANSFORM_TEX(i.uv0, _EmissionMap)).rgb * _EmissionColor.rgb;
    col += lerp(emission, half3(0, 0, 0), i.isOutline);
#endif

    // outline
#ifdef MTOON_OUTLINE_COLOR_FIXED
    col = lerp(col, _OutlineColor, i.isOutline);
#elif MTOON_OUTLINE_COLOR_MIXED
    col = lerp(col, _OutlineColor * lerp(half3(1, 1, 1), col, _OutlineLightingMix), i.isOutline);
#else
#endif

    // debug
#ifdef MTOON_DEBUG_NORMAL
    #ifdef MTOON_FORWARD_ADD
        return float4(0, 0, 0, 0);
    #else
        return float4(worldNormal * 0.5 + 0.5, alpha);
    #endif
#elif MTOON_DEBUG_LITSHADERATE
    #ifdef MTOON_FORWARD_ADD
        return float4(0, 0, 0, 0);
    #else
        return float4(lighting, alpha);
    #endif
#endif


    half4 result = half4(col, alpha);
    UNITY_APPLY_FOG(i.fogCoord, result);

	return result;
}

#endif // MTOON_CORE_INCLUDED
