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
    /// Interaction logic for IngresarTiempo.xaml
    /// </summary>
    public partial class IngresarTiempo : UserControl
    {
        #region Constructor
        public IngresarTiempo()
        {
            InitializeComponent();
            ListaResultado = new List<resultStruct>();
        }
        #endregion

        #region Properties
        private List<resultStruct> ListaResultado
        {
            get;
            set;
        }
        #endregion

        #region Methods
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            TxtNombreEquipo.Text = string.Empty;
            TxtPmEquipo.Text = string.Empty;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {            
            if (!string.IsNullOrEmpty(TxtTiempo.Text))
            {
                decimal parseValue;
                if (decimal.TryParse(TxtTiempo.Text, out parseValue) && parseValue > 0)
                {
                    var index = ExcelFileHandler.Instance.ListaDeEquiposUnicos.FindIndex(x => x.PM == TxtPmEquipo.Text);
                    ExcelFileHandler.Instance.ListaDeEquiposUnicos[index].TiempoEstandar = TxtTiempo.Text;
                    ActualizarTiemposDeEquipos();
                    TxtNombreEquipo.Text = string.Empty;
                    TxtPmEquipo.Text = string.Empty;
                    TxtTiempo.Text = string.Empty;
                    MessageBox.Show("Tiempo estándar registrado correctamente", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("Favor introducir un valor numérico positivo", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Favor introducir un valor en Tiempo Estándar", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ActualizarTiemposDeEquipos()
        {
            var equipoActualizado = ExcelFileHandler.Instance.ListaDeEquiposUnicos.Find(equipo => equipo.PM.Equals(TxtPmEquipo.Text));
            var listaEquipos = ExcelFileHandler.Instance.ListaDeEquiposCalibrar.FindAll(equipo => equipo.PM.Contains(equipoActualizado.PM));
            foreach (var equipo in listaEquipos)
            {
                equipo.TiempoEstandar = equipoActualizado.TiempoEstandar;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPm"></param>
        /// <param name="pDescripcion"></param>
        public void ActualizarCampos(string pPm, string pDescripcion)
        {
            TxtNombreEquipo.Text = pDescripcion;
            TxtPmEquipo.Text = pPm;
            var index = ExcelFileHandler.Instance.ListaDeEquiposUnicos.FindIndex(x => x.PM == TxtPmEquipo.Text);
            TxtTiempo.Text = ExcelFileHandler.Instance.ListaDeEquiposUnicos[index].TiempoEstandar;            
        }
        #endregion

        #region Structures
        private struct resultStruct
        {
            public string PM { get; set; }            
            public string TiempoEstandard { get; set; }
        }
        #endregion
    }
}
