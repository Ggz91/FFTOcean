Shader "Unlit/FFTOcean"
{
    Properties
    {
        _WaterColor("WaterColor", Color) = (0.1, 0.3, 0.8, 1)
        _Gloss("SpecGloss", float) = 128
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _OceanHeightMap;
            sampler2D _OceanDisplaceMap;
            sampler2D _OceanNormalMap;
            sampler2D _OceanJacobMap;
            float _OceanScale[3];
            float4 _WaterColor;
            float _Gloss;

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD2; //是否为尖浪
                float3 world_pos : TEXCOORD3; //世界坐标
            };

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

                //尖浪相关
                float jacob = tex2Dlod(_OceanJacobMap, float4(o.uv.x, o.uv.y, 0, 1)).r;
                o.color = lerp(float4(0, 0, 0, 1), float4(1, 1, 1, 1), jacob);

                //世界坐标
                o.world_pos = mul(unity_ObjectToWorld, v.vertex).rgb;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //尖浪直接返回
                if(i.color.r > 0.9f )
                {
                    //return i.color;
                }
                float4 normal = float4(1, 1, 1, 1);
                normal.xyz = normalize(tex2D(_OceanNormalMap, i.uv).rgb);
                //计算光照影响
                //反射光
                float4 res = _WaterColor;
                float rel_factor = saturate(dot(normalize(_WorldSpaceLightPos0.xyz), normal.xyz));
                float4 relfection = rel_factor * _LightColor0 * _WaterColor;
                res.rgb += relfection.rgb;
                //高光
                float3 view_dir = _WorldSpaceCameraPos.xyz - i.world_pos;
                float4 half_reflect = float4(normalize(view_dir + _WorldSpaceLightPos0), 1);
                float spe_factor = saturate(dot(half_reflect, normal));
                float4 specture = 0.1 * pow(spe_factor, _Gloss) * _LightColor0;
                res.rgb += specture.rgb;
                return res;
            }
            ENDCG
        }
    }
}
