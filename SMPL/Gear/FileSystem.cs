using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Linq;
using SMPL.Data;
using System.ComponentModel;
using Newtonsoft.Json.Bson;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SMPL.Gear
{
	public static class FileSystem
	{
		private const string frag =
@"
uniform sampler2D Texture;
uniform sampler2D RawTexture;
uniform float Time;

uniform bool HasMask;
uniform bool MaskOut;
uniform float MaskRed;
uniform float MaskGreen;
uniform float MaskBlue;
uniform float MaskOpacity;
uniform float MaskMargin;

uniform float ReplaceRed;
uniform float ReplaceGreen;
uniform float ReplaceBlue;
uniform float ReplaceOpacity;
uniform float ReplaceWithRed;
uniform float ReplaceWithGreen;
uniform float ReplaceWithBlue;
uniform float ReplaceWithOpacity;
uniform float ReplaceMargin;

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

bool isInMargin(vec4 color1, vec4 color2, float margin)
{
	return
	color1.x > color2.x - margin && color1.x < color2.x + margin &&
	color1.y > color2.y - margin && color1.y < color2.y + margin &&
	color1.z > color2.z - margin && color1.z < color2.z + margin &&
	color1.w > color2.w - margin && color1.w < color2.w + margin;
}

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
	vec4 replace = vec4(ReplaceRed, ReplaceGreen, ReplaceBlue, ReplaceOpacity);
	vec4 replaceWith = vec4(ReplaceWithRed, ReplaceWithGreen, ReplaceWithBlue, ReplaceWithOpacity);
	if (isInMargin(vec4(color, alpha), replace, ReplaceMargin))
	{
		color = replaceWith.xyz;
		alpha = replaceWith.w;
	}
	// ==================================================================================================================
	if (HasMask)
	{
		vec4 mask = vec4(MaskRed, MaskGreen, MaskBlue, MaskOpacity);
		bool isInMargin = isInMargin(vec4(color, alpha), mask, MaskMargin);

		if (MaskOut)
		{
			if (isInMargin) alpha = 0.0;
		}
		else
		{
			if (!isInMargin) alpha = 0.0;
			else color = colorR;
		}
	}
	// ==================================================================================================================
	gl_FragColor = vec4(color, alpha);
}";
		private const string vert =
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
		private static void EditPictures(Data.Color color, string directoryPath = "folder/pictures", bool onlyOutline = false,
			bool fillDiagonals = false, bool fill = false)
		{
			if (Directory.Exists(directoryPath) == false)
			{
				Debug.LogError(1, $"Directory '{directoryPath}' not found.");
				return;
			}
			var outlineOrFill = fill ? "filled" : "outlined";
			var outliningOrFilling = fill ? "Filling" : "Outlining";
			var resultPath = $"{directoryPath}\\____{outlineOrFill} pictures";
			var col = Data.Color.From(color);
			var done = 0;
			var errors = 0;

			Console.Log($"{outliningOrFilling} pictures...");
			Directory.CreateDirectory(resultPath);
			var directories = Directory.GetDirectories($"{directoryPath}");
			for (int i = 0; i < directories.Length; i++)
			{
				EditFolder(directories[i]);
			}
			EditFolder($"{directoryPath}");
			Console.Log($"{outliningOrFilling} pictures - done. Result can be found in '{resultPath}'.\n" +
				$"Total {outlineOrFill}: {done}, Skipped: {errors}");

			void EditFolder(string folder)
			{
				if (folder == $"{directoryPath}\\____filled pictures" ||
					folder == $"{directoryPath}\\____outlined pictures") return;

				//var result = "";
				//var path = folder.Split('\\');
				//var adding = false;
				//for (int i = 0; i < path.Length; i++)
				//{
				//	if (adding)
				//	{
				//		result = result.Insert(result.Length, path[i]);
				//		if (i != path.Length - 1)
				//		{
				//			result = result.Insert(result.Length, "\\");
				//		}
				//	}
				//	if (path[i] == "Content")
				//	{
				//		adding = true;
				//	}
				//}
				var files = Directory.GetFiles(folder);
				for (int i = 0; i < files.Length; i++)
				{
					EditFile($"{files[i]}");
				}
				var currentDirectories = Directory.GetDirectories(folder).ToList();
				while (currentDirectories.Count > 0)
				{
					EditFolder(currentDirectories[0]);
					currentDirectories.RemoveAt(0);
				}
			}
			void EditFile(string path)
			{
				try
				{
					var img = new Image(path);
					var transparent = new SFML.Graphics.Color(0, 0, 0, 0);
					var offset = fill ? 0u : 2u;
					var resultImg = new Image(img.Size.X + offset, img.Size.Y + offset, transparent);
					for (uint y = 0; y < img.Size.Y; y++)
					{
						for (uint x = 0; x < img.Size.X; x++)
						{
							var curCol = img.GetPixel(x, y);
							if (curCol.A == 0) continue;

							if (fill == false)
							{
								var validX = x - 1 != uint.MaxValue && x + 1 != img.Size.X;
								var validY = y - 1 != uint.MaxValue && y + 1 != img.Size.Y;
								var valid = validX && validY;

								if (fillDiagonals && valid && img.GetPixel(x - 1, y - 1).A == 0) resultImg.SetPixel(x, y, col);
								if (fillDiagonals && valid && img.GetPixel(x + 1, y - 1).A == 0) resultImg.SetPixel(x + 2, y, col);
								if (fillDiagonals && valid && img.GetPixel(x - 1, y + 1).A == 0) resultImg.SetPixel(x, y + 2, col);
								if (fillDiagonals && valid && img.GetPixel(x + 1, y + 1).A == 0) resultImg.SetPixel(x + 2, y + 2, col);

								if (validX && img.GetPixel(x - 1, y).A == 0) resultImg.SetPixel(x, y + 1, col);
								if (validX && img.GetPixel(x + 1, y).A == 0) resultImg.SetPixel(x + 2, y + 1, col);
								if (validY && img.GetPixel(x, y - 1).A == 0) resultImg.SetPixel(x + 1, y, col);
								if (validY && img.GetPixel(x, y + 1).A == 0) resultImg.SetPixel(x + 1, y + 2, col);

								if (curCol.A != 0)
								{
									if (x == 0)
									{
										resultImg.SetPixel(x, y + 1, col);
										if (fillDiagonals) { resultImg.SetPixel(x, y, col); resultImg.SetPixel(x, y + 2, col); }
									}
									if (y == 0)
									{
										resultImg.SetPixel(x + 1, y, col);
										if (fillDiagonals) { resultImg.SetPixel(x, y, col); resultImg.SetPixel(x + 2, y, col); }
									}
									if (x == img.Size.X - 1)
									{
										resultImg.SetPixel(x + 2, y + 1, col);
										if (fillDiagonals) { resultImg.SetPixel(x + 2, y, col); resultImg.SetPixel(x + 2, y + 2, col); }
									}
									if (y == img.Size.Y - 1)
									{
										resultImg.SetPixel(x + 1, y + 2, col);
										if (fillDiagonals) { resultImg.SetPixel(x, y + 2, col); resultImg.SetPixel(x + 2, y + 2, col); }
									}
								}
							}
							else curCol = col;
							resultImg.SetPixel(x + offset / 2, y + offset / 2, onlyOutline ? transparent : curCol);
						}
					}

					var split = path.Split('\\');
					var name = split[^1];
					var newPath = "";
					if (split.Length > 2)
					{
						for (int i = 1; i < split.Length - 1; i++)
						{
							newPath += $"{split[i]}\\";
						}
					}
					Directory.CreateDirectory($"{resultPath}\\{newPath}");
					var result = resultImg.SaveToFile($"{resultPath}\\{newPath}{split[^1]}");
					img.Dispose();
					resultImg.Dispose();
					done++;
				}
				catch (Exception) { errors++; }
			}
		}
		private static void CreateShaderFiles()
		{
			System.IO.File.WriteAllText("shaders.vert", vert);
			System.IO.File.WriteAllText("shaders.frag", frag);
		}

		// =========

		internal static void Initialize()
		{
			CreateShaderFiles();
		}
		internal static string CreateDirectoryForFile(string filePath)
		{
			filePath = filePath.Replace('/', '\\');
			var path = filePath.Split('\\');
			var full = $"{MainDirectory}{filePath}";
			var curPath = MainDirectory;
			for (int i = 0; i < path.Length - 1; i++)
			{
				var p = $"{curPath}\\{path[i]}";
				if (Directory.Exists(p) == false) System.IO.Directory.CreateDirectory(p);

				curPath = p;
			}
			return full;
		}

		// =========

		public static string MainDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		public static bool FilesExist(params string[] paths)
		{
			if (paths == null) return false;
			for (int i = 0; i < paths.Length; i++)
				if (System.IO.File.Exists(paths[i]))
					return false;
			return true;
		}
		public static bool DirectoriesExist(params string[] paths)
		{
			if (paths == null) return false;
			for (int i = 0; i < paths.Length; i++)
				if (Directory.Exists(paths[i]))
					return false;
			return true;
		}
		public static void DeleteFiles(params string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
				if (Exists(paths[i]))
					File.Delete(paths[i]);

			bool Exists(string path)
			{
				if (File.Exists(path) == false)
				{
					Debug.LogError(1, $"No file was found on path '{path}'.");
					return false;
				}
				return true;
			}
		}
		public static void MoveFiles(string targetDirectory, params string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
				if (Exists(paths[i]))
					try { File.Move(paths[i], $"{targetDirectory}\\{Path.GetFileName(paths[i])}"); }
					catch (Exception)
					{
						Debug.LogError(1, $"Could not move files to directory '{targetDirectory}'.");
						return;
					}

			bool Exists(string path)
			{
				var file = $"{targetDirectory}\\{Path.GetFileName(path)}";
				if (File.Exists(file))
					File.Delete(file);
				if (File.Exists(path) == false)
				{
					Debug.LogError(1, $"No file was found on path '{path}'.");
					return false;
				}
				return true;
			}
		}
		public static string[] GetFileNames(bool includeExtensions, params string[] directoriesPaths)
		{
			var result = new List<string>();
			if (directoriesPaths == null) return result.ToArray();
			for (int i = 0; i < directoriesPaths.Length; i++)
			{
				if (Directory.Exists(directoriesPaths[i]) == false)
					continue;
				var files = Directory.GetFiles(directoriesPaths[i]);
				for (int j = 0; j < files.Length; j++)
					result.Add(includeExtensions ? Path.GetFileName(files[j]) : Path.GetFileNameWithoutExtension(files[j]));
			}
			return result.ToArray();
		}
		public static void CreateDirectories(params string[] paths)
		{
			for (int i = 0; i < paths.Length; i++)
				Directory.CreateDirectory(paths[i]);
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
			var full = $"{MainDirectory}\\{filePath}";

			if (Directory.Exists(full) == false)
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
		public static void OutlinePictures(Data.Color color, string directoryPath = "folder/pictures", bool onlyOutline = false,
			bool fillDiagonals = false)
		{
			EditPictures(color, directoryPath, onlyOutline, fillDiagonals);
		}
		public static void FillPictures(Data.Color color, string directoryPath = "folder/pictures")
		{
			EditPictures(color, directoryPath, false, false, true);
		}
	}
}
