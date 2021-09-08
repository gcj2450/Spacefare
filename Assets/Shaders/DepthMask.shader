Shader "Masked/Mask"
{
    SubShader {
		// render the mask after regular geometry,
		// but before masked geometrty and transparent things
		Tags {"Queue" = "Geometry+10" }
		
		// Draw only in the depth buffer channel, no colors
		ColorMask 0
		ZWrite On
		
		Pass {}
	}
}
