
uniform float time;

uniform float wind_strength_x;
uniform float wind_strength_y;
uniform float wind_speed_x;
uniform float wind_speed_y;

uniform float vibrate_strength_x;
uniform float vibrate_strength_y;

uniform float sin_strength_x;
uniform float sin_speed_x;
uniform float sin_strength_y;
uniform float sin_speed_y;

uniform float cos_strength_x;
uniform float cos_speed_x;
uniform float cos_strength_y;
uniform float cos_speed_y;

void main()
{
	vec4 vertex = gl_Vertex;
	// ==================================================================================================================
	vertex.x += cos(gl_Vertex.y * 0.02 + (time * wind_speed_x) * 3.8) * wind_strength_x + sin(gl_Vertex.y * 0.02 +
		(time * wind_speed_x) * 6.3) * wind_strength_x * 0.3;
	vertex.y += sin(gl_Vertex.x * 0.02 + (time * wind_speed_y) * 2.4) * wind_strength_y + cos(gl_Vertex.x * 0.02 +
		(time * wind_speed_y) * 5.2) * wind_strength_y * 0.3;
	// ==================================================================================================================
	vertex.x += cos(gl_Vertex.y * time) * vibrate_strength_x;
	vertex.y += sin(gl_Vertex.x * time) * vibrate_strength_y;
	// ==================================================================================================================
	vertex.x += sin(time * sin_speed_x) * sin_strength_x;
	vertex.y += sin(time * sin_speed_y) * sin_strength_y;
	// ==================================================================================================================
	vertex.x += cos(time * cos_speed_x) * cos_strength_x;
	vertex.y += cos(time * cos_speed_y) * cos_strength_y;
	// ==================================================================================================================
	gl_Position = gl_ProjectionMatrix * vertex;
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}