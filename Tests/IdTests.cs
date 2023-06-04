using cpGames.core;

namespace Tests;

[TestClass]
public class IdTests
{
    #region Nested type: EmptyIdProvider
    private class EmptyIdProvider : IIdProvider
    {
        #region IIdProvider Members
        public byte IdSize => 1;

        public bool HasId(Id id)
        {
            return false;
        }
        #endregion
    }
    #endregion

    #region Methods
    public static Id GenerateId(IdGenerator idGenerator, IIdProvider provider)
    {
        var generateIdOutcome = idGenerator.GenerateId(provider, out var id);
        if (!generateIdOutcome)
        {
            Assert.Fail(generateIdOutcome.ErrorMessage);
        }
        return id;
    }

    [TestMethod]
    public void IdEqualityTest()
    {
        var id1 = new Id("abc");
        var id2 = new Id("abc");
        var id3 = new Id("cba");
        Assert.IsTrue(id1 == id2);
        Assert.AreEqual(id1, id2);
        Assert.IsFalse(id1 == id3);
        Assert.AreNotEqual(id1, id3);
    }


    [TestMethod]
    public void IdDictionaryTest()
    {
        var dictionary = new Dictionary<Id, string>();
        for (byte b = 0; b < 255; b++)
        {
            var id = new Id(b);
            dictionary[id] = id.ToString();
        }
        for (byte b = 0; b < 255; b++)
        {
            var id = new Id(b);
            Assert.IsTrue(dictionary.TryGetValue(id, out var value));
            Assert.AreEqual(id.ToString(), value);
        }
    }


    [TestMethod]
    public void IdDictionaryTest2()
    {
        var dictionary = new Dictionary<Id, string>();
        dictionary.Add(new Id(0x0D), "13");
        dictionary.Add(new Id(0x0F), "15");
        dictionary.Add(new Id(0x0C), "12");
        dictionary.Add(new Id(0x11), "17");
        dictionary.Add(new Id(0x13), "19");
        dictionary.Add(new Id(0x12), "18");
        dictionary.Add(new Id(0x14), "20");
        dictionary.Add(new Id(0x0E), "14");
        dictionary.Add(new Id(0x18), "24");
        dictionary.Add(new Id(0x03), "3");
        dictionary.Add(new Id(0x04), "4");
        dictionary.Add(new Id(0x00), "0");
        dictionary.Add(new Id(0x01), "1");
        dictionary.Add(new Id(0x07), "7");
        dictionary.Add(new Id(0x06), "6");
        
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x0D), out var value));
        Assert.AreEqual("13", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x0F), out value));
        Assert.AreEqual("15", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x0C), out value));
        Assert.AreEqual("12", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x11), out value));
        Assert.AreEqual("17", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x13), out value));
        Assert.AreEqual("19", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x12), out value));
        Assert.AreEqual("18", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x14), out value));
        Assert.AreEqual("20", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x0E), out value));
        Assert.AreEqual("14", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x18), out value));
        Assert.AreEqual("24", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x03), out value));
        Assert.AreEqual("3", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x04), out value));
        Assert.AreEqual("4", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x00), out value));
        Assert.AreEqual("0", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x01), out value));
        Assert.AreEqual("1", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x07), out value));
        Assert.AreEqual("7", value);
        Assert.IsTrue(dictionary.TryGetValue(new Id(0x06), out value));
        Assert.AreEqual("6", value);
    }

    [TestMethod]
    public void AddressEqualityTest()
    {
        var id1 = new Id("abc");
        var id2 = new Id("abc");
        var id3 = new Id("cba");

        var address1 = new Address(id1, id2);
        var address2 = new Address(id1, id2);
        var address3 = new Address(id2, id3);
        Assert.AreEqual(address1, address2);
        Assert.AreNotEqual(address1, address3);
    }

    [TestMethod]
    public void AddressContainsTest()
    {
        var id1 = new Id("ab");
        var id2 = new Id("cd");
        var id3 = new Id("ef");

        var address1 = new Address(id1, id2, id3);
        var address2 = new Address(id1, id2);

        Assert.IsTrue(address1.Contains(address2));
        Assert.IsFalse(address2.Contains(address1));

        var address3 = new Address(id2, id1);
        Assert.IsFalse(address1.Contains(address3));

        var address4 = new Address(id1, id2, id3);
        Assert.IsTrue(address1.Contains(address4));
        Assert.IsTrue(address4.Contains(address1));

        var address5 = new Address(id1, id3, id2);
        Assert.IsFalse(address1.Contains(address5));
    }

    [TestMethod]
    public void AddressFromIdsTest()
    {
        var addressSize = 10;
        var rnd = new Random();
        var idProvider = new EmptyIdProvider();

        var idsOriginal = new List<Id>();
        for (var i = 0; i < addressSize; i++)
        {
            var idSize = new byte[1];
            while (idSize[0] == 0)
            {
                rnd.NextBytes(idSize);
            }
            var idGenerator = new IdGenerator(idSize[0]);
            Assert.IsTrue(idGenerator.GenerateId(idProvider, out var id));
            idsOriginal.Add(id);
        }

        var address = new Address(idsOriginal.ToArray());

        var idsResult = address.GetIds();
        Assert.AreEqual(idsOriginal.Count, idsResult.Count);

        for (var i = 0; i < idsOriginal.Count; i++)
        {
            Assert.IsTrue(idsOriginal[i] == idsResult[i]);
            Assert.AreEqual(idsOriginal[i], idsResult[i]);
        }
    }

    [TestMethod]
    public void AddressAppendTest()
    {
        var addressSize = 10;
        var rnd = new Random();
        var idProvider = new EmptyIdProvider();

        var idsOriginal = new List<Id>();
        for (var i = 0; i < addressSize; i++)
        {
            var idSize = new byte[1];
            while (idSize[0] == 0)
            {
                rnd.NextBytes(idSize);
            }
            var idGenerator = new IdGenerator(idSize[0]);
            Assert.IsTrue(idGenerator.GenerateId(idProvider, out var id));
            idsOriginal.Add(id);
        }

        var address = new Address();
        address = idsOriginal.Aggregate(address, (current, t) => current.Append(t));

        var idsResult = address.GetIds();
        Assert.AreEqual(idsOriginal.Count, idsResult.Count);

        for (var i = 0; i < idsOriginal.Count; i++)
        {
            Assert.IsTrue(idsOriginal[i] == idsResult[i]);
            Assert.AreEqual(idsOriginal[i], idsResult[i]);
        }
    }

    [TestMethod]
    public void AddressGetIdByIndexTest()
    {
        var addressSize = 10;
        var rnd = new Random();
        var idProvider = new EmptyIdProvider();

        var idsOriginal = new List<Id>();
        for (var i = 0; i < addressSize; i++)
        {
            var idSize = new byte[1];
            while (idSize[0] == 0)
            {
                rnd.NextBytes(idSize);
            }
            var idGenerator = new IdGenerator(idSize[0]);
            Assert.IsTrue(idGenerator.GenerateId(idProvider, out var id));
            idsOriginal.Add(id);
        }

        var address = new Address();
        address = idsOriginal.Aggregate(address, (current, t) => current.Append(t));

        for (var i = 0; i < addressSize; i++)
        {
            var id = address.GetId(i);
            Assert.AreEqual(id, idsOriginal[i]);
        }
    }

    [TestMethod]
    public void AddressIsPartialAddressTest()
    {
        var addressSize = 10;
        var rnd = new Random();
        var idProvider = new EmptyIdProvider();

        var idsOriginal = new List<Id>();
        for (var i = 0; i < addressSize; i++)
        {
            var idSize = new byte[4];
            while (idSize[0] == 0)
            {
                rnd.NextBytes(idSize);
            }
            var idGenerator = new IdGenerator(idSize[0]);
            Assert.IsTrue(idGenerator.GenerateId(idProvider, out var id));
            idsOriginal.Add(id);
        }

        var address = new Address();
        address = idsOriginal.Aggregate(address, (current, t) => current.Append(t));

        for (var i = 0; i < addressSize; i++)
        {
            var id = address.GetId(i);
            Assert.AreEqual(id, idsOriginal[i]);
        }
    }

    [TestMethod]
    public void ConversionTest()
    {
        byte[] addressSizes = { 1, 4 };
        foreach (var addressSize in addressSizes)
        {
            var idProvider = new EmptyIdProvider();
            var idGenerator = new IdGenerator(addressSize);
            var id = GenerateId(idGenerator, idProvider);
            var fancyIdStr = id.ToString();
            var idFromFancyStr = new Id(fancyIdStr);
            Assert.AreEqual(id, idFromFancyStr);

            var simpleIdStr = id.ToString();
            var idFromSimpleStr = new Id(simpleIdStr);
            Assert.AreEqual(id, idFromSimpleStr);
        }
    }
    #endregion
}