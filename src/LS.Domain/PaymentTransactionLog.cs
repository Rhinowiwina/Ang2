using System;
using LS.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace LS.Domain
{
    public class PaymentTransactionLog : IEntity<string> {

        public PaymentTransactionLog() {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }
        public string CompanyID { get; set; }
        public Company Company { get; set; }
        public string TransactionType { get; set; }
        public string OrderType { get; set; }
        public string MerchantID { get; set; }
        public string ReferenceTransaction { get; set; }
        public double Amount { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
