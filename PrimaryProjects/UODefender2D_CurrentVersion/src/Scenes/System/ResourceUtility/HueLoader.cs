using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace UODef.System.ResourceUtility
{

    public class HueMeta
    {
        public int HueID { get; set; } 
        public string HueName { get; set; }
		public List<ushort> ColorGradientRaw { get; set; }
		public Godot.Collections.Array<Vector3>? ColorGradient { get; set; }	// We must use godot array b/c we need to convert it convertable to a godot varient so that the shader can consume it

		public HueMeta()
        {
		}

		/// <summary>
		///		Takes the ushort's from the file and converts them into vector4 RGBAs so that we don't have to do
		///			it within the shader
		/// </summary>
        public void CalculateColorGradientRGB()
        {
			ColorGradient = new Godot.Collections.Array<Vector3>();
			foreach (ushort colorBits in ColorGradientRaw)
			{
				ColorGradient.Add(GetRGBForShader(colorBits));
			}
        }

		/// <summary>
		///		For our shader to nicely consume these values, throw them from 0.0 to 1.0 instead of 0 to 255
		/// </summary>
		/// <param name="hue">ushort representing a coded RGB value</param>
		/// <returns></returns>
		public static Vector3 GetRGBForShader(ushort hue)
		{
			return new Vector3(HueToColorR(hue)/255.0f, HueToColorG(hue)/255.0f, HueToColorB(hue)/255.0f);
		}

		/// <summary>
		///		Split out the conversion from ushort to its Red value - maybe useful somewhere.
		/// </summary>
		/// <param name="hue">ushort representing a coded RGB value</param>
		/// <returns></returns>
		public static float HueToColorR(ushort hue)
		{
			return ((hue & 0x7c00) >> 10) * (255 / 31);
		}

		/// <summary>
		///		Split out the conversion from ushort to its Green value - maybe useful somewhere.
		/// </summary>
		/// <param name="hue">ushort representing a coded RGB value</param>
		/// <returns></returns>
		public static float HueToColorG(ushort hue)
		{
			return ((hue & 0x3e0) >> 5) * (255 / 31);
		}

		/// <summary>
		///		Split out the conversion from ushort to its Blue value - maybe useful somewhere.
		/// </summary>
		/// <param name="hue">ushort representing a coded RGB value</param>
		/// <returns></returns>
		public static float HueToColorB(ushort hue)
		{
			return (hue & 0x1f) * (255 / 31);
		}
	}

	/// <summary>
	///     Does the work of reading the hue meta file
	/// </summary>
	public class HueLoader
    {
        public HueLoader()
        {
        }



		public Dictionary<int, HueMeta> LoadHueMetaFile(string hueMetaDataFilePath)
		{
			Dictionary<int, HueMeta> hueMetaDictOut = new();

			try
			{
				List<HueMeta> fileOutput = null;
				using var file = FileAccess.Open(hueMetaDataFilePath, FileAccess.ModeFlags.Read);

				string fileContents = file.GetAsText();
				fileOutput = JsonSerializer.Deserialize<List<HueMeta>>(fileContents);
				for (int i = 0; i < fileOutput.Count; i++)
				{
					// Convert the coded ushort to an RGB file
					hueMetaDictOut[fileOutput[i].HueID] = fileOutput[i];
					hueMetaDictOut[fileOutput[i].HueID].CalculateColorGradientRGB();
				}
			}
			catch (Exception e)
			{
				GD.PushError(e);
			}
			// We assume that we require hues
			if (hueMetaDictOut.Count == 0)
			{
				GD.PushError($"Unable to locate HueSheet Meta Files In Requested Directory: [{hueMetaDataFilePath}] 1XP4QGF6");
			}
			return hueMetaDictOut;
		}

    }
}
