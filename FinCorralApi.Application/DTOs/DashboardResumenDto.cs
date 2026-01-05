using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCorralApi.Application.DTOs
{
    public class DashboardResumenDto
    {
        public decimal TotalPrestado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal InteresesGanados { get; set; }
        public int PrestamosActivos { get; set; }
        public int TotalClientes { get; set; }
    }

}
