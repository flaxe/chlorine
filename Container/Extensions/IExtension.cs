namespace Carbone.Extensions
{
	public interface IExtension<in TExtension> where TExtension : class
	{
		void Extend(Container container, TExtension? parent);
	}
}