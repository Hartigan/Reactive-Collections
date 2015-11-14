using System;
using JetBrains.Annotations;

namespace ReactiveCollections.Extensions
{
	public static class ObservableExtensions
	{
		[NotNull]
		public static IDisposable WeakSubscribe<T>(
			[NotNull] this IObservable<T> observable,
			[NotNull] IObserver<T> observer)
		{
			observable.ArgumentNotNull(nameof(observable));
			observer.ArgumentNotNull(nameof(observer));
			return new WeakSubscription<T>(observable, observer);
		}

		private class WeakSubscription<T> : IDisposable, IObserver<T>
		{
			[NotNull] private readonly WeakReference _reference;
			[NotNull] private readonly IDisposable _subscription;
			private bool _disposed;

			public WeakSubscription(IObservable<T> observable, IObserver<T> observer)
			{
				_reference = new WeakReference(observer);
				_subscription = observable.Subscribe(this);
			}

			void IObserver<T>.OnCompleted()
			{
				var observer = (IObserver<T>)_reference.Target;
				if (observer != null) observer.OnCompleted();
				else Dispose();
			}

			void IObserver<T>.OnError(Exception error)
			{
				var observer = (IObserver<T>)_reference.Target;
				if (observer != null) observer.OnError(error);
				else Dispose();
			}

			void IObserver<T>.OnNext(T value)
			{
				var observer = (IObserver<T>)_reference.Target;
				if (observer != null) observer.OnNext(value);
				else Dispose();
			}

			public void Dispose()
			{
				if (!_disposed)
				{
					_disposed = true;
					_subscription.Dispose();
				}
			}
		}
	}


	
}