using System;

namespace KoenZomers.Tools.SunSetRiseLib
{
    /// <summary>
    /// Library to allow calculating sunrise and sunset for a specific coordinate on the world
    /// </summary>
    public class SunSetRiseLib
    {
        /// <summary>
        /// Convert radian angle to degrees
        /// </summary>
        private static double RadianToDegrees(double angleRadian)
        {
            return (180.0 * angleRadian / Math.PI);
        }

        /// <summary>
        /// Convert degree angle to radians
        /// </summary>
        private static double DegreesToRadian(double angleDegrees)
        {
            return (Math.PI * angleDegrees / 180.0);
        }

        /// <summary>
        /// Calculates the Julian day from calendar day. See https://en.wikipedia.org/wiki/Julian_day.
        /// </summary>
        /// <param name="year"> 4 digit year</param>
        /// <param name="month">January = 1</param>
        /// <param name="day">1 - 31</param>
        /// <returns>The Julian day corresponding to the date</returns>
        /// <remarks>Number is returned for start of day. Fractional days should be	added later.</remarks>
        private static double CalculateJulianDay(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }
            double a = Math.Floor(year / 100.0);
            double b = 2 - a + Math.Floor(a / 4);

            double julianDay = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5;
            return julianDay;
        }

        /// <summary>
        /// Calculates the Julian day from calendar day. See https://en.wikipedia.org/wiki/Julian_day.
        /// </summary>
        /// <param name="date">Calendar day to calculate the Julian Day for</param>
        /// <returns>The Julian day corresponding to the date</returns>
        /// <remarks>Number is returned for start of day. Fractional days should be	added later.</remarks>
        private static double CalculateJulianDay(DateTime date)
        {
            return CalculateJulianDay(date.Year, date.Month, date.Day);
        }

        /// <summary>
        /// Calculates the Julian Day compared to the year 2000
        /// </summary>
        /// <param name="julianDay">The Julian Day to convert</param>
        /// <returns>Corresponding Julian Day calculated from the year 2000</returns>
        private static double CalculateTimeJulianCentury(double julianDay)
        {
            double t = (julianDay - 2451545.0) / 36525.0;
            return t;
        }

        /// <summary>
        /// Calculate the Geometric Mean Longitude of the Sun	
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>The Geometric Mean Longitude of the Sun in degrees	</returns>
        private static double CalculateGeometricMeanLongitudeSun(double t)
        {
            double longitude = 280.46646 + t * (36000.76983 + 0.0003032 * t);
            while (longitude > 360.0)
            {
                longitude -= 360.0;
            }
            while (longitude < 0.0)
            {
                longitude += 360.0;
            }
            return longitude;
        }

        /// <summary>
        /// Calculate the Geometric Mean Anomaly of the Sun	
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>The Geometric Mean Anomaly of the Sun in degrees</returns>
        private static double CalculateGeometricMeanAnomalySun(double t)
        {
            double m = 357.52911 + t * (35999.05029 - 0.0001537 * t);
            return m;
        }

        /// <summary>
        /// Calculate the eccentricity of earth's orbit
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>The unitless eccentricity</returns>
        private static double CalculateEccentricityEarthOrbit(double t)
        {
            double e = 0.016708634 - t * (0.000042037 + 0.0000001267 * t);
            return e;
        }

        /// <summary>
        /// Calculate the equation of center for the sun
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>The equation of center for the sun	in degrees</returns>
        private static double CalculateSunEqOfCenter(double t)
        {
            double m = CalculateGeometricMeanAnomalySun(t);

            double mrad = DegreesToRadian(m);
            double sinm = Math.Sin(mrad);
            double sin2m = Math.Sin(mrad + mrad);
            double sin3m = Math.Sin(mrad + mrad + mrad);

            double c = sinm * (1.914602 - t * (0.004817 + 0.000014 * t)) + sin2m * (0.019993 - 0.000101 * t) + sin3m * 0.000289;
            return c;
        }

        /// <summary>
        /// Calculate the true longitude of the sun
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>Sun's true longitude in degrees</returns>
        private static double CalculateSunTrueLong(double t)
        {
            double l0 = CalculateGeometricMeanLongitudeSun(t);
            double c = CalculateSunEqOfCenter(t);

            double o = l0 + c;
            return o;
        }

        /// <summary>
        /// Calculate the apparent longitude of the sun
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>Sun's apparent longitude in degrees</returns>
        private static double CalculateSunApparentLong(double t)
        {
            double o = CalculateSunTrueLong(t);

            double omega = 125.04 - 1934.136 * t;
            double lambda = o - 0.00569 - 0.00478 * Math.Sin(DegreesToRadian(omega));
            return lambda;
        }

        /// <summary>
        /// Calculate the mean obliquity of the ecliptic
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>Mean obliquity in degrees</returns>
        private static double CalculateMeanObliquityOfEcliptic(double t)
        {
            double seconds = 21.448 - t * (46.8150 + t * (0.00059 - t * (0.001813)));
            double e0 = 23.0 + (26.0 + (seconds / 60.0)) / 60.0;
            return e0;
        }

        /// <summary>
        /// Calculate the corrected obliquity of the ecliptic
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>Corrected obliquity in degrees</returns>
        private static double CalculateObliquityCorrection(double t)
        {
            double e0 = CalculateMeanObliquityOfEcliptic(t);

            double omega = 125.04 - 1934.136 * t;
            double e = e0 + 0.00256 * Math.Cos(DegreesToRadian(omega));
            return e;
        }

        /// <summary>
        /// Calculate the declination of the sun
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>Sun's declination in degrees</returns>
        private static double CalculateSunDeclination(double t)
        {
            double e = CalculateObliquityCorrection(t);
            double lambda = CalculateSunApparentLong(t);

            double sint = Math.Sin(DegreesToRadian(e)) * Math.Sin(DegreesToRadian(lambda));
            double theta = RadianToDegrees(Math.Asin(sint));
            return theta;
        }

        /// <summary>
        /// Calculate the difference between true solar time and mean solar time
        /// </summary>
        /// <param name="t">Number of Julian centuries since the year 2000</param>
        /// <returns>Equation of time in minutes of time</returns>
        private static double CalculateEquationOfTime(double t)
        {
            double epsilon = CalculateObliquityCorrection(t);
            double l0 = CalculateGeometricMeanLongitudeSun(t);
            double e = CalculateEccentricityEarthOrbit(t);
            double m = CalculateGeometricMeanAnomalySun(t);

            double y = Math.Tan(DegreesToRadian(epsilon) / 2.0);
            y *= y;

            double sin2l0 = Math.Sin(2.0 * DegreesToRadian(l0));
            double sinm = Math.Sin(DegreesToRadian(m));
            double cos2l0 = Math.Cos(2.0 * DegreesToRadian(l0));
            double sin4l0 = Math.Sin(4.0 * DegreesToRadian(l0));
            double sin2m = Math.Sin(2.0 * DegreesToRadian(m));

            double Etime = y * sin2l0 - 2.0 * e * sinm + 4.0 * e * y * sinm * cos2l0
            - 0.5 * y * y * sin4l0 - 1.25 * e * e * sin2m;

            return RadianToDegrees(Etime) * 4.0;	// in minutes of time
        }

        /// <summary>
        /// Calculate the hour angle of the sun at sunrise for the latitude
        /// </summary>
        /// <param name="lat">Latitude of observer in degrees</param>
        /// <param name="solarDec">Declination angle of sun in degrees</param>
        /// <returns>Hour angle of sunrise in radians</returns>
        private static double CalculateHourAngleSunrise(double lat, double solarDec)
        {
            double latRad = DegreesToRadian(lat);
            double sdRad = DegreesToRadian(solarDec);

            double HAarg = (Math.Cos(DegreesToRadian(90.833)) / (Math.Cos(latRad) * Math.Cos(sdRad)) - Math.Tan(latRad) * Math.Tan(sdRad));

            double ha = (Math.Acos(Math.Cos(DegreesToRadian(90.833)) / (Math.Cos(latRad) * Math.Cos(sdRad)) - Math.Tan(latRad) * Math.Tan(sdRad)));
            return ha;
        }

        /// <summary>
        /// Calculate the Universal Coordinated Time (UTC) of sunset for the given day at the given location on earth
        /// </summary>
        /// <param name="JD">julian day</param>
        /// <param name="latitude">Latitude of observer in degrees</param>
        /// <param name="longitude">Longitude of observer in degrees</param>
        /// <returns>Time in minutes from zero Z</returns>
        private static double CalculateSunSetUTC(double JD, double latitude, double longitude)
        {
            var t = CalculateTimeJulianCentury(JD);
            var eqTime = CalculateEquationOfTime(t);
            var solarDec = CalculateSunDeclination(t);
            var hourAngle = CalculateHourAngleSunrise(latitude, solarDec);
            hourAngle = -hourAngle;
            var delta = longitude + RadianToDegrees(hourAngle);
            var timeUTC = 720 - (4.0 * delta) - eqTime;	// in minutes
            return timeUTC;
        }

        /// <summary>
        /// Calculate the Universal Coordinated Time (UTC) of sunrise for the given day at the given location on earth
        /// </summary>
        /// <param name="JD">julian day</param>
        /// <param name="latitude">Latitude of observer in degrees</param>
        /// <param name="longitude">Longitude of observer in degrees</param>
        /// <returns>Time in minutes from zero Z</returns>
        private static double CalculateSunRiseUTC(double JD, double latitude, double longitude)
        {
            var t = CalculateTimeJulianCentury(JD);
            var eqTime = CalculateEquationOfTime(t);
            var solarDec = CalculateSunDeclination(t);
            var hourAngle = CalculateHourAngleSunrise(latitude, solarDec);
            var delta = longitude + RadianToDegrees(hourAngle);
            var timeUTC = 720 - (4.0 * delta) - eqTime;	// in minutes
            return timeUTC;
        }

        /// <summary>
        /// Create a datetime out of the variables
        /// </summary>
        private static DateTime? GetDateTime(double time, double timezone, DateTime date, bool dst)
        {
            double JD = CalculateJulianDay(date);
            var timeLocal = time + (timezone * 60.0);
            var riseT = CalculateTimeJulianCentury(JD + time / 1440.0);
            timeLocal += ((dst) ? 60.0 : 0.0);
            return GetDateTime(timeLocal, date);
        }

        /// <summary>
        /// Create a datetime out of the variables
        /// </summary>
        private static DateTime? GetDateTime(double minutes, DateTime date)
        {
            if (double.IsNaN(minutes)) return null;
            return date.Add(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Calculate the time of the sunrise today at the provided coordinates
        /// </summary>
        /// <param name="latitude">Latitude for which to calculate the sunrise</param>
        /// <param name="longitude">Longitude for which to calculate the sunrise</param>
        /// <returns>DateTime containing the time when the sun will rise today</returns>
        public static DateTime? SunriseToday(double latitude, double longitude)
        {
            return SunriseAt(latitude, longitude, DateTime.Today);
        }

        /// <summary>
        /// Calculate the time of the sunrise at the provided coordinates at the provided date
        /// </summary>
        /// <param name="latitude">Latitude for which to calculate the sunrise</param>
        /// <param name="longitude">Longitude for which to calculate the sunrise</param>
        /// <param name="date">Date for which to calculate the sunrise</param>
        /// <returns>DateTime containing the time when the sun will rise at the provided date</returns>
        public static DateTime? SunriseAt(double latitude, double longitude, DateTime date)
        {
#if NET20
            return SunriseAt(latitude, longitude, date, TimeZone.CurrentTimeZone.GetUtcOffset(date).Hours);
#else
            return SunriseAt(latitude, longitude, date, TimeZoneInfo.Local.GetUtcOffset(date).Hours);
#endif
        }

        /// <summary>
        /// Calculate the time of the sunrise at the provided coordinates at the provided date in the provided UTC offset
        /// </summary>
        /// <param name="latitude">Latitude for which to calculate the sunrise</param>
        /// <param name="longitude">Longitude for which to calculate the sunrise</param>
        /// <param name="date">Date for which to calculate the sunrise</param>
        /// <param name="utcOffset">Hours from UTC in which the location currently is in</param>
        /// <returns>DateTime containing the time when the sun will rise at the provided date</returns>
        public static DateTime? SunriseAt(double latitude, double longitude, DateTime date, double utcOffset)
        {
            double julianDay = CalculateJulianDay(date);
            double timeUTC = CalculateSunRiseUTC(julianDay, latitude, longitude);
            double sunrise = CalculateSunRiseUTC(julianDay + (timeUTC / 1440.0), latitude, longitude);
            return GetDateTime(sunrise, utcOffset, date, false);
        }

        /// <summary>
        /// Calculate the time of the sunset today at the provided coordinates
        /// </summary>
        /// <param name="latitude">Latitude for which to calculate the sunset</param>
        /// <param name="longitude">Longitude for which to calculate the sunset</param>
        /// <returns>DateTime containing the time when the sun will set today</returns>
        public static DateTime? SunsetToday(double latitude, double longitude)
        {
            return SunsetAt(latitude, longitude, DateTime.Today);
        }

        /// <summary>
        /// Calculate the time of the sunset at the provided coordinates at the provided date
        /// </summary>
        /// <param name="latitude">Latitude for which to calculate the sunset</param>
        /// <param name="longitude">Longitude for which to calculate the sunset</param>
        /// <param name="date">Date for which to calculate the sunset</param>
        /// <returns>DateTime containing the time when the sun will set at the provided date</returns>
        public static DateTime? SunsetAt(double latitude, double longitude, DateTime date)
        {
#if NET20
            return SunsetAt(latitude, longitude, date, TimeZone.CurrentTimeZone.GetUtcOffset(date).Hours);
#else
            return SunsetAt(latitude, longitude, date, TimeZoneInfo.Local.GetUtcOffset(date).Hours);
#endif
        }

        /// <summary>
        /// Calculate the time of the sunset at the provided coordinates at the provided date in the provided UTC offset
        /// </summary>
        /// <param name="latitude">Latitude for which to calculate the sunset</param>
        /// <param name="longitude">Longitude for which to calculate the sunset</param>
        /// <param name="date">Date for which to calculate the sunset</param>
        /// <param name="utcOffset">Hours from UTC in which the location currently is in</param>
        /// <returns>DateTime containing the time when the sun will set at the provided date</returns>
        public static DateTime? SunsetAt(double latitude, double longitude, DateTime date, double utcOffset)
        {
            double julianDay = CalculateJulianDay(date);
            double timeUTC = CalculateSunSetUTC(julianDay, latitude, longitude);
            double sunset = CalculateSunSetUTC(julianDay + (timeUTC / 1440.0), latitude, longitude);
            return GetDateTime(sunset, utcOffset, date, false);
        }
    }
}