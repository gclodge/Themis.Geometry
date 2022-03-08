namespace Themis.Geometry
{
    public static class Functions
    {
        public const double DEG_TO_RAD = Math.PI / 180.0;
        public const double RAD_TO_DEG = 180.0 / Math.PI;

        public static double ToRadians(double angleDeg)
        {
            return angleDeg * DEG_TO_RAD;
        }

        public static double ToDegrees(double angleRad)
        {
            return angleRad * RAD_TO_DEG;
        }
    }
}
