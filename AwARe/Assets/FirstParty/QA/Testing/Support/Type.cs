// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

namespace AwARe.Testing
{
    /// <summary>
    /// Empty class containing only a type.
    /// </summary>
    /// <typeparam name="T">Type contained.</typeparam>
    public class Type<T>
    {
        public static implicit operator System.Type(Type<T> _) =>
            typeof(T);
    }
}