using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Web;
using System.Text;

using NT = NTweaks.Collections.Generic;
using NUnit.Framework;

[TestFixture()]
public class LookupMapFixture
{

    #region "Setup"

    private NT.Lookup<string, string> _lookupF;
    private NT.Lookup<string, string> _lookupR;

    private const string _key1 = "Key1";
    private const string _key2 = "Key2";

    private const string _key3 = "Key3";
    private const string _value1 = "Value1";
    private const string _value2 = "Value2";
    private const string _value3 = "Value3";

    private const string _value4 = "Value4";

    private NT.LookupMap<string, string> _lookupMap;

    [SetUp()]
    public void Setup()
    {
        //Create some dummy info
        _lookupF = new NT.Lookup<string, string>();
        _lookupF.Add(_key1, _value1);
        _lookupF.Add(_key1, _value2);
        _lookupF.Add(_key2, _value1);

        _lookupR = new NT.Lookup<string, string>();
        _lookupR.Add(_value1, _key1);
        _lookupR.Add(_value2, _key1);
        _lookupR.Add(_value1, _key2);

        //Create a lookupmap and inject dummy items
        _lookupMap = new NT.LookupMap<string, string>(_lookupF, _lookupR);

    }

    #endregion

    #region "Simple Methods"

    /// <summary>
    /// Checks we're able to get the total count
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Count()
    {
        var count = _lookupMap.Count();
        Assert.AreEqual(3, count);
    }

    /// <summary>
    /// Make sure that All returns each item 
    /// </summary>
    public void All()
    {

        var all = _lookupMap.All();

        Assert.AreEqual(3, all.Count());

        Assert.AreEqual(_key1, all.ElementAt(0).Key);
        Assert.AreEqual(_value1, all.ElementAt(0).Value);

        Assert.AreEqual(_key2, all.ElementAt(1).Key);
        Assert.AreEqual(_value2, all.ElementAt(1).Value);

        Assert.AreEqual(_key1, all.ElementAt(2).Key);
        Assert.AreEqual(_value2, all.ElementAt(2).Value);

    }

    /// <summary>
    /// Make sure that Keys returns each key 
    /// </summary>
    public void AllLeft()
    {

        var keys = _lookupMap.AllLeft();

        Assert.AreEqual(2, keys.Count());
        Assert.AreEqual(_key1, keys.ElementAt(0));
        Assert.AreEqual(_key2, keys.ElementAt(1));
    }

    /// <summary>
    /// Make sure that Values returns distinct values 
    /// </summary>
    public void AllRight()
    {

        var values = _lookupMap.AllRight();

        Assert.AreEqual(2, values.Count());
        Assert.AreEqual(_value1, values.ElementAt(0));
        Assert.AreEqual(_value2, values.ElementAt(1));
    }

    #endregion


    #region "Adding"

    /// <summary>
    /// Checks that Add throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_ThrowsOnInvalidArgs()
    {
        Assert.Throws<ArgumentNullException>(() => _lookupMap.Add(null, _value1));
        Assert.Throws<ArgumentNullException>(() => _lookupMap.Add(_key1, null));

    }


    /// <summary>
    /// Checks we can add a item that is new on both sides
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CanAdd()
    {
        _lookupMap.Add(_key3, _value3);

        //Check the overall item counts
        Assert.AreEqual(4, _lookupF.Count());
        Assert.AreEqual(4, _lookupR.Count());

        //Get the values from the dicts and check is ok
        Assert.AreEqual(_value3, _lookupF[_key3].First());
        Assert.AreEqual(_key3, _lookupR[_value3].First());

    }

    /// <summary>
    /// Nothing breaks if we attempt to add an RHS to an already existing LHS
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CannotAddDuplicates()
    {
        _lookupMap.Add(_key1, _value1);

        //Check the overall counts
        Assert.AreEqual(3, _lookupF.Count());
        Assert.AreEqual(3, _lookupR.Count());

    }


    #endregion

    #region "Item"


    /// <summary>
    /// Checks that Item throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]

    public void Item_ThrowsOnInvalidArgs()
    {
        Assert.Throws<ArgumentNullException>(() =>
            {
                var v = _lookupMap.Forward[null];
            });
        Assert.Throws<ArgumentNullException>(() =>
            {
                var v = _lookupMap.Reverse[null];
            });

    }

    /// <summary>
    /// Item returns values when we request an existing key
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Item_ReturnsValuesForValidKey()
    {
        Assert.AreEqual(2, _lookupMap.Forward[_key1].Count());
        Assert.AreEqual(2, _lookupMap.Reverse[_value1].Count());

        Assert.AreEqual(1, _lookupMap.Forward[_key2].Count());
        Assert.AreEqual(1, _lookupMap.Reverse[_value2].Count());

    }


    #endregion


    #region "Remove"

    /// <summary>
    /// Checks that Remove throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Remove_ThrowsOnInvalidArgs()
    {
        Assert.Throws<ArgumentNullException>(() => _lookupMap.Remove(null, _value1));
        Assert.Throws<ArgumentNullException>(() => _lookupMap.Remove(_key1, null));

    }

    /// <summary>
    /// Check we can remove a Value from a key and leave the rest of the key ok
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Remove_CanRemoveForward()
    {
        _lookupMap.Forward.Remove(_key1);

        Assert.IsFalse(_lookupF.ContainsKey(_key1));
        Assert.AreEqual(1, _lookupR.Count(_value1));
        Assert.IsFalse(_lookupR.ContainsKey(_value2));

    }


    /// <summary>
    /// Check we can remove a Value from a key and leave the rest of the key ok
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Remove_CanRemoveReverse()
    {
        _lookupMap.Reverse.Remove(_value1);

        Assert.IsFalse(_lookupF.ContainsKey(_key2));
        Assert.AreEqual(1, _lookupR.Count(_value2));
        Assert.IsFalse(_lookupR.ContainsKey(_value1));

    }

    #endregion

}