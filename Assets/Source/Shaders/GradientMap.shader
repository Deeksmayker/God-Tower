Shader"Hidden/GradientMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientMap ("Gradient Map", 2D) = "white" {}
        //_Intensity ("Intensity", float) = 1; 
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex, _GradientMap;
float _Intensity;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				float grayscale = (col.r + col.g + col.b) / 3;
				grayscale *= _Intensity;
				fixed4 gradient = tex2D(_GradientMap, grayscale);
                gradient *= gradient.a;
                gradient += (1 - gradient.a) * col;
                
                return gradient;
            }
            ENDCG
        }
    }
}
