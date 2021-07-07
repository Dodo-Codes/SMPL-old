
uniform float Time;

uniform float WindStrengthX;
uniform float WindStrengthY;
uniform float WindSpeedX;
uniform float WindSpeedY;

uniform float VibrateStrengthX;
uniform float VibrateStrengthY;

uniform float SinStrengthX;
uniform float SinSpeedX;
uniform float SinStrengthY;
uniform float SinSpeedY;

uniform float CosStrengthX;
uniform float CosSpeedX;
uniform float CosStrengthY;
uniform float CosSpeedY;

void main()
{
	vec4 vertex = gl_Vertex;
	// ==================================================================================================================
	vertex.x += cos(gl_Vertex.y * 0.02 + (Time * WindSpeedX) * 3.8) * WindStrengthX + sin(gl_Vertex.y * 0.02 +
		(Time * WindSpeedX) * 6.3) * WindStrengthX * 0.3;
	vertex.y += sin(gl_Vertex.x * 0.02 + (Time * WindSpeedY) * 2.4) * WindStrengthY + cos(gl_Vertex.x * 0.02 +
		(Time * WindSpeedY) * 5.2) * WindStrengthY * 0.3;
	// ==================================================================================================================
	vertex.x += cos(gl_Vertex.y * Time) * VibrateStrengthX;
	vertex.y += sin(gl_Vertex.x * Time) * VibrateStrengthY;
	// ==================================================================================================================
	vertex.x += sin(Time * SinSpeedX) * SinStrengthX;
	vertex.y += sin(Time * SinSpeedY) * SinStrengthY;
	// ==================================================================================================================
	vertex.x += cos(Time * CosSpeedX) * CosStrengthX;
	vertex.y += cos(Time * CosSpeedY) * CosStrengthY;
	// ==================================================================================================================
	gl_Position = gl_ProjectionMatrix * vertex;
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}