using Chlorine.Execution;

namespace Chlorine.Queues
{
	public interface IExecutionQueue
	{
		IExecutable Peek();

		bool Enqueue(IExecutable executable);
		void Dequeue(IExecutable executable);
	}
}