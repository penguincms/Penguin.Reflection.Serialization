using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Penguin.Reflection.Serialization.Objects
{
    /// <summary>
    /// Placeholder class used to provide basic functionality for Meta types as well as providing common type for list placement
    /// </summary>
    public class AbstractMeta : IAbstractMeta
    {
        private const string CANT_CREATE_MESSAGE = "Cant create instance of AbstractMeta";

        #region Properties

        /// <summary>
        /// An ID associated with an index in the MetaConstructor list
        /// </summary>
        public int i { get; set; }

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
            if (this.GetType() == typeof(AbstractMeta))
            {
                throw new Exception(CANT_CREATE_MESSAGE);
            }

            this.i = id;
        }

        /// <summary>
        /// Creates an abstract meta instance with the intent of creating temporary objects for using serialization code without needing a serialized object
        /// </summary>
        public AbstractMeta()
        {
            this.i = -1;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Hydrates all child properties
        /// </summary>
        /// <param name="meta">The dictionary containing the reference list of objects-Ids for used with hydration</param>
        public virtual void Hydrate(IDictionary<int, IAbstractMeta> meta = null)
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
        public T HydrateChild<T>(T toHydrate, IDictionary<int, IAbstractMeta> meta) where T : IAbstractMeta
        {
            Contract.Requires(meta != null);

            if (toHydrate == null)
            {
                return default;
            }
            else if (toHydrate.i < 0 && !toHydrate.IsHydrated)
            {
                //For manually created instances, in case they have automatically
                //generated children
                toHydrate.Hydrate(meta);
                toHydrate.IsHydrated = true;

                return toHydrate;
            }
            else if (toHydrate.i < 0 || toHydrate.IsHydrated)
            {
                return toHydrate;
            }
            else
            {
                toHydrate = (T)meta[toHydrate.i];
                toHydrate.IsHydrated = true;
                toHydrate.Hydrate(meta);
                return toHydrate;
            }
        }

        internal virtual void HydrateList<T>(IList<T> toHydrate, IDictionary<int, IAbstractMeta> meta) where T : IAbstractMeta
        {
            if (toHydrate is null)
            {
                return;
            }

            for (int i = 0; i < toHydrate.Count; i++)
            {
                if (!toHydrate[i].IsHydrated)
                {
                    if (toHydrate[i].i >= 0)
                    {
                        toHydrate[i] = (T)this.HydrateChild(meta[toHydrate[i].i], meta);
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