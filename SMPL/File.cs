using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using static SMPL.Events;

namespace SMPL
{
	public static class File
	{
		internal struct QueuedAsset
		{
			public string path;
			public Asset asset;

			public QueuedAsset(string path, Asset asset)
			{
				this.path = path;
				this.asset = asset;
			}
		}

		public enum Asset
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

		internal static void Initialize() => CreateShaderFiles();
		internal static void UpdateMainThreadAssets()
		{
			var instances = new SortedDictionary<int, List<Events>>(Events.instances); // thread safe iteration
			if (assetLoadBegin)
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnAssetsLoadingStartSetup(); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnAssetsLoadingStart(); }
			}
			if (assetLoadUpdate)
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnAssetsLoadingUpdateSetup(); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnAssetsLoadingUpdate(); }
			}
			if (assetLoadEnd)
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnAssetsLoadingEndSetup(); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnAssetsLoadingEnd(); }
			}

			assetLoadBegin = false;
			assetLoadUpdate = false;
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
				for (int i = 0; i < curQueuedAssets.Count; i++)
				{
					var asset = curQueuedAssets[i].asset;
					var path = curQueuedAssets[i].path;
					path = path.Replace('/', '\\');
					try
					{
						switch (asset)
						{
							case Asset.Texture: textures[path] = new Texture(path); break;
							case Asset.Font: fonts[path] = new Font(path); break;
							case Asset.Sound: sounds[path] = new Sound(new SoundBuffer(path)); break;
							case Asset.Music: music[path] = new Music(path); break;
						}
					}
					catch (Exception)
					{
						Debug.LogError(-1, $"Failed to load asset {asset} from file '{path}'.");
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
			var full = $"{Directory}{filePath}";
			var curPath = Directory;
			for (int i = 0; i < path.Length - 1; i++)
			{
				var p = $"{curPath}\\{path[i]}";
				if (System.IO.Directory.Exists(p) == false) System.IO.Directory.CreateDirectory(p);

				curPath = p;
			}
			return full;
		}

		public static bool AssetIsLoaded(string filePath = "folder/file.extension")
		{
			return textures.ContainsKey(filePath) || fonts.ContainsKey(filePath) ||
				sounds.ContainsKey(filePath) || music.ContainsKey(filePath);
		}
		public static void LoadAsset(Asset asset, string filePath = "folder/file.extension")
		{
			queuedAssets.Add(new QueuedAsset()
			{
				asset = asset,
				path = filePath
			});
		}
		/// <summary>
		/// Creates or overwrites a file at <paramref name="filePath"/> and fills it with <paramref name="text"/>. Any <paramref name="text"/> can be retrieved from a file with <see cref="LoadText"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static void SaveText(string text, string filePath = "folder/file.extension")
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
		/// Reads all the text from the file at <paramref name="filePath"/> and returns it as a <see cref="string"/> if successful. Returns <paramref name="null"/> otherwise. A text can be saved to a file with <see cref="SaveText"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static string LoadText(string filePath = "folder/file.extension")
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
uniform sampler2D Texture;
uniform sampler2D RawTexture;
uniform float Time;

uniform bool HasMask;
uniform bool MaskOut;
uniform float MaskRed;
uniform float MaskGreen;
uniform float MaskBlue;

uniform float ReplaceRed;
uniform float ReplaceGreen;
uniform float ReplaceBlue;
uniform float ReplaceOpacity;
uniform float ReplaceWithRed;
uniform float ReplaceWithGreen;
uniform float ReplaceWithBlue;
uniform float ReplaceWithOpacity;

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
uniform float EdgeSensitivity;
uniform float EdgeThickness;
uniform float EdgeRed;
uniform float EdgeGreen;
uniform float EdgeBlue;

uniform float PixelateOpacity;
uniform float PixelateStrength;

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

uniform float IgnoreDark;
uniform float IgnoreBright;

void main(void)
{
	vec2 uv = gl_TexCoord[0].st;
	float alpha = texture2D(Texture, uv).a * gl_Color.a;
	vec3 color = texture2D(Texture, gl_TexCoord[0].xy);

	float alphaR = texture2D(RawTexture, uv).a * gl_Color.a;
	vec3 colorR = texture2D(RawTexture, gl_TexCoord[0].xy);
	// ==================================================================================================================
	vec2 coord = gl_TexCoord[0].xy;
	alpha = mix(alpha, min(cos(Time * (BlinkSpeed)), texture2D(Texture, coord).w), BlinkOpacity / 2);
	// ==================================================================================================================
	float factorx = sin(Time * StretchSpeedX) * StretchStrengthX;
	float factory = sin(Time * StretchSpeedY) * StretchStrengthY;
	coord.x += factorx / (1 + 2 * factorx);
	coord.y += factory / (1 + 2 * factory);
	color = mix(vec4(color, alpha), texture2D(Texture, coord), StretchOpacity);
	// ==================================================================================================================
	vec3 luminanceVector = vec3(0.2125, 0.7154, 0.0721);
	vec4 sample = texture2D(Texture, coord);

	float luminance = dot(luminanceVector, sample.rgb);
	float luminance2 = -dot(luminanceVector, sample.rgb);
	luminance = max(0.0, luminance - IgnoreDark);
	luminance2 = max(0.0, luminance2 + (1 - IgnoreBright));
	sample.rgb *= sign(luminance);
	sample.rgb *= sign(luminance2);

	color = mix(vec4(color, alpha), sample, 1);
	// ==================================================================================================================
	coord.x += sin(radians(2000 * Time * WaterSpeedX + coord.y * 250)) * 0.02 * WaterStrengthX;
	coord.y += cos(radians(2000 * Time * WaterSpeedY + coord.x * 500)) * 0.03 * WaterStrengthY;
	color = mix(vec4(color, alpha), texture2D(Texture, coord), WaterOpacity);
	// ==================================================================================================================
	coord.x += sin(radians(3000 * Time + coord.x * 0)) * cos(Time) * EarthquakeStrengthX;
	coord.y += cos(radians(3000 * Time + coord.y * 0)) * sin(Time) * EarthquakeStrengthY;
	color = mix(vec4(color, alpha), texture2D(Texture, coord), EarthquakeOpacity);
	// ==================================================================================================================
	vec2 offx = vec2(BlurStrengthX, 0.0);
	vec2 offy = vec2(0.0, BlurStrengthY);
	vec4 pixel = texture2D(Texture, gl_TexCoord[0].xy) * 4.0 +
					  texture2D(Texture, gl_TexCoord[0].xy - offx) * 2.0 +
					  texture2D(Texture, gl_TexCoord[0].xy + offx) * 2.0 +
					  texture2D(Texture, gl_TexCoord[0].xy - offy) * 2.0 +
					  texture2D(Texture, gl_TexCoord[0].xy + offy) * 2.0 +
					  texture2D(Texture, gl_TexCoord[0].xy - offx - offy) * 1.0 +
					  texture2D(Texture, gl_TexCoord[0].xy - offx + offy) * 1.0 +
					  texture2D(Texture, gl_TexCoord[0].xy + offx - offy) * 1.0 +
					  texture2D(Texture, gl_TexCoord[0].xy + offx + offy) * 1.0;

	color = mix(vec4(color, alpha), gl_Color * (pixel / 16.0), BlurOpacity);
	// ==================================================================================================================
	color = mix(color, vec3(FillRed, FillGreen, FillBlue), FillOpacity);
	// ==================================================================================================================
	vec2 vtexCoords = gl_TexCoord[0].xy;
	vec4 col = texture2D(Texture, vtexCoords);

	if (col.a != 1)
	{
		float au = texture2D(Texture, vec2(vtexCoords.x, vtexCoords.y - OutlineOffset)).a;
		float ad = texture2D(Texture, vec2(vtexCoords.x, vtexCoords.y + OutlineOffset)).a;
		float al = texture2D(Texture, vec2(vtexCoords.x - OutlineOffset, vtexCoords.y)).a;
		float ar = texture2D(Texture, vec2(vtexCoords.x + OutlineOffset, vtexCoords.y)).a;

		if (au == 1.0 || ad == 1.0 || al == 1.0 || ar == 1.0)
		{
			color = mix(color, vec3(OutlineRed, OutlineGreen, OutlineBlue), OutlineOpacity);
			alpha = mix(alpha, 1, OutlineOpacity);
		}
	}
	// ==================================================================================================================
	vec2 offx2 = vec2(1.0 - EdgeThickness, 0.0);
	vec2 offy2 = vec2(0.0, 1.0 - EdgeThickness);
	vec4 hEdge = texture2D(Texture, gl_TexCoord[0].xy - offy2) * -2.0 +
				 texture2D(Texture, gl_TexCoord[0].xy + offy2) * 2.0 +
				 texture2D(Texture, gl_TexCoord[0].xy - offx2 - offy2) * -1.0 +
				 texture2D(Texture, gl_TexCoord[0].xy - offx2 + offy2) * 1.0 +
				 texture2D(Texture, gl_TexCoord[0].xy + offx2 - offy2) * -1.0 +
				 texture2D(Texture, gl_TexCoord[0].xy + offx2 + offy2) * 1.0;

	vec4 vEdge = texture2D(Texture, gl_TexCoord[0].xy - offx2) * 2.0 +
				 texture2D(Texture, gl_TexCoord[0].xy + offx2) * -2.0 +
				 texture2D(Texture, gl_TexCoord[0].xy - offx2 - offy2) * 1.0 +
				 texture2D(Texture, gl_TexCoord[0].xy - offx2 + offy2) * -1.0 +
				 texture2D(Texture, gl_TexCoord[0].xy + offx2 - offy2) * 1.0 +
				 texture2D(Texture, gl_TexCoord[0].xy + offx2 + offy2) * -1.0;

	vec3 result = sqrt(hEdge.rgb * hEdge.rgb + vEdge.rgb * vEdge.rgb);
	if (length(result) > (EdgeSensitivity * 6.0)) color = mix(color, vec3(EdgeRed, EdgeGreen, EdgeBlue), EdgeOpacity);
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
	float factor = 1.0 / (PixelateStrength + 0.001);
	vec2 pos = floor(gl_TexCoord[0].xy * factor + 0.5) / factor;
	color = mix(vec4(color, alpha), gl_Color * texture2D(Texture, pos), PixelateOpacity);
	// ==================================================================================================================
	float margin = 0.004;
	vec4 replace = vec4(ReplaceRed, ReplaceGreen, ReplaceBlue, ReplaceOpacity);
	vec4 replaceWith = vec4(ReplaceWithRed, ReplaceWithGreen, ReplaceWithBlue, ReplaceWithOpacity);
	if (color.x > replace.x - margin && color.x < replace.x + margin &&
		color.y > replace.y - margin && color.y < replace.y + margin &&
		color.z > replace.z - margin && color.z < replace.z + margin &&
		alpha > replace.w - margin && alpha < replace.w + margin)
	{
		color = replaceWith.xyz;
		alpha = replaceWith.w;
	}
	// ==================================================================================================================
	if (HasMask)
	{
		vec3 mask = vec3(MaskRed, MaskGreen, MaskBlue);

		if (MaskOut)
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
}";
	}
}
