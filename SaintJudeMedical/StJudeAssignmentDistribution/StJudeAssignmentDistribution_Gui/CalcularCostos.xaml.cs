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
    /// Interaction logic for CalcularCostos.xaml
    /// </summary>
    public partial class CalcularCostos : Page
    {
        public CalcularCostos()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            LblCostoTitle.Content = string.Empty;
            LblCostoPM.Content = string.Empty;
            LblCosto.Content = string.Empty;
            var homePage = new LoginPage();
            NavigationService.Navigate(homePage);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            TxtCosto.Text = string.Empty;
            TxtOtrosCostos.Text = string.Empty;
            CmbxPM.SelectedValue = string.Empty;
            LblCostoTitle.Content = string.Empty;
            LblCostoPM.Content = string.Empty;
            LblCosto.Content = string.Empty;
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            var costo = TxtCosto.Text;
            var otrosCostos = TxtOtrosCostos.Text;
            var pmSelected =  string.Empty;
            try
            {
                pmSelected = CmbxPM.SelectedValue.ToString();
            }
            catch(Exception){                
            }
            if (string.IsNullOrEmpty(costo) || string.IsNullOrEmpty(pmSelected))
            {
                if (string.IsNullOrEmpty(costo))
                {
                    MessageBox.Show("Favor introducir un valor en el campo Costo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (string.IsNullOrEmpty(pmSelected))
                {
                    MessageBox.Show("Favor seleccionar un PM", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                decimal valorCosto;
                if (decimal.TryParse(costo, out valorCosto))
                {
                    decimal valorOtrosCostos;
                    decimal.TryParse(otrosCostos, out valorOtrosCostos);                    
                    var equipoSeleccionado = ExcelFileHandler.Instance.ListaDeEquiposUnicos.Find(equipo => equipo.PM.Equals(pmSelected));
                    var costoTotal = (valorCosto * int.Parse(equipoSeleccionado.TiempoEstandar)) + valorOtrosCostos;
                    LblCostoTitle.Content = "El costo de calibración para el equipo";
                    LblCostoPM.Content = "PM: " + equipoSeleccionado.PM;
                    LblCosto.Content = "es de: " + costoTotal.ToString("N") + " Colones";
                }
                else
                {
                    MessageBox.Show("Favor introducir un valor decimal positivo en el campo Costo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> equipos = new List<string>();
            foreach (var equipo in ExcelFileHandler.Instance.ListaDeEquiposUnicos)
            {
                equipos.Add(equipo.PM);
            }
            CmbxPM.ItemsSource = equipos;
        }
    }
}
