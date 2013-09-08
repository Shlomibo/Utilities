using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Interop
{
	/// <summary>
	/// Represents a multi-string
	/// </summary>
	public class MultiString : IList<string>
	{
		#region Consts

		private const int NOT_FOUND = -1;
		#endregion

		#region Fields

		private List<string> strings;
		#endregion

		#region Properties

		public string this[int index]
		{
			get { return this.strings[index]; }
			set
			{
				this.strings[index] = value;
				OnChanged();
			}
		}

		public int Count
		{
			get { return this.strings.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
		#endregion

		#region Events

		public event EventHandler Changed = (s, e) => { };
		#endregion

		#region Ctor

		public MultiString()
		{
			this.strings = new List<string>();
		}

		public MultiString(int capacity)
		{
			this.strings = new List<string>(capacity);
		}

		public MultiString(IEnumerable<string> strings)
		{
			this.strings = new List<string>(strings);
		}

		public unsafe MultiString(char* mszMultiString, int bufferSize)
			: this(mszMultiString, bufferSize, true) { }

		public unsafe MultiString(char* mszMultiString)
			: this(mszMultiString, int.MaxValue, false) { }

		public unsafe MultiString(string multiString)
		{
			fixed (char* mszMultiString = multiString)
			{
				var multiStringObj = new MultiString(mszMultiString, multiString.Length + 1);
				this.strings = multiStringObj.strings;
			}
		}
		#endregion

		#region Methods

		private unsafe MultiString(char* mszMultiString, int bufferSize, bool hasBufferSize)
		{
			if (mszMultiString != null)
			{
				string lastString;

				for (int index = 0; (index < bufferSize) &&
					((mszMultiString[index] != '\0') || (mszMultiString[index + 1] != '\0'));
					index += lastString.Length + 1)
				{
					lastString = hasBufferSize
						? CreateString(mszMultiString + index, bufferSize - index)
						: new string(mszMultiString + index);
					Add(lastString);
				} 
			}
		}

		private void OnChanged()
		{
			this.Changed(this, EventArgs.Empty);
		}

		private unsafe string CreateString(char* szString, int maxCount)
		{
			var newStr = new StringBuilder(maxCount);

			for (int i = 0; (i < maxCount) && (szString[i] != '\0'); i++)
			{
				newStr.Append(szString[i]);
			}

			return newStr.ToString();
		}

		public int IndexOf(string str)
		{
			int index = NOT_FOUND;

			for (int i = 0; i < this.strings.Count; i++)
			{
				if (this.strings[i].ToString() == str)
				{
					index = i;
					break;
				}
			}

			return index;
		}

		public void RemoveAt(int index)
		{
			this.strings.RemoveAt(index);
			OnChanged();
		}

		public void Insert(int index, string str)
		{
			this.strings.Insert(index, str);
			OnChanged();
		}

		public void Add(string str)
		{
			this.strings.Add(str);
			OnChanged();
		}

		public void Clear()
		{
			this.strings.Clear();
			OnChanged();
		}

		public bool Contains(string str)
		{
			return this.strings.Contains(str);
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[i + arrayIndex] = this[i];
			}
		}

		public bool Remove(string str)
		{
			bool didRemoved = this.strings.Remove(str);
			OnChanged();

			return didRemoved;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<string>).GetEnumerator();
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return this.strings.GetEnumerator();
		}

		public override string ToString()
		{
			return string.Join("\0", this) + "\0";
		}
		#endregion
	}
}
