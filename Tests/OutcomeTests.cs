using cpGames.core;

namespace Tests;

[TestClass]
public class OutcomeTests
{
    private const string FAIL_1 = "Fail 1";
    private const string FAIL_2 = "Fail 2";
    #region Methods
    private Outcome GetSuccess1()
    {
        return Outcome.Success();
    }

    private Outcome GetSuccess2()
    {
        return Outcome.Success();
    }

    private Outcome GetFail_1()
    {
        return Outcome.Fail(FAIL_1);
    }

    private Outcome GetFail_2()
    {
        return Outcome.Fail(FAIL_2);
    }

    [TestMethod]
    public void OverloadsAndTest()
    {
        var result =
            GetSuccess1() &&
            GetSuccess2();

        Assert.IsTrue(result);
        Assert.AreEqual(result.ErrorMessage, string.Empty);
        
        result =
            GetSuccess1() &&
            GetFail_1();
        Assert.IsFalse(result);
        Assert.AreEqual(result.ErrorMessage, FAIL_1);

        result =
            GetSuccess1() &&
            GetSuccess2() &&
            GetFail_2() &&
            GetFail_1();
        Assert.IsFalse(result);
        Assert.AreEqual(result.ErrorMessage, FAIL_2);
    }

    [TestMethod]
    public void OverloadsOrTest()
    {
        var result =
            GetSuccess1() ||
            GetSuccess2();

        Assert.IsTrue(result);
        Assert.AreEqual(result.ErrorMessage, string.Empty);


        result =
            GetFail_1() ||
            GetFail_2();
        Assert.IsFalse(result);

        result =
            GetSuccess1() ||
            GetFail_1();
        Assert.IsTrue(result);
        Assert.AreEqual(result.ErrorMessage, string.Empty);

        result =
            GetSuccess1() ||
            GetSuccess2() ||
            GetFail_2() ||
            GetFail_1();
        Assert.IsTrue(result);
        Assert.AreEqual(result.ErrorMessage, string.Empty);
    }

    [TestMethod]
    public void OverloadsAndOrTest()
    {
        var result =
            (GetSuccess1() && GetSuccess2()) || GetFail_1();
        Assert.IsTrue(result);
        Assert.AreEqual(result.ErrorMessage, string.Empty);

        result =
            (GetSuccess1() || GetFail_1()) && GetFail_2();
        Assert.IsFalse(result);

        result =
            (GetSuccess1() || GetFail_1()) &&
            (GetSuccess2() || GetFail_2());
        Assert.IsTrue(result);
        Assert.AreEqual(result.ErrorMessage, string.Empty);

        result =
            (GetSuccess1() && GetFail_1()) ||
            (GetSuccess2() && GetFail_2());
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void EqualityTests()
    {
        Assert.AreEqual(GetSuccess1(), GetSuccess2());
        Assert.AreEqual(GetFail_1(), GetFail_2());
        Assert.AreNotEqual(GetSuccess1(), GetFail_1());
        Assert.IsFalse(!GetSuccess1());
        Assert.IsTrue(!GetFail_1());
    }
    #endregion
}