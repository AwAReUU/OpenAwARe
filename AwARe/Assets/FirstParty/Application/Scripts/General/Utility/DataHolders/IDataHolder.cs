
namespace AwARe
{
    /// <summary>
    /// An interface for Objects storing a data/logic class counterpart.
    /// </summary>
    /// <typeparam name="T">The data/logic type stored.</typeparam>
    public interface IDataHolder<T>
    {
        /// <summary>
        /// Gets the data represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data represented.
        /// </value>
        public T Data { get; }
    }
}