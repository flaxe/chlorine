namespace Chlorine
{
	public delegate void FutureResolved();
	public delegate void FutureResolved<in TResult>(TResult result);
	public delegate void FutureRejected(Error reason);
}