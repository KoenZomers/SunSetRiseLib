using System;
using KoenZomers.Tools.SunSetRiseLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KoenZomers.Tools.SunSetRiseLib.UnitTest
{
  [TestClass]
  public class SunSetRiseTests
  {
    private const double NewYorkLat = 40.72;
    private const double NewYorkLong = -74.02;
    private const double ReykjavikLat = 64.133;
    private const double ReykjavikLong = -21.9;
    private const double MurmanskLat = 68.96;
    private const double MurmanskLong = 32.95;

    private DateTime NotNullable(DateTime? date)
    {
      return date.Value;
    } 

    [TestMethod]
    public void TestSummer()
    {
      var TestDate = new DateTime(2018, 6, 21);
      var Expected = 9; //5am local, 9am UTC
      var Sunrise = SunSetRiseLib.SunriseAt(NewYorkLat, NewYorkLong, TestDate);
      Assert.AreEqual(Expected, NotNullable(Sunrise).ToUniversalTime().Hour);
    }

    [TestMethod]
    public void TestWinter()
    {
      var TestDate = new DateTime(2018, 12, 21);
      var Expected = 12; //7am local, 12pm UTC
      var Sunrise = SunSetRiseLib.SunriseAt(NewYorkLat, NewYorkLong, TestDate);
      Assert.AreEqual(Expected, NotNullable(Sunrise).ToUniversalTime().Hour);
    }

    [TestMethod]
    public void TestRounding()
    {
      var TestDate = new DateTime(2018, 12, 26);
      var Expected = TestDate.AddHours(12).AddMinutes(18).AddSeconds(46); //7:18:46 local, 12:18:46 UTC
      var Sunrise = SunSetRiseLib.SunriseAt(NewYorkLat, NewYorkLong, TestDate, 0);
      Assert.IsTrue(Math.Abs((Expected - NotNullable(Sunrise)).TotalSeconds) < 30, string.Format("Expected: 12:18:46 or 12:19:00 (UTC), Actual: {0:h:mm:ss}", Sunrise));
    }

    [TestMethod]
    public void TestAccuracy()
    {
      var TestDate = new DateTime(2018, 12, 25);
      var Expected = 24;
      var Sunrise = SunSetRiseLib.SunriseAt(NewYorkLat, NewYorkLong, TestDate);
      Assert.AreEqual(Expected, NotNullable(Sunrise).Second);
    }

    [TestMethod]
    public void TestPrevDaySunrise()
    {
      var TestDate = new DateTime(2018, 5, 21);
      var Expected = TestDate.DayOfYear - 1;
      var Sunrise = SunSetRiseLib.SunriseAt(MurmanskLat, MurmanskLong, TestDate, 0); //Using UTC
      Assert.AreEqual(Expected, NotNullable(Sunrise).DayOfYear);
    }

    [TestMethod]
    public void TestNextDaySunset()
    {
      var TestDate = new DateTime(2018, 6, 21);
      var Expected = TestDate.DayOfYear + 1;
      var Sunset = SunSetRiseLib.SunsetAt(ReykjavikLat, ReykjavikLong, TestDate, 0);
      Assert.AreEqual(Expected, Sunset.Value.DayOfYear);
    }

    [TestMethod]
    public void TestNoSunrise()
    {
      var TestDate = new DateTime(2018, 6, 21);
      var Sunrise = SunSetRiseLib.SunriseAt(MurmanskLat, MurmanskLong, TestDate, 3);
      Assert.IsNull(Sunrise);
    }

    [TestMethod]
    public void TestNoSunset()
    {
      var TestDate = new DateTime(2018, 6, 21);
      var Sunset = SunSetRiseLib.SunsetAt(MurmanskLat, MurmanskLong, TestDate, 3);
      Assert.IsNull(Sunset);
    }
  }
}
