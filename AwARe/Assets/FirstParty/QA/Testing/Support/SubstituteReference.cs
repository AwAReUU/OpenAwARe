// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.Testing
{
    /// <summary>
    /// An override for <see cref="AYellowpaper.InterfaceReference"/> allowing substitutes to be injected (during testing).
    /// </summary>
    /// <typeparam name="TInterface">The interface injected.</typeparam>
    [System.Serializable]
    public class SubstituteReference<TInterface> : AYellowpaper.InterfaceReference<TInterface>
        where TInterface : class
    {
        [SerializeReference]
        private TInterface @interface;

        /// <inheritdoc/>
        public override TInterface Value
        {
            get => @interface;
            set => @interface = value;
        }

        /// <inheritdoc/>
        public override Object UnderlyingValue
        {
            get => null;
            set {}
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubstituteReference{TInterface}"/> class.
        /// </summary>
        /// <param name="interface">an instance of the interface.</param>
        public SubstituteReference(TInterface @interface) { this.@interface = @interface; }
    }
}