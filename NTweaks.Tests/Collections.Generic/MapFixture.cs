using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Text;

using NTweaks.Collections.Generic;
using NUnit.Framework;

[TestFixture()]
public class MapFixture
{

    #region "Setup"

    private Dictionary<string, string> _dictF;

    private Dictionary<string, string> _dictR;
    private const string _key1 = "Key1";
    private const string _key2 = "Key2";

    private const string _key3 = "Key3";
    private const string _value1 = "Value1";
    private const string _value2 = "Value2";
    private const string _value3 = "Value3";

    private const string _value4 = "Value4";

    private Map<string, string> _map;
    [SetUp()]

    public void Setup()
    {
        //Create some dummy info
        _dictF = new Dictionary<string, string>();
        _dictF.Add(_key1, _value1);
        _dictF.Add(_key2, _value2);

        _dictR = new Dictionary<string, string>();
        _dictR.Add(_value1, _key1);
        _dictR.Add(_value2, _key2);

        //Create a map and inject dummy items
        _map = new Map<string, string>(_dictF, _dictR);

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
        var count = _map.Count();

        Assert.AreEqual(2, count);

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
        Assert.Throws<ArgumentNullException>(() => _map.Add(null, _value1));
        Assert.Throws<ArgumentNullException>(() => _map.Add(_key1, null));

    }


    /// <summary>
    /// Checks we can add new map item
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CanAdd()
    {
        _map.Add(_key3, _value3);

        //Check the backing dict counts
        Assert.AreEqual(3, _dictF.Count);
        Assert.AreEqual(3, _dictR.Count);

        //Get the values from the dicts and check is ok
        Assert.AreEqual(_value3, _dictF[_key3]);
        Assert.AreEqual(_key3, _dictR[_value3]);

    }

    /// <summary>
    /// Adding a duplicate in either direction thrown an Exception
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CannotAddDuplicates()
    {
        Assert.Throws<ArgumentException>(() => _map.Add(_key1, _value3));
        Assert.Throws<ArgumentException>(() => _map.Add(_key3, _value1));

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
                var v = _map.Forward[null];
            });
        Assert.Throws<ArgumentNullException>(() =>
            {
                var v = _map.Reverse[null];
            });
            
    }

    /// <summary>
    /// Item returns values when we request an existing key
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Item_ReturnsValuesForValidKey()
    {
        Assert.AreEqual(_value1, _map.Forward[_key1]);
        Assert.AreEqual(_key1, _map.Reverse[_value1]);

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
        Assert.Throws<ArgumentNullException>(() => _map.Forward.Remove(null));
        Assert.Throws<ArgumentNullException>(() => _map.Reverse.Remove(null));

    }

    /// <summary>
    /// Check we can remove a Value from a key and leave the rest of the key ok
    /// </summary>
    /// <remarks></remarks>
    [Test()]

    public void Remove_CanRemoveForward()
    {
        _map.Forward.Remove(_key1);

        Assert.IsFalse(_dictF.ContainsKey(_key1));
        Assert.IsFalse(_dictR.ContainsKey(_value1));

    }


    /// <summary>
    /// Check we can remove a Value from a key and leave the rest of the key ok
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Remove_CanRemoveReverse()
    {
        _map.Reverse.Remove(_value1);

        Assert.IsFalse(_dictF.ContainsKey(_key1));
        Assert.IsFalse(_dictR.ContainsKey(_value1));

    }

    #endregion

}