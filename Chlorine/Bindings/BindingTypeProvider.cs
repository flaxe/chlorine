using System;
using Chlorine.Providers;

namespace Chlorine.Bindings
{
	public struct BindingTypeProvider
	{
		private readonly Binder _binder;

		private readonly Type _type;
		private readonly object _id;
		private readonly IProvider _provider;

		internal BindingTypeProvider(Binder binder, Type type, object id, IProvider provider)
		{
			_binder = binder;
			_type = type;
			_id = id;
			_provider = provider;
		}

		public void AsSingleton()
		{
			_binder.Bind(_type, _id, new SingletonProvider(_provider));
		}

		public void AsTransient()
		{
			_binder.Bind(_type, _id, _provider);
		}
	}
}