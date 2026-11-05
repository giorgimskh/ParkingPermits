namespace ParkingPermits
{
    public class Permit
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public string OwnerName { get; set; }
        public string PermitType { get; set; }
        public string Zone { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsPaid { get; set; }

        // calculated: not columns, worked out from the values above
        public bool IsExpired
        {
            get { return ValidUntil.Date < DateTime.Today; }
        }

        // a permit can be used only when it is paid and not expired
        public bool IsActive
        {
            get { return IsPaid && !IsExpired; }
        }

        // a permit holds a parking spot until it expires
        // (an unpaid one keeps its spot reserved while it waits for payment)
        public bool TakesSpot
        {
            get { return !IsExpired; }
        }

        // days the permit can still be used; 0 when it is expired or not paid yet
        public int DaysLeft
        {
            get
            {
                if (!IsActive)
                    return 0;
                return (ValidUntil.Date - DateTime.Today).Days;
            }
        }

        public bool ExpiresSoon
        {
            get { return IsActive && DaysLeft <= 30; }
        }

        public string Validity
        {
            get
            {
                if (IsExpired)
                    return "Expired";
                if (!IsPaid)
                    return "Awaiting payment";
                if (ExpiresSoon)
                    return "Expires soon";
                return "Valid";
            }
        }

        public string Payment
        {
            get { return IsPaid ? "Paid" : "Unpaid"; }
        }
    }
}
