namespace ParkingPermits
{
    // The car park itself: four zones, each with the same number of spots.
    // Change SpotsPerZone here and the whole program (map, checks, overview) follows.
    public static class ParkingLot
    {
        public const int SpotsPerZone = 8;

        public static readonly string[] Zones = { "A", "B", "C", "D" };
    }
}
