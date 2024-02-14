// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

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