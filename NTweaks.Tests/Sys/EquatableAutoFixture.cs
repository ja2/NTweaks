using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

using NTweaks.Sys;

/// <summary>
/// Test the EquatableAuto equality automation class
/// </summary>
/// <remarks></remarks>
//[TestFixture()]
public class EquatableAutoFixture
{

    #region "Fakes"
    
    /// <summary>
    /// Simple class to test auto generation of equals and gethashcode
    /// </summary>    
    private class AutoEquatable : EquatableAuto<AutoEquatable>
    {

        [EquatableAutoProperty()]
        public string Text { get; set; }
        [EquatableAutoProperty()]
        public int Number { get; set; }
        [EquatableAutoProperty()]
        public int? Nullable { get; set; }
        public string MoreText { get; set; }

    }

    /// <summary>
    /// Simple class to test auto generation handles no properties set
    /// </summary>    
    private class AutoEquatableNoProps : EquatableAuto<AutoEquatableNoProps>
    {

        public string Text { get; set; }
        public int Number { get; set; }
        public string MoreText { get; set; }

    }

    #endregion
        
    [Test()]
    public void Equals_Auto_True()
	{
		var foo = new AutoEquatable {
			Text = "Foo",
			Number = 1234,
			Nullable = 1111,
			MoreText = "Hello"
		};
		var bar = new AutoEquatable {
			Text = "Foo",
			Number = 1234,
			Nullable = 1111,
			MoreText = "World"
		};

		Assert.IsTrue(foo.Equals(bar));		//Equals function
		Assert.IsTrue(foo == bar);		//Equals operator
		Assert.IsTrue(foo.Equals((object)bar));		//Equals untyped
		Assert.IsTrue((new List<AutoEquatable>{ foo }).Contains(bar));		//Matches within list
	}

    [Test()]
    public void Equals_Auto_False()
	{
		var foo = new AutoEquatable {
			Text = "Foo",
			Number = 1234,
			Nullable = 1111,
			MoreText = "Hello"
		};
		var bar = new AutoEquatable {
			Text = "Bar",
			Number = 9876,
			Nullable = 1111,
			MoreText = "Hello"
		};

		Assert.IsFalse(foo.Equals(bar));		//Equals function
		Assert.IsFalse(foo == bar);		//Equals operator
		Assert.IsFalse(foo.Equals((object)bar));		//Equals untyped
		Assert.IsFalse((new List<AutoEquatable>{ foo }).Contains(bar));		//Matches within list
	}

    [Test()]

    public void Equals_Auto_Nulls()
	{
		var foo = new AutoEquatable {
			Text = "Foo",
			Number = 1234,
			Nullable = 1111,
			MoreText = "Hello"
		};
		AutoEquatable bar = null;

		Assert.IsFalse(foo.Equals(bar));		//Equals function
		Assert.IsFalse(foo == bar);		//Equals operator
		Assert.IsFalse(foo.Equals((object)bar));		//Equals untyped
		Assert.IsFalse((new List<AutoEquatable>{ foo }).Contains(bar));		//Matches within list
	}


    [Test()]

    public void Equals_Auto_NullProperties()
	{
		var foo = new AutoEquatable {
			Text = null,
			Number = 1234,
			Nullable = null,
			MoreText = "Hello"
		};
		var bar = new AutoEquatable {
			Text = "Bar",
			Number = 1234,
			Nullable = 1111,
			MoreText = "World"
		};

		Assert.IsFalse(foo.Equals(bar));		//Equals function
		Assert.IsFalse(foo == bar);		//Equals operator
		Assert.IsFalse(foo.Equals((object)bar));		//Equals untyped
		Assert.IsFalse((new List<AutoEquatable>{ foo }).Contains(bar));		//Matches within list
	}


    [Test()]

    public void GetHashCode_Auto()
    {
        var foo = new AutoEquatable
        {
            Text = "Foo",
            Number = 1234,
            Nullable = 1111,
            MoreText = "Hello"
        };

        var hash = "Foo".GetHashCode() ^ 1234.GetHashCode() ^ 1111.GetHashCode();

        Assert.AreEqual(hash, foo.GetHashCode());
        //Equals function

    }

    [Test()]

    public void GetHashCode_Auto_NullProperties()
    {
        var foo = new AutoEquatable
        {
            Text = null,
            Number = 1234,
            Nullable = null,
            MoreText = "Hello"
        };

        var hash = 1234.GetHashCode();

        Assert.AreEqual(hash, foo.GetHashCode());        //Equals function

    }

    [Test()]

    public void Auto_NoPropertyAttributes()
    {
        var foo = new AutoEquatableNoProps
        {
            Text = "Foo",
            Number = 1234,
            MoreText = "Hello"
        };
        var bar = new AutoEquatableNoProps
        {
            Text = "Bar",
            Number = 9876,
            MoreText = "Hello"
        };

        Assert.Throws<InvalidOperationException>(() => foo.Equals(bar));
        Assert.Throws<InvalidOperationException>(() =>
            {
                var b = foo == bar;
            });

        Assert.Throws<InvalidOperationException>(() => foo.GetHashCode());


    }
    
}