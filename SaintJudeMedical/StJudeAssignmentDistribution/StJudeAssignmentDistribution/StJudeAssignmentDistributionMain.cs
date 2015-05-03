using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using StJudeAssignmentDistribution_Gui;
using StJudeAssignmentDistribution_Implementor;
using StJudeAssignmentDistribution_Library;

namespace StJudeAssignmentDistribution
{
    public class StJudeAssignmentDistributionMain
    {
        public static void Main(string[] args)
        {
            ExcelFileHandler.Instance.ReadExcelFile();
            ExcelFileHandler.Instance.GenerarListaDeEquiposUnicos();
            var windowsThread = new Thread(delegate() 
                {
                    var navWindow = new NavigationWindow()
                    {
                        Title = "St. Jude Sistema de Distribución de Calibraciones",                        
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        WindowStyle = WindowStyle.ToolWindow,
                        WindowState = WindowState.Normal,
                        ResizeMode = ResizeMode.NoResize,
                        Width = 880,
                        Height = 420,
                        ShowsNavigationUI = false,
                    };

                    var loginPage = new LoginPage();
                    navWindow.Navigate(loginPage);
                    navWindow.ShowDialog();
                });            
            windowsThread.SetApartmentState(ApartmentState.STA);            
            windowsThread.Start();            
        }
    }
}
