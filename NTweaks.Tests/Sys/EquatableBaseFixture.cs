using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

using NTweaks.Sys;

/// <summary>
/// Test the EquatableBase class
/// </summary>
/// <remarks></remarks>
//[TestFixture()]
public class EquatableBaseFixture
{

    #region "Fakes"

    /// <summary>
    /// A fake implmentation where we can force the return values
    /// </summary>
    private class FakeEquatable : EquatableBase<FakeEquatable>
    {

        private readonly bool _equals;

        private readonly int _hashcode;
        public FakeEquatable(bool @equals, int hashcode)
        {
            _equals = @equals;
            _hashcode = hashcode;
        }

        public override bool Equals(FakeEquatable other)
        {
            return _equals;
        }

        public override int GetHashCode()
        {
            return _hashcode;
        }

    }

    /// <summary>
    /// Fake to test equality between inherited versions
    /// </summary>
    /// <remarks></remarks>
    private class FakeEquatableChild : FakeEquatable
    {

        public FakeEquatableChild(bool @equals, int hashcode)
            : base(@equals, hashcode)
        {
        }

    }
    
    #endregion

    #region "Testing where overrides are used"


    /// <summary>
    /// Test that equal instances are returned as such
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Equals_True()
	{
		//Make the objects acknowledge that they are equal to each other
		var foo = new FakeEquatable(true, 1234);
		var bar = new FakeEquatable(true, 1234);

		Assert.IsTrue(foo.Equals(bar));
		//Equals function
		Assert.IsTrue(foo == bar);
		//Equals operator
		Assert.IsTrue(foo.Equals((object)bar));
		//Equals untyped
		Assert.IsTrue((new List<FakeEquatable>{ foo }).Contains(bar));
		//Matches within list
	}

    /// <summary>
    /// Test that unequal instances are returned as such
    /// </summary>
    [Test()]
    public void Equals_False()
	{
		//Make the objects acknowledge that they are not equal to each other
		var foo = new FakeEquatable(false, 1234);
		var bar = new FakeEquatable(false, 9876);

		Assert.IsFalse(foo.Equals(bar));
		//Equals function
		Assert.IsFalse(foo == bar);
		//Equals operator
		Assert.IsFalse(foo.Equals((object)bar));
		//Equals untyped
		Assert.IsFalse((new List<FakeEquatable>{ foo }).Contains(bar));
		//Matches within list

	}

    /// <summary>
    /// Test that instances of different classes are returned as unequal
    /// </summary>
    [Test()]
    public void Equals_DifferentClass_False()
    {
        //Make the objects acknowledge that they are equal to each other
        var foo = new FakeEquatable(false, 1234);
        var bar = new {
            Text = "Bar",
            Number = 1234,
            MoreText = "Hello"
        };

        Assert.IsFalse(foo.Equals(bar));        //Equals function        
        Assert.IsFalse(foo.Equals((object)bar));        //Equals untyped

    }


    /// <summary>
    /// Check that the overridden GetHashCode correctly returns
    /// </summary>
    [Test()]
    public void GetHashCode_Returns()
    {
        //Make the objects acknowledge that they are equal to each other
        var foo = new FakeEquatable(true, 1234);

        Assert.AreEqual(1234, foo.GetHashCode());

    }

    /// <summary>
    /// Test that the RHS being null returns unequal (and doesn't throw)
    /// </summary>
    [Test()]
    public void Equals_Nulls()
	{
		//Make the objects acknowledge that they are equal to each other
		var foo = new FakeEquatable(false, 1234);
		FakeEquatable bar = null;

		Assert.IsFalse(foo.Equals(bar));		//Equals function        
		Assert.IsFalse(foo == bar);		//Equals operator
		Assert.IsFalse(bar == foo);		//Equals operator
		Assert.IsFalse(foo.Equals((object)bar));		//Equals untyped
		Assert.IsFalse((new List<FakeEquatable>{ foo }).Contains(bar));		//Matches within list
	}

    /// <summary>
    /// Test that classes that inherit from each other are equal if their Equals overrides return as such
    /// </summary>
    [Test()]
    public void Equals_Inherited_True()
	{
		var foo = new FakeEquatable(true, 1234);
		var bar = new FakeEquatableChild(true, 1234);

		Assert.IsTrue(foo.Equals(bar));
		//Equals function
		Assert.IsTrue(foo == bar);
		//Equals operator
		Assert.IsTrue(foo.Equals((object)bar));
		//Equals untyped
		Assert.IsTrue((new List<FakeEquatable>{ foo }).Contains(bar));
		//Matches within list

	}

    /// <summary>
    /// Test that classes that inherit from each other are unequal if their Equals overrides return as such
    /// </summary>
    [Test()]
    public void Equals_Inherited_False()
	{
		var foo = new FakeEquatable(false, 1234);
		var bar = new FakeEquatableChild(false, 9876);

		Assert.IsFalse(foo.Equals(bar));
		//Equals function
		Assert.IsFalse(foo == bar);
		//Equals operator
		Assert.IsFalse(foo.Equals((object)bar));
		//Equals untyped
		Assert.IsFalse((new List<FakeEquatable>{ foo }).Contains(bar));
		//Matches within list

	}


    #endregion

}