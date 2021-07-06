
uniform float time;

uniform float windstrengthx;
uniform float windstrengthy;
uniform float windspeedx;
uniform float windspeedy;

uniform float vibratestrengthx;
uniform float vibratestrengthy;

uniform float sinstrengthx;
uniform float sinspeedx;
uniform float sinstrengthy;
uniform float sinspeedy;

uniform float cosstrengthx;
uniform float cosspeedx;
uniform float cosstrengthy;
uniform float cosspeedy;

void main()
{
	vec4 vertex = gl_Vertex;
	// ==================================================================================================================
	vertex.x += cos(gl_Vertex.y * 0.02 + (time * windspeedx) * 3.8) * windstrengthx + sin(gl_Vertex.y * 0.02 +
		(time * windspeedx) * 6.3) * windstrengthx * 0.3;
	vertex.y += sin(gl_Vertex.x * 0.02 + (time * windspeedy) * 2.4) * windstrengthy + cos(gl_Vertex.x * 0.02 +
		(time * windspeedy) * 5.2) * windstrengthy * 0.3;
	// ==================================================================================================================
	vertex.x += cos(gl_Vertex.y * time) * vibratestrengthx;
	vertex.y += sin(gl_Vertex.x * time) * vibratestrengthy;
	// ==================================================================================================================
	vertex.x += sin(time * sinspeedx) * sinstrengthx;
	vertex.y += sin(time * sinspeedy) * sinstrengthy;
	// ==================================================================================================================
	vertex.x += cos(time * cosspeedx) * cosstrengthx;
	vertex.y += cos(time * cosspeedy) * cosstrengthy;
	// ==================================================================================================================
	gl_Position = gl_ProjectionMatrix * vertex;
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}