# Sunrise and sunset calculation in C#

Heavily cleaned up and simplified C# library to calculate the sunrise and sunset for a location on earth on any given date. Compiled for .NET Standard 2.0.

## Usage

```C#
// Date for which to calculate the sunrise and sunset
var date = DateTime.Today;

// Latitude for which to calculate the sunrise/sunset
var latitude = 52.3702157;

// Longitude for which to calculate the sunrise/sunset
var longitude = 4.8951679;
            
// Hours from UTC which this location is in
var utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;

// Write the output to the screen
Console.WriteLine("Date: " + date.ToLongDateString());
Console.WriteLine("UTC Offset: " + utcOffset);
Console.WriteLine("Coordinates: LONG " + longitude + " LAT " + latitude);
Console.WriteLine("Sunrise: " + SunSetRiseLib.SunriseAt(latitude, longitude, date, utcOffset));
Console.WriteLine("SunSet: " + SunSetRiseLib.SunsetAt(latitude, longitude, date, utcOffset));
```

![Sample output](./SampleOutput.png)

## NuGet

Also available as NuGet Package: [KoenZomers.SunSetRise](https://www.nuget.org/packages/KoenZomers.SunSetRise)

## Version History

Version 1.2.0.0 - November 12, 2018

- Change in the way the sunrise and sunsets are being calculated following https://www.esrl.noaa.gov/gmd/grad/solcalc/
- Various bugfixes
- Timezones in between timezones, i.e. Darwin Australia at UTC+9.5 are now also supported
- Unit Tests have been added
- Library is now compiled against .NET Standard 2.0
- Documentation XML is now included in the NuGet package so you'll see the method documentation in the project where you add the NuGet package
- This update may contain non backwards compatible changes in method calls. Please validate your code and the expected outcome when upgrading.

Huge thanks to @microalps for doing all the hard work behind [this excellent update](https://github.com/KoenZomers/SunSetRiseLib/pull/1)!

Version 1.1.2.1 - October 8, 2017

Minor update to return a NULL when the sunset or sunrise can't be calculated based on the provided lat or long in combination with the timezone. Before it would throw an exception instead.

Version 1.1.2.0 - August 16, 2017

Minor update just to accommodate the NuGet package name change to get it aligned with my other NuGet packages

Version 1.1.1.0 - March 27, 2017

Changed namespace of code to make it look cleaner when used

Version 1.1.0.0 - March 27, 2017

Initial version as forked. Cleaned up code, compiled against .NET 4.6.2 and signed the assemblies.

## Credits

Forked from https://github.com/sely2k/SunSetRiseLib

## Feedback

Comments\suggestions\bug reports are welcome!

Koen Zomers
mail@koenzomers.nl
