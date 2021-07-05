
uniform sampler2D texture;
uniform sampler2D raw_texture;
uniform float time;

uniform bool has_mask;
uniform bool mask_out;
uniform float mask_red;
uniform float mask_green;
uniform float mask_blue;

uniform float adjust_gamma;
uniform float adjust_desaturation;
uniform float adjust_inversion;
uniform float adjust_contrast;
uniform float adjust_brightness;

uniform float blink_effect;
uniform float blink_speed;

uniform float blur_effect;
uniform float blur_strength_x;
uniform float blur_strength_y;

uniform float earthquake_effect;
uniform float earthquake_effect_x;
uniform float earthquake_effect_y;

uniform float stretch_effect;
uniform float stretch_effect_x;
uniform float stretch_effect_y;
uniform float stretch_speed_x;
uniform float stretch_speed_y;

uniform float outline_effect;
uniform float outline_offset;
uniform float outline_red;
uniform float outline_green;
uniform float outline_blue;

uniform float water_effect;
uniform float water_strength_x;
uniform float water_strength_y;
uniform float water_speed_x;
uniform float water_speed_y;

uniform float edge_effect;
uniform float edge_threshold;
uniform float edge_thickness;
uniform float edge_red;
uniform float edge_green;
uniform float edge_blue;

uniform float pixel_effect;
uniform float pixel_threshold;

uniform float grid_effect_x;
uniform float grid_effect_y;
uniform float grid_thickness_x;
uniform float grid_thickness_y;
uniform float grid_spacing_x;
uniform float grid_spacing_y;
uniform float grid_x_red;
uniform float grid_x_green;
uniform float grid_x_blue;
uniform float grid_y_red;
uniform float grid_y_green;
uniform float grid_y_blue;

uniform float fill_effect;
uniform float fill_red;
uniform float fill_green;
uniform float fill_blue;

void main(void)
{
	vec2 uv = gl_TexCoord[0].st;
	float alpha = texture2D(texture, uv).a * gl_Color.a;
	vec3 color = texture2D(texture, gl_TexCoord[0].xy);

	float alphaR = texture2D(raw_texture, uv).a * gl_Color.a;
	vec3 colorR = texture2D(raw_texture, gl_TexCoord[0].xy);
	// ==================================================================================================================
	vec2 coord = gl_TexCoord[0].xy;
	vec4 pixel_color = texture2D(texture, coord);
	float blink_alpha = cos(time * (blink_speed));
	alpha = mix(alpha, min(blink_alpha, pixel_color.w), blink_effect / 2);
	// ==================================================================================================================
	if (stretch_effect > 0)
	{
		float factorx = sin(time * stretch_speed_x) * stretch_effect_x;
		float factory = sin(time * stretch_speed_y) * stretch_effect_y;
		coord.x += factorx / (1 + 2 * factorx);
		coord.y += factory / (1 + 2 * factory);
	}
	color = mix(vec4(color, alpha), texture2D(texture, coord), stretch_effect);
	// ==================================================================================================================
	if (water_effect > 0)
	{
		coord.x += sin(radians(2000 * time * water_speed_x + coord.y * 250)) * 0.02 * water_strength_x;
		coord.y += cos(radians(2000 * time * water_speed_y + coord.x * 500)) * 0.03 * water_strength_y;
	}
	color = mix(vec4(color, alpha), texture2D(texture, coord), water_effect);
	// ==================================================================================================================
	if (earthquake_effect > 0)
	{
		coord.x += sin(radians(3000 * time + coord.x * 0)) * cos(time) * earthquake_effect_x;
		coord.y += cos(radians(3000 * time + coord.y * 0)) * sin(time) * earthquake_effect_y;
	}
	color = mix(vec4(color, alpha), texture2D(texture, coord), earthquake_effect);
	// ==================================================================================================================
	vec2 offx = vec2(blur_strength_x, 0.0);
	vec2 offy = vec2(0.0, blur_strength_y);
	vec4 pixel = texture2D(texture, gl_TexCoord[0].xy) * 4.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offx) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offx) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offy) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offy) * 2.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offx - offy) * 1.0 +
					  texture2D(texture, gl_TexCoord[0].xy - offx + offy) * 1.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offx - offy) * 1.0 +
					  texture2D(texture, gl_TexCoord[0].xy + offx + offy) * 1.0;

	color = mix(vec4(color, alpha), gl_Color * (pixel / 16.0), blur_effect);
	// ==================================================================================================================
	color = mix(color, vec3(fill_red, fill_green, fill_blue), fill_effect);
	// ==================================================================================================================
	vec2 v_texCoords = gl_TexCoord[0].xy;
	vec4 col = texture2D(texture, v_texCoords);

	if (col.a != 1)
	{
		float au = texture2D(texture, vec2(v_texCoords.x, v_texCoords.y - outline_offset)).a;
		float ad = texture2D(texture, vec2(v_texCoords.x, v_texCoords.y + outline_offset)).a;
		float al = texture2D(texture, vec2(v_texCoords.x - outline_offset, v_texCoords.y)).a;
		float ar = texture2D(texture, vec2(v_texCoords.x + outline_offset, v_texCoords.y)).a;

		if (au == 1.0 || ad == 1.0 || al == 1.0 || ar == 1.0)
		{
			color = mix(color, vec3(outline_red, outline_green, outline_blue), outline_effect);
			alpha = mix(alpha, 1, outline_effect);
		}
	}
	// ==================================================================================================================
	vec2 offx2 = vec2(1.0 - edge_thickness, 0.0);
	vec2 offy2 = vec2(0.0, 1.0 - edge_thickness);
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
	if (edge > (edge_threshold * 6.0)) color = mix(color, vec3(edge_red, edge_green, edge_blue), edge_effect);
	// ==================================================================================================================
	if (mod(gl_FragCoord.x, round(grid_thickness_x + grid_spacing_x)) < round(grid_spacing_x))
	{
		color = mix(vec4(color, alpha), vec4(grid_x_red, grid_x_green, grid_x_blue, alpha), grid_effect_x);
	}
	if (mod(gl_FragCoord.y, round(grid_thickness_y + grid_spacing_y)) < round(grid_spacing_y))
	{
		color = mix(vec4(color, alpha), vec4(grid_y_red, grid_y_green, grid_y_blue, alpha), grid_effect_y);
	}
	// ==================================================================================================================
	color = pow(color, vec3(1.0 / (1 - adjust_gamma)));
	color = mix(color, vec3(0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b), clamp(adjust_desaturation, 0, 1));
	color = mix(color, vec3(1.0) - color, clamp(adjust_inversion, 0, 1));
	color = color * gl_Color.rgb;
	color = (color - vec3(0.5)) * ((adjust_contrast + 1) / (1 - adjust_contrast)) + 0.5;
	color = vec3(clamp(color.r, 0, 1), clamp(color.g, 0, 1), clamp(color.b, 0, 1));
	color = color + vec3(adjust_brightness);
	color = vec3(clamp(color.r, 0, 1), clamp(color.g, 0, 1), clamp(color.b, 0, 1));
	// ==================================================================================================================
	float factor = 1.0 / (pixel_threshold + 0.001);
	vec2 pos = floor(gl_TexCoord[0].xy * factor + 0.5) / factor;
	color = mix(vec4(color, alpha), gl_Color * texture2D(texture, pos), pixel_effect);
	// ==================================================================================================================
	if (has_mask)
	{
		vec3 mask = vec3(mask_red, mask_green, mask_blue);

		if (mask_out)
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