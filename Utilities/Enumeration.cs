using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public abstract class Enumeration<T> : IEquatable<Enumeration<T>>
	{
		#region Fields

		private static Dictionary<Type, Definition<T>> definedEnums;
		#endregion

		#region Properties

		public string Name { get; private set; }
		public T Value { get; private set; }
		#endregion

		#region Ctor

		static Enumeration()
		{
			definedEnums = new Dictionary<Type, Definition<T>>();
		}

		protected Enumeration(string name, T value) : this(name, value, false) { }

		protected Enumeration(string name, T value, bool isInheriting)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			else if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name");
			}
			
			Type type = GetType();

			if (!definedEnums.ContainsKey(type))
			{
				Definition<T> definition;

				if (isInheriting && definedEnums.ContainsKey(type.BaseType))
				{
					definition = new Definition<T>
					{
						Enums = new HashSet<Enumeration<T>>(definedEnums[type.BaseType].Enums),
						Values = new HashSet<T>(definedEnums[type.BaseType].Values),
						Names = new HashSet<string>(definedEnums[type.BaseType].Names),
					};
				}
				else
				{
					definition = new Definition<T>
					{
						Enums = new HashSet<Enumeration<T>>(),
						Values = new HashSet<T>(),
						Names = new HashSet<string>(),
					};
				}

				definedEnums.Add(type, definition);
			}

			if (definedEnums[GetType()].Names.Contains(name))
			{
				throw new ArgumentException("Enumeation already exists", "name");
			}
			else
			{
				definedEnums[GetType()].Names.Add(name);
				definedEnums[GetType()].Values.Add(value);
				definedEnums[GetType()].Enums.Add(this);
			}

			this.Name = name;
			this.Value = value;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return this.Name;
		}

		public static TEnum AsEnumeration<TEnum>(T value) where TEnum : Enumeration<T>
		{
			if (!IsDefined<TEnum>(value))
			{
				throw new ArgumentOutOfRangeException("value", "Invalid enumeration value");
			}

			return (TEnum)definedEnums[typeof(TEnum)].Enums.First(@enum => @enum.Value.Equals(value));
		}

		public static bool IsDefined<TEnum>(string name) where TEnum : Enumeration<T>
		{
			return definedEnums[typeof(TEnum)].Names.Contains(name);
		}

		public static bool IsDefined<TEnum>(T value) where TEnum : Enumeration<T>
		{
			return definedEnums[typeof(TEnum)].Values.Contains(value);
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			// TODO: write your implementation of Equals() here
			return Equals((Enumeration<T>)obj);
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			// TODO: write your implementation of GetHashCode() here
			return GetType().GetHashCode() ^
				this.Name.GetHashCode() ^
				this.Value.GetHashCode();
		}

		public bool Equals(Enumeration<T> other)
		{
			return (other != null) &&
				(GetType() == other.GetType()) &&
				(this.Name == other.Name) &&
				this.Value.Equals(other.Value);
		}

		public static IEnumerable<string> GetNames<TEnum>() where TEnum : Enumeration<T>
		{
			foreach (string name in definedEnums[typeof(TEnum)].Names)
			{
				yield return name;
			}
		}

		public static Type GetUnderlyingType()
		{
			return typeof(T);
		}

		public static IEnumerable<T> GetValues<TEnum>() where TEnum : Enumeration<T>
		{
			foreach (T value in definedEnums[typeof(TEnum)].Values)
			{
				yield return value;
			}
		}

		public static bool TryParse<TEnum>(string name, bool ignoreCase, out TEnum @enum) 
			where TEnum : Enumeration<T>
		{
			@enum = null;
			bool didSucceed = true;

			if (string.IsNullOrWhiteSpace(name))
			{
				didSucceed = false;
			}
			else
			{
				StringComparison comparer = ignoreCase
					? StringComparison.InvariantCultureIgnoreCase
					: StringComparison.InvariantCulture;

				@enum = (TEnum)definedEnums[typeof(TEnum)].Enums.FirstOrDefault(value =>
					value.Name.Equals(name, comparer));

				if (@enum == null)
				{
					didSucceed = false;
				}
			}

			return didSucceed;
		}

		public static bool TryParse<TEnum>(string name, out TEnum @enum) 
			where TEnum : Enumeration<T>
		{
			return TryParse(name, false, out @enum);
		}

		public static TEnum Parse<TEnum>(string name, bool ignoreCase) where TEnum : Enumeration<T>
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			else if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("name");
			}

			TEnum @enum;

			if (!TryParse(name, ignoreCase, out @enum))
			{
				throw new OverflowException();
			}

			return @enum;
		}

		public static TEnum Parse<TEnum>(string name) where TEnum : Enumeration<T>
		{
			return Parse<TEnum>(name, false);
		}

		public static bool TryToObject<TEnum>(T value, out TEnum @enum)
			where TEnum : Enumeration<T>
		{
			@enum = (TEnum)definedEnums[typeof(TEnum)].Enums.FirstOrDefault(item =>
				((item.Value == null) && (value == null)) ||
				item.Value.Equals(value));

			return @enum != null;
		}

		public static TEnum ToObject<TEnum>(T value) where TEnum : Enumeration<T>
		{
			TEnum @enum;

			if (!TryToObject(value, out @enum))
			{
				throw new OverflowException();
			}

			return @enum;
		}
		#endregion

		#region Oerators

		public static implicit operator T(Enumeration<T> @enum)
		{
			return @enum.Value;
		}

		public static bool operator ==(Enumeration<T> left, Enumeration<T> right)
		{
			if (object.ReferenceEquals(left, right))
			{
				return true;
			}
			else if (object.ReferenceEquals(left, null))
			{
				return false;
			}
			else
			{
				return left.Equals(right);
			}
		}

		public static bool operator !=(Enumeration<T> left, Enumeration<T> right)
		{
			return !(left == right);
		}
		#endregion

		#region DefinitionSets

		private struct Definition<T>
		{
			public HashSet<Enumeration<T>> Enums { get; set; }
			public HashSet<T> Values { get; set; }
			public HashSet<string> Names { get; set; }
		}
		#endregion
	}
}
