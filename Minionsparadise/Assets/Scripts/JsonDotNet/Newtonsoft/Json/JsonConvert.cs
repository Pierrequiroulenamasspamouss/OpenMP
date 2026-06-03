namespace Newtonsoft.Json
{
	public static class JsonConvert
	{
		public static readonly string True = "true";

		public static readonly string False = "false";

		public static readonly string Null = "null";

		public static readonly string Undefined = "undefined";

		public static readonly string PositiveInfinity = "Infinity";

		public static readonly string NegativeInfinity = "-Infinity";

		public static readonly string NaN = "NaN";

		internal static readonly long InitialJavaScriptDateTicks = 621355968000000000L;

		public static string ToString(global::System.DateTime value)
		{
			using (global::System.IO.StringWriter stringWriter = global::Newtonsoft.Json.Utilities.StringUtils.CreateStringWriter(64))
			{
				WriteDateTimeString(stringWriter, value, GetUtcOffset(value), value.Kind);
				return stringWriter.ToString();
			}
		}

		public static string ToString(global::System.DateTimeOffset value)
		{
			using (global::System.IO.StringWriter stringWriter = global::Newtonsoft.Json.Utilities.StringUtils.CreateStringWriter(64))
			{
				WriteDateTimeString(stringWriter, value.UtcDateTime, value.Offset, global::System.DateTimeKind.Local);
				return stringWriter.ToString();
			}
		}

		private static global::System.TimeSpan GetUtcOffset(global::System.DateTime dateTime)
		{
			return global::System.TimeZoneInfo.Local.GetUtcOffset(dateTime);
		}

		internal static void WriteDateTimeString(global::System.IO.TextWriter writer, global::System.DateTime value)
		{
			WriteDateTimeString(writer, value, GetUtcOffset(value), value.Kind);
		}

		internal static void WriteDateTimeString(global::System.IO.TextWriter writer, global::System.DateTime value, global::System.TimeSpan offset, global::System.DateTimeKind kind)
		{
			long value2 = ConvertDateTimeToJavaScriptTicks(value, offset);
			writer.Write("\"\\/Date(");
			writer.Write(value2);
			switch (kind)
			{
			case global::System.DateTimeKind.Unspecified:
			case global::System.DateTimeKind.Local:
			{
				writer.Write((offset.Ticks >= 0) ? "+" : "-");
				int num = global::System.Math.Abs(offset.Hours);
				if (num < 10)
				{
					writer.Write(0);
				}
				writer.Write(num);
				int num2 = global::System.Math.Abs(offset.Minutes);
				if (num2 < 10)
				{
					writer.Write(0);
				}
				writer.Write(num2);
				break;
			}
			}
			writer.Write(")\\/\"");
		}

		private static long ToUniversalTicks(global::System.DateTime dateTime)
		{
			if (dateTime.Kind == global::System.DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			return ToUniversalTicks(dateTime, GetUtcOffset(dateTime));
		}

		private static long ToUniversalTicks(global::System.DateTime dateTime, global::System.TimeSpan offset)
		{
			if (dateTime.Kind == global::System.DateTimeKind.Utc)
			{
				return dateTime.Ticks;
			}
			long num = dateTime.Ticks - offset.Ticks;
			if (num > 3155378975999999999L)
			{
				return 3155378975999999999L;
			}
			if (num < 0)
			{
				return 0L;
			}
			return num;
		}

		internal static long ConvertDateTimeToJavaScriptTicks(global::System.DateTime dateTime, global::System.TimeSpan offset)
		{
			long universialTicks = ToUniversalTicks(dateTime, offset);
			return UniversialTicksToJavaScriptTicks(universialTicks);
		}

		internal static long ConvertDateTimeToJavaScriptTicks(global::System.DateTime dateTime)
		{
			return ConvertDateTimeToJavaScriptTicks(dateTime, true);
		}

		internal static long ConvertDateTimeToJavaScriptTicks(global::System.DateTime dateTime, bool convertToUtc)
		{
			long universialTicks = (convertToUtc ? ToUniversalTicks(dateTime) : dateTime.Ticks);
			return UniversialTicksToJavaScriptTicks(universialTicks);
		}

		private static long UniversialTicksToJavaScriptTicks(long universialTicks)
		{
			return (universialTicks - InitialJavaScriptDateTicks) / 10000;
		}

		internal static global::System.DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
		{
			return new global::System.DateTime(javaScriptTicks * 10000 + InitialJavaScriptDateTicks, global::System.DateTimeKind.Utc);
		}

		public static string ToString(bool value)
		{
			if (!value)
			{
				return False;
			}
			return True;
		}

		public static string ToString(char value)
		{
			return ToString(char.ToString(value));
		}

		public static string ToString(global::System.Enum value)
		{
			return value.ToString("D");
		}

		public static string ToString(int value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(short value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(ushort value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(uint value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(long value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(ulong value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(float value)
		{
			return EnsureDecimalPlace(value, value.ToString("R", global::System.Globalization.CultureInfo.InvariantCulture));
		}

		public static string ToString(double value)
		{
			return EnsureDecimalPlace(value, value.ToString("R", global::System.Globalization.CultureInfo.InvariantCulture));
		}

		private static string EnsureDecimalPlace(double value, string text)
		{
			if (double.IsNaN(value) || double.IsInfinity(value) || text.IndexOf('.') != -1 || text.IndexOf('E') != -1)
			{
				return text;
			}
			return text + ".0";
		}

		private static string EnsureDecimalPlace(string text)
		{
			if (text.IndexOf('.') != -1)
			{
				return text;
			}
			return text + ".0";
		}

		public static string ToString(byte value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(sbyte value)
		{
			return value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string ToString(decimal value)
		{
			return EnsureDecimalPlace(value.ToString(null, global::System.Globalization.CultureInfo.InvariantCulture));
		}

		public static string ToString(global::System.Guid value)
		{
			return '"' + value.ToString("D", global::System.Globalization.CultureInfo.InvariantCulture) + '"';
		}

		public static string ToString(global::System.TimeSpan value)
		{
			return '"' + value.ToString() + '"';
		}

		public static string ToString(global::System.Uri value)
		{
			return '"' + value.ToString() + '"';
		}

		public static string ToString(string value)
		{
			return ToString(value, '"');
		}

		public static string ToString(string value, char delimter)
		{
			return global::Newtonsoft.Json.Utilities.JavaScriptUtils.ToEscapedJavaScriptString(value, delimter, true);
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return Null;
			}
			global::System.IConvertible convertible = value as global::System.IConvertible;
			if (convertible != null)
			{
				switch (convertible.GetTypeCode())
				{
				case global::System.TypeCode.String:
					return ToString(convertible.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Char:
					return ToString(convertible.ToChar(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Boolean:
					return ToString(convertible.ToBoolean(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.SByte:
					return ToString(convertible.ToSByte(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Int16:
					return ToString(convertible.ToInt16(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.UInt16:
					return ToString(convertible.ToUInt16(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Int32:
					return ToString(convertible.ToInt32(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Byte:
					return ToString(convertible.ToByte(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.UInt32:
					return ToString(convertible.ToUInt32(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Int64:
					return ToString(convertible.ToInt64(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.UInt64:
					return ToString(convertible.ToUInt64(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Single:
					return ToString(convertible.ToSingle(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Double:
					return ToString(convertible.ToDouble(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.DateTime:
					return ToString(convertible.ToDateTime(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.Decimal:
					return ToString(convertible.ToDecimal(global::System.Globalization.CultureInfo.InvariantCulture));
				case global::System.TypeCode.DBNull:
					return Null;
				}
			}
			else
			{
				if (value is global::System.DateTimeOffset)
				{
					return ToString((global::System.DateTimeOffset)value);
				}
				if (value is global::System.Guid)
				{
					return ToString((global::System.Guid)value);
				}
				if (value is global::System.Uri)
				{
					return ToString((global::System.Uri)value);
				}
				if (value is global::System.TimeSpan)
				{
					return ToString((global::System.TimeSpan)value);
				}
			}
			throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.", global::System.Globalization.CultureInfo.InvariantCulture, value.GetType()));
		}

		private static bool IsJsonPrimitiveTypeCode(global::System.TypeCode typeCode)
		{
			switch (typeCode)
			{
			case global::System.TypeCode.DBNull:
			case global::System.TypeCode.Boolean:
			case global::System.TypeCode.Char:
			case global::System.TypeCode.SByte:
			case global::System.TypeCode.Byte:
			case global::System.TypeCode.Int16:
			case global::System.TypeCode.UInt16:
			case global::System.TypeCode.Int32:
			case global::System.TypeCode.UInt32:
			case global::System.TypeCode.Int64:
			case global::System.TypeCode.UInt64:
			case global::System.TypeCode.Single:
			case global::System.TypeCode.Double:
			case global::System.TypeCode.Decimal:
			case global::System.TypeCode.DateTime:
			case global::System.TypeCode.String:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsJsonPrimitiveType(global::System.Type type)
		{
			if (global::Newtonsoft.Json.Utilities.ReflectionUtils.IsNullableType(type))
			{
				type = global::System.Nullable.GetUnderlyingType(type);
			}
			if (type == typeof(global::System.DateTimeOffset))
			{
				return true;
			}
			if (type == typeof(byte[]))
			{
				return true;
			}
			if (type == typeof(global::System.Uri))
			{
				return true;
			}
			if (type == typeof(global::System.TimeSpan))
			{
				return true;
			}
			if (type == typeof(global::System.Guid))
			{
				return true;
			}
			return IsJsonPrimitiveTypeCode(global::System.Type.GetTypeCode(type));
		}

		internal static bool IsJsonPrimitive(object value)
		{
			if (value == null)
			{
				return true;
			}
			global::System.IConvertible convertible = value as global::System.IConvertible;
			if (convertible != null)
			{
				return IsJsonPrimitiveTypeCode(convertible.GetTypeCode());
			}
			if (value is global::System.DateTimeOffset)
			{
				return true;
			}
			if (value is byte[])
			{
				return true;
			}
			if (value is global::System.Uri)
			{
				return true;
			}
			if (value is global::System.TimeSpan)
			{
				return true;
			}
			if (value is global::System.Guid)
			{
				return true;
			}
			return false;
		}

		public static string SerializeObject(object value)
		{
			return SerializeObject(value, global::Newtonsoft.Json.Formatting.None, (global::Newtonsoft.Json.JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, global::Newtonsoft.Json.Formatting formatting)
		{
			return SerializeObject(value, formatting, (global::Newtonsoft.Json.JsonSerializerSettings)null);
		}

		public static string SerializeObject(object value, params global::Newtonsoft.Json.JsonConverter[] converters)
		{
			return SerializeObject(value, global::Newtonsoft.Json.Formatting.None, converters);
		}

		public static string SerializeObject(object value, global::Newtonsoft.Json.Formatting formatting, params global::Newtonsoft.Json.JsonConverter[] converters)
		{
			object obj;
			if (converters == null || converters.Length <= 0)
			{
				obj = null;
			}
			else
			{
				global::Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings = new global::Newtonsoft.Json.JsonSerializerSettings();
				jsonSerializerSettings.Converters = converters;
				obj = jsonSerializerSettings;
			}
			global::Newtonsoft.Json.JsonSerializerSettings settings = (global::Newtonsoft.Json.JsonSerializerSettings)obj;
			return SerializeObject(value, formatting, settings);
		}

		public static string SerializeObject(object value, global::Newtonsoft.Json.Formatting formatting, global::Newtonsoft.Json.JsonSerializerSettings settings)
		{
			global::Newtonsoft.Json.JsonSerializer jsonSerializer = global::Newtonsoft.Json.JsonSerializer.Create(settings);
			global::System.Text.StringBuilder sb = new global::System.Text.StringBuilder(128);
			global::System.IO.StringWriter stringWriter = new global::System.IO.StringWriter(sb, global::System.Globalization.CultureInfo.InvariantCulture);
			using (global::Newtonsoft.Json.JsonTextWriter jsonTextWriter = new global::Newtonsoft.Json.JsonTextWriter(stringWriter))
			{
				jsonTextWriter.Formatting = formatting;
				jsonSerializer.Serialize(jsonTextWriter, value);
			}
			return stringWriter.ToString();
		}

		public static object DeserializeObject(string value)
		{
			return DeserializeObject(value, (global::System.Type)null, (global::Newtonsoft.Json.JsonSerializerSettings)null);
		}

		public static object DeserializeObject(string value, global::Newtonsoft.Json.JsonSerializerSettings settings)
		{
			return DeserializeObject(value, null, settings);
		}

		public static object DeserializeObject(string value, global::System.Type type)
		{
			return DeserializeObject(value, type, (global::Newtonsoft.Json.JsonSerializerSettings)null);
		}

		public static T DeserializeObject<T>(string value)
		{
			return global::Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, (global::Newtonsoft.Json.JsonSerializerSettings)null);
		}

		public static T DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
		{
			return DeserializeObject<T>(value);
		}

		public static T DeserializeObject<T>(string value, params global::Newtonsoft.Json.JsonConverter[] converters)
		{
			return (T)DeserializeObject(value, typeof(T), converters);
		}

		public static T DeserializeObject<T>(string value, global::Newtonsoft.Json.JsonSerializerSettings settings)
		{
			return (T)DeserializeObject(value, typeof(T), settings);
		}

		public static object DeserializeObject(string value, global::System.Type type, params global::Newtonsoft.Json.JsonConverter[] converters)
		{
			object obj;
			if (converters == null || converters.Length <= 0)
			{
				obj = null;
			}
			else
			{
				global::Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings = new global::Newtonsoft.Json.JsonSerializerSettings();
				jsonSerializerSettings.Converters = converters;
				obj = jsonSerializerSettings;
			}
			global::Newtonsoft.Json.JsonSerializerSettings settings = (global::Newtonsoft.Json.JsonSerializerSettings)obj;
			return DeserializeObject(value, type, settings);
		}

		public static object DeserializeObject(string value, global::System.Type type, global::Newtonsoft.Json.JsonSerializerSettings settings)
		{
			global::System.IO.StringReader reader = new global::System.IO.StringReader(value);
			global::Newtonsoft.Json.JsonSerializer jsonSerializer = global::Newtonsoft.Json.JsonSerializer.Create(settings);
			object result;
			using (global::Newtonsoft.Json.JsonReader jsonReader = new global::Newtonsoft.Json.JsonTextReader(reader))
			{
				result = jsonSerializer.Deserialize(jsonReader, type);
				if (jsonReader.Read() && jsonReader.TokenType != global::Newtonsoft.Json.JsonToken.Comment)
				{
					throw new global::Newtonsoft.Json.JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
			return result;
		}

		public static void PopulateObject(string value, object target)
		{
			PopulateObject(value, target, null);
		}

		public static void PopulateObject(string value, object target, global::Newtonsoft.Json.JsonSerializerSettings settings)
		{
			global::System.IO.StringReader reader = new global::System.IO.StringReader(value);
			global::Newtonsoft.Json.JsonSerializer jsonSerializer = global::Newtonsoft.Json.JsonSerializer.Create(settings);
			using (global::Newtonsoft.Json.JsonReader jsonReader = new global::Newtonsoft.Json.JsonTextReader(reader))
			{
				jsonSerializer.Populate(jsonReader, target);
				if (jsonReader.Read() && jsonReader.TokenType != global::Newtonsoft.Json.JsonToken.Comment)
				{
					throw new global::Newtonsoft.Json.JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
				}
			}
		}
	}
}
