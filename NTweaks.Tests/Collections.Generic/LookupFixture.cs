using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using NT = NTweaks.Collections.Generic;
using NUnit.Framework;

[TestFixture()]
public class LookupFixture
{

    #region "Setup"

    private Dictionary<string, List<string>> _dict;
    private const string _key = "Key";
    private const string _key2 = "Key2";

    private const string _key3 = "Key3";
    private const string _value1 = "Value1";
    private const string _value2 = "Value2";
    private const string _value3 = "Value3";

    private const string _value4 = "Value4";

    private NT.Lookup<string, string> _lookup;

    [SetUp()]
    public void Setup()
    {
        //Create some dummy lookup info in the backing dictionary
        _dict = new Dictionary<string, List<string>>();
        _dict.Add(_key, new List<String> { _value1, _value2 });
        _dict.Add(_key2, new List<String> { _value1 });

        //Create a lookup and inject dummy items
        _lookup = new NT.Lookup<string, string>(_dict);

    }

    #endregion

    #region "Simple Methods"

    /// <summary>
    /// Checks we're able to get the total count
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Count_All()
    {
        var count = _lookup.Count();

        Assert.AreEqual(3, count);

    }

    /// <summary>
    /// Checks we're able to get the number of lookup items for a given key
    /// </summary>
    /// <remarks></remarks>
    [Test()]

    public void Count_Key()
    {
        var count = _lookup[_key].Count();

        Assert.AreEqual(2, count);

    }

    #endregion


    #region "Value Adding"

    /// <summary>
    /// Checks that Add throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    [ExpectedException(typeof(ArgumentNullException))]

    public void Add_ThrowsOnInvalidArgs_key()
    {
        _lookup.Add(null, _value1);

    }


    /// <summary>
    /// Checks we can add a Value to a key with no previous items
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CanAddToNewKey()
    {
        _lookup.Add(_key3, _value3);

        //Check the overall item count
        Assert.AreEqual(3, _dict.Count);

        //Get the values from the dict and check is ok
        var vals = _dict[_key3];
        Assert.AreEqual(1, vals.Count);
        Assert.AreEqual(_value3, vals.ElementAt(0));


    }

    /// <summary>
    /// Checks we can add a Value to a key with previous items
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CanAddToExistingKey()
    {
        _lookup.Add(_key2, _value3);

        //Check the overall item count
        Assert.AreEqual(2, _dict.Count);

        //Get the values from the dict and check is ok
        var vals = _dict[_key2];
        Assert.AreEqual(2, vals.Count);
        Assert.AreEqual(_value1, vals[0]);
        Assert.AreEqual(_value3, vals[1]);



    }

    /// <summary>
    /// Nothing breaks if we attempt to add a Value to a key it that already has it mapped
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Add_CanTryToReAddValueToGroup()
    {
        _lookup.Add(_key, _value1);

        //Check the overall item count
        Assert.AreEqual(2, _dict.Count);

        //Get the values from the dict and check is ok
        var vals = _dict[_key];
        Assert.AreEqual(2, vals.Count);
        Assert.AreEqual(_value1, vals[0]);
        Assert.AreEqual(_value2, vals[1]);


    }


    #endregion

    #region "GetValues (id argument)"


    /// <summary>
    /// Checks that Item throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Item_key_ThrowsOnInvalidArgs_key()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var v = _lookup[null];
        });

    }

    /// <summary>
    /// Item returns values when we request an existing key
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Item_key_ReturnsValuesForValidKey()
    {
        var vals = _lookup[_key];

        Assert.IsNotNull(vals);
        Assert.AreEqual(2, vals.Count());
        Assert.AreEqual(_value1, vals.ElementAt(0));
        Assert.AreEqual(_value2, vals.ElementAt(1));

    }


    /// <summary>
    /// Item returns an empty list when we request an invalid key
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Item_key_ReturnsEmptyListForInvalidKey()
    {
        var vals = _lookup[_key3];

        Assert.IsNotNull(vals);
        Assert.AreEqual(0, vals.Count());

    }

    #endregion


    #region "Remove"

    /// <summary>
    /// Checks that Remove throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]

    public void Remove_ThrowsOnInvalidArgs_key()
    {
        Assert.Throws<ArgumentNullException>(() => _lookup.Remove(null, _value1));
    }

    /// <summary>
    /// Check we can remove a Value from a key and leave the rest of the key ok
    /// </summary>
    /// <remarks></remarks>
    [Test()]

    public void Remove_SingleValueFromKey()
    {
        _lookup.Remove(_key, _value2);

        //Get the items from the dict and check is ok
        var vals = _dict[_key];
        Assert.AreEqual(1, vals.Count);
        Assert.AreEqual(_value1, vals.First());


    }


    /// <summary>
    /// Check we can remove the last Value from a key
    /// </summary>
    /// <remarks></remarks>
    [Test()]

    public void Remove_LastValueFromKey()
    {
        //Remove a key via the lookup
        _lookup.Remove(_key2, _value1);

        //The key should no longer be there
        Assert.AreEqual(false, _dict.ContainsKey(_key2));
        Assert.AreEqual(1, _dict.Count);

    }

    /// <summary>
    /// Checks that Remove throws on invalid args
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void RemoveAll_ThrowsOnInvalidArgs_key()
    {
        Assert.Throws<ArgumentNullException>(() => _lookup.Remove(null));

    }

    /// <summary>
    /// Check we can remove a key entirely
    /// </summary>
    /// <remarks></remarks>
    [Test()]
    public void Remove_AllValuesFromKey()
    {
        _lookup.Remove(_key);

        //The key should no longer be there
        Assert.AreEqual(false, _dict.ContainsKey(_key));
        Assert.AreEqual(1, _dict.Count);

    }



    #endregion

}