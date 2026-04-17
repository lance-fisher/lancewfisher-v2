Shader "Bloodlines/VectorImport/VectorGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
        }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex GradientVert
            #pragma fragment GradientFrag

            #include "UnityCG.cginc"
            #include "VectorGradient.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 settingIndex : TEXCOORD2;
            };

            struct v2f
            {
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 settingIndex : TEXCOORD2;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            v2f GradientVert(appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.color = IN.color;
                OUT.color *= _Color;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.settingIndex = IN.settingIndex;
                return OUT;
            }

            fixed4 GradientFrag(v2f i) : SV_Target
            {
                fixed4 gradientColor = EvaluateGradient(i.settingIndex.x, i.uv, _MainTex, _MainTex_TexelSize.xy);
                return gradientColor * i.color;
            }
            ENDCG
        }
    }
}
