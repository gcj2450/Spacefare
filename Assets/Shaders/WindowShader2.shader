Shader "Windows/WindowShader2"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-10" }
        Pass 
		{
			Stencil {
				Ref 2
				Comp Always
				Pass Replace
			}
			ZWrite Off

			
		}
    }
}