
uniform sampler2D texture;
uniform sampler2D rawtexture;
uniform float time;

uniform bool hasmask;
uniform bool maskout;
uniform float maskred;
uniform float maskgreen;
uniform float maskblue;

uniform float Gamma;
uniform float Desaturation;
uniform float Inversion;
uniform float Contrast;
uniform float Brightness;

uniform float BlinkOpacity;
uniform float BlinkSpeed;

uniform float BlurOpacity;
uniform float BlurStrengthX;
uniform float BlurStrengthY;

uniform float EarthquakeOpacity;
uniform float EarthquakeStrengthX;
uniform float EarthquakeStrengthY;

uniform float StretchOpacity;
uniform float StretchStrengthX;
uniform float StretchStrengthY;
uniform float StretchSpeedX;
uniform float StretchSpeedY;

uniform float OutlineOpacity;
uniform float OutlineOffset;
uniform float OutlineRed;
uniform float OutlineGreen;
uniform float OutlineBlue;

uniform float WaterOpacity;
uniform float WaterStrengthX;
uniform float WaterStrengthY;
uniform float WaterSpeedX;
uniform float WaterSpeedY;

uniform float EdgeOpacity;
uniform float EdgeThreshold;
uniform float EdgeThickness;
uniform float EdgeRed;
uniform float EdgeGreen;
uniform float EdgeBlue;

uniform float PixelateOpacity;
uniform float PixelateThreshold;

uniform float GridOpacityX;
uniform float GridOpacityY;
uniform float GridCellWidth;
uniform float GridCellHeight;
uniform float GridCellSpacingX;
uniform float GridCellSpacingY;
uniform float GridRedX;
uniform float GridGreenX;
uniform float GridBlueX;
uniform float GridRedY;
uniform float GridGreenY;
uniform float GridBlueY;

uniform float FillOpacity;
uniform float FillRed;
uniform float FillGreen;
uniform float FillBlue;

