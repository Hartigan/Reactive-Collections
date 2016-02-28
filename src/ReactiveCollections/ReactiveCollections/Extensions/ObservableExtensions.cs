using System;
using System.Reactive;
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

		[NotNull]
		public static IDisposable WeakSubscribe<T>(
			[NotNull] this IObservable<T> observable,
			[NotNull] Action<T> onNext)
		{
			observable.ArgumentNotNull(nameof(observable));
			onNext.ArgumentNotNull(nameof(onNext));
			return new WeakSubscription<T>(observable, Observer.Create(onNext));
		}

		private class WeakSubscription<T> : IDisposable, IObserver<T>
		{
			[NotNull] private readonly WeakReference _reference;
			[NotNull] private readonly IDisposable _subscription;
			private Action _dispose;
			private bool _disposed;

			public WeakSubscription(IObservable<T> observable, IObserver<T> observer)
			{
				_reference = new WeakReference(observer);
				_subscription = observable.Subscribe(this);
				_dispose = () =>
				{
					if (!_disposed)
					{
						_disposed = true;
						_subscription.Dispose();
						GC.KeepAlive(observer);
					}
				};
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
				_dispose?.Invoke();
				_dispose = null;
			}
		}
	}


	
}