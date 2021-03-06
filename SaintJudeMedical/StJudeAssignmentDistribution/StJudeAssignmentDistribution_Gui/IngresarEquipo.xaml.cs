﻿using System;
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
    /// Interaction logic for IngresarEquipo.xaml
    /// </summary>
    public partial class IngresarEquipo : Page
    {
        #region Constructor
        public IngresarEquipo()
        {            
            InitializeComponent();
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var _LoginPage = new LoginPage();
            NavigationService.Navigate(_LoginPage);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChangeDGHeaders()
        {
            GridResultado.Columns[1].Header = "Organización";
            GridResultado.Columns[2].Header = "Descripción";
            GridResultado.Columns[3].Header = "Tiempo Estándar";
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadDGData()
        {
            List<EquipoActualizado> data = new List<EquipoActualizado>();
            foreach (var element in ExcelFileHandler.Instance.ListaDeEquiposUnicos)
            {
                var row = new EquipoActualizado()
                {
                    PM = element.PM,                                       
                    Organizacion = element.EquipmentOrg,
                    Descripcion = element.Description,
                    Tiempo = element.TiempoEstandar,
                };
                data.Add(row);
            }
            GridResultado.ItemsSource = data;
            ChangeDGHeaders();
            var generarArchivo = new System.Threading.Thread(delegate()
            {
                ExcelFileHandler.Instance.GenerarArchivoEquipo(data);
            });
            generarArchivo.IsBackground = true;
            generarArchivo.Start();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDGData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            TxtNombreEquipo.Text = string.Empty;
            TxtPmEquipo.Text = string.Empty;
            LoadDGData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtNombreEquipo.Text) && string.IsNullOrEmpty(TxtPmEquipo.Text))
            {
                MessageBox.Show("Favor introducir un parámetro de búsqueda", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var pm = TxtPmEquipo.Text.Trim();
                var descripcion = TxtNombreEquipo.Text;
                List<EquipoActualizado> dataPm = new List<EquipoActualizado>();
                List<EquipoActualizado> dataDesc = new List<EquipoActualizado>();                
                if (!string.IsNullOrEmpty(pm))
                {
                    foreach (var element in ExcelFileHandler.Instance.ListaDeEquiposUnicos)
                    {
                        if (element.PM.ToUpperInvariant().Contains(pm.ToUpperInvariant()))
                        {
                            var row = new EquipoActualizado()
                            {
                                PM = element.PM,
                                Organizacion = element.EquipmentOrg,
                                Descripcion = element.Description,
                                Tiempo = element.TiempoEstandar,
                            };
                            dataPm.Add(row);
                        }                        
                    }               
                }
                if (!string.IsNullOrEmpty(descripcion))
                {
                    foreach (var element in ExcelFileHandler.Instance.ListaDeEquiposUnicos)
                    {
                        if (element.Description.ToUpperInvariant().Contains(descripcion.ToUpperInvariant()))
                        {
                            var row = new EquipoActualizado()
                            {
                                PM = element.PM,
                                Organizacion = element.EquipmentOrg,
                                Descripcion = element.Description,
                                Tiempo = element.TiempoEstandar,
                            };
                            dataDesc.Add(row);
                        }
                    }
                }
                var resultado = dataPm.Union(dataDesc);
                if ((dataPm.Count + dataDesc.Count) > 0)
                {
                    GridResultado.ItemsSource = null;
                    GridResultado.ItemsSource = resultado;
                    ChangeDGHeaders();
                }
                else
                {
                    MessageBox.Show("No se encontró ningún elemento con esos parámetros de búsqueda.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var elem = (EquipoActualizado)GridResultado.SelectedItem;
            IngTiempo.Visibility = System.Windows.Visibility.Visible;            
            IngTiempo.ActualizarCampos(elem.PM, elem.Descripcion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IngTiempo_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((IngresarTiempo)sender).Visibility == System.Windows.Visibility.Visible)
            {
                TxtNombreEquipo.IsEnabled = false;
                TxtPmEquipo.IsEnabled = false;
                GridResultado.IsEnabled = false;
                BtnBack.IsEnabled = false;
                BtnCancelar.IsEnabled = false;
                BtnBuscar.IsEnabled = false;
            }
            else
            {
                TxtNombreEquipo.IsEnabled = true;
                TxtPmEquipo.IsEnabled = true;
                GridResultado.IsEnabled = true;
                BtnBack.IsEnabled = true;
                BtnCancelar.IsEnabled = true;
                BtnBuscar.IsEnabled = true;
                GridResultado.ItemsSource = null;
                LoadDGData();
            }
        }
        #endregion

        #region Attributes
        #endregion                             
    }
}
