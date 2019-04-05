using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GenericServices.ExtensionMethods
{
	/// <summary>
	/// FullMemberName
	/// </summary>
	public static class FullMemberName
	{
		/// <summary>
		/// ToFullMemberNameString
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string ToFullMemberNameString<T>(this Expression<T> expression) where T:class
		{
			return String.Join(".", expression.ToString().Split('.').Skip(1));
		}
	}
}
