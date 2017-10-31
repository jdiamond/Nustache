using System;
using System.Collections.Generic;
using System.Text;

namespace Nustache.Core
{

	public static class GenericIDictionaryUtil
	{
		private static readonly Type OpenIDictionaryType = typeof(IDictionary<,>);

		public static bool IsInstanceOfGenericIDictionary(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			var objType = value.GetType();

			bool isGenericIDictionaryType = ImplementsGenericIDictionary(objType);

			return isGenericIDictionaryType;
		}

		private static bool ImplementsGenericIDictionary(Type type)
		{
			return (type.FindInterfaces(IsGenericIDictionary, null).Length > 0);
		}

		private static bool IsGenericIDictionary(Type type, object searchCrit)
		{
			return (type.Name == OpenIDictionaryType.Name)
						&& type.IsGenericType
						&& type.GetGenericTypeDefinition() == OpenIDictionaryType;
		}
	}


}
