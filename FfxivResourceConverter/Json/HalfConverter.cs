// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter.Json
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using Newtonsoft.Json;
	using SharpDX;

	public class HalfConverter : JsonConverter<Half>
	{
		public override Half ReadJson(JsonReader reader, Type objectType, [AllowNull] Half existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			string s = (string)reader.Value;
			float val = float.Parse(s);
			return (Half)val;
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] Half value, JsonSerializer serializer)
		{
			float val = value;
			writer.WriteValue(val.ToString());
		}
	}
}
