Shader "Custom/TextureShader"
{
    Properties
    {

    }
    SubShader
    {
    Tags {"RenderType" = "Opaque"}
    LOD 200
        /*
        Tags {"Queue" = "Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        */
        CGPROGRAM

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows //alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        // Require 2D Array
        #pragma require 2darray

        #define NUMBER_OF_LAYERS 32


        UNITY_DECLARE_TEX2DARRAY(_textureArray);

        struct Input
        {
            float2 uv_textureArray;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _color[NUMBER_OF_LAYERS];
        float _totalWeight;
        half _layerWeight[NUMBER_OF_LAYERS];
        float _scrollTimer[NUMBER_OF_LAYERS];
        float _textureScale[NUMBER_OF_LAYERS];
        //Displace layer [index] with data from index. 0 for no displacement layer, 1-32 for displacement from layer (1-32)-1)
        float _displacementIndex[NUMBER_OF_LAYERS];
        //float _scrollDirection[2];
        float _scrollDirectionX[NUMBER_OF_LAYERS];
        float _scrollDirectionY[NUMBER_OF_LAYERS];
        float _scrollOffsetX[NUMBER_OF_LAYERS];
        float _scrollOffsetY[NUMBER_OF_LAYERS];
        fixed4 t[NUMBER_OF_LAYERS];
        fixed displace[NUMBER_OF_LAYERS];

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {

            // Calculate displacement   Using built-in time
            /*[unroll(NUMBER_OF_LAYERS)]
            for (int currentLayer = 0; currentLayer < NUMBER_OF_LAYERS; currentLayer++) {
                t[currentLayer] = UNITY_SAMPLE_TEX2DARRAY(_textureArray, float3((IN.uv_textureArray.x + _scrollOffsetX[currentLayer] + (_scrollDirectionX[currentLayer] * _Time.y)) * _textureScale[currentLayer], (IN.uv_textureArray.y + _scrollOffsetY[currentLayer] + (_scrollDirectionY[currentLayer] * _Time.y)) * _textureScale[currentLayer], currentLayer));
                displace[currentLayer] = (t[currentLayer].r + t[currentLayer].g + t[currentLayer].b) / 3;
                displace[currentLayer] -= 0.5;
            }*/

            // Calculate displacement
            [unroll(NUMBER_OF_LAYERS)]
            for (int currentLayer = 0; currentLayer < NUMBER_OF_LAYERS; currentLayer++) {
                t[currentLayer] = UNITY_SAMPLE_TEX2DARRAY(_textureArray, float3((IN.uv_textureArray.x + _scrollOffsetX[currentLayer] + (_scrollDirectionX[currentLayer] * _scrollTimer[currentLayer])) * _textureScale[currentLayer], (IN.uv_textureArray.y + _scrollOffsetY[currentLayer] + (_scrollDirectionY[currentLayer] * _scrollTimer[currentLayer])) * _textureScale[currentLayer], currentLayer));
                displace[currentLayer] = (t[currentLayer].r + t[currentLayer].g + t[currentLayer].b) / 3;
                displace[currentLayer] -= 0.5;
            }

            // 2nd time for displacement by displaced textures
            [unroll(NUMBER_OF_LAYERS)]
            for (currentLayer = 0; currentLayer < NUMBER_OF_LAYERS; currentLayer++) {
                if (_displacementIndex[currentLayer] != 0) {
                    t[currentLayer] = UNITY_SAMPLE_TEX2DARRAY(_textureArray, float3(((IN.uv_textureArray.x + _scrollOffsetX[currentLayer] + (_scrollDirectionX[currentLayer] * _scrollTimer[currentLayer]) + displace[_displacementIndex[currentLayer] - 1]) * _textureScale[currentLayer]), ((IN.uv_textureArray.y + _scrollOffsetY[currentLayer] + (_scrollDirectionY[currentLayer] * _scrollTimer[currentLayer]) + displace[_displacementIndex[currentLayer] - 1]) * _textureScale[currentLayer]), currentLayer));
                    displace[currentLayer] = (t[currentLayer].r + t[currentLayer].g + t[currentLayer].b) / 3;
                    displace[currentLayer] -= 0.5;
                }
            }


            [unroll(NUMBER_OF_LAYERS)]
            for (currentLayer = 0; currentLayer < NUMBER_OF_LAYERS; currentLayer++) {
                if (_displacementIndex[currentLayer] != 0) {
                    t[currentLayer] = UNITY_SAMPLE_TEX2DARRAY(_textureArray, float3(((IN.uv_textureArray.x + _scrollOffsetX[currentLayer] + (_scrollDirectionX[currentLayer] * _scrollTimer[currentLayer]) + displace[_displacementIndex[currentLayer] - 1]) * _textureScale[currentLayer]), ((IN.uv_textureArray.y + _scrollOffsetY[currentLayer] + (_scrollDirectionY[currentLayer] * _scrollTimer[currentLayer]) + 0.9*displace[_displacementIndex[currentLayer] - 1]) * _textureScale[currentLayer]), currentLayer));
                }
                _totalWeight -= _layerWeight[currentLayer] * (1 - t[currentLayer].a);
            }

            // Metallic and smoothness come from slider variables
            o.Albedo = 0;
            [unroll(NUMBER_OF_LAYERS)]
            for (currentLayer = 0; currentLayer < NUMBER_OF_LAYERS; currentLayer++)
                o.Albedo += t[currentLayer].rgb * _color[currentLayer] * _layerWeight[currentLayer] * t[currentLayer].a/_totalWeight;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;        // For transparent shader // (t[0].a * _layerWeight[0] + t[1].a * _layerWeight[1] + t[2].a * _layerWeight[2] + t[3].a * _layerWeight[3] + t[4].a * _layerWeight[4] + t[5].a * _layerWeight[5] + t[6].a * _layerWeight[6] + t[7].a * _layerWeight[7] + t[8].a * _layerWeight[8] + t[9].a * _layerWeight[9] + t[10].a * _layerWeight[10] + t[11].a * _layerWeight[11] + t[12].a * _layerWeight[12] + t[13].a * _layerWeight[13] + t[14].a * _layerWeight[14] + t[15].a * _layerWeight[15] + t[16].a * _layerWeight[16] + t[17].a * _layerWeight[17] + t[18].a * _layerWeight[18] + t[19].a * _layerWeight[19] + t[20].a * _layerWeight[20] + t[21].a * _layerWeight[21] + t[22].a * _layerWeight[22] + t[23].a * _layerWeight[23] + t[24].a * _layerWeight[24] + t[25].a * _layerWeight[25] + t[26].a * _layerWeight[26] + t[27].a * _layerWeight[27] + t[28].a * _layerWeight[28] + t[29].a * _layerWeight[29] + t[30].a * _layerWeight[30] + t[31].a * _layerWeight[31]) / _totalWeight;
            
        }
        ENDCG
    }
        FallBack "Diffuse"
}
