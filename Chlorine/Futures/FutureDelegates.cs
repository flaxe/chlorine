namespace Chlorine
{
	public delegate void FutureResolved();
	public delegate void FutureResolved<in TResult>(TResult result);
	public delegate void FutureRejected(Error reason);

	public delegate IFuture FuturePromised();
	public delegate IFuture FuturePromised<in TInput>(TInput result);
	public delegate IFuture<TOutput> FutureResultPromised<TOutput>();
	public delegate IFuture<TOutput> FutureResultPromised<TOutput, in TInput>(TInput result);
}