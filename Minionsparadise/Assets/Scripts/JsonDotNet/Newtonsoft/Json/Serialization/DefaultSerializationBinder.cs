namespace Newtonsoft.Json.Serialization
{
	public class DefaultSerializationBinder : global::System.Runtime.Serialization.SerializationBinder
	{
		internal struct TypeNameKey
		{
			internal readonly string AssemblyName;

			internal readonly string TypeName;

			public TypeNameKey(string assemblyName, string typeName)
			{
				AssemblyName = assemblyName;
				TypeName = typeName;
			}

			public override int GetHashCode()
			{
				return ((AssemblyName != null) ? AssemblyName.GetHashCode() : 0) ^ ((TypeName != null) ? TypeName.GetHashCode() : 0);
			}

			public override bool Equals(object obj)
			{
				if (!(obj is global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey))
				{
					return false;
				}
				return Equals((global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey)obj);
			}

			public bool Equals(global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey other)
			{
				if (AssemblyName == other.AssemblyName)
				{
					return TypeName == other.TypeName;
				}
				return false;
			}
		}

		internal static readonly global::Newtonsoft.Json.Serialization.DefaultSerializationBinder Instance = new global::Newtonsoft.Json.Serialization.DefaultSerializationBinder();

		private readonly global::Newtonsoft.Json.Utilities.ThreadSafeStore<global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey, global::System.Type> _typeCache = new global::Newtonsoft.Json.Utilities.ThreadSafeStore<global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey, global::System.Type>(GetTypeFromTypeNameKey);

		private static global::System.Type GetTypeFromTypeNameKey(global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey typeNameKey)
		{
			string assemblyName = typeNameKey.AssemblyName;
			string typeName = typeNameKey.TypeName;
			if (assemblyName != null)
			{
				global::System.Reflection.Assembly assembly = global::System.Reflection.Assembly.Load(new global::System.Reflection.AssemblyName(assemblyName));
				if (assembly == null)
				{
					throw new global::Newtonsoft.Json.JsonSerializationException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Could not load assembly '{0}'.", global::System.Globalization.CultureInfo.InvariantCulture, assemblyName));
				}
				global::System.Type type = assembly.GetType(typeName);
				if (type == null)
				{
					throw new global::Newtonsoft.Json.JsonSerializationException(global::Newtonsoft.Json.Utilities.StringUtils.FormatWith("Could not find type '{0}' in assembly '{1}'.", global::System.Globalization.CultureInfo.InvariantCulture, typeName, assembly.FullName));
				}
				return type;
			}
			return global::System.Type.GetType(typeName);
		}

		public override global::System.Type BindToType(string assemblyName, string typeName)
		{
			return _typeCache.Get(new global::Newtonsoft.Json.Serialization.DefaultSerializationBinder.TypeNameKey(assemblyName, typeName));
		}
	}
}
