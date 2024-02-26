using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CreditBroker.Models.DataBaseModel
{
    public class CreditContext:DbContext
    {
        public CreditContext() : base("name=CreditContext")
        {
            
        }
        public virtual DbSet<CreditBroker> CreditBrokers { get; set; }
    }
}