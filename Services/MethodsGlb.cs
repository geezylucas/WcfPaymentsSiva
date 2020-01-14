using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WcfPaymentsSiva.Models;

namespace WcfPaymentsSiva.Services
{
    public class MethodsGlb
    {
        private GTDBEntities db = new GTDBEntities();

        /// <summary>
        /// Método para crear num referencia en operaciones cajeros Async.
        /// </summary>
        /// <returns></returns>
        public async Task<string> RandomNumReferenciaAsync()
        {
            try
            {
                var numreferencia = string.Empty;
                int num = 1;
                var date = DateTime.Today;

                var query = await db.OperacionesSerBIpagos.Where(t => DbFunctions.TruncateTime(t.DateTOpSerBI) == date).ToListAsync();

                if (query.Count == 0)
                    numreferencia = string.Format("{0}", num.ToString("D7"));
                else
                {
                    var operaciones = query.OrderByDescending(i => i.Id).FirstOrDefault();

                    var numreferantiguo = operaciones.NoReferencia;

                    var convertnum = Convert.ToUInt64(numreferantiguo);

                    numreferencia = string.Format("{0}", (convertnum + 1).ToString("D7"));
                }

                return numreferencia;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Método para crear num referencia en operaciones cajeros.
        /// </summary>
        /// <returns></returns>
        public string RandomNumReferencia()
        {
            try
            {
                var numreferencia = string.Empty;
                int num = 1;
                var date = DateTime.Today;

                var query = db.OperacionesSerBIpagos.Where(t => DbFunctions.TruncateTime(t.DateTOpSerBI) == date).ToList();

                if (query.Count == 0)
                    numreferencia = string.Format("{0}", num.ToString("D7"));
                else
                {
                    var operaciones = query.OrderByDescending(i => i.Id).FirstOrDefault();

                    var numreferantiguo = operaciones.NoReferencia;

                    var convertnum = Convert.ToUInt64(numreferantiguo);

                    numreferencia = string.Format("{0}", (convertnum + 1).ToString("D7"));
                }

                return numreferencia;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}