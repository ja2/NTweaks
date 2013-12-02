using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using System.Reflection;
using System.Linq.Expressions;

namespace NTweaks.Sys
{

    [AttributeUsage(AttributeTargets.Property)]
    public class EquatableAutoPropertyAttribute : System.Attribute
    { }


    /// <summary>
    /// Defines a base class to fully implement all parts of IEquatable in a re-usable fashion
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks></remarks>
    public abstract class EquatableAuto<T> : EquatableBase<T> where T : EquatableAuto<T>
    {

        /// <summary>
        /// When True the generated Equals and GetHashCode body values will be output to Console
        /// </summary>        
        protected bool Debug { get; set; }

        private delegate bool EqualsMethod(T @this, T other);
        private delegate int GetHashCodeMethod(T @this);

        private static EqualsMethod _equals;
        private static GetHashCodeMethod _getHashCode;

        private static bool _delegatesCompiled;

        public EquatableAuto()
        {
            if (!_delegatesCompiled)
            {
                CompileDelegates();
                _delegatesCompiled = true;
            }

        }

        #region "Delegate Generation"

        private void CompileDelegates()
        {
            //Note: This defines a default equals and gethashcode implementation based on the 
            //properties marked with the attribute.

            //Get all properties of the current type that have the equatable attribute
            var pis = this.GetType().GetProperties().Where(p => p.IsDefined(typeof(EquatableAutoPropertyAttribute), true)).ToList();

            if (pis.Any())
            {
                CompileEqualsDelegate(pis);
                CompileGetHashCodeDelegate(pis);
            }

        }

        private void CompileEqualsDelegate(IEnumerable<PropertyInfo> pis)
        {
            var type = this.GetType();

            //Make the parameter expressions for the inputs
            var thisExp = Expression.Parameter(type, "this");
            var otherExp = Expression.Parameter(type, "other");

            //For equals expression
            Expression equalsExp = null;


            foreach (var pi in pis)
            {

                //Get the property accessors
                var thisPropExp = Expression.Property(thisExp, pi.Name);
                var otherPropExp = Expression.Property(otherExp, pi.Name);

                //Build up an item in the equals expression
                BinaryExpression testExp = Expression.Equal(thisPropExp, otherPropExp);
                if (equalsExp == null)
                {
                    equalsExp = testExp;
                }
                else
                {
                    equalsExp = Expression.AndAlso(equalsExp, testExp);
                }

            }

            if (Debug)
                Console.WriteLine(String.Format("Equality Test for '{0}': {1}", type.Name, equalsExp.ToString()));

            //Compile to usable method
            if (equalsExp != null)
                _equals = Expression.Lambda<EqualsMethod>(equalsExp, thisExp, otherExp).Compile();

        }



        private void CompileGetHashCodeDelegate(IEnumerable<PropertyInfo> pis)
        {
            var type = this.GetType();

            //Make the parameter expressions for the inputs
            var thisExp = Expression.Parameter(type, "this");

            Expression hashExp = null;


            foreach (var pi in pis)
            {

                //Get the property accessors
                var thisPropExp = Expression.Property(thisExp, pi.Name);

                //Build up the hash expression
                Expression getHashExp = Expression.Call(thisPropExp, "GetHashCode", System.Type.EmptyTypes);

                //Set it to handle nulls (for reference types and nullable value types)
                if (!pi.PropertyType.IsValueType || Nullable.GetUnderlyingType(pi.PropertyType) != null)
                {
                    getHashExp = Expression.Condition(Expression.Equal(thisPropExp, Expression.Constant(null)), Expression.Constant(0), getHashExp);
                }

                if (hashExp == null)
                {
                    hashExp = getHashExp;
                }
                else
                {
                    hashExp = Expression.ExclusiveOr(hashExp, getHashExp);
                }

            }

            if (Debug)
                Console.WriteLine(string.Format("GetHashCode for '{0}': {1}", type.Name, hashExp.ToString()));

            //Compile to usable method
            if (hashExp != null)
                _getHashCode = Expression.Lambda<GetHashCodeMethod>(hashExp, thisExp).Compile();

        }

        #endregion

        /// <summary>
        /// GetHashCode generates a hashcode using code generated from the properties decorated with the EquatableAutoProperty attribute
        /// </summary>
        public override int GetHashCode()
        {

            if (_getHashCode == null)
                throw new InvalidOperationException("The EquatableAuto.GetHashCode() method requires one or more properties to be marked with the EquatableAutoProperty attribute");

            return _getHashCode((T)this);

        }

        /// <summary>
        /// Equals checks for equality using code generated from the properties decorated with the EquatableAutoProperty attribute
        /// </summary>
        public override bool Equals(T other)
        {

            if (_equals == null)
                throw new InvalidOperationException("The EquatableAuto.Equals(T other) method requires one or more properties to be marked with the EquatableAutoProperty attribute");

            if ((Object)other == null)
                return false;

            return _equals((T)this, other);

        }

    }

}