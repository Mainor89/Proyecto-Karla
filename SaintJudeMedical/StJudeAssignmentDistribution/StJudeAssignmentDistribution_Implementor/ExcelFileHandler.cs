using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.Configuration;
//========== Our Projects References ==========//
using StJudeAssignmentDistribution_Library;


namespace StJudeAssignmentDistribution_Implementor
{
    public class ExcelFileHandler
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        private ExcelFileHandler()
        {
            MyApp = null;
            MyBook = null;
            MySheet = null;
            ListaDeEquiposCalibrar = new List<Equipo>();
            ListaDeEquiposUnicos = new List<Equipo>();
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Obtiene la instancia estatica del lector de excel
        /// </summary>
        public static ExcelFileHandler Instance
        {
            get
            {
                if (_LectorExcel == null)
                {
                    lock (LockObj)
                    {
                        if (_LectorExcel == null)
                        {
                            _LectorExcel = new ExcelFileHandler();
                        }
                    }
                }
                return _LectorExcel;
            }
        }

        /// <summary>
        /// Lista de equipos a calibrar
        /// </summary>
        public List<Equipo> ListaDeEquiposCalibrar
        {
            get;
            set;
        }

        /// <summary>
        /// Lista de Equipos únicos
        /// </summary>
        public List<Equipo> ListaDeEquiposUnicos
        {
            get;
            set;
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Lee el excel y carga todos los equipos que se requieren calibrar
        /// </summary>
        public void ReadExcelFile()
        {
            try
            {
                var firstColumn = ConfigurationSettings.AppSettings.Get(EXCEL_FIRST_COLUMN_KEY);
                var latestColumn = ConfigurationSettings.AppSettings.Get(EXCEL_LATEST_COLUMN_KEY);
                MyApp = new Excel.Application();
                MyApp.Visible = false;
                MyBook = MyApp.Workbooks.Open(ConfigurationSettings.AppSettings.Get(EXCEL_RESOURCE_KEY));
                MySheet = (Excel.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here
                var lastRow = MySheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;                
                for (int index = 2; index <= lastRow; index++)
                {
                    System.Array MyValues = (System.Array)MySheet.get_Range(firstColumn +
                       index.ToString(), latestColumn + index.ToString()).Cells.Value;
                    var equipoTemp = new Equipo()
                    {
                        Select = MyValues.GetValue(1, 1) != null ? MyValues.GetValue(1, 1).ToString():string.Empty,
                        WorkOrder = MyValues.GetValue(1, 2) != null ? MyValues.GetValue(1, 2).ToString() : string.Empty,
                        Description = MyValues.GetValue(1, 3) != null ? MyValues.GetValue(1, 3).ToString() : string.Empty,
                        Status = MyValues.GetValue(1, 4) != null ? MyValues.GetValue(1, 4).ToString() : string.Empty,
                        Equipment = MyValues.GetValue(1, 5) != null ? MyValues.GetValue(1, 5).ToString() : string.Empty,
                        EquipmentOrg = MyValues.GetValue(1, 6) != null ? MyValues.GetValue(1, 6).ToString() : string.Empty,
                        PM = MyValues.GetValue(1, 7) != null ? MyValues.GetValue(1, 7).ToString() : string.Empty,
                        PMType = MyValues.GetValue(1, 8) != null ? MyValues.GetValue(1, 8).ToString() : string.Empty,
                        MaintenancePattern = MyValues.GetValue(1, 9) != null ? MyValues.GetValue(1, 9).ToString() : string.Empty,
                        Sequence = MyValues.GetValue(1, 10) != null ? MyValues.GetValue(1, 10).ToString() : string.Empty,
                        ScheduledStartDate = DateTime.Parse(MyValues.GetValue(1, 11).ToString()),
                        WorkPackage = MyValues.GetValue(1, 12) != null ? MyValues.GetValue(1, 12).ToString() : string.Empty,
                        WO_Type = MyValues.GetValue(1, 13) != null ? MyValues.GetValue(1, 13).ToString() : string.Empty,
                        ErrorMessage = MyValues.GetValue(1, 14) != null ? MyValues.GetValue(1, 14).ToString() : string.Empty,
                        TiempoEstandar = "5",
                    };
                    ListaDeEquiposCalibrar.Add(equipoTemp);
                }
                MyBook.Close(true);
                MyApp.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Genera la lista de equipos unicos basandose en la lista de equipos cargados para calibrar
        /// </summary>
        public void GenerarListaDeEquiposUnicos()
        {
            try
            {
                foreach (var equipo in ListaDeEquiposCalibrar)
                {
                    var index = ListaDeEquiposUnicos.FindIndex(equipoTemp => equipoTemp.PM == equipo.PM);
                    if (index < 0)
                    {
                        ListaDeEquiposUnicos.Add(equipo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        /// <summary>
        /// Genera el archivo con las asignaciones de las calibraciones que el sistema creo
        /// </summary>
        /// <param name="pAsignaciones"></param>
        public void GenerarArchivoAsignaciones(List<Asignacion> pAsignaciones)
        {
            try
            {
                var archivoAsignaciones = ConfigurationSettings.AppSettings.Get(EXCEL_ASSIGNMENTS_RESULT_KEY);
                Excel.Application excel = new Excel.Application();
                excel.DisplayAlerts = false;
                Excel.Workbook wb = excel.Workbooks.Add();
                Excel.Worksheet hoja;//crea una hoja
                hoja = (Excel.Worksheet)wb.Worksheets[1];//indicas que esa hoja va a contener la hoja 1 del libro
                hoja.Cells[1, 1] = "Nombre";
                hoja.Cells[1, 2] = "PM";
                hoja.Cells[1, 3] = "Work Order";
                hoja.Cells[1, 4] = "Tiempo";
                var row = 2;
                foreach (var asignacion in pAsignaciones)
                {
                    hoja.Cells[row, 1] = asignacion.Nombre;
                    hoja.Cells[row, 2] = asignacion.PM;
                    hoja.Cells[row, 3] = asignacion.WorkOrder;
                    hoja.Cells[row, 4] = asignacion.Tiempo;
                    row++;
                }
                wb.SaveAs(archivoAsignaciones);
                wb.Close(true);
                excel.Quit();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Genera el archivo con los tiempos de los equipos actualizados
        /// </summary>
        /// <param name="pEquipos"></param>
        public void GenerarArchivoEquipo(List<EquipoActualizado> pEquipos)
        {
            try
            {
                var archivoEquipos = ConfigurationSettings.AppSettings.Get(EXCEL_EQUIPMENT_RESULT_KEY);
                Excel.Application excel = new Excel.Application();
                excel.DisplayAlerts = false;
                Excel.Workbook wb = excel.Workbooks.Add();                
                Excel.Worksheet hoja;//crea una hoja
                hoja = (Excel.Worksheet)wb.Worksheets[1];//indicas que esa hoja va a contener la hoja 1 del libro
                hoja.Cells[1, 1] = "PM";
                hoja.Cells[1, 2] = "Organización";
                hoja.Cells[1, 3] = "Descripción";
                hoja.Cells[1, 4] = "Tiempo";
                var row = 2;
                foreach (var equipo in pEquipos)
                {
                    hoja.Cells[row, 1] = equipo.PM;
                    hoja.Cells[row, 2] = equipo.Organizacion;
                    hoja.Cells[row, 3] = equipo.Descripcion;
                    hoja.Cells[row, 4] = equipo.Tiempo;
                    row++;
                }
                wb.SaveAs(archivoEquipos);
                wb.Close(true);
                excel.Quit();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Atributos
        /// <summary>
        /// Instancia para Singleton
        /// </summary>
        private static ExcelFileHandler _LectorExcel;
        /// <summary>
        /// Lock utilizado para la inicializacion de la instancia
        /// </summary>
        protected static readonly Object LockObj = new Object();
        /// <summary>
        /// 
        /// </summary>
        private Excel.Workbook MyBook;
        /// <summary>
        /// 
        /// </summary>
        private Excel.Application MyApp;
        /// <summary>
        /// 
        /// </summary>
        private Excel.Worksheet MySheet;
        #endregion

        #region Constantes
        /// <summary>
        /// 
        /// </summary>
        private const string EXCEL_RESOURCE_KEY = "ResourceFilePath";
        /// <summary>
        /// 
        /// </summary>
        private const string EXCEL_ASSIGNMENTS_RESULT_KEY = "ResultFilePathAssignments";
        /// <summary>
        /// 
        /// </summary>
        private const string EXCEL_EQUIPMENT_RESULT_KEY = "ResultFilePathEquipment";
        /// <summary>
        /// 
        /// </summary>
        private const string EXCEL_FIRST_COLUMN_KEY = "FirstColumn";
        /// <summary>
        /// 
        /// </summary>
        private const string EXCEL_LATEST_COLUMN_KEY = "LatestColumn";
        #endregion
    }
}
