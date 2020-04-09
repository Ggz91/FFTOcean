Shader "Unlit/FFTOcean"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _OceanHeightMap;
            sampler2D _OceanDisplaceMap;
            sampler2D _OceanNormalMap;
            float _OceanScale[3];

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                float height = tex2Dlod(_OceanHeightMap, float4(o.uv.x, o.uv.y, 0, 1)).r;
                float3 displace = tex2Dlod(_OceanDisplaceMap, float4(o.uv.x, o.uv.y, 0, 1)).rgb;
                float4 real_pos = v.vertex;
                real_pos.y += height * _OceanScale[1];
                real_pos.x += displace.x * _OceanScale[0];
                real_pos.z += displace.z * _OceanScale[2];
                o.vertex = UnityObjectToClipPos(real_pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.uv.x, 0, 0,1);
            }
            ENDCG
        }
    }
}
