using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

		[NotNull]
		public static IObservable<T> ToKeepAliveObservable<T>([NotNull] this IObservable<T> source, [NotNull] object target)
		{
			source.ArgumentNotNull(nameof(source));
			target.ArgumentNotNull(nameof(target));

			IObservable<T> result = Observable.Create<T>(observer =>
			{
				IDisposable subscription = source.Subscribe(observer);

				return Disposable.Create(() =>
				{
					subscription.Dispose();
					GC.KeepAlive(target);
				});
			});
			return result;
		}
	}
}