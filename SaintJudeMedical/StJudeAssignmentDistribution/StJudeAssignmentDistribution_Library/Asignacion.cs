using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StJudeAssignmentDistribution_Library
{
    public class Asignacion
    {
        #region Constructor
        public Asignacion()
        {

        }
        #endregion

        #region Properties
        public string Nombre { get; set; }
        public string PM { get; set; }
        public string WorkOrder { get; set; }
        public string Tiempo { get; set; }
        #endregion
    }
}
