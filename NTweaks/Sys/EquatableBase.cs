using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace NTweaks.Sys
{
    
	/// <summary>
	/// Defines a base class to fully implement all parts of IEquatable in a re-usable fashion
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>To use, just override GetHashCode and Equals(T other). Other Equals methods and operators will follow from this.</remarks>
	public abstract class EquatableBase<T> : IEquatable<T> where T : EquatableBase<T>
	{

        public abstract override int GetHashCode();
		
		public abstract bool Equals(T other);

		public override sealed bool Equals(object obj)
		{

			if (!(obj is T))
				return false;

			return Equals((T)obj);

		}

		public static bool operator ==(EquatableBase<T> lhs, EquatableBase<T> rhs)
		{

			if ((Object)lhs == null)
				return rhs == null;

			return lhs.Equals(rhs);

		}

		public static bool operator !=(EquatableBase<T> lhs, EquatableBase<T> rhs)
		{

			return !(lhs == rhs);

		}

	}

}