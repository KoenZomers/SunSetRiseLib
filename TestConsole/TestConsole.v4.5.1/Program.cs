using System;

namespace KoenZomers.Tools.SunSetRiseLib
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use https://www.esrl.noaa.gov/gmd/grad/solcalc/ to validate the outcome against

            // Date for which to calculate the sunrise and sunset
            var date = DateTime.Today;

            // Latitude for which to calculate the sunrise/sunset
            var latitude = -12.46;

            // Longitude for which to calculate the sunrise/sunset
            var longitude = 130.842;

            // Hours from UTC which this location is in
            var utcOffset = 9.5;

            // Write the output to the screen
            Console.WriteLine("Date: " + date.ToLongDateString());
            Console.WriteLine("UTC Offset: " + utcOffset);
            Console.WriteLine("Coordinates: LONG " + longitude + " LAT " + latitude);
            Console.WriteLine("Sunrise: " + SunSetRiseLib.SunriseAt(latitude, longitude, date, utcOffset));
            Console.WriteLine("SunSet: " + SunSetRiseLib.SunsetAt(latitude, longitude, date, utcOffset));

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.ReadLine();
            }
        }
    }
}
