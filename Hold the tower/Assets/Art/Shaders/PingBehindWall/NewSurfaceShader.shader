// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/NewSurfaceShader Texture"
{
    Properties
    {
        ObjectColorBehindWall("Color Behind", Color) = (1, 1, 1, 1)
        ObjectColorInFront("Color In Front ", Color) = (1, 1, 1, 1)
    }

        SubShader
    {
        Tags {
            "Queue" = "Transparent+10"
        }

        Pass
        {
            ZWrite On
            ZTest Greater
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            uniform fixed4 ObjectColorInFront;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = ObjectColorInFront;
                o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }
    }
}
