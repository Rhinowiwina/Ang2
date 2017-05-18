namespace LS.Core.Interfaces
{
    public interface IValidatedAddress
    {
        int Id { get; set; }
        string Street1 { get; set; }
        string Street2 { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
        bool BypassRestraints { get; set; }
    }
}
