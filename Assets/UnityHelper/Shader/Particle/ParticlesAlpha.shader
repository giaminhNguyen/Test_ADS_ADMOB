// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mobile/Particles/Alpha"
{
    Properties
    {
        [HDR]_TintColor("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
        
        [Toggle]_UseLerpColor("Use Lerp Color",Float) = 0
        
        [HDR]_DarkColor("Dark Color", Color) = (0.5, 0.5, 0.5, 0.5)
        [HDR]_LightColor("Dark Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _MainTex("Particle Texture", 2D) = "white" {}
        _Boost("Boost",Range(1,10)) = 1
        _AlphaClipThreshold("Alpha Clip Threshold",Range(0, 1)) = 0.1
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _MainTex_ST;
    fixed4 _TintColor;
    fixed4 _DarkColor;
    fixed4 _LightColor;
    float _AlphaClipThreshold;
    float _UseLerpColor;
    float _Boost;

    struct appdata_t
    {
        fixed4 color : COLOR0;
        float4 position : POSITION;
        float4 texcoord : TEXCOORD0;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        fixed4 color : COLOR0;
        UNITY_FOG_COORDS(1)
    };

    v2f vert(appdata_t v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color = v.color;
        UNITY_TRANSFER_FOG(o, o.vertex);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        
        fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
        
        float aClip = 1;
        if(col.a <= _AlphaClipThreshold)
        {
            aClip = -1;
        }
        
        clip (aClip);
        
        fixed4 color = _TintColor;
        if(_UseLerpColor > 0)
        {
            color = lerp(_DarkColor,_LightColor,col.a);
        }
        col *= color * _Boost;
        
        UNITY_APPLY_FOG_COLOR(i.fogCoord, col, (fixed4)0);
        
        return col;
    }

    ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            ENDCG
        }
    }
}