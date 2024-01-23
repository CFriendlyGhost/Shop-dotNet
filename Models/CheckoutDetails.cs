namespace Shop.Models
{
    public class CheckoutDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool SameAddress { get; set; }
        public bool SaveInfo { get; set; }
        public string PaymentMethod { get; set; }
        public string CcName { get; set; }
        public string CcNumber { get; set; }
        public string CcExpiration { get; set; }
        public string CcCvv { get; set; }
    }
}
