Shader "Custom/ThickOutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        // Invert culling so that only the outline shell is visible.
        Cull Front

        Pass
        {
            Name "OutlinePass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float _OutlineThickness;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            // This vertex shader scales the mesh around its pivot.
            v2f vert(appdata v)
            {
                v2f o;
                // Compute a uniform scaling factor.
                float scale = 1.0 + _OutlineThickness;
                // Scale the vertex position uniformly.
                float4 scaledPos = v.vertex * scale;
                o.pos = UnityObjectToClipPos(scaledPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
