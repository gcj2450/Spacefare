Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        //_Color ("Main Color", Color) = (1,.5,.5,1)
        
    }
    SubShader
    {
		// a single pass
		Pass
		{
			// use fixed function per-vertex lighting
			Material
			{
				//Diffuse [_Color]
			}
			Lighting On
		}
		
		Pass
		{
			ZTest Always
		}
        
    }
    FallBack "Diffuse"
}
