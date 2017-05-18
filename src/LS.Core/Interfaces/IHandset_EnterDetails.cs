using System.Collections.Generic;
namespace LS.Core.Interfaces
{
    public interface IHandset_EnterDetails {
        List<IHandset> Handsets { get; set; }
        string Order_ID { get; set; }
        string EmployeeAccount { get; set; }
        string Sales_Channel { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
        bool PreOrder { get; set; }
        double HandsetPrice { get; set; }
    }
}
