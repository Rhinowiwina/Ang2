using System.Data.Entity.ModelConfiguration;
using LS.Domain;

namespace LS.Repositories.EFConfigs
{
    public class DevDataEFConfig : EntityTypeConfiguration<DevData>
    {
        public DevDataEFConfig()
        {
            Property(o => o.DateSubmitted);

            Property(o => o.OrigState)
                .HasMaxLength(100);

            Property(o => o.E_Prg)
                .HasMaxLength(100);

            Property(o => o.Num_HouseHold)
                .HasMaxLength(4);

            Property(o => o.AuthCode)
                .HasMaxLength(100);

            Property(o => o.PreQual)
                .HasMaxLength(100);

            Property(o => o.Fname)
                .HasMaxLength(50);

            Property(o => o.MI)
                .HasMaxLength(50);

            Property(o => o.Lname)
                .HasMaxLength(50);

            Property(o => o.SSN_Decrypted)
                .HasMaxLength(50);

            Property(o => o.DOB_Decrypted)
                .HasMaxLength(50);

            Property(o => o.Address)
                .HasMaxLength(100);

            Property(o => o.City)
                .HasMaxLength(50);

            Property(o => o.State)
                .HasMaxLength(2);

            Property(o => o.Zip);

            Property(o => o.BillAsInstall);

            Property(o => o.Bill_Address)
                .HasMaxLength(100);

            Property(o => o.Bill_City)
                .HasMaxLength(50);

            Property(o => o.Bill_State)
                .HasMaxLength(2);

            Property(o => o.Bill_Zip);

            Property(o => o.DayPhone)
                .HasMaxLength(20);

            Property(o => o.Avg_Income)
                .HasMaxLength(100);

            Property(o => o.Income_Frequency)
                .HasMaxLength(50);

            Property(o => o.Qualifying_Beneficiary)
                .HasMaxLength(2);

            Property(o => o.Beneficiary_FirstName)
                .HasMaxLength(30);

            Property(o => o.Beneficiary_LastName)
                .HasMaxLength(30); ;

            Property(o => o.Beneficiary_SSN_Decrypted);

            Property(o => o.Beneficiary_DOB_Decrypted);

            Property(o => o.HOH_Spouse)
                .HasMaxLength(10);

            Property(o => o.HOH_Adults_Parent)
                .HasMaxLength(10);

            Property(o => o.HOH_Adults_Child)
                .HasMaxLength(10);

            Property(o => o.HOH_Adults_Relative)
                .HasMaxLength(10);

            Property(o => o.HOH_Adults_Roommate)
                .HasMaxLength(10);

            Property(o => o.HOH_Adults_Other)
                .HasMaxLength(10);

            Property(o => o.HOH_Adults_Other_Text)
                .HasMaxLength(200);

            Property(o => o.HOH_Expenses)
                .HasMaxLength(100);

            Property(o => o.HOH_Share_Lifeline)
                .HasMaxLength(50);

            Property(o => o.HOH_Share_Lifeline_Names)
                .HasMaxLength(50);

            Property(o => o.HOH_Agree_MultHoues)
               .HasMaxLength(50);

            Property(o => o.HOH_Agree_Violation)
                .HasMaxLength(50);

            Property(o => o.DocumentType)
               .HasMaxLength(100);

            Property(o => o.DocumentNumber)
                .HasMaxLength(100);
        }
    }
}
