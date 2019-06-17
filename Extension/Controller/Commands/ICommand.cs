using Chlorine.Execution;

namespace Chlorine.Commands
{
	public interface ICommand : IExecutable
	{
		void Cancel(Error error);
		void Fail(Error error);
		void Apply();
	}
}