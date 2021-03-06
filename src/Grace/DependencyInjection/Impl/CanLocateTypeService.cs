﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Interface for service that tests if a type can be resolved
    /// </summary>
    public interface ICanLocateTypeService
    {
        /// <summary>
        /// Can the service be located
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="type">type to be located</param>
        /// <param name="filter">filter for locate</param>
        /// <param name="key">key to use for locate</param>
        /// <param name="includeAutoRegister"></param>
        /// <returns></returns>
        bool CanLocate(IInjectionScope injectionScope, Type type, ActivationStrategyFilter filter, object key = null, bool includeAutoRegister = true);
    }

    /// <summary>
    /// Class tests if a type can be located
    /// </summary>
    public class CanLocateTypeService : ICanLocateTypeService
    {
        /// <summary>
        /// Can the service be located
        /// </summary>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="type">type to be located</param>
        /// <param name="filter">filter for locate</param>
        /// <param name="key">key to use for locate</param>
        /// <param name="includeAutoRegister"></param>
        /// <returns></returns>
        public bool CanLocate(IInjectionScope injectionScope, Type type, ActivationStrategyFilter filter, object key = null, bool includeAutoRegister = true)
        {
            if (key != null)
            {
                var collection = injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(type);

                if (collection?.GetKeyedStrategy(key) != null)
                {
                    return true;
                }

                return injectionScope.Parent?.CanLocate(type, filter, key) ?? false;
            }

            if (injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(type) != null)
            {
                return true;
            }

            if (injectionScope.WrapperCollectionContainer.GetActivationStrategyCollection(type) != null)
            {
                return true;
            }

            if (type.IsArray)
            {
                return true;
            }

            if (type.IsConstructedGenericType)
            {
                var generic = type.GetGenericTypeDefinition();

                if (injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(generic) != null)
                {
                    return true;
                }

                var collection = injectionScope.WrapperCollectionContainer.GetActivationStrategyCollection(generic);

                if (collection != null)
                {
                    foreach (var strategy in collection.GetStrategies())
                    {
                        var wrappedType = strategy.GetWrappedType(type);

                        if (CanLocate(injectionScope, wrappedType, filter))
                        {
                            return true;
                        }
                    }
                }

                if (generic == typeof(IEnumerable<>) ||
                    generic == typeof(IList<>) ||
                    generic == typeof(ICollection<>) ||
                    generic == typeof(IReadOnlyList<>) ||
                    generic == typeof(IReadOnlyCollection<>) ||
                    generic == typeof(List<>) ||
                    generic == typeof(ReadOnlyCollection<>) ||
                    generic == typeof(ImmutableLinkedList<>) ||
                    generic == typeof(ImmutableArray<>))
                {
                    return injectionScope.ScopeConfiguration.AutoRegisterUnknown;
                }
            }

            if (!type.GetTypeInfo().IsInterface && 
                !type.GetTypeInfo().IsSubclassOf(typeof(MulticastDelegate)) && 
                type.GetTypeInfo().DeclaredConstructors.Any(c => c.IsPublic && !c.IsAbstract && !c.IsStatic))
            {
                return  includeAutoRegister &&
                        injectionScope.ScopeConfiguration.AutoRegisterUnknown &&
                        type.GetTypeInfo().IsPrimitive == false &&
                        type != typeof(string) &&
                        type != typeof(DateTime);
            }

            if (type == typeof(ILocatorService) ||
                type == typeof(IExportLocatorScope) ||
                type == typeof(IInjectionContext) ||
                type == typeof(StaticInjectionContext))
            {
                return true;
            }

            return injectionScope.Parent?.CanLocate(type, filter) ?? false;
        }
    }
}
