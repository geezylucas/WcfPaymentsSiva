//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfPaymentsSiva.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CuentasTelepeajes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CuentasTelepeajes()
        {
            this.Tags = new HashSet<Tags>();
        }
    
        public long Id { get; set; }
        public string NumCuenta { get; set; }
        public string SaldoCuenta { get; set; }
        public string TypeCuenta { get; set; }
        public bool StatusCuenta { get; set; }
        public bool StatusResidenteCuenta { get; set; }
        public System.DateTime DateTCuenta { get; set; }
        public long ClienteId { get; set; }
        public string IdCajero { get; set; }
    
        public virtual Clientes Clientes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tags> Tags { get; set; }
    }
}
