using System;
using System.Collections.Generic;
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
using StJudeAssignmentDistribution_Implementor;
using StJudeAssignmentDistribution_Library;

namespace StJudeAssignmentDistribution_Gui
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        #region Constructor
        public LoginPage()
        {
            InitializeComponent();
            _IngresarEquipoPage = new IngresarEquipo();
            _DistribuirCalibracionesPage = new DistribuirCalibraciones();
            _CalcularCostosPage = new CalcularCostos();            
        }
        #endregion

        #region Methods
        private void BtnIngresarEquipo_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(_IngresarEquipoPage);
        }

        private void BtnDistribuirCalibraciones_Click(object sender, RoutedEventArgs e)
        {
            if (ExcelFileHandler.Instance.ListaDeEquiposUnicos.Exists(equipo => equipo.TiempoEstandar.Equals("0")))
            {
                MessageBox.Show("Existen equipos sin tiempo estándar asignado. Favor asignarles un tiempo.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                NavigationService.Navigate(_DistribuirCalibracionesPage);
            }            
        }

        private void BtnCalcularCostos_Click(object sender, RoutedEventArgs e)
        {            
            NavigationService.Navigate(_CalcularCostosPage);
        }
        #endregion

        #region Attributes
        public IngresarEquipo _IngresarEquipoPage;
        public DistribuirCalibraciones _DistribuirCalibracionesPage;
        public CalcularCostos _CalcularCostosPage;
        #endregion        
    }
}
