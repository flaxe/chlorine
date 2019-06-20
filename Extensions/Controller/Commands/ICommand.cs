using Chlorine.Controller.Execution;

namespace Chlorine.Controller.Commands
{
	public interface ICommand : IExecutable
	{
		void Cancel(Error error);
		void Fail(Error error);
		void Apply();
	}
}