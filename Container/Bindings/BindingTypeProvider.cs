using System;
using Carbone.Providers;

namespace Carbone.Bindings
{
	public readonly struct BindingTypeProvider
	{
		private readonly Binder _binder;

		private readonly Type _type;
		private readonly object? _id;
		private readonly BindingCondition? _condition;
		private readonly IProvider _provider;

		internal BindingTypeProvider(Binder binder, Type type, object? id, BindingCondition? condition, IProvider provider)
		{
			_binder = binder;
			_type = type;
			_id = id;
			_condition = condition;
			_provider = provider;
		}

		public void AsSingleton()
		{
			_binder.Register(_type, _id, _condition, new SingletonProvider(_provider));
		}

		public void AsTransient()
		{
			_binder.Register(_type, _id, _condition, _provider);
		}
	}
}