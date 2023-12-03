using System;

namespace AwARe.Logic
{
    /// <summary>
    /// A static class with (Unity) safe alternatives.
    /// </summary>
    public static class Safe
    {
        /// <summary>
        /// Implements a Unity safe version of x ??= y
        /// </summary>
        /// <typeparam name="T">Any Type</typeparam>
        /// <param name="field">Reference to x. </param>
        /// <param name="remedy">Get-function to obtain y. </param>
        /// <returns></returns>
        public static T Load<T>(ref T field, Func<T> remedy)=>
            field = field != null ? field : remedy();
    }
}