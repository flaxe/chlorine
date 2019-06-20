using Chlorine.Controller.Execution;

namespace Chlorine.Controller.Queues
{
	public interface IExecutionQueue
	{
		IExecutable Peek();

		bool Enqueue(IExecutable executable);
		void Dequeue(IExecutable executable);
	}
}