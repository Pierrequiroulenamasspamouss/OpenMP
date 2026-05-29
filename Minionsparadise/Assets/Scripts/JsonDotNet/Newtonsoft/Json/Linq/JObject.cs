namespace Newtonsoft.Json.Linq
{
	public class JObject : global::Newtonsoft.Json.Linq.JContainer, global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken>, global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>, global::System.Collections.Generic.IEnumerable<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>, global::System.Collections.IEnumerable, global::System.ComponentModel.INotifyPropertyChanged, global::System.ComponentModel.ICustomTypeDescriptor
	{
		private class JPropertKeyedCollection : global::System.Collections.ObjectModel.KeyedCollection<string, global::Newtonsoft.Json.Linq.JToken>
		{
			private static readonly global::System.Reflection.PropertyInfo DictionaryProperty = typeof(global::System.Collections.ObjectModel.KeyedCollection<string, global::Newtonsoft.Json.Linq.JToken>).GetProperty("Dictionary", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.DeclaredOnly);

			private static readonly global::System.Reflection.PropertyInfo ItemsProperty = typeof(global::System.Collections.ObjectModel.Collection<global::Newtonsoft.Json.Linq.JToken>).GetProperty("Items", global::System.Reflection.BindingFlags.Instance | global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.DeclaredOnly);

			public new global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken> Dictionary
			{
				get
				{
					if (DictionaryProperty != null)
					{
						return (global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken>)DictionaryProperty.GetValue(this, null);
					}
					return null;
				}
			}

			public JPropertKeyedCollection(global::System.Collections.Generic.IEqualityComparer<string> comparer)
				: base(comparer, 0)
			{
			}

			protected override string GetKeyForItem(global::Newtonsoft.Json.Linq.JToken item)
			{
				return ((global::Newtonsoft.Json.Linq.JProperty)item).Name;
			}

			protected override void InsertItem(int index, global::Newtonsoft.Json.Linq.JToken item)
			{
				global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken> dictionary = Dictionary;
				if (dictionary == null)
				{
					base.InsertItem(index, item);
					return;
				}
				string keyForItem = GetKeyForItem(item);
				dictionary[keyForItem] = item;
				if (ItemsProperty != null)
				{
					global::System.Collections.Generic.IList<global::Newtonsoft.Json.Linq.JToken> list = (global::System.Collections.Generic.IList<global::Newtonsoft.Json.Linq.JToken>)ItemsProperty.GetValue(this, null);
					if (list != null)
					{
						list.Insert(index, item);
					}
				}
			}
		}

		private global::Newtonsoft.Json.Linq.JObject.JPropertKeyedCollection _properties = new global::Newtonsoft.Json.Linq.JObject.JPropertKeyedCollection(global::System.StringComparer.Ordinal);

		protected override global::System.Collections.Generic.IList<global::Newtonsoft.Json.Linq.JToken> ChildrenTokens
		{
			get
			{
				return _properties;
			}
		}

		public override global::Newtonsoft.Json.Linq.JTokenType Type
		{
			get
			{
				return global::Newtonsoft.Json.Linq.JTokenType.Object;
			}
		}

		public override global::Newtonsoft.Json.Linq.JToken this[object key]
		{
			get
			{
				global::Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Accessed JObject values with invalid key value: {0}. Object property name expected.", global::System.Globalization.CultureInfo.InvariantCulture, global::Newtonsoft.Json.Utilities.MiscellaneousUtils.ToString(key)));
				}
				return this[text];
			}
			set
			{
				global::Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(key, "o");
				string text = key as string;
				if (text == null)
				{
					throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Set JObject values with invalid key value: {0}. Object property name expected.", global::System.Globalization.CultureInfo.InvariantCulture, global::Newtonsoft.Json.Utilities.MiscellaneousUtils.ToString(key)));
				}
				this[text] = value;
			}
		}

		public global::Newtonsoft.Json.Linq.JToken this[string propertyName]
		{
			get
			{
				global::Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(propertyName, "propertyName");
				global::Newtonsoft.Json.Linq.JProperty jProperty = Property(propertyName);
				if (jProperty == null)
				{
					return null;
				}
				return jProperty.Value;
			}
			set
			{
				global::Newtonsoft.Json.Linq.JProperty jProperty = Property(propertyName);
				if (jProperty != null)
				{
					jProperty.Value = value;
					return;
				}
				Add(new global::Newtonsoft.Json.Linq.JProperty(propertyName, value));
				OnPropertyChanged(propertyName);
			}
		}

		global::System.Collections.Generic.ICollection<string> global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken>.Keys
		{
			get
			{
				return _properties.Dictionary.Keys;
			}
		}

		global::System.Collections.Generic.ICollection<global::Newtonsoft.Json.Linq.JToken> global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken>.Values
		{
			get
			{
				return _properties.Dictionary.Values;
			}
		}

		bool global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public event global::System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public JObject()
		{
		}

		public JObject(global::Newtonsoft.Json.Linq.JObject other)
			: base(other)
		{
		}

		public JObject(params object[] content)
			: this((object)content)
		{
		}

		public JObject(object content)
		{
			Add(content);
		}

		internal override bool DeepEquals(global::Newtonsoft.Json.Linq.JToken node)
		{
			global::Newtonsoft.Json.Linq.JObject jObject = node as global::Newtonsoft.Json.Linq.JObject;
			if (jObject != null)
			{
				return ContentsEqual(jObject);
			}
			return false;
		}

		internal override void InsertItem(int index, global::Newtonsoft.Json.Linq.JToken item)
		{
			if (item == null || item.Type != global::Newtonsoft.Json.Linq.JTokenType.Comment)
			{
				base.InsertItem(index, item);
			}
		}

		internal override void ValidateToken(global::Newtonsoft.Json.Linq.JToken o, global::Newtonsoft.Json.Linq.JToken existing)
		{
			global::Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(o, "o");
			if (o.Type != global::Newtonsoft.Json.Linq.JTokenType.Property)
			{
				throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Can not add {0} to {1}.", global::System.Globalization.CultureInfo.InvariantCulture, o.GetType(), GetType()));
			}
			global::Newtonsoft.Json.Linq.JProperty jProperty = (global::Newtonsoft.Json.Linq.JProperty)o;
			if (existing != null)
			{
				global::Newtonsoft.Json.Linq.JProperty jProperty2 = (global::Newtonsoft.Json.Linq.JProperty)existing;
				if (jProperty.Name == jProperty2.Name)
				{
					return;
				}
			}
			if (_properties.Dictionary != null && _properties.Dictionary.TryGetValue(jProperty.Name, out existing))
			{
				throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Can not add property {0} to {1}. Property with the same name already exists on object.", global::System.Globalization.CultureInfo.InvariantCulture, jProperty.Name, GetType()));
			}
		}

		internal void InternalPropertyChanged(global::Newtonsoft.Json.Linq.JProperty childProperty)
		{
			OnPropertyChanged(childProperty.Name);
		}

		internal void InternalPropertyChanging(global::Newtonsoft.Json.Linq.JProperty childProperty)
		{
		}

		internal override global::Newtonsoft.Json.Linq.JToken CloneToken()
		{
			return new global::Newtonsoft.Json.Linq.JObject(this);
		}

		public global::System.Collections.Generic.IEnumerable<global::Newtonsoft.Json.Linq.JProperty> Properties()
		{
			return global::System.Linq.Enumerable.Cast<global::Newtonsoft.Json.Linq.JProperty>(ChildrenTokens);
		}

		public global::Newtonsoft.Json.Linq.JProperty Property(string name)
		{
			if (_properties.Dictionary == null)
			{
				return null;
			}
			if (name == null)
			{
				return null;
			}
			global::Newtonsoft.Json.Linq.JToken value;
			_properties.Dictionary.TryGetValue(name, out value);
			return (global::Newtonsoft.Json.Linq.JProperty)value;
		}

		public global::Newtonsoft.Json.Linq.JEnumerable<global::Newtonsoft.Json.Linq.JToken> PropertyValues()
		{
			return new global::Newtonsoft.Json.Linq.JEnumerable<global::Newtonsoft.Json.Linq.JToken>(global::System.Linq.Enumerable.Select(Properties(), (global::Newtonsoft.Json.Linq.JProperty p) => p.Value));
		}

		public new static global::Newtonsoft.Json.Linq.JObject Load(global::Newtonsoft.Json.JsonReader reader)
		{
			global::Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == global::Newtonsoft.Json.JsonToken.None && !reader.Read())
			{
				throw new global::System.Exception("Error reading JObject from JsonReader.");
			}
			if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
			{
				throw new global::System.Exception(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Error reading JObject from JsonReader. Current JsonReader item is not an object: {0}", global::System.Globalization.CultureInfo.InvariantCulture, reader.TokenType));
			}
			global::Newtonsoft.Json.Linq.JObject jObject = new global::Newtonsoft.Json.Linq.JObject();
			jObject.SetLineInfo(reader as global::Newtonsoft.Json.IJsonLineInfo);
			jObject.ReadTokenFrom(reader);
			return jObject;
		}

		public new static global::Newtonsoft.Json.Linq.JObject Parse(string json)
		{
			global::Newtonsoft.Json.JsonReader reader = new global::Newtonsoft.Json.JsonTextReader(new global::System.IO.StringReader(json));
			return Load(reader);
		}

		public new static global::Newtonsoft.Json.Linq.JObject FromObject(object o)
		{
			return FromObject(o, new global::Newtonsoft.Json.JsonSerializer());
		}

		public new static global::Newtonsoft.Json.Linq.JObject FromObject(object o, global::Newtonsoft.Json.JsonSerializer jsonSerializer)
		{
			global::Newtonsoft.Json.Linq.JToken jToken = global::Newtonsoft.Json.Linq.JToken.FromObjectInternal(o, jsonSerializer);
			if (jToken != null && jToken.Type != global::Newtonsoft.Json.Linq.JTokenType.Object)
			{
				throw new global::System.ArgumentException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Object serialized to {0}. JObject instance expected.", global::System.Globalization.CultureInfo.InvariantCulture, jToken.Type));
			}
			return (global::Newtonsoft.Json.Linq.JObject)jToken;
		}

		public override void WriteTo(global::Newtonsoft.Json.JsonWriter writer, params global::Newtonsoft.Json.JsonConverter[] converters)
		{
			writer.WriteStartObject();
			foreach (global::Newtonsoft.Json.Linq.JProperty childrenToken in ChildrenTokens)
			{
				childrenToken.WriteTo(writer, converters);
			}
			writer.WriteEndObject();
		}

		public void Add(string propertyName, global::Newtonsoft.Json.Linq.JToken value)
		{
			Add(new global::Newtonsoft.Json.Linq.JProperty(propertyName, value));
		}

		bool global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken>.ContainsKey(string key)
		{
			if (_properties.Dictionary == null)
			{
				return false;
			}
			return _properties.Dictionary.ContainsKey(key);
		}

		public bool Remove(string propertyName)
		{
			global::Newtonsoft.Json.Linq.JProperty jProperty = Property(propertyName);
			if (jProperty == null)
			{
				return false;
			}
			jProperty.Remove();
			return true;
		}

		public bool TryGetValue(string propertyName, out global::Newtonsoft.Json.Linq.JToken value)
		{
			global::Newtonsoft.Json.Linq.JProperty jProperty = Property(propertyName);
			if (jProperty == null)
			{
				value = null;
				return false;
			}
			value = jProperty.Value;
			return true;
		}

		void global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>.Add(global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken> item)
		{
			Add(new global::Newtonsoft.Json.Linq.JProperty(item.Key, item.Value));
		}

		void global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>.Clear()
		{
			RemoveAll();
		}

		bool global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>.Contains(global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken> item)
		{
			global::Newtonsoft.Json.Linq.JProperty jProperty = Property(item.Key);
			if (jProperty == null)
			{
				return false;
			}
			return jProperty.Value == item.Value;
		}

		void global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>.CopyTo(global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new global::System.ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new global::System.ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
			}
			if (arrayIndex >= array.Length)
			{
				throw new global::System.ArgumentException("arrayIndex is equal to or greater than the length of array.");
			}
			if (base.Count > array.Length - arrayIndex)
			{
				throw new global::System.ArgumentException("The number of elements in the source JObject is greater than the available space from arrayIndex to the end of the destination array.");
			}
			int num = 0;
			foreach (global::Newtonsoft.Json.Linq.JProperty childrenToken in ChildrenTokens)
			{
				array[arrayIndex + num] = new global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>(childrenToken.Name, childrenToken.Value);
				num++;
			}
		}

		bool global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>.Remove(global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken> item)
		{
			if (!((global::System.Collections.Generic.ICollection<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>>)this).Contains(item))
			{
				return false;
			}
			((global::System.Collections.Generic.IDictionary<string, global::Newtonsoft.Json.Linq.JToken>)this).Remove(item.Key);
			return true;
		}

		internal override int GetDeepHashCode()
		{
			return ContentsHashCode();
		}

		public global::System.Collections.Generic.IEnumerator<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>> GetEnumerator()
		{
			foreach (global::Newtonsoft.Json.Linq.JProperty property in ChildrenTokens)
			{
				yield return new global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>(property.Name, property.Value);
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		global::System.ComponentModel.PropertyDescriptorCollection global::System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return ((global::System.ComponentModel.ICustomTypeDescriptor)this).GetProperties((global::System.Attribute[])null);
		}

		private static global::System.Type GetTokenPropertyType(global::Newtonsoft.Json.Linq.JToken token)
		{
			if (token is global::Newtonsoft.Json.Linq.JValue)
			{
				global::Newtonsoft.Json.Linq.JValue jValue = (global::Newtonsoft.Json.Linq.JValue)token;
				if (jValue.Value == null)
				{
					return typeof(object);
				}
				return jValue.Value.GetType();
			}
			return token.GetType();
		}

		global::System.ComponentModel.PropertyDescriptorCollection global::System.ComponentModel.ICustomTypeDescriptor.GetProperties(global::System.Attribute[] attributes)
		{
			global::System.ComponentModel.PropertyDescriptorCollection propertyDescriptorCollection = new global::System.ComponentModel.PropertyDescriptorCollection(null);
			using (global::System.Collections.Generic.IEnumerator<global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					global::System.Collections.Generic.KeyValuePair<string, global::Newtonsoft.Json.Linq.JToken> current = enumerator.Current;
					propertyDescriptorCollection.Add(new global::Newtonsoft.Json.Linq.JPropertyDescriptor(current.Key, GetTokenPropertyType(current.Value)));
				}
				return propertyDescriptorCollection;
			}
		}

		global::System.ComponentModel.AttributeCollection global::System.ComponentModel.ICustomTypeDescriptor.GetAttributes()
		{
			return global::System.ComponentModel.AttributeCollection.Empty;
		}

		string global::System.ComponentModel.ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string global::System.ComponentModel.ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		global::System.ComponentModel.TypeConverter global::System.ComponentModel.ICustomTypeDescriptor.GetConverter()
		{
			return new global::System.ComponentModel.TypeConverter();
		}

		global::System.ComponentModel.EventDescriptor global::System.ComponentModel.ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		global::System.ComponentModel.PropertyDescriptor global::System.ComponentModel.ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object global::System.ComponentModel.ICustomTypeDescriptor.GetEditor(global::System.Type editorBaseType)
		{
			return null;
		}

		global::System.ComponentModel.EventDescriptorCollection global::System.ComponentModel.ICustomTypeDescriptor.GetEvents(global::System.Attribute[] attributes)
		{
			return global::System.ComponentModel.EventDescriptorCollection.Empty;
		}

		global::System.ComponentModel.EventDescriptorCollection global::System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return global::System.ComponentModel.EventDescriptorCollection.Empty;
		}

		object global::System.ComponentModel.ICustomTypeDescriptor.GetPropertyOwner(global::System.ComponentModel.PropertyDescriptor pd)
		{
			return null;
		}
	}
}
