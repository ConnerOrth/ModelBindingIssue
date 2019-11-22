using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static ModelBindingIssue.Helpers.PropertyCopy;

namespace ModelBindingIssue.Helpers
{
    public static class PropertyCopy
    {
        [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class IgnoreOnCopyAttribute : Attribute { }

        /// <summary>
        /// Copies all public, readable properties (except empty/default GUIDs at source) from the source object to the
        /// target. The target type does not have to have a parameterless constructor,
        /// as no new instance needs to be created.
        /// </summary>
        /// <remarks>Only the properties of the source and target types themselves
        /// are taken into account, regardless of the actual types of the arguments.</remarks>
        /// <typeparam name="TSource">Type of the source</typeparam>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <param name="source">Source to copy properties from</param>
        /// <param name="target">Target to copy properties to</param>
        public static void Copy<TSource, TTarget>(TSource source, TTarget target)
            where TSource : class
            where TTarget : class
        {
            PropertyCopier<TSource, TTarget>.Copy(source, target);
        }
    }

    /// <summary>
    /// Generic class which copies to its target type from a source
    /// type specified in the Copy method. The types are specified
    /// separately to take advantage of type inference on generic
    /// method arguments.
    /// </summary>
    public static class PropertyCopy<TTarget> where TTarget : class, new()
    {
        /// <summary>
        /// Copies all readable properties from the source to a new instance
        /// of TTarget.
        /// </summary>
        public static TTarget CopyFrom<TSource>(TSource source) where TSource : class
        {
            return PropertyCopier<TSource, TTarget>.Copy(source);
        }
    }

    /// <summary>
    /// Static class to efficiently store the compiled delegate which can
    /// do the copying. We need a bit of work to ensure that exceptions are
    /// appropriately propagated, as the exception is generated at type initialization
    /// time, but we wish it to be thrown as an ArgumentException.
    /// Note that this type we do not have a constructor constraint on TTarget, because
    /// we only use the constructor when we use the form which creates a new instance.
    /// </summary>
    internal static class PropertyCopier<TSource, TTarget>
    {
        /// <summary>
        /// Delegate to create a new instance of the target type given an instance of the
        /// source type. This is a single delegate from an expression tree.
        /// </summary>
        private static Func<TSource, TTarget> creator;

        /// <summary>
        /// List of properties to grab values from. The corresponding targetProperties
        /// list contains the same properties in the target type. Unfortunately we can't
        /// use expression trees to do this, because we basically need a sequence of statements.
        /// We could build a DynamicMethod, but that's significantly more work :) Please mail
        /// me if you really need this...
        /// </summary>
        private static readonly List<PropertyInfo> sourceProperties = new List<PropertyInfo>();
        private static readonly List<PropertyInfo> targetProperties = new List<PropertyInfo>();
        private static List<MemberBinding> bindings = new List<MemberBinding>();
        private static Exception initializationException;

        private static readonly Dictionary<PropertyInfo, PropertyInfo> propertyMap = new Dictionary<PropertyInfo, PropertyInfo>();

        internal static TTarget Copy(TSource source)
        {
            if (creator == null)
            {
                try
                {
                    bindings = BuildBindings<TSource, TTarget>();
                    creator = BuildCreator();
                    initializationException = null;
                }
                catch (Exception e)
                {
                    creator = null;
                    initializationException = e;
                }
            }
            if (initializationException != null)
            {
                throw initializationException;
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return creator(source);
        }

        internal static void Copy(TSource source, TTarget target)
        {
            BuildBindings();
            CopyImplementation(source, target);
        }

        private static void CopyImplementation(TSource source, TTarget target)
        {
            if (initializationException != null)
            {
                throw initializationException;
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var map in propertyMap)
            {
                if (map.Key.PropertyType == typeof(Guid))
                {
                    //skip Guid if it's empty at source and non-empty at target.
                    if (map.Key.GetValue(source, null).Equals(default(Guid))
                        && !map.Value.GetValue(target, null).Equals(default(Guid)))
                    {
                        continue;
                    }
                }
                map.Value.SetValue(target, map.Key.GetValue(source, null), null);
            }
        }

        private static Func<TSource, TTarget> BuildCreator()
        {
            ParameterExpression sourceParameter = Expression.Parameter(typeof(TSource), "source");
            Expression initializer = Expression.MemberInit(Expression.New(typeof(TTarget)), bindings);
            return Expression.Lambda<Func<TSource, TTarget>>(initializer, sourceParameter).Compile();
        }

        private static List<MemberBinding> BuildBindings<TFrom, TTo>()
        {
            propertyMap.Clear();
            ParameterExpression sourceParameter = Expression.Parameter(typeof(TFrom), "source");
            var bindings = new List<MemberBinding>();
            foreach (PropertyInfo sourceProperty in typeof(TFrom).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreOnCopyAttribute)) == null))
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeof(TTo).GetProperty(sourceProperty.Name);
                if (targetProperty == null && typeof(TTo) != typeof(TTarget))
                {
                    continue;
                }
                if (targetProperty == null)
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " is not present and accessible in " + typeof(TTo).FullName);
                }
                if (!targetProperty.CanWrite)
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " is not writable in " + typeof(TTo).FullName);
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " is static in " + typeof(TTo).FullName);
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    throw new ArgumentException("Property " + sourceProperty.Name + " has an incompatible type in " + typeof(TTo).FullName);
                }
                bindings.Add(Expression.Bind(targetProperty, Expression.Property(sourceParameter, sourceProperty)));
                propertyMap.Add(sourceProperty, targetProperty);
            }
            return bindings;
        }

        private static List<MemberBinding> BuildBindings()
        {
            propertyMap.Clear();

            foreach (PropertyInfo sourceProperty in typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreOnCopyAttribute)) == null))
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeof(TTarget).GetProperty(sourceProperty.Name);
                if (targetProperty == null
                    || !targetProperty.CanWrite
                    || (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0
                    || !targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    continue;
                }
                propertyMap.Add(sourceProperty, targetProperty);
            }
            foreach (PropertyInfo sourceProperty in typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreOnCopyAttribute)) == null))
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeof(TSource).GetProperty(sourceProperty.Name);
                if (targetProperty == null
                    || !targetProperty.CanWrite
                    || (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0
                    || !targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    continue;
                }
                if (propertyMap.ContainsKey(targetProperty))
                {
                    continue;
                }
                propertyMap.Add(sourceProperty, targetProperty);
            }

            return bindings;
        }
    }
}