void main(void)
{
	vec2 uv = gl_TexCoord[0].st;
	float alpha = texture2D(texture, uv).a * gl_Color.a;
	vec3 color = texture2D(texture, gl_TexCoord[0].xy);

	float alphaR = texture2D(rawtexture, uv).a * gl_Color.a;
	vec3 colorR = texture2D(rawtexture, gl_TexCoord[0].xy);
	// ==================================================================================================================
	vec2 coord = gl_TexCoord[0].xy;
	vec4 pixelcolor = texture2D(texture, coord);
	float blinkalpha = cos(time * (BlinkSpeed));
	alpha = mix(alpha, min(blinkalpha, pixelcolor.w), BlinkOpacity / 2);
	// ==================================================================================================================
	if (StretchOpacity > 0)
	{
		float factorx = sin(time * StretchSpeedX) * StretchStrengthX;
		float factory = sin(time * StretchSpeedY) * StretchStrengthY;
		coord.x += factorx / (1 + 2 * factorx);
		coord.y += factory / (1 + 2 * factory);
	}
	color = mix(vec4(color, alpha), texture2D(texture, coord), StretchOpacity);
	// ==================================================================================================================
	if (WaterOpacity > 0)
	{
		coord.x += sin(radians(2000 * time * WaterSpeedX + coord.y * 250)) * 0.02 * WaterStrengthX;
		coord.y += cos(radians(2000 * time * WaterSpeedY + coord.x * 500)) * 0.03 * WaterStrengthY;
	}
	color = mix(vec4(color, alpha), texture2D(texture, coord), WaterOpacity);
	// ==================================================================================================================
	if (EarthquakeOpacity > 0)
	{
		coord.x += sin(radians(3000 * time + coord.x * 0)) * cos(time) * EarthquakeStrengthX;
		coord.y += cos(radians(3000 * time + coord.y * 0)) * sin(time) * EarthquakeStrengthY;
	}
	color = mix(vec4(color, alpha), texture2D(texture, coord), EarthquakeOpacity);
	// ==================================================================================================================
	vec2 offx = vec2(BlurStrengthX, 0.0);
	vec2 offy = vec2(0.0, BlurStrengthY);
	vec4 pixel = texture2D(texture, gl_TexCoord[0].xy) * 4.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offx) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offx) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offy) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offy) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offx - offy) * 1.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offx + offy) * 1.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offx - offy) * 1.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offx + offy) * 1.0;

	color = mix(vec4(color, alpha), gl_Color * (pixel / 16.0), BlurOpacity);
	// ==================================================================================================================
	color = mix(color, vec3(FillRed, FillGreen, FillBlue), FillOpacity);
	// ==================================================================================================================
	vec2 vtexCoords = gl_TexCoord[0].xy;
	vec4 col = texture2D(texture, vtexCoords);

	if (col.a != 1)
	{
		float au = texture2D(texture, vec2(vtexCoords.x, vtexCoords.y - OutlineOffset)).a;
		float ad = texture2D(texture, vec2(vtexCoords.x, vtexCoords.y + OutlineOffset)).a;
		float al = texture2D(texture, vec2(vtexCoords.x - OutlineOffset, vtexCoords.y)).a;
		float ar = texture2D(texture, vec2(vtexCoords.x + OutlineOffset, vtexCoords.y)).a;

		if (au == 1.0 || ad == 1.0 || al == 1.0 || ar == 1.0)
		{
			color = mix(color, vec3(OutlineRed, OutlineGreen, OutlineBlue), OutlineOpacity);
			alpha = mix(alpha, 1, OutlineOpacity);
		}
	}
	// ==================================================================================================================
	vec2 offx2 = vec2(1.0 - EdgeThickness, 0.0);
	vec2 offy2 = vec2(0.0, 1.0 - EdgeThickness);
	vec4 hEdge = texture2D(texture, gl_TexCoord[0].xy - offy2) * -2.0 +
				 texture2D(texture, gl_TexCoord[0].xy + offy2) * 2.0 +
				 texture2D(texture, gl_TexCoord[0].xy - offx2 - offy2) * -1.0 +
				 texture2D(texture, gl_TexCoord[0].xy - offx2 + offy2) * 1.0 +
				 texture2D(texture, gl_TexCoord[0].xy + offx2 - offy2) * -1.0 +
				 texture2D(texture, gl_TexCoord[0].xy + offx2 + offy2) * 1.0;

	vec4 vEdge = texture2D(texture, gl_TexCoord[0].xy - offx2) * 2.0 +
				 texture2D(texture, gl_TexCoord[0].xy + offx2) * -2.0 +
				 texture2D(texture, gl_TexCoord[0].xy - offx2 - offy2) * 1.0 +
				 texture2D(texture, gl_TexCoord[0].xy - offx2 + offy2) * -1.0 +
				 texture2D(texture, gl_TexCoord[0].xy + offx2 - offy2) * 1.0 +
				 texture2D(texture, gl_TexCoord[0].xy + offx2 + offy2) * -1.0;

	vec3 result = sqrt(hEdge.rgb * hEdge.rgb + vEdge.rgb * vEdge.rgb);
	float edge = length(result);
	if (edge > (EdgeThreshold * 6.0)) color = mix(color, vec3(EdgeRed, EdgeGreen, EdgeBlue), EdgeOpacity);
	// ==================================================================================================================
	if (mod(gl_FragCoord.x, round(GridCellWidth + GridCellSpacingX)) < round(GridCellSpacingX))
	{
		color = mix(vec4(color, alpha), vec4(GridRedX, GridGreenX, GridBlueX, alpha), GridOpacityX);
	}
	if (mod(gl_FragCoord.y, round(GridCellHeight + GridCellSpacingY)) < round(GridCellSpacingY))
	{
		color = mix(vec4(color, alpha), vec4(GridRedY, GridGreenY, GridBlueY, alpha), GridOpacityY);
	}
	// ==================================================================================================================
	color = pow(color, vec3(1.0 / (1 - Gamma)));
	color = mix(color, vec3(0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b), clamp(Desaturation, 0, 1));
	color = mix(color, vec3(1.0) - color, clamp(Inversion, 0, 1));
	color = color * gl_Color.rgb;
	color = (color - vec3(0.5)) * ((Contrast + 1) / (1 - Contrast)) + 0.5;
	color = vec3(clamp(color.r, 0, 1), clamp(color.g, 0, 1), clamp(color.b, 0, 1));
	color = color + vec3(Brightness);
	color = vec3(clamp(color.r, 0, 1), clamp(color.g, 0, 1), clamp(color.b, 0, 1));
	// ==================================================================================================================
	float factor = 1.0 / (PixelateThreshold + 0.001);
	vec2 pos = floor(gl_TexCoord[0].xy * factor + 0.5) / factor;
	color = mix(vec4(color, alpha), gl_Color * texture2D(texture, pos), PixelateOpacity);
	// ==================================================================================================================
	if (hasmask)
	{
		vec3 mask = vec3(maskred, maskgreen, maskblue);

		if (maskout)
		{
			if (color == mask) alpha = 0.0;
		}
		else
		{
			if (color != mask) alpha = 0.0;
			else color = colorR;
		}
	}
	// ==================================================================================================================
	gl_FragColor = vec4(color, alpha);
}