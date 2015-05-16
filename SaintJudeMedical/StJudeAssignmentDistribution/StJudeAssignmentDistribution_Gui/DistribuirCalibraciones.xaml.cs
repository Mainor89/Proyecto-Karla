using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using StJudeAssignmentDistribution_Implementor;
using StJudeAssignmentDistribution_Library;
using System.Threading;

namespace StJudeAssignmentDistribution_Gui
{
    /// <summary>
    /// Interaction logic for DistribuirCalibraciones.xaml
    /// </summary>
    public partial class DistribuirCalibraciones : Page
    {
        #region Constructor
        public DistribuirCalibraciones()
        {
            InitializeComponent();
            _EquiposPendientes = new List<Equipo>();
            _ResultadoDistribucionTiempo = new Dictionary<int, decimal>();
            _ResultadoDistribucionEquipos = new Dictionary<int, List<Equipo>>();
        }
        #endregion

        #region Methods
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var homePage = new LoginPage();
            NavigationService.Navigate(homePage);
        }

        private void BtnDistribuir_Click(object sender, RoutedEventArgs e)
        {
            _ResultadoDistribucionEquipos.Clear();
            _ResultadoDistribucionTiempo.Clear();
            _EquiposPendientes.Clear();
            if (string.IsNullOrEmpty(TxtCantidadPersonas.Text) || !int.TryParse(TxtCantidadPersonas.Text, out _CantidadPersonas))
            {
                MessageBox.Show("Favor introducir un número entero positivo mayor a 0", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {                
                for (var tecnicoId = 0; tecnicoId < _CantidadPersonas; tecnicoId++)
                {
                    _ResultadoDistribucionTiempo.Add(tecnicoId, 0);
                    _ResultadoDistribucionEquipos.Add(tecnicoId, new List<Equipo>());
                }
                var cantidadHoras = decimal.Parse(ConfigurationSettings.AppSettings.Get(HORAS_DIARIAS_KEY));
                var cantidadHorasSemanales = cantidadHoras * decimal.Parse(ConfigurationSettings.AppSettings.Get(DIAS_DE_LA_SEMANA_KEY));
                var cantidadHorasMensuales = cantidadHorasSemanales * decimal.Parse(ConfigurationSettings.AppSettings.Get(SEMANAS_DEL_MES_KEY));
                var tecnicoActual = 0;
                Equipo equipoPasado = null;
                foreach (var equipo in ExcelFileHandler.Instance.ListaDeEquiposCalibrar)
                {
                    var assigned = false;
                    for (var tecnicoTemp = (tecnicoActual % _CantidadPersonas); tecnicoTemp < _CantidadPersonas; tecnicoTemp++)
                    {
                        if (_ResultadoDistribucionTiempo[tecnicoTemp] + decimal.Parse(equipo.TiempoEstandar) <= cantidadHorasMensuales)
                        {
                            _ResultadoDistribucionTiempo[tecnicoTemp] += decimal.Parse(equipo.TiempoEstandar);
                            _ResultadoDistribucionEquipos[tecnicoTemp].Add(equipo);
                            assigned = true;
                        }
                        if (assigned)
                        {
                            tecnicoActual = tecnicoTemp + 1;
                            break;
                        }
                    }
                    if (!assigned)
                    {
                        tecnicoActual = 0;
                        _EquiposPendientes.Add(equipo);
                    }
                }
                DistribuirEquiposSobrantes();
                MostrarGridDistribucion();
            }
        }

        /// <summary>
        /// Distribuye el equipo entre los tecnicos
        /// </summary>
        /// <param name="pTecnicoIncial"></param>
        /// <param name="pEquipo"></param>
        /// <param name="pCantidadHoras"></param>
        /// <returns></returns>
        private bool AsignarEquipo(int pTecnicoIncial, Equipo pEquipo, decimal pCantidadHoras)
        {
            var asignado = false;
            var tecnicoTemp = pTecnicoIncial;
            while (!asignado)
            {
                if (_ResultadoDistribucionTiempo[tecnicoTemp] + decimal.Parse(pEquipo.TiempoEstandar) <= pCantidadHoras)
                {                    
                    _ResultadoDistribucionTiempo[tecnicoTemp] += decimal.Parse(pEquipo.TiempoEstandar);
                    _ResultadoDistribucionEquipos[tecnicoTemp].Add(pEquipo);
                    asignado = true;
                }
                else
                {
                    tecnicoTemp = SeleccionarTecnicoSiguiente();
                    if (pTecnicoIncial == tecnicoTemp)
                    {
                        break;
                    }
                }
            }
            return asignado;
        }

        /// <summary>
        /// Retorna el Id del técnico que tiene menos horas asignadas
        /// </summary>
        /// <returns></returns>
        private int SeleccionarTecnicoSiguiente()
        {            
            var menorTiempo = _ResultadoDistribucionTiempo.Values.Min();
            List<int> tecnicos = new List<int>();
            foreach (var tecnico in _ResultadoDistribucionTiempo.Keys)
            {
                if (_ResultadoDistribucionTiempo[tecnico] == menorTiempo)
                {
                    tecnicos.Add(tecnico);                    
                }
            }
            if (tecnicos.Count == 1)
            {
                return tecnicos[0];
            }
            else
            {
                var generadorRandom = new Random();
                var indice = tecnicos[(generadorRandom.Next() % tecnicos.Count)];
                return indice;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void MostrarGridDistribucion()
        {
            var datosGrid = new List<Asignacion>();
            foreach (var tecnico in _ResultadoDistribucionEquipos.Keys)
            {
                foreach (var equipo in _ResultadoDistribucionEquipos[tecnico])
                {
                    var row = new Asignacion()
                    {
                        Nombre = "Técnico " + (tecnico + 1),
                        PM = equipo.PM,
                        WorkOrder = equipo.WorkOrder,
                        Tiempo = equipo.TiempoEstandar
                    };
                    datosGrid.Add(row);
                }
            }
            DGDistribucion.ItemsSource = datosGrid;
            ChangeDGHeaders();
            var generarArchivo = new System.Threading.Thread(delegate()
                {
                    ExcelFileHandler.Instance.GenerarArchivoAsignaciones(datosGrid);
                });
            generarArchivo.IsBackground = true;
            generarArchivo.Start();
            MessageBox.Show("Archivo de distribución de calibraciones generado con éxito", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChangeDGHeaders()
        {
            DGDistribucion.Columns[0].Header = "Técnico";
            DGDistribucion.Columns[1].Header = "PM";
            DGDistribucion.Columns[2].Header = "Orden de Trabajo";
            DGDistribucion.Columns[3].Header = "Tiempo Estándar";
        }

        /// <summary>
        /// 
        /// </summary>
        private void DistribuirEquiposSobrantes()
        {
            _ResultadoDistribucionTiempo.OrderBy(x => x.Value);
            var tecnicoId = 0;
            foreach (var equipo in _EquiposPendientes)
            {
                tecnicoId = tecnicoId % _CantidadPersonas;
                _ResultadoDistribucionTiempo[tecnicoId] += decimal.Parse(equipo.TiempoEstandar);
                _ResultadoDistribucionEquipos[tecnicoId].Add(equipo);
                tecnicoId++;
            }            
        }
        #endregion        

        #region Attributes
        private List<Equipo> _EquiposPendientes;
        private Dictionary<int, decimal> _ResultadoDistribucionTiempo;
        private Dictionary<int, List<Equipo>> _ResultadoDistribucionEquipos;
        private int _CantidadPersonas;
        #endregion

        #region Constantes
        public const string HORAS_DIARIAS_KEY = "HorasDiarias";
        public const string DIAS_DE_LA_SEMANA_KEY = "CantidadDiasDeLaSemana";
        public const string SEMANAS_DEL_MES_KEY = "SemanasDelMes";
        #endregion

        #region Structures

        private struct dgStruct
        {
            public string Nombre { get; set; }
            public string PM { get; set; }
            public string WorkOrder { get; set; }
            public string Tiempo { get; set; }
        }
        #endregion
    }
}
