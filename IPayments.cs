using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfPaymentsSiva
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPayments" in both code and config file together.
    [ServiceContract]
    public interface IPayments
    {
        // TODO: Add your service operations here
        [OperationContract]
        string Consultar(string XMLRequest);

        [OperationContract]
        string Pagar(string XMLRequest);

        [OperationContract]
        string Reversar(string XMLRequest);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Variables
    {
        [DataMember]
        public string Convenio { get; set; }
        [DataMember]
        public string Proveedor { get; set; }
        [DataMember]
        public string CodigoRetorno { get; set; }
        [DataMember]
        public string MensajeRetorno { get; set; }
        [DataMember]
        public string AutorizacionProveedor { get; set; }
        [DataMember]
        public string AutorizacionBanco { get; set; }
        [DataMember]
        public string Iden01 { get; set; }
        [DataMember]
        public string Iden02 { get; set; }
        [DataMember]
        public string Iden03 { get; set; }
        [DataMember]
        public string Iden04 { get; set; }
        [DataMember]
        public string Iden05 { get; set; }
        [DataMember]
        public string Iden06 { get; set; }
        [DataMember]
        public string Val01 { get; set; }
        [DataMember]
        public string Val02 { get; set; }
        [DataMember]
        public string Val03 { get; set; }
        [DataMember]
        public string Val04 { get; set; }
        [DataMember]
        public string Val05 { get; set; }
        [DataMember]
        public string Val06 { get; set; }
    }
}
