using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CGM;
using System;

namespace LS.ApiBindingModels
{
    public class LoginMsgBindingModel {
        public string Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public string Msg { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime BeginDate { get; set; }
        [Required(ErrorMessage = "Expiration Date is required")]
        public DateTime ExpirationDate { get; set; }
        [Required(ErrorMessage = "Importance Level is required")]
        public bool Active { get; set; }
        public int MsgLevel { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}