Shader "Custom/TextureShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _LayerWeight("Layer Weight", Int) = 1
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _textureArray("Texture Array", 2DArray) = "white" {}
    }
        SubShader
        {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        // Require 2D Array
        #pragma require 2darray

       
            

        UNITY_DECLARE_TEX2DARRAY(_textureArray);

        struct Input
        {
            float2 uv_textureArray;
        };
        static const int NUMBER_OF_LAYERS = 32;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _totalWeight;
        half _layerWeight[NUMBER_OF_LAYERS];
		float _scrollTimer[NUMBER_OF_LAYERS];
        //Displace layer [index] with data from index. 0 for no displacement layer, 1-32 for displacement from layer (1-32)-1)
        int _displacementIndex[NUMBER_OF_LAYERS];
        bool _displacementLayer[NUMBER_OF_LAYERS];
        float _scrollDirection[2];
        fixed _scrollDirectionX;
        fixed _scrollDirectionY;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Calculate displacement
            fixed4 d = UNITY_SAMPLE_TEX2DARRAY(_textureArray, float3((IN.uv_textureArray.x + (_scrollDirection[0] * _scrollTimer[0])), (IN.uv_textureArray.y + (_scrollDirection[1] * _scrollTimer[0])), 1));
            fixed displacement = (d.r + d.g + d.b) / 3;
            displacement -= 0.5;
            //displacement = 0;
            fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_textureArray, float3(IN.uv_textureArray.x + displacement, IN.uv_textureArray.y + displacement, 0)) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
