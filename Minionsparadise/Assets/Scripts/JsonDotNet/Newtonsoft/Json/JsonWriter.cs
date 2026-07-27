namespace Newtonsoft.Json
{
	public abstract class JsonWriter : global::System.IDisposable
	{
		private enum State
		{
			Start = 0,
			Property = 1,
			ObjectStart = 2,
			Object = 3,
			ArrayStart = 4,
			Array = 5,
			ConstructorStart = 6,
			Constructor = 7,
			Bytes = 8,
			Closed = 9,
			Error = 10
		}

		private static readonly global::Newtonsoft.Json.JsonWriter.State[][] stateArray = new global::Newtonsoft.Json.JsonWriter.State[8][]
		{
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.ConstructorStart,
				global::Newtonsoft.Json.JsonWriter.State.ConstructorStart,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.ConstructorStart,
				global::Newtonsoft.Json.JsonWriter.State.ConstructorStart,
				global::Newtonsoft.Json.JsonWriter.State.ConstructorStart,
				global::Newtonsoft.Json.JsonWriter.State.ConstructorStart,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.Property,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Property,
				global::Newtonsoft.Json.JsonWriter.State.Property,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.Start,
				global::Newtonsoft.Json.JsonWriter.State.Property,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.Object,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.Array,
				global::Newtonsoft.Json.JsonWriter.State.Constructor,
				global::Newtonsoft.Json.JsonWriter.State.Constructor,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.Start,
				global::Newtonsoft.Json.JsonWriter.State.Property,
				global::Newtonsoft.Json.JsonWriter.State.ObjectStart,
				global::Newtonsoft.Json.JsonWriter.State.Object,
				global::Newtonsoft.Json.JsonWriter.State.ArrayStart,
				global::Newtonsoft.Json.JsonWriter.State.Array,
				global::Newtonsoft.Json.JsonWriter.State.Constructor,
				global::Newtonsoft.Json.JsonWriter.State.Constructor,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			},
			new global::Newtonsoft.Json.JsonWriter.State[10]
			{
				global::Newtonsoft.Json.JsonWriter.State.Start,
				global::Newtonsoft.Json.JsonWriter.State.Object,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Array,
				global::Newtonsoft.Json.JsonWriter.State.Array,
				global::Newtonsoft.Json.JsonWriter.State.Constructor,
				global::Newtonsoft.Json.JsonWriter.State.Constructor,
				global::Newtonsoft.Json.JsonWriter.State.Error,
				global::Newtonsoft.Json.JsonWriter.State.Error
			}
		};

		private int _top;

		private readonly global::System.Collections.Generic.List<global::Newtonsoft.Json.Linq.JTokenType> _stack;

		private global::Newtonsoft.Json.JsonWriter.State _currentState;

		private global::Newtonsoft.Json.Formatting _formatting;

		public bool CloseOutput { get; set; }

		protected internal int Top
		{
			get
			{
				return _top;
			}
		}

		public global::Newtonsoft.Json.WriteState WriteState
		{
			get
			{
				switch (_currentState)
				{
				case global::Newtonsoft.Json.JsonWriter.State.Error:
					return global::Newtonsoft.Json.WriteState.Error;
				case global::Newtonsoft.Json.JsonWriter.State.Closed:
					return global::Newtonsoft.Json.WriteState.Closed;
				case global::Newtonsoft.Json.JsonWriter.State.ObjectStart:
				case global::Newtonsoft.Json.JsonWriter.State.Object:
					return global::Newtonsoft.Json.WriteState.Object;
				case global::Newtonsoft.Json.JsonWriter.State.ArrayStart:
				case global::Newtonsoft.Json.JsonWriter.State.Array:
					return global::Newtonsoft.Json.WriteState.Array;
				case global::Newtonsoft.Json.JsonWriter.State.ConstructorStart:
				case global::Newtonsoft.Json.JsonWriter.State.Constructor:
					return global::Newtonsoft.Json.WriteState.Constructor;
				case global::Newtonsoft.Json.JsonWriter.State.Property:
					return global::Newtonsoft.Json.WriteState.Property;
				case global::Newtonsoft.Json.JsonWriter.State.Start:
					return global::Newtonsoft.Json.WriteState.Start;
				default:
					throw new global::Newtonsoft.Json.JsonWriterException("Invalid state: " + _currentState);
				}
			}
		}

		public global::Newtonsoft.Json.Formatting Formatting
		{
			get
			{
				return _formatting;
			}
			set
			{
				_formatting = value;
			}
		}

		protected JsonWriter()
		{
			_stack = new global::System.Collections.Generic.List<global::Newtonsoft.Json.Linq.JTokenType>(8);
			_stack.Add(global::Newtonsoft.Json.Linq.JTokenType.None);
			_currentState = global::Newtonsoft.Json.JsonWriter.State.Start;
			_formatting = global::Newtonsoft.Json.Formatting.None;
			CloseOutput = true;
		}

		private void Push(global::Newtonsoft.Json.Linq.JTokenType value)
		{
			_top++;
			if (_stack.Count <= _top)
			{
				_stack.Add(value);
			}
			else
			{
				_stack[_top] = value;
			}
		}

		private global::Newtonsoft.Json.Linq.JTokenType Pop()
		{
			global::Newtonsoft.Json.Linq.JTokenType result = Peek();
			_top--;
			return result;
		}

		private global::Newtonsoft.Json.Linq.JTokenType Peek()
		{
			return _stack[_top];
		}

		public abstract void Flush();

		public virtual void Close()
		{
			AutoCompleteAll();
		}

		public virtual void WriteStartObject()
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.StartObject);
			Push(global::Newtonsoft.Json.Linq.JTokenType.Object);
		}

		public virtual void WriteEndObject()
		{
			AutoCompleteClose(global::Newtonsoft.Json.JsonToken.EndObject);
		}

		public virtual void WriteStartArray()
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.StartArray);
			Push(global::Newtonsoft.Json.Linq.JTokenType.Array);
		}

		public virtual void WriteEndArray()
		{
			AutoCompleteClose(global::Newtonsoft.Json.JsonToken.EndArray);
		}

		public virtual void WriteStartConstructor(string name)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.StartConstructor);
			Push(global::Newtonsoft.Json.Linq.JTokenType.Constructor);
		}

		public virtual void WriteEndConstructor()
		{
			AutoCompleteClose(global::Newtonsoft.Json.JsonToken.EndConstructor);
		}

		public virtual void WritePropertyName(string name)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.PropertyName);
		}

		public virtual void WriteEnd()
		{
			WriteEnd(Peek());
		}

		public void WriteToken(global::Newtonsoft.Json.JsonReader reader)
		{
			global::Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(reader, "reader");
			int initialDepth = ((reader.TokenType == global::Newtonsoft.Json.JsonToken.None) ? (-1) : (IsStartToken(reader.TokenType) ? reader.Depth : (reader.Depth + 1)));
			WriteToken(reader, initialDepth);
		}

		internal void WriteToken(global::Newtonsoft.Json.JsonReader reader, int initialDepth)
		{
			do
			{
				switch (reader.TokenType)
				{
				case global::Newtonsoft.Json.JsonToken.StartObject:
					WriteStartObject();
					break;
				case global::Newtonsoft.Json.JsonToken.StartArray:
					WriteStartArray();
					break;
				case global::Newtonsoft.Json.JsonToken.StartConstructor:
				{
					string strA = reader.Value.ToString();
					if (string.Compare(strA, "Date", global::System.StringComparison.Ordinal) == 0)
					{
						WriteConstructorDate(reader);
					}
					else
					{
						WriteStartConstructor(reader.Value.ToString());
					}
					break;
				}
				case global::Newtonsoft.Json.JsonToken.PropertyName:
					WritePropertyName(reader.Value.ToString());
					break;
				case global::Newtonsoft.Json.JsonToken.Comment:
					WriteComment(reader.Value.ToString());
					break;
				case global::Newtonsoft.Json.JsonToken.Integer:
					WriteValue(global::System.Convert.ToInt64(reader.Value, global::System.Globalization.CultureInfo.InvariantCulture));
					break;
				case global::Newtonsoft.Json.JsonToken.Float:
					WriteValue(global::System.Convert.ToDouble(reader.Value, global::System.Globalization.CultureInfo.InvariantCulture));
					break;
				case global::Newtonsoft.Json.JsonToken.String:
					WriteValue(reader.Value.ToString());
					break;
				case global::Newtonsoft.Json.JsonToken.Boolean:
					WriteValue(global::System.Convert.ToBoolean(reader.Value, global::System.Globalization.CultureInfo.InvariantCulture));
					break;
				case global::Newtonsoft.Json.JsonToken.Null:
					WriteNull();
					break;
				case global::Newtonsoft.Json.JsonToken.Undefined:
					WriteUndefined();
					break;
				case global::Newtonsoft.Json.JsonToken.EndObject:
					WriteEndObject();
					break;
				case global::Newtonsoft.Json.JsonToken.EndArray:
					WriteEndArray();
					break;
				case global::Newtonsoft.Json.JsonToken.EndConstructor:
					WriteEndConstructor();
					break;
				case global::Newtonsoft.Json.JsonToken.Date:
					WriteValue((global::System.DateTime)reader.Value);
					break;
				case global::Newtonsoft.Json.JsonToken.Raw:
					WriteRawValue((string)reader.Value);
					break;
				case global::Newtonsoft.Json.JsonToken.Bytes:
					WriteValue((byte[])reader.Value);
					break;
				default:
					throw global::Newtonsoft.Json.Utilities.MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", reader.TokenType, "Unexpected token type.");
				case global::Newtonsoft.Json.JsonToken.None:
					break;
				}
			}
			while (initialDepth - 1 < reader.Depth - (IsEndToken(reader.TokenType) ? 1 : 0) && reader.Read());
		}

		private void WriteConstructorDate(global::Newtonsoft.Json.JsonReader reader)
		{
			if (!reader.Read())
			{
				throw new global::System.Exception("Unexpected end while reading date constructor.");
			}
			if (reader.TokenType != global::Newtonsoft.Json.JsonToken.Integer)
			{
				throw new global::System.Exception("Unexpected token while reading date constructor. Expected Integer, got " + reader.TokenType);
			}
			long javaScriptTicks = (long)reader.Value;
			global::System.DateTime value = global::Newtonsoft.Json.JsonConvert.ConvertJavaScriptTicksToDateTime(javaScriptTicks);
			if (!reader.Read())
			{
				throw new global::System.Exception("Unexpected end while reading date constructor.");
			}
			if (reader.TokenType != global::Newtonsoft.Json.JsonToken.EndConstructor)
			{
				throw new global::System.Exception("Unexpected token while reading date constructor. Expected EndConstructor, got " + reader.TokenType);
			}
			WriteValue(value);
		}

		private bool IsEndToken(global::Newtonsoft.Json.JsonToken token)
		{
			switch (token)
			{
			case global::Newtonsoft.Json.JsonToken.EndObject:
			case global::Newtonsoft.Json.JsonToken.EndArray:
			case global::Newtonsoft.Json.JsonToken.EndConstructor:
				return true;
			default:
				return false;
			}
		}

		private bool IsStartToken(global::Newtonsoft.Json.JsonToken token)
		{
			switch (token)
			{
			case global::Newtonsoft.Json.JsonToken.StartObject:
			case global::Newtonsoft.Json.JsonToken.StartArray:
			case global::Newtonsoft.Json.JsonToken.StartConstructor:
				return true;
			default:
				return false;
			}
		}

		private void WriteEnd(global::Newtonsoft.Json.Linq.JTokenType type)
		{
			switch (type)
			{
			case global::Newtonsoft.Json.Linq.JTokenType.Object:
				WriteEndObject();
				break;
			case global::Newtonsoft.Json.Linq.JTokenType.Array:
				WriteEndArray();
				break;
			case global::Newtonsoft.Json.Linq.JTokenType.Constructor:
				WriteEndConstructor();
				break;
			default:
				throw new global::Newtonsoft.Json.JsonWriterException("Unexpected type when writing end: " + type);
			}
		}

		private void AutoCompleteAll()
		{
			while (_top > 0)
			{
				WriteEnd();
			}
		}

		private global::Newtonsoft.Json.Linq.JTokenType GetTypeForCloseToken(global::Newtonsoft.Json.JsonToken token)
		{
			switch (token)
			{
			case global::Newtonsoft.Json.JsonToken.EndObject:
				return global::Newtonsoft.Json.Linq.JTokenType.Object;
			case global::Newtonsoft.Json.JsonToken.EndArray:
				return global::Newtonsoft.Json.Linq.JTokenType.Array;
			case global::Newtonsoft.Json.JsonToken.EndConstructor:
				return global::Newtonsoft.Json.Linq.JTokenType.Constructor;
			default:
				throw new global::Newtonsoft.Json.JsonWriterException("No type for token: " + token);
			}
		}

		private global::Newtonsoft.Json.JsonToken GetCloseTokenForType(global::Newtonsoft.Json.Linq.JTokenType type)
		{
			switch (type)
			{
			case global::Newtonsoft.Json.Linq.JTokenType.Object:
				return global::Newtonsoft.Json.JsonToken.EndObject;
			case global::Newtonsoft.Json.Linq.JTokenType.Array:
				return global::Newtonsoft.Json.JsonToken.EndArray;
			case global::Newtonsoft.Json.Linq.JTokenType.Constructor:
				return global::Newtonsoft.Json.JsonToken.EndConstructor;
			default:
				throw new global::Newtonsoft.Json.JsonWriterException("No close token for type: " + type);
			}
		}

		private void AutoCompleteClose(global::Newtonsoft.Json.JsonToken tokenBeingClosed)
		{
			int num = 0;
			for (int i = 0; i < _top; i++)
			{
				int index = _top - i;
				if (_stack[index] == GetTypeForCloseToken(tokenBeingClosed))
				{
					num = i + 1;
					break;
				}
			}
			if (num == 0)
			{
				throw new global::Newtonsoft.Json.JsonWriterException("No token to close.");
			}
			for (int j = 0; j < num; j++)
			{
				global::Newtonsoft.Json.JsonToken closeTokenForType = GetCloseTokenForType(Pop());
				if (_currentState != global::Newtonsoft.Json.JsonWriter.State.ObjectStart && _currentState != global::Newtonsoft.Json.JsonWriter.State.ArrayStart)
				{
					WriteIndent();
				}
				WriteEnd(closeTokenForType);
			}
			global::Newtonsoft.Json.Linq.JTokenType jTokenType = Peek();
			switch (jTokenType)
			{
			case global::Newtonsoft.Json.Linq.JTokenType.Object:
				_currentState = global::Newtonsoft.Json.JsonWriter.State.Object;
				break;
			case global::Newtonsoft.Json.Linq.JTokenType.Array:
				_currentState = global::Newtonsoft.Json.JsonWriter.State.Array;
				break;
			case global::Newtonsoft.Json.Linq.JTokenType.Constructor:
				_currentState = global::Newtonsoft.Json.JsonWriter.State.Array;
				break;
			case global::Newtonsoft.Json.Linq.JTokenType.None:
				_currentState = global::Newtonsoft.Json.JsonWriter.State.Start;
				break;
			default:
				throw new global::Newtonsoft.Json.JsonWriterException("Unknown JsonType: " + jTokenType);
			}
		}

		protected virtual void WriteEnd(global::Newtonsoft.Json.JsonToken token)
		{
		}

		protected virtual void WriteIndent()
		{
		}

		protected virtual void WriteValueDelimiter()
		{
		}

		protected virtual void WriteIndentSpace()
		{
		}

		protected void AutoComplete(global::Newtonsoft.Json.JsonToken tokenBeingWritten)
		{
			int num;
			switch (tokenBeingWritten)
			{
			default:
				num = (int)tokenBeingWritten;
				break;
			case global::Newtonsoft.Json.JsonToken.Integer:
			case global::Newtonsoft.Json.JsonToken.Float:
			case global::Newtonsoft.Json.JsonToken.String:
			case global::Newtonsoft.Json.JsonToken.Boolean:
			case global::Newtonsoft.Json.JsonToken.Null:
			case global::Newtonsoft.Json.JsonToken.Undefined:
			case global::Newtonsoft.Json.JsonToken.Date:
			case global::Newtonsoft.Json.JsonToken.Bytes:
				num = 7;
				break;
			}
			global::Newtonsoft.Json.JsonWriter.State state = stateArray[num][(int)_currentState];
			if (state == global::Newtonsoft.Json.JsonWriter.State.Error)
			{
				throw new global::Newtonsoft.Json.JsonWriterException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Token {0} in state {1} would result in an invalid JavaScript object.", global::System.Globalization.CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), _currentState.ToString()));
			}
			if ((_currentState == global::Newtonsoft.Json.JsonWriter.State.Object || _currentState == global::Newtonsoft.Json.JsonWriter.State.Array || _currentState == global::Newtonsoft.Json.JsonWriter.State.Constructor) && tokenBeingWritten != global::Newtonsoft.Json.JsonToken.Comment)
			{
				WriteValueDelimiter();
			}
			else if (_currentState == global::Newtonsoft.Json.JsonWriter.State.Property && _formatting == global::Newtonsoft.Json.Formatting.Indented)
			{
				WriteIndentSpace();
			}
			global::Newtonsoft.Json.WriteState writeState = WriteState;
			if ((tokenBeingWritten == global::Newtonsoft.Json.JsonToken.PropertyName && writeState != global::Newtonsoft.Json.WriteState.Start) || writeState == global::Newtonsoft.Json.WriteState.Array || writeState == global::Newtonsoft.Json.WriteState.Constructor)
			{
				WriteIndent();
			}
			_currentState = state;
		}

		public virtual void WriteNull()
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Null);
		}

		public virtual void WriteUndefined()
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Undefined);
		}

		public virtual void WriteRaw(string json)
		{
		}

		public virtual void WriteRawValue(string json)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Undefined);
			WriteRaw(json);
		}

		public virtual void WriteValue(string value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.String);
		}

		public virtual void WriteValue(int value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(uint value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(long value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(ulong value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(float value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Float);
		}

		public virtual void WriteValue(double value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Float);
		}

		public virtual void WriteValue(bool value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Boolean);
		}

		public virtual void WriteValue(short value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(ushort value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(char value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.String);
		}

		public virtual void WriteValue(byte value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(sbyte value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Integer);
		}

		public virtual void WriteValue(decimal value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Float);
		}

		public virtual void WriteValue(global::System.DateTime value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Date);
		}

		public virtual void WriteValue(global::System.DateTimeOffset value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Date);
		}

		public virtual void WriteValue(global::System.Guid value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.String);
		}

		public virtual void WriteValue(global::System.TimeSpan value)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.String);
		}

		public virtual void WriteValue(int? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(uint? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(long? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(ulong? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(float? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(double? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(bool? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(short? value)
		{
			object obj = value;
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				WriteValue((short)obj);
			}
		}

		public virtual void WriteValue(ushort? value)
		{
			object obj = value;
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				WriteValue((ushort)obj);
			}
		}

		public virtual void WriteValue(char? value)
		{
			object obj = value;
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				WriteValue((char)obj);
			}
		}

		public virtual void WriteValue(byte? value)
		{
			object obj = value;
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				WriteValue((byte)obj);
			}
		}

		public virtual void WriteValue(sbyte? value)
		{
			object obj = value;
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				WriteValue((sbyte)obj);
			}
		}

		public virtual void WriteValue(decimal? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(global::System.DateTime? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(global::System.DateTimeOffset? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(global::System.Guid? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(global::System.TimeSpan? value)
		{
			if (!value.HasValue)
			{
				WriteNull();
			}
			else
			{
				WriteValue(value.Value);
			}
		}

		public virtual void WriteValue(byte[] value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				AutoComplete(global::Newtonsoft.Json.JsonToken.Bytes);
			}
		}

		public virtual void WriteValue(global::System.Uri value)
		{
			if (value == null)
			{
				WriteNull();
			}
			else
			{
				AutoComplete(global::Newtonsoft.Json.JsonToken.String);
			}
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				WriteNull();
				return;
			}
			if (value is global::System.IConvertible)
			{
				global::System.IConvertible convertible = value as global::System.IConvertible;
				switch (convertible.GetTypeCode())
				{
				case global::System.TypeCode.String:
					WriteValue(convertible.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Char:
					WriteValue(convertible.ToChar(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Boolean:
					WriteValue(convertible.ToBoolean(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.SByte:
					WriteValue(convertible.ToSByte(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Int16:
					WriteValue(convertible.ToInt16(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.UInt16:
					WriteValue(convertible.ToUInt16(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Int32:
					WriteValue(convertible.ToInt32(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Byte:
					WriteValue(convertible.ToByte(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.UInt32:
					WriteValue(convertible.ToUInt32(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Int64:
					WriteValue(convertible.ToInt64(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.UInt64:
					WriteValue(convertible.ToUInt64(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Single:
					WriteValue(convertible.ToSingle(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Double:
					WriteValue(convertible.ToDouble(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.DateTime:
					WriteValue(convertible.ToDateTime(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.Decimal:
					WriteValue(convertible.ToDecimal(global::System.Globalization.CultureInfo.InvariantCulture));
					return;
				case global::System.TypeCode.DBNull:
					WriteNull();
					return;
				}
			}
			else
			{
				if (value is global::System.DateTimeOffset)
				{
					WriteValue((global::System.DateTimeOffset)value);
					return;
				}
				if (value is byte[])
				{
					WriteValue((byte[])value);
					return;
				}
				if (value is global::System.Guid)
				{
					WriteValue((global::System.Guid)value);
					return;
				}
				if (value is global::System.Uri)
				{
					WriteValue((global::System.Uri)value);
					return;
				}
				if (value is global::System.TimeSpan)
				{
					WriteValue((global::System.TimeSpan)value);
					return;
				}
			}
			throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.", global::System.Globalization.CultureInfo.InvariantCulture, value.GetType()));
		}

		public virtual void WriteComment(string text)
		{
			AutoComplete(global::Newtonsoft.Json.JsonToken.Comment);
		}

		public virtual void WriteWhitespace(string ws)
		{
			if (ws != null && !global::Newtonsoft.Json.Utilities.StringUtils.IsWhiteSpace(ws))
			{
				throw new global::Newtonsoft.Json.JsonWriterException("Only white space characters should be used.");
			}
		}

		void global::System.IDisposable.Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (WriteState != global::Newtonsoft.Json.WriteState.Closed)
			{
				Close();
			}
		}
	}
}
