namespace Chlorine
{
	public delegate void FutureResolved();
	public delegate void FutureResolved<in TResult>(TResult result);
	public delegate void FutureRejected(Error reason);

	public delegate IFuture FuturePromised();
	public delegate IFuture FuturePromised<in TResult>(TResult result);
	public delegate IFuture<T> FutureResultPromised<T>();
	public delegate IFuture<T> FutureResultPromised<T, in TResult>(TResult result);
}