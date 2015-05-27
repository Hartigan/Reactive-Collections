using System;
using JetBrains.Annotations;

namespace ReactiveCollections.Extensions
{
	internal static class InternalExtensions
	{
		[AssertionMethod]
		public static void ArgumentNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this object obj, string name)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(name);
			}
		}
	}
}