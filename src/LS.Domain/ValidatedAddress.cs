using LS.Core.Interfaces;

namespace LS.Domain
{
    public class ValidatedAddress : IValidatedAddress
    {
        public int Id { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool BypassRestraints { get; set; }
    }
}
