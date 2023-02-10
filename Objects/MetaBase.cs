using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using System;
using System.Collections.Generic;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// Placeholder class used to provide basic functionality for Meta types as well as providing common type for list placement
    /// </summary>
    public class AbstractMeta : IAbstractMeta, IHydratable
    {
        private const string CANT_CREATE_MESSAGE = "Cant create instance of AbstractMeta";

        #region Properties

        /// <summary>
        /// An ID associated with an index in the MetaConstructor list
        /// </summary>
        public int I { get; set; }

        /// <summary>
        /// Whether or not this object has been hydrated to resolve child references
        /// </summary>
        public bool IsHydrated { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the abstract meta with the specified id.
        /// </summary>
        /// <param name="id">An ID associated with an index in the MetaConstructor list</param>
        public AbstractMeta(int id)
        {
            if (GetType() == typeof(AbstractMeta))
            {
                throw new Exception(CANT_CREATE_MESSAGE);
            }

            I = id;
        }

        /// <summary>
        /// Creates an abstract meta instance with the intent of creating temporary objects for using serialization code without needing a serialized object
        /// </summary>
        public AbstractMeta()
        {
            I = -1;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Hydrates all child properties
        /// </summary>
        /// <param name="meta">The dictionary containing the reference list of objects-Ids for used with hydration</param>
        public virtual void Hydrate(IDictionary<int, IHydratable> meta = null)
        {
        }

        //TODO: Change this to a ref
        /// <summary>
        /// Recursively hydrates a child property using the provided dictionary
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="toHydrate">The property to be hydrated</param>
        /// <param name="meta">The dictionary containing the reference list of objects-Ids for used with hydration</param>
        /// <returns>A hydrated version of the object</returns>
        public static T HydrateChild<T>(T toHydrate, IDictionary<int, IHydratable> meta) where T : IHydratable
        {
            if (toHydrate == null)
            {
                return default;
            }
            else if (toHydrate.I < 0 && !toHydrate.IsHydrated)
            {
                //For manually created instances, in case they have automatically
                //generated children
                toHydrate.Hydrate(meta);
                toHydrate.IsHydrated = true;

                return toHydrate;
            }
            else if (toHydrate.I < 0 || toHydrate.IsHydrated)
            {
                return toHydrate;
            }
            else
            {
                if (meta is null)
                {
                    throw new ArgumentNullException(nameof(meta));
                }

                toHydrate = (T)meta[toHydrate.I];
                toHydrate.IsHydrated = true;
                toHydrate.Hydrate(meta);
                return toHydrate;
            }
        }

        internal virtual void HydrateList<T>(IList<T> toHydrate, IDictionary<int, IHydratable> meta) where T : IHydratable
        {
            if (toHydrate is null)
            {
                return;
            }

            for (int i = 0; i < toHydrate.Count; i++)
            {
                if (!toHydrate[i].IsHydrated)
                {
                    if (toHydrate[i].I >= 0)
                    {
                        toHydrate[i] = (T)HydrateChild(meta[toHydrate[i].I], meta);
                    }
                    else
                    {
                        toHydrate[i].IsHydrated = true;
                        toHydrate[i].Hydrate(meta);
                    }
                }
            }
        }

        #endregion Methods
    }
}