using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;
using WcfPaymentsSiva.Models;
using WcfPaymentsSiva.Services;

namespace WcfPaymentsSiva
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Payments" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Payments.svc or Payments.svc.cs at the Solution Explorer and start debugging.
    public class Payments : IPayments
    {
        private readonly GTDBEntities db = new GTDBEntities();
        private readonly MethodsGlb methods = new MethodsGlb();

        // VERIFICAR PETICION DE DATOS
        /// <summary>
        /// Consultar: { Convenio, Proveedor, iden01=>NumCuenta || iden02=>NumTag }
        /// Respuesta: { Convenio, Proveedor, AutorizacionProveedor, AutorizacionBanco, CodigoRetorno, MensajeRetorno, iden01=>NumCuenta || iden02=>NumTag, iden04=>NomCliente, val01=>SaldoCuenta || val02=>SaldoTag}
        /// </summary>
        /// <param name="XMLRequested"></param>
        /// <returns></returns>
        public string Consultar(string XMLRequested)
        {
            // VALIDAR SI NOS MANDAN EL TAG O NUMERO DE CUENTA
            // SI ES POR TAG, MANDAR NUM CUENTA EN 0 Y SALDO CUENTA EN 0 
            // DE LA MISMA MANERA APLICA PARA CUENTA 

            // VARIABLES
            string XMLSubmit = string.Empty;
            Variables variables = new Variables();

            try
            {
                string decodeXML = HttpUtility.HtmlDecode(XMLRequested);
                variables.Proveedor = "00000010";
                variables.AutorizacionProveedor = "26977730";

                XmlDocument XMLRequestedDoc = new XmlDocument();
                XMLRequestedDoc.LoadXml(decodeXML);

                XmlNodeList nodeEncabezado = XMLRequestedDoc.SelectNodes("/mensaje/encabezado");
                XmlNodeList nodeIdentificador = XMLRequestedDoc.SelectNodes("/mensaje/identificador");

                if (nodeEncabezado.Count > 0 && nodeIdentificador.Count > 0)
                {
                    if (nodeEncabezado[0]["proveedor"].InnerText == variables.Proveedor)
                    {
                        variables.Convenio = nodeEncabezado[0]["convenio"].InnerText;

                        if ((nodeIdentificador[0]["iden01"].InnerText != string.Empty && nodeIdentificador[0]["iden02"].InnerText == string.Empty) || (nodeIdentificador[0]["iden01"].InnerText == string.Empty && nodeIdentificador[0]["iden02"].InnerText != string.Empty))
                        {
                            if (nodeIdentificador[0]["iden01"].InnerText != string.Empty)
                            {
                                string cuenta = nodeIdentificador[0]["iden01"].InnerText;
                                var cliente = db.CuentasTelepeajes.Join(db.Clientes, ct => ct.ClienteId, cl => cl.Id,
                                                (ct, cl) => new { ct, cl }).FirstOrDefault(x => x.ct.NumCuenta == cuenta);

                                if (cliente != null)
                                {
                                    variables.AutorizacionBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText;
                                    variables.CodigoRetorno = "00";
                                    variables.MensajeRetorno = "CONSULTA REALIZADA CON EXITO";
                                    variables.Iden01 = cliente.ct.NumCuenta.ToString();
                                    variables.Iden02 = string.Empty;
                                    variables.Iden04 = cliente.cl.Nombre + " " + cliente.cl.Apellidos;
                                    variables.Val01 = (double.Parse(cliente.ct.SaldoCuenta.ToString()) / 100).ToString("F2");
                                    variables.Val02 = string.Empty;
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                                else
                                {
                                    // SI ESTAN MAL LOS DATOS 
                                    variables.AutorizacionBanco = string.Empty;
                                    variables.CodigoRetorno = "04";
                                    variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                    variables.Iden01 = string.Empty;
                                    variables.Iden02 = string.Empty;
                                    variables.Iden04 = string.Empty;
                                    variables.Val01 = string.Empty;
                                    variables.Val02 = string.Empty;
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                            }
                            else if (nodeIdentificador[0]["iden02"].InnerText != string.Empty)
                            {
                                string tag = nodeIdentificador[0]["iden02"].InnerText;
                                var cliente = db.Tags.Join(db.CuentasTelepeajes, ta => ta.CuentaId, cu => cu.Id,
                                                (ta, cu) => new { ta, cu }).FirstOrDefault(x => x.ta.NumTag == tag);

                                if (cliente != null)
                                {
                                    variables.AutorizacionBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText;
                                    variables.CodigoRetorno = "00";
                                    variables.MensajeRetorno = "CONSULTA REALIZADA CON EXITO";
                                    variables.Iden01 = string.Empty;
                                    variables.Iden02 = cliente.ta.NumTag;
                                    variables.Iden04 = cliente.cu.Clientes.Nombre + " " + cliente.cu.Clientes.Apellidos;
                                    variables.Val01 = string.Empty;
                                    variables.Val02 = (double.Parse(cliente.ta.SaldoTag.ToString()) / 100).ToString("F2");
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                                else
                                {
                                    // SI ESTAN MAL LOS DATOS 
                                    variables.AutorizacionBanco = string.Empty;
                                    variables.CodigoRetorno = "04";
                                    variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                    variables.Iden01 = string.Empty;
                                    variables.Iden02 = string.Empty;
                                    variables.Iden04 = string.Empty;
                                    variables.Val01 = string.Empty;
                                    variables.Val02 = string.Empty;
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                            }
                            else
                            {
                                // SI ESTAN MAL LOS DATOS 
                                variables.AutorizacionBanco = string.Empty;
                                variables.CodigoRetorno = "04";
                                variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                variables.Iden01 = string.Empty;
                                variables.Iden02 = string.Empty;
                                variables.Iden04 = string.Empty;
                                variables.Val01 = string.Empty;
                                variables.Val02 = string.Empty;
                                variables.Val03 = string.Empty;
                                variables.Val04 = string.Empty;
                            }
                        }
                        else
                        {
                            // SI ESTAN MAL LOS DATOS 
                            variables.AutorizacionBanco = string.Empty;
                            variables.CodigoRetorno = "04";
                            variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                            variables.Iden01 = string.Empty;
                            variables.Iden02 = string.Empty;
                            variables.Iden04 = string.Empty;
                            variables.Val01 = string.Empty;
                            variables.Val02 = string.Empty;
                            variables.Val03 = string.Empty;
                            variables.Val04 = string.Empty;
                        }
                    }
                    else
                    {
                        // SI EL CONVENIO Y EL PROVEEDOR NO COINCIDEN
                        variables.AutorizacionBanco = string.Empty;
                        variables.CodigoRetorno = "02";
                        variables.MensajeRetorno = "IDENTIFICADOR NO VIGENTE";
                        variables.Iden01 = string.Empty;
                        variables.Iden02 = string.Empty;
                        variables.Iden04 = string.Empty;
                        variables.Val01 = string.Empty;
                        variables.Val02 = string.Empty;
                        variables.Val03 = string.Empty;
                        variables.Val04 = string.Empty;
                    }
                }
                else
                {
                    // SI EL DOCUMENTO XML NO CONTIENE IDENTIFICADOR Y ENCABEZADO
                    variables.AutorizacionBanco = string.Empty;
                    variables.CodigoRetorno = "01";
                    variables.MensajeRetorno = "IDENTIFICADOR NO EXISTE";
                    variables.Iden01 = string.Empty;
                    variables.Iden02 = string.Empty;
                    variables.Iden04 = string.Empty;
                    variables.Val01 = string.Empty;
                    variables.Val02 = string.Empty;
                    variables.Val03 = string.Empty;
                    variables.Val04 = string.Empty;
                }
            }
            catch (Exception)
            {
                variables.AutorizacionBanco = string.Empty;
                variables.CodigoRetorno = "05";
                variables.MensajeRetorno = "ERROR NO DETERMINADO ENVIADO POR PROVEEDOR";
                variables.Iden01 = string.Empty;
                variables.Iden02 = string.Empty;
                variables.Iden04 = string.Empty;
                variables.Val01 = string.Empty;
                variables.Val02 = string.Empty;
                variables.Val03 = string.Empty;
                variables.Val04 = string.Empty;
            }

            XMLSubmit = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>" +
                        "<mensaje>" +
                        "<encabezado>" +
                        "<convenio>" + variables.Convenio + "</convenio>" +
                        "<proveedor>" + variables.Proveedor + "</proveedor>" +
                        "<codigoRetorno>" + variables.CodigoRetorno + "</codigoRetorno>" +
                        "<mensajeRetorno>" + variables.MensajeRetorno + "</mensajeRetorno>" +
                        "<autorizacionProveedor>" + variables.AutorizacionProveedor + "</autorizacionProveedor>" +
                        "<autorizacionBanco>" + variables.AutorizacionBanco + "</autorizacionBanco>" +
                        "</encabezado>" +
                        "<identificador>" +
                        "<!--identificadores principales-->" +
                        "<iden01 largo=\"NUMERO DE CUENTA\" corto=\"NUMERO D\">" + variables.Iden01 + "</iden01>" +
                        "<iden02 largo=\"NUMERO DE TAG\" corto=\"NUMERO T\">" + variables.Iden02 + "</iden02>" +
                        "<iden03 largo=\"\" corto=\"\"></iden03>" +
                        "<!--identificadores adicionales-->" +
                        "<iden04 largo=\"NOMBRE DEL CLIENTE\" corto=\"NOMBRE D\">" + variables.Iden04 + "</iden04>" +
                        "<iden05 largo=\"\" corto=\"\"></iden05>" +
                        "<iden06 largo=\"\" corto=\"\"></iden06>" +
                        "</identificador>" +
                        "<valor>" +
                        "<val01 largo=\"SALDO CUENTA\" corto=\"SALDO CU\">" + variables.Val01 + "</val01>" +
                        "<val02 largo=\"SALDO TAG\" corto=\"SALDO TA\">" + variables.Val02 + "</val02>" +
                        "<val03 largo=\"SALDO MODIFICAR\" corto=\"SALDO MO\">" + variables.Val03 + "</val03>" +
                        "<val04 largo=\"NUMERO REFERENCIA\" corto=\"NUMERO R\">" + variables.Val04 + "</val04>" +
                        "<val05 largo=\"\" corto=\"\"></val05>" +
                        "<!--valor utilizado únicamente para la mora-->" +
                        "<val06 largo=\"\" corto=\"\"></val06>" +
                        "</valor>" +
                        "</mensaje>";

            return XMLSubmit;
        }

        // VERIFICAR PETICION DE DATOS
        /// <summary>
        /// Pagar: { Convenio, Proveedor, iden01=>NumCuenta || iden02=>NumTag, val03=>SaldoModificar }
        /// Respuesta: { Convenio, Proveedor, AutorizacionProveedor, AutorizacionBanco,  CodigoRetorno, MensajeRetorno, iden01=>NumCuenta || iden02=>NumTag, iden04=>NomCliente ,val01=>SaldoCuenta || val02=>SaldoTag, val03=>SaldoModificar, val04=>NoReferencia }
        /// </summary>
        /// <param name="XMLRequested"></param>
        /// <returns></returns>
        public string Pagar(string XMLRequested)
        {
            // VALIDAR: CON LA CUENTA SI TIENE INFORMACIÓN DE UN TAG INDIVIDUAL
            // NO PODER PAGAR A LA CUENTA

            // VARIABLES
            string XMLSubmit = string.Empty;
            Variables variables = new Variables();

            try
            {
                variables.Proveedor = "00000010";
                variables.AutorizacionProveedor = "26977730";

                string decodeXML = HttpUtility.HtmlDecode(XMLRequested);
                XmlDocument XMLRequestedDoc = new XmlDocument();
                XMLRequestedDoc.LoadXml(decodeXML);

                XmlNodeList nodeEncabezado = XMLRequestedDoc.SelectNodes("/mensaje/encabezado");
                XmlNodeList nodeIdentificador = XMLRequestedDoc.SelectNodes("/mensaje/identificador");
                XmlNodeList nodeValores = XMLRequestedDoc.SelectNodes("/mensaje/valor");

                if (nodeEncabezado.Count > 0 && nodeIdentificador.Count > 0 && nodeValores.Count > 0)
                {
                    if (nodeEncabezado[0]["proveedor"].InnerText == variables.Proveedor)
                    {
                        variables.Convenio = nodeEncabezado[0]["convenio"].InnerText;

                        if ((nodeIdentificador[0]["iden01"].InnerText != string.Empty && nodeIdentificador[0]["iden02"].InnerText == string.Empty) || (nodeIdentificador[0]["iden01"].InnerText == string.Empty && nodeIdentificador[0]["iden02"].InnerText != string.Empty))
                        {
                            if (nodeIdentificador[0]["iden01"].InnerText != string.Empty)
                            {
                                string cuenta = nodeIdentificador[0]["iden01"].InnerText;
                                var cliente = db.Clientes.Join(db.CuentasTelepeajes, cl => cl.Id, cu => cu.ClienteId,
                                                (cl, cu) => new { cl, cu }).FirstOrDefault(x => x.cu.NumCuenta == cuenta);

                                if (cliente != null)
                                {
                                    if (cliente.cu.TypeCuenta == "Colectiva")
                                    {
                                        if (nodeValores[0]["val03"].InnerText != string.Empty)
                                        {
                                            double saldoAnterior = double.Parse(cliente.cu.SaldoCuenta) / 100;
                                            double saldoActual = Math.Round(saldoAnterior + Convert.ToDouble(nodeValores[0]["val03"].InnerText), 2);

                                            variables.Val04 = methods.RandomNumReferencia();

                                            db.OperacionesSerBIpagos.Add(new OperacionesSerBIpagos
                                            {
                                                NumAutoriBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText,
                                                NumAutoriProveedor = variables.AutorizacionProveedor,
                                                Numero = nodeIdentificador[0]["iden01"].InnerText,
                                                SaldoAnterior = saldoAnterior,
                                                SaldoModificar = Convert.ToDouble(nodeValores[0]["val03"].InnerText),
                                                SaldoActual = saldoActual,
                                                StatusOperacion = true,
                                                NoReferencia = variables.Val04,
                                                Tipo = "CUENTA",
                                                DateTOpSerBI = DateTime.Now,
                                                Concepto = "CUENTA PAGAR"
                                            });

                                            db.Configuration.ValidateOnSaveEnabled = false;

                                            cliente.cu.SaldoCuenta = saldoActual.ToString("F2").Replace(".", string.Empty);

                                            if (!cliente.cu.StatusCuenta)
                                            {
                                                if (saldoActual >= 15.25)
                                                    cliente.cu.StatusCuenta = true;

                                                cliente.cu.Tags.ToList().ForEach(x =>
                                                {
                                                    if (saldoActual >= 15.25)
                                                        x.StatusTag = true;

                                                    x.SaldoTag = saldoActual.ToString("F2").Replace(".", string.Empty);
                                                    db.Tags.Attach(x);
                                                    db.Entry(x).State = System.Data.Entity.EntityState.Modified;
                                                });
                                            }
                                            else
                                            {
                                                cliente.cu.Tags.ToList().ForEach(x =>
                                                {
                                                    x.SaldoTag = saldoActual.ToString("F2").Replace(".", string.Empty);
                                                    db.Tags.Attach(x);
                                                    db.Entry(x).State = System.Data.Entity.EntityState.Modified;
                                                });
                                            }

                                            db.CuentasTelepeajes.Attach(cliente.cu);
                                            db.Entry(cliente.cu).State = System.Data.Entity.EntityState.Modified;

                                            db.SaveChanges();

                                            variables.AutorizacionBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText;
                                            variables.CodigoRetorno = "00";
                                            variables.MensajeRetorno = "PAGO REALIZADO CON EXITO";
                                            variables.Iden01 = cliente.cu.NumCuenta.ToString();
                                            variables.Iden02 = string.Empty;
                                            variables.Iden04 = cliente.cu.Clientes.Nombre + " " + cliente.cu.Clientes.Apellidos;
                                            variables.Val01 = saldoActual.ToString("F2");
                                            variables.Val02 = string.Empty;
                                            variables.Val03 = nodeValores[0]["val03"].InnerText;
                                        }
                                        else
                                        {
                                            // SI NO TIENE SALDO PARA MODIFICAR
                                            variables.AutorizacionBanco = string.Empty;
                                            variables.CodigoRetorno = "03";
                                            variables.MensajeRetorno = "NO TIENE SALDO PENDIENTE";
                                            variables.Iden01 = string.Empty;
                                            variables.Iden02 = string.Empty;
                                            variables.Iden04 = string.Empty;
                                            variables.Val01 = string.Empty;
                                            variables.Val02 = string.Empty;
                                            variables.Val03 = string.Empty;
                                            variables.Val04 = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // SI ESTAN MAL LOS DATOS 
                                        variables.AutorizacionBanco = string.Empty;
                                        variables.CodigoRetorno = "04";
                                        variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                        variables.Iden01 = string.Empty;
                                        variables.Iden02 = string.Empty;
                                        variables.Iden04 = string.Empty;
                                        variables.Val01 = string.Empty;
                                        variables.Val02 = string.Empty;
                                        variables.Val03 = string.Empty;
                                        variables.Val04 = string.Empty;
                                    }
                                }
                                else
                                {
                                    // SI ESTAN MAL LOS DATOS 
                                    variables.AutorizacionBanco = string.Empty;
                                    variables.CodigoRetorno = "04";
                                    variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                    variables.Iden01 = string.Empty;
                                    variables.Iden02 = string.Empty;
                                    variables.Iden04 = string.Empty;
                                    variables.Val01 = string.Empty;
                                    variables.Val02 = string.Empty;
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                            }
                            else if (nodeIdentificador[0]["iden02"].InnerText != string.Empty)
                            {
                                string tag = nodeIdentificador[0]["iden02"].InnerText;
                                var cliente = db.Tags.Join(db.CuentasTelepeajes, ta => ta.CuentaId, cu => cu.Id,
                                                      (ta, cu) => new { ta, cu }).FirstOrDefault(x => x.ta.NumTag == tag);

                                if (cliente != null)
                                {
                                    if (cliente.cu.TypeCuenta == "Individual")
                                    {
                                        if (nodeValores[0]["val03"].InnerText != string.Empty)
                                        {
                                            double saldoAnterior = double.Parse(cliente.ta.SaldoTag) / 100;
                                            double saldoActual = Math.Round(saldoAnterior + Convert.ToDouble(nodeValores[0]["val03"].InnerText), 2);

                                            variables.Val04 = methods.RandomNumReferencia();
                                            db.OperacionesSerBIpagos.Add(new OperacionesSerBIpagos
                                            {
                                                NumAutoriBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText,
                                                NumAutoriProveedor = variables.AutorizacionProveedor,
                                                Numero = nodeIdentificador[0]["iden02"].InnerText,
                                                SaldoAnterior = saldoAnterior,
                                                SaldoModificar = Convert.ToDouble(nodeValores[0]["val03"].InnerText),
                                                SaldoActual = saldoActual,
                                                StatusOperacion = true,
                                                NoReferencia = variables.Val04,
                                                Tipo = "TAG",
                                                DateTOpSerBI = DateTime.Now,
                                                Concepto = "TAG PAGAR"

                                            });

                                            db.Configuration.ValidateOnSaveEnabled = false;

                                            cliente.ta.SaldoTag = saldoActual.ToString("F2").Replace(".", string.Empty);

                                            if (!cliente.ta.StatusTag)
                                            {
                                                if (saldoActual >= 15.25)
                                                    cliente.ta.StatusTag = true;
                                            }

                                            db.Tags.Attach(cliente.ta);
                                            db.Entry(cliente.ta).State = System.Data.Entity.EntityState.Modified;

                                            db.SaveChanges();

                                            variables.AutorizacionBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText;
                                            variables.CodigoRetorno = "00";
                                            variables.MensajeRetorno = "PAGO REALIZADO CON EXITO";
                                            variables.Iden01 = string.Empty;
                                            variables.Iden02 = cliente.ta.NumTag;
                                            variables.Iden04 = cliente.cu.Clientes.Nombre + " " + cliente.cu.Clientes.Apellidos;
                                            variables.Val01 = string.Empty;
                                            variables.Val02 = saldoActual.ToString("F2");
                                            variables.Val03 = nodeValores[0]["val03"].InnerText;
                                        }
                                        else
                                        {
                                            // SI NO TIENE SALDO PARA MODIFICAR
                                            variables.AutorizacionBanco = string.Empty;
                                            variables.CodigoRetorno = "03";
                                            variables.MensajeRetorno = "NO TIENE SALDO PENDIENTE";
                                            variables.Iden01 = string.Empty;
                                            variables.Iden02 = string.Empty;
                                            variables.Iden04 = string.Empty;
                                            variables.Val01 = string.Empty;
                                            variables.Val02 = string.Empty;
                                            variables.Val03 = string.Empty;
                                            variables.Val04 = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        // SI ESTAN MAL LOS DATOS 
                                        variables.AutorizacionBanco = string.Empty;
                                        variables.CodigoRetorno = "04";
                                        variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                        variables.Iden01 = string.Empty;
                                        variables.Iden02 = string.Empty;
                                        variables.Iden04 = string.Empty;
                                        variables.Val01 = string.Empty;
                                        variables.Val02 = string.Empty;
                                        variables.Val03 = string.Empty;
                                        variables.Val04 = string.Empty;
                                    }
                                }
                                else
                                {
                                    // SI ESTAN MAL LOS DATOS 
                                    variables.AutorizacionBanco = string.Empty;
                                    variables.CodigoRetorno = "04";
                                    variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                    variables.Iden01 = string.Empty;
                                    variables.Iden02 = string.Empty;
                                    variables.Iden04 = string.Empty;
                                    variables.Val01 = string.Empty;
                                    variables.Val02 = string.Empty;
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                            }
                            else
                            {
                                // SI ESTAN MAL LOS DATOS 
                                variables.AutorizacionBanco = string.Empty;
                                variables.CodigoRetorno = "04";
                                variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                variables.Iden01 = string.Empty;
                                variables.Iden02 = string.Empty;
                                variables.Iden04 = string.Empty;
                                variables.Val01 = string.Empty;
                                variables.Val02 = string.Empty;
                                variables.Val03 = string.Empty;
                                variables.Val04 = string.Empty;
                            }
                        }
                        else
                        {
                            // SI ESTAN MAL LOS DATOS 
                            variables.AutorizacionBanco = string.Empty;
                            variables.CodigoRetorno = "04";
                            variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                            variables.Iden01 = string.Empty;
                            variables.Iden02 = string.Empty;
                            variables.Iden04 = string.Empty;
                            variables.Val01 = string.Empty;
                            variables.Val02 = string.Empty;
                            variables.Val03 = string.Empty;
                            variables.Val04 = string.Empty;
                        }
                    }
                    else
                    {
                        // SI EL CONVENIO Y EL PROVEEDOR NO COINCIDEN
                        variables.AutorizacionBanco = string.Empty;
                        variables.CodigoRetorno = "02";
                        variables.MensajeRetorno = "IDENTIFICADOR NO VIGENTE";
                        variables.Iden01 = string.Empty;
                        variables.Iden02 = string.Empty;
                        variables.Iden04 = string.Empty;
                        variables.Val01 = string.Empty;
                        variables.Val02 = string.Empty;
                        variables.Val03 = string.Empty;
                        variables.Val04 = string.Empty;
                    }
                }
                else
                {
                    // SI EL DOCUMENTO XML NO CONTIENE IDENTIFICADOR Y ENCABEZADO
                    variables.AutorizacionBanco = string.Empty;
                    variables.CodigoRetorno = "01";
                    variables.MensajeRetorno = "IDENTIFICADOR NO EXISTE";
                    variables.Iden01 = string.Empty;
                    variables.Iden02 = string.Empty;
                    variables.Iden04 = string.Empty;
                    variables.Val01 = string.Empty;
                    variables.Val02 = string.Empty;
                    variables.Val03 = string.Empty;
                    variables.Val04 = string.Empty;
                }
            }
            catch (Exception)
            {
                variables.AutorizacionBanco = string.Empty;
                variables.CodigoRetorno = "05";
                variables.MensajeRetorno = "ERROR NO DETERMINADO ENVIADO POR PROVEEDOR";
                variables.Iden01 = string.Empty;
                variables.Iden02 = string.Empty;
                variables.Iden04 = string.Empty;
                variables.Val01 = string.Empty;
                variables.Val02 = string.Empty;
                variables.Val03 = string.Empty;
                variables.Val04 = string.Empty;
            }

            XMLSubmit = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>" +
                       "<mensaje>" +
                       "<encabezado>" +
                       "<convenio>" + variables.Convenio + "</convenio>" +
                       "<proveedor>" + variables.Proveedor + "</proveedor>" +
                       "<codigoRetorno>" + variables.CodigoRetorno + "</codigoRetorno>" +
                       "<mensajeRetorno>" + variables.MensajeRetorno + "</mensajeRetorno>" +
                       "<autorizacionProveedor>" + variables.AutorizacionProveedor + "</autorizacionProveedor>" +
                       "<autorizacionBanco>" + variables.AutorizacionBanco + "</autorizacionBanco>" +
                       "<!--Canal aplicacion desde donde vendra la transaccion--> " +
                       "<canal></canal>" +
                       "</encabezado>" +
                       "<identificador>" +
                       "<!--identificadores principales-->" +
                       "<iden01 largo=\"NUMERO DE CUENTA\" corto=\"NUMERO D\">" + variables.Iden01 + "</iden01>" +
                       "<iden02 largo=\"NUMERO DE TAG\" corto=\"NUMERO T\">" + variables.Iden02 + "</iden02>" +
                       "<iden03 largo=\"\" corto=\"\"></iden03>" +
                       "<!--identificadores adicionales-->" +
                       "<iden04 largo=\"NOMBRE DEL CLIENTE\" corto=\"NOMBRE D\">" + variables.Iden04 + "</iden04>" +
                       "<iden05 largo=\"\" corto=\"\"></iden05>" +
                       "<iden06 largo=\"\" corto=\"\"></iden06>" +
                       "</identificador>" +
                       "<valor>" +
                       "<val01 largo=\"SALDO CUENTA\" corto=\"SALDO CU\">" + variables.Val01 + "</val01>" +
                       "<val02 largo=\"SALDO TAG\" corto=\"SALDO TA\">" + variables.Val02 + "</val02>" +
                       "<val03 largo=\"SALDO MODIFICAR\" corto=\"SALDO MO\">" + variables.Val03 + "</val03>" +
                       "<val04 largo=\"NUMERO REFERENCIA\" corto=\"NUMERO R\">" + variables.Val04 + "</val04>" +
                       "<val05 largo=\"\" corto=\"\"></val05>" +
                       "<!--valor utilizado únicamente para la mora-->" +
                       "<val06 largo=\"\" corto=\"\"></val06>" +
                       "</valor>" +
                       "</mensaje>";

            return XMLSubmit;
        }

        // INVALIDAR SI EL TAG O CUENTA DESPUÉS DEL DESCUENTO TIENEN MENOR SALDO (15.25 para ambos)
        // MEJORAR CÓDIGO Y VERICIAR XML
        // VERIFICAR PETICION DE DATOS
        /// <summary>
        /// Reversar: { Convenio, Proveedor, val03=>SaldoModificar, val04=>NoReferencia }
        /// Respuesta: { Convenio, Proveedor, AutorizacionProveedor, AutorizacionBanco,  CodigoRetorno, MensajeRetorno, iden01=>NumCuenta || iden02=>NumTag, iden04=>NomCliente, val01=>SaldoCuenta || val02=>SaldoTag, val03=>SaldoModificar, val04=>NoReferencia }
        /// </summary>
        /// <param name="XMLRequested"></param>
        /// <returns></returns>
        public string Reversar(string XMLRequested)
        {
            // VARIABLES
            string XMLSubmit = string.Empty;
            Variables variables = new Variables();

            try
            {
                variables.Proveedor = "00000010";
                variables.AutorizacionProveedor = "26977730";

                string decodeXML = HttpUtility.HtmlDecode(XMLRequested);
                XmlDocument XMLRequestedDoc = new XmlDocument();
                XMLRequestedDoc.LoadXml(decodeXML);

                XmlNodeList nodeEncabezado = XMLRequestedDoc.SelectNodes("/mensaje/encabezado");
                XmlNodeList nodeIdentificador = XMLRequestedDoc.SelectNodes("/mensaje/identificador");
                XmlNodeList nodeValores = XMLRequestedDoc.SelectNodes("/mensaje/valor");

                if (nodeEncabezado.Count > 0 && nodeIdentificador.Count > 0 && nodeValores.Count > 0)
                {
                    if (nodeEncabezado[0]["proveedor"].InnerText == variables.Proveedor)
                    {
                        variables.Convenio = nodeEncabezado[0]["convenio"].InnerText;

                        if (nodeValores[0]["val03"].InnerText != string.Empty && nodeValores[0]["val04"].InnerText != string.Empty)
                        {
                            string NoReferencia = nodeValores[0]["val04"].InnerText;
                            var date = DateTime.Today;
                            var operacionesCliente = db.OperacionesSerBIpagos.Where(x => x.NoReferencia == NoReferencia && DbFunctions.TruncateTime(x.DateTOpSerBI) == date).FirstOrDefault();

                            if (operacionesCliente != null)
                            {
                                if (operacionesCliente.SaldoModificar.Value.ToString("F2") == nodeValores[0]["val03"].InnerText && operacionesCliente.StatusOperacion == true)
                                {
                                    var saldoanterior = 0.0d;
                                    var saldoreversar = 0.0d;

                                    switch (operacionesCliente.Tipo)
                                    {
                                        case "CUENTA":

                                            var cliente = db.CuentasTelepeajes.Join(db.Clientes, cue => cue.ClienteId, clie => clie.Id, (cue, clie) => new { cue, clie }).FirstOrDefault(x => x.cue.NumCuenta == operacionesCliente.Numero);

                                            saldoanterior = double.Parse(cliente.cue.SaldoCuenta) / 100;

                                            saldoreversar = Math.Round(saldoanterior - Convert.ToDouble(nodeValores[0]["val03"].InnerText), 2);

                                            db.Configuration.ValidateOnSaveEnabled = false;

                                            cliente.cue.SaldoCuenta = saldoreversar.ToString("F2").Replace(".", string.Empty);

                                            if (!cliente.cue.StatusCuenta)
                                            {
                                                if (saldoreversar < 15.25)
                                                    cliente.cue.StatusCuenta = false;

                                                cliente.cue.Tags.ToList().ForEach(x =>
                                                {
                                                    if (saldoreversar < 15.25)
                                                        x.StatusTag = false;

                                                    x.SaldoTag = saldoreversar.ToString("F2").Replace(".", string.Empty);
                                                    db.Tags.Attach(x);
                                                    db.Entry(x).State = System.Data.Entity.EntityState.Modified;
                                                });
                                            }
                                            else
                                            {
                                                cliente.cue.Tags.ToList().ForEach(x =>
                                                {
                                                    if (saldoreversar < 15.25)
                                                        x.StatusTag = false;

                                                    x.SaldoTag = saldoreversar.ToString("F2").Replace(".", string.Empty);
                                                    db.Tags.Attach(x);
                                                    db.Entry(x).State = System.Data.Entity.EntityState.Modified;
                                                });
                                            }

                                            db.CuentasTelepeajes.Attach(cliente.cue);
                                            db.Entry(cliente.cue).State = System.Data.Entity.EntityState.Modified;

                                            /**                                             **/

                                            variables.Val04 = methods.RandomNumReferencia();

                                            db.OperacionesSerBIpagos.Add(new OperacionesSerBIpagos
                                            {
                                                NumAutoriBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText,
                                                NumAutoriProveedor = variables.AutorizacionProveedor,
                                                Numero = NoReferencia,
                                                SaldoAnterior = saldoanterior,
                                                SaldoModificar = Convert.ToDouble(nodeValores[0]["val03"].InnerText),
                                                SaldoActual = saldoreversar,
                                                StatusOperacion = true,
                                                NoReferencia = variables.Val04,
                                                Tipo = "CUENTA",
                                                DateTOpSerBI = DateTime.Now,
                                                Concepto = "CUENTA REVERSAR"
                                            });

                                            operacionesCliente.StatusOperacion = false;
                                            db.OperacionesSerBIpagos.Attach(operacionesCliente);
                                            db.Entry(operacionesCliente).State = EntityState.Modified;

                                            db.SaveChanges();

                                            variables.AutorizacionBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText;
                                            variables.CodigoRetorno = "00";
                                            variables.MensajeRetorno = "REVERSION REALIZADA CON EXITO";
                                            variables.Iden01 = cliente.cue.NumCuenta;
                                            variables.Iden02 = string.Empty;
                                            variables.Iden04 = cliente.clie.Nombre + " " + cliente.clie.Apellidos;
                                            variables.Val01 = saldoreversar.ToString("F2");
                                            variables.Val02 = string.Empty;
                                            variables.Val03 = operacionesCliente.SaldoModificar.ToString();

                                            break;
                                        case "TAG":

                                            var clientetag = db.Tags.Join(db.CuentasTelepeajes, ta => ta.CuentaId, cue => cue.Id, (ta, cue) => new { ta, cue }).FirstOrDefault(x => x.ta.NumTag == operacionesCliente.Numero);

                                            if (clientetag != null)
                                            {
                                                saldoanterior = double.Parse(clientetag.ta.SaldoTag) / 100;

                                                saldoreversar = Math.Round(saldoanterior - Convert.ToDouble(nodeValores[0]["val03"].InnerText), 2);

                                                db.Configuration.ValidateOnSaveEnabled = false;

                                                clientetag.ta.SaldoTag = saldoreversar.ToString("F2").Replace(".", string.Empty);

                                                if (!clientetag.ta.StatusTag)
                                                {
                                                    if (saldoreversar < 15.25)
                                                        clientetag.ta.StatusTag = false;
                                                }

                                                db.Tags.Attach(clientetag.ta);
                                                db.Entry(clientetag.ta).State = System.Data.Entity.EntityState.Modified;

                                                /**                  **/

                                                variables.Val04 = methods.RandomNumReferencia();

                                                db.OperacionesSerBIpagos.Add(new OperacionesSerBIpagos
                                                {
                                                    NumAutoriBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText,
                                                    NumAutoriProveedor = variables.AutorizacionProveedor,
                                                    Numero = NoReferencia,
                                                    SaldoAnterior = saldoanterior,
                                                    SaldoModificar = Convert.ToDouble(nodeValores[0]["val03"].InnerText),
                                                    SaldoActual = saldoreversar,
                                                    StatusOperacion = true,
                                                    NoReferencia = variables.Val04,
                                                    Tipo = "TAG",
                                                    DateTOpSerBI = DateTime.Now,
                                                    Concepto = "TAG REVERSAR"
                                                });


                                                operacionesCliente.StatusOperacion = false;
                                                db.OperacionesSerBIpagos.Attach(operacionesCliente);
                                                db.Entry(operacionesCliente).State = EntityState.Modified;

                                                db.SaveChanges();

                                                variables.AutorizacionBanco = nodeEncabezado[0]["autorizacionBanco"].InnerText;
                                                variables.CodigoRetorno = "00";
                                                variables.MensajeRetorno = "REVERSION REALIZADA CON EXITO";
                                                variables.Iden01 = string.Empty;
                                                variables.Iden02 = clientetag.ta.NumTag;
                                                variables.Iden04 = $"{clientetag.cue.Clientes.Nombre} {clientetag.cue.Clientes.Apellidos}";
                                                variables.Val01 = string.Empty;
                                                variables.Val02 = saldoreversar.ToString("F2");
                                                variables.Val03 = operacionesCliente.SaldoModificar.ToString();
                                            }
                                            else
                                            {
                                                // SI ESTAN MAL LOS DATOS 
                                                variables.AutorizacionBanco = string.Empty;
                                                variables.CodigoRetorno = "04";
                                                variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                                variables.Iden01 = string.Empty;
                                                variables.Iden02 = string.Empty;
                                                variables.Iden04 = string.Empty;
                                                variables.Val01 = string.Empty;
                                                variables.Val02 = string.Empty;
                                                variables.Val03 = string.Empty;
                                                variables.Val04 = string.Empty;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                /*  */
                                else
                                {
                                    // SI ESTAN MAL LOS DATOS 
                                    variables.AutorizacionBanco = string.Empty;
                                    variables.CodigoRetorno = "04";
                                    variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                    variables.Iden01 = string.Empty;
                                    variables.Iden02 = string.Empty;
                                    variables.Iden04 = string.Empty;
                                    variables.Val01 = string.Empty;
                                    variables.Val02 = string.Empty;
                                    variables.Val03 = string.Empty;
                                    variables.Val04 = string.Empty;
                                }
                            }
                            /*  */
                            else
                            {
                                // SI ESTAN MAL LOS DATOS 
                                variables.AutorizacionBanco = string.Empty;
                                variables.CodigoRetorno = "04";
                                variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                                variables.Iden01 = string.Empty;
                                variables.Iden02 = string.Empty;
                                variables.Iden04 = string.Empty;
                                variables.Val01 = string.Empty;
                                variables.Val02 = string.Empty;
                                variables.Val03 = string.Empty;
                                variables.Val04 = string.Empty;
                            }
                        }
                        else
                        {
                            /* */
                            // SI ESTAN MAL LOS DATOS 
                            variables.AutorizacionBanco = string.Empty;
                            variables.CodigoRetorno = "04";
                            variables.MensajeRetorno = "ERROR EN DATOS DE IDENTIFICADOR";
                            variables.Iden01 = string.Empty;
                            variables.Iden02 = string.Empty;
                            variables.Iden04 = string.Empty;
                            variables.Val01 = string.Empty;
                            variables.Val02 = string.Empty;
                            variables.Val03 = string.Empty;
                            variables.Val04 = string.Empty;
                        }
                    }
                    else
                    {
                        // SI EL CONVENIO Y EL PROVEEDOR NO COINCIDEN
                        variables.AutorizacionBanco = string.Empty;
                        variables.CodigoRetorno = "02";
                        variables.MensajeRetorno = "IDENTIFICADOR NO VIGENTE";
                        variables.Iden01 = string.Empty;
                        variables.Iden02 = string.Empty;
                        variables.Iden04 = string.Empty;
                        variables.Val01 = string.Empty;
                        variables.Val02 = string.Empty;
                        variables.Val03 = string.Empty;
                    }
                }
                else
                {
                    // SI EL DOCUMENTO XML NO CONTIENE IDENTIFICADOR Y ENCABEZADO
                    variables.AutorizacionBanco = string.Empty;
                    variables.CodigoRetorno = "01";
                    variables.MensajeRetorno = "IDENTIFICADOR NO EXISTE";
                    variables.Iden01 = string.Empty;
                    variables.Iden02 = string.Empty;
                    variables.Iden04 = string.Empty;
                    variables.Val01 = string.Empty;
                    variables.Val02 = string.Empty;
                    variables.Val03 = string.Empty;
                }
            }
            catch (Exception)
            {
                variables.AutorizacionBanco = string.Empty;
                variables.CodigoRetorno = "05";
                variables.MensajeRetorno = "ERROR NO DETERMINADO ENVIADO POR PROVEEDOR";
                variables.Iden01 = string.Empty;
                variables.Iden02 = string.Empty;
                variables.Iden04 = string.Empty;
                variables.Val01 = string.Empty;
                variables.Val02 = string.Empty;
                variables.Val03 = string.Empty;
            }

            XMLSubmit = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>" +
                        "<mensaje>" +
                        "<encabezado>" +
                        "<convenio>" + variables.Convenio + "</convenio>" +
                        "<proveedor>" + variables.Proveedor + "</proveedor>" +
                        "<codigoRetorno>" + variables.CodigoRetorno + "</codigoRetorno>" +
                        "<mensajeRetorno>" + variables.MensajeRetorno + "</mensajeRetorno>" +
                        "<autorizacionProveedor>" + variables.AutorizacionProveedor + "</autorizacionProveedor>" +
                        "<autorizacionBanco>" + variables.AutorizacionBanco + "</autorizacionBanco>" +
                        "</encabezado>" +
                        "<identificador>" +
                        "<!--identificadores principales-->" +
                        "<iden01 largo=\"NUMERO DE CUENTA\" corto=\"NUMERO D\">" + variables.Iden01 + "</iden01>" +
                        "<iden02 largo=\"NUMERO DE TAG\" corto=\"NUMERO T\">" + variables.Iden02 + "</iden02>" +
                        "<iden03 largo=\"\" corto=\"\"></iden03>" +
                        "<!--identificadores adicionales-->" +
                        "<iden04 largo=\"NOMBRE DEL CLIENTE\" corto=\"NOMBRE D\">" + variables.Iden04 + "</iden04>" +
                        "<iden05 largo=\"\" corto=\"\"></iden05>" +
                        "<iden06 largo=\"\" corto=\"\"></iden06>" +
                        "</identificador>" +
                        "<valor>" +
                        "<val01 largo=\"SALDO CUENTA\" corto=\"SALDO CU\">" + variables.Val01 + "</val01>" +
                        "<val02 largo=\"SALDO TAG\" corto=\"SALDO TA\">" + variables.Val02 + "</val02>" +
                        "<val03 largo=\"SALDO MODIFICAR\" corto=\"SALDO MO\">" + variables.Val03 + "</val03>" +
                        "<val04 largo=\"NUMERO REFERENCIA\" corto=\"NUMERO R\">" + variables.Val04 + "</val04>" +
                        "<val05 largo=\"\" corto=\"\"></val05>" +
                        "<!--valor utilizado únicamente para la mora-->" +
                        "<val06 largo=\"\" corto=\"\"></val06>" +
                        "</valor>" +
                        "</mensaje>";

            return XMLSubmit;
        }
    }
}

#region XMLRequested para método consultar
/*
<?xml version="1.0" encoding="ISO-8859-1"?>
<mensaje>
<encabezado>
<convenio>8888</convenio>
<proveedor>8888</proveedor>
<codigoRetorno></codigoRetorno>
<mensajeRetorno></mensajeRetorno>
<autorizacionProveedor></autorizacionProveedor>
<autorizacionBanco>554433</autorizacionBanco>
</encabezado>
<identificador>
<!--identificadores principales-->
<iden01 largo="NUMERO DE CUENTA" corto="NUMERO D">19026234</iden01>
<iden02 largo="NUMERO DE TAG" corto="NUMERO T"></iden02>
<iden03 largo="" corto=""></iden03>
<!--identificadores adicionales-->
<iden04 largo="NOMBRE DEL CLIENTE" corto="NOMBRE D"></iden04>
<iden05 largo="" corto=""></iden05>
<iden06 largo="" corto=""></iden06>
</identificador>
<valor>
<val01 largo="SALDO CUENTA" corto="SALDO CU"></val01>
<val02 largo="SALDO TAG" corto="SALDO TA"></val02>
<val03 largo="SALDO MODIFICAR" corto="SALDO MO"></val03>
<val04 largo="NUMERO REFERENCIA" corto="NUMERO R"></val04>
<val05 largo="" corto=""></val05>
<!--valor utilizado únicamente para la mora-->
<val06 largo="" corto=""></val06>
</valor>
</mensaje>
*/
//<?xml version="1.0" encoding="ISO-8859-1"?><mensaje><encabezado><convenio>8888</convenio><proveedor>8888</proveedor><codigoRetorno></codigoRetorno><mensajeRetorno></mensajeRetorno><autorizacionProveedor></autorizacionProveedor><autorizacionBanco>554433</autorizacionBanco></encabezado><identificador><!--identificadores principales--><iden01 largo="NUMERO DE CUENTA" corto="NUMERO D"></iden01><iden02 largo="NUMERO DE TAG" corto="NUMERO T">50100007722</iden02><iden03 largo="" corto=""></iden03><!--identificadores adicionales--><iden04 largo="NOMBRE DEL CLIENTE" corto="NOMBRE D"></iden04><iden05 largo="" corto=""></iden05><iden06 largo="" corto=""></iden06></identificador><valor><val01 largo="SALDO CUENTA" corto="SALDO CU"></val01><val02 largo="SALDO TAG" corto="SALDO TA"></val02><val03 largo="SALDO MODIFICAR" corto="SALDO MO"></val03><val04 largo="NUMERO REFERENCIA" corto="NUMERO R"></val04><val05 largo="" corto=""></val05><!--valor utilizado únicamente para la mora--><val06 largo="" corto=""></val06></valor></mensaje>

#endregion

#region XMLRequested para método pagar
/*
<?xml version="1.0" encoding="ISO-8859-1"?>
<mensaje>
<encabezado>
<convenio>8888</convenio>
<proveedor>8888</proveedor>
<codigoRetorno></codigoRetorno>
<mensajeRetorno></mensajeRetorno>
<autorizacionProveedor></autorizacionProveedor>
<autorizacionBanco>554433</autorizacionBanco>
</encabezado>
<identificador>
<!--identificadores principales-->
<iden01 largo="NUMERO DE CUENTA" corto="NUMERO D">19021167</iden01>
<iden02 largo="NUMERO DE TAG" corto="NUMERO T"></iden02>
<iden03 largo="" corto=""></iden03>
<!--identificadores adicionales-->
<iden04 largo="NOMBRE DEL CLIENTE" corto="NOMBRE D"></iden04>
<iden05 largo="" corto=""></iden05>
<iden06 largo="" corto=""></iden06>
</identificador>
<valor>
<val01 largo="SALDO CUENTA" corto="SALDO CU"></val01>
<val02 largo="SALDO TAG" corto="SALDO TA"></val02>
<val03 largo="SALDO MODIFICAR" corto="SALDO MO">10.10</val03>
<val04 largo="NUMERO REFERENCIA" corto="NUMERO R"></val04>
<val05 largo="" corto=""></val05>
<!--valor utilizado únicamente para la mora-->
<val06 largo="" corto=""></val06>
</valor>
</mensaje>
/<?xml version="1.0" encoding="ISO-8859-1"?><mensaje><encabezado><convenio>8888</convenio><proveedor>8888</proveedor><codigoRetorno></codigoRetorno><mensajeRetorno></mensajeRetorno><autorizacionProveedor></autorizacionProveedor><autorizacionBanco>554433</autorizacionBanco></encabezado><identificador><!--identificadores principales--><iden01 largo="NUMERO DE CUENTA" corto="NUMERO D"></iden01><iden02 largo="NUMERO DE TAG" corto="NUMERO T">50100012218</iden02><iden03 largo="" corto=""></iden03><!--identificadores adicionales--><iden04 largo="NOMBRE DEL CLIENTE" corto="NOMBRE D"></iden04><iden05 largo="" corto=""></iden05><iden06 largo="" corto=""></iden06></identificador><valor><val01 largo="SALDO CUENTA" corto="SALDO CU"></val01><val02 largo="SALDO TAG" corto="SALDO TA"></val02><val03 largo="SALDO MODIFICAR" corto="SALDO MO">0.60</val03><val04 largo="NUMERO REFERENCIA" corto="NUMERO R"></val04><val05 largo="" corto=""></val05><!--valor utilizado únicamente para la mora--><val06 largo="" corto=""></val06></valor></mensaje>
 */
#endregion

#region XMLRequested para método reversar
/*
 <?xml version="1.0" encoding="ISO-8859-1"?>
<mensaje>
<encabezado>
<convenio>8888</convenio>
<proveedor>8888</proveedor>
<codigoRetorno></codigoRetorno>
<mensajeRetorno></mensajeRetorno>
<autorizacionProveedor>1234</autorizacionProveedor>
<autorizacionBanco>554433</autorizacionBanco>
</encabezado>
<identificador>
<!--identificadores principales-->
<iden01 largo="NUMERO DE CUENTA" corto="NUMERO D"></iden01>
<iden02 largo="NUMERO DE TAG" corto="NUMERO T"></iden02>
<iden03 largo="" corto=""></iden03>
<!--identificadores adicionales-->
<iden04 largo="NOMBRE DEL CLIENTE" corto="NOMBRE D"></iden04>
<iden05 largo="" corto=""></iden05>
<iden06 largo="" corto=""></iden06>
</identificador>
<valor>
<val01 largo="SALDO CUENTA" corto="SALDO CU"></val01>
<val02 largo="SALDO TAG" corto="SALDO TA"></val02>
<val03 largo="SALDO MODIFICAR" corto="SALDO MO">0.60</val03>
<val04 largo="NUMERO REFERENCIA" corto="NUMERO R">0000011</val04>
<val05 largo="" corto=""></val05>
<!--valor utilizado únicamente para la mora-->
<val06 largo="" corto=""></val06>
</valor>
</mensaje>
//<?xml version="1.0" encoding="ISO-8859-1"?><mensaje><encabezado><convenio>8888</convenio><proveedor>8888</proveedor><codigoRetorno></codigoRetorno><mensajeRetorno></mensajeRetorno><autorizacionProveedor>1234</autorizacionProveedor><autorizacionBanco>554433</autorizacionBanco></encabezado><identificador><!--identificadores principales--><iden01 largo="NUMERO DE CUENTA" corto="NUMERO D"></iden01><iden02 largo="NUMERO DE TAG" corto="NUMERO T"></iden02><iden03 largo="" corto=""></iden03><!--identificadores adicionales--><iden04 largo="NOMBRE DEL CLIENTE" corto="NOMBRE D"></iden04><iden05 largo="" corto=""></iden05><iden06 largo="" corto=""></iden06></identificador><valor><val01 largo="SALDO CUENTA" corto="SALDO CU"></val01><val02 largo="SALDO TAG" corto="SALDO TA"></val02><val03 largo="SALDO MODIFICAR" corto="SALDO MO">0.60</val03><val04 largo="NUMERO REFERENCIA" corto="NUMERO R">0000011</val04><val05 largo="" corto=""></val05><!--valor utilizado únicamente para la mora--><val06 largo="" corto=""></val06></valor></mensaje>
 */
#endregion