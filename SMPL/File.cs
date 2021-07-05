﻿using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace SMPL
{
	public static class File
	{
		internal struct QueuedAsset
		{
			public string path;
			public AssetType asset;
			public string error;

			public QueuedAsset(string path, AssetType asset, string error)
			{
				this.path = path;
				this.asset = asset;
				this.error = error;
			}
		}

		public enum AssetType
		{
			Texture, Font, Sound, Music
		}
		public static double PercentLoaded { get; private set; }
		public static string Directory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		internal static bool assetLoadBegin, assetLoadUpdate, assetLoadEnd;
		internal static List<QueuedAsset> queuedAssets = new();
		internal static Dictionary<string, Texture> textures = new();
		internal static Dictionary<string, Font> fonts = new();
		internal static Dictionary<string, Sound> sounds = new();
		internal static Dictionary<string, Music> music = new();

		internal static void Initialize()
		{
			CreateShaderFiles();
		}
		internal static void UpdateMainThreadAssets()
		{
			if (assetLoadBegin) FileEvents.instance.OnLoadingStart();
			assetLoadBegin = false;

			if (assetLoadUpdate) FileEvents.instance.OnLoadingUpdate();
			assetLoadUpdate = false;

			if (assetLoadEnd) FileEvents.instance.OnLoadingEnd();
			assetLoadEnd = false;
		}
		// thread for loading resources
		internal static void LoadQueuedResources()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1);
				PercentLoaded = 100;
				if (queuedAssets == null || queuedAssets.Count == 0) continue;

				// thread-safe local list in case the main thread queues something while this one foreaches the list
				var curQueuedAssets = new List<QueuedAsset>(queuedAssets);
				var loadedCount = 0;
				assetLoadBegin = true;
				foreach (var queuedAsset in curQueuedAssets)
				{
					var asset = queuedAsset.asset;
					var path = queuedAsset.path;
					path = path.Replace('/', '\\');
					var fullPath = $"{Directory}{path}";
					try
					{
						switch (asset)
						{
							case AssetType.Texture: textures[path] = new Texture(path); break;
							case AssetType.Font: fonts[path] = new Font(path); break;
							case AssetType.Sound: sounds[path] = new Sound(new SoundBuffer(path)); break;
							case AssetType.Music: music[path] = new Music(path); break;
						}
					}
					catch (Exception)
					{
						Debug.LogError(1, queuedAsset.error);
						continue;
					}
					loadedCount++;
					var percent = Number.ToPercent(loadedCount, new Bounds(0, queuedAssets.Count));
					PercentLoaded = percent;
					assetLoadUpdate = true;
					Thread.Sleep(1);
				}
				queuedAssets.Clear(); // done loading, clear queue
				assetLoadEnd = true;
			}
		}
		internal static string CreateDirectoryForFile(string filePath)
		{
			filePath = filePath.Replace('/', '\\');
			var path = filePath.Split('\\');
			var full = $"{File.Directory}{filePath}";
			var curPath = File.Directory;
			for (int i = 0; i < path.Length - 1; i++)
			{
				var p = $"{curPath}\\{path[i]}";
				if (System.IO.Directory.Exists(p) == false) System.IO.Directory.CreateDirectory(p);

				curPath = p;
			}
			return full;
		}

		/// <summary>
		/// Creates or overwrites a file at <paramref name="filePath"/> and fills it with <paramref name="text"/>. Any <paramref name="text"/> can be retrieved from a file with <see cref="Load"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static void Save(string text, string filePath = "folder/file.extension")
		{
			var full = CreateDirectoryForFile(filePath);

			try
			{
				System.IO.File.WriteAllText(full, text);
			}
			catch (Exception)
			{
				Debug.LogError(1, $"Could not save file '{full}'.");
				return;
			}
		}
		/// <summary>
		/// Reads all the text from the file at <paramref name="filePath"/> and returns it as a <see cref="string"/> if successful. Returns <paramref name="null"/> otherwise. A text can be saved to a file with <see cref="Save"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static string Load(string filePath = "folder/file.extension")
		{
			filePath = filePath.Replace('/', '\\');
			var full = $"{Directory}\\{filePath}";

			if (System.IO.Directory.Exists(full) == false)
			{
				Console.Log($"Could not load file '{full}'. Directory/file not found.");
				return default;
			}
			try
			{
				return System.IO.File.ReadAllText(full);
			}
			catch (Exception)
			{
				Console.Log($"Could not load file '{full}'.");
				return default;
			}
		}

		internal static void CreateShaderFiles()
		{
			System.IO.File.WriteAllText("shaders.vert", vert);
			System.IO.File.WriteAllText("shaders.frag", frag);
		}
		internal static readonly string frag =
@"
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
}";
		internal static readonly string vert =
@"
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
}";  
	}
}
