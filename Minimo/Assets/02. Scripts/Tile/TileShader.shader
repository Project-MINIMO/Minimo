Shader "Custom/TileShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  
        _Transparency ("Transparency Factor", Range(0,1)) = 0.5
        _WorldYThreshold ("World Y Threshold", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Transparency;
            float _WorldYThreshold;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex  : SV_POSITION;
                float2 uv      : TEXCOORD0;
                float3 worldPos: TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // 월드 좌표 계산
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // 각 타일의 월드 Y 좌표가 threshold보다 높으면 투명도 조정
                if(i.worldPos.y > _WorldYThreshold)
                {
                    col.a *= _Transparency;
                }
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}