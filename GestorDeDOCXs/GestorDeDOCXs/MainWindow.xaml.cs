using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GestorDeDOCXs
{
    public partial class MainWindow : Window
    {
        CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog()
        {
            Multiselect = true,
            EnsurePathExists = true,
            EnsureFileExists = false,
            AllowNonFileSystemItems = false,
            InitialDirectory = "c:\\"
        };

        HashSet<Archivo> m_hsArchivosSeleccionados = new HashSet<Archivo>();
        HashSet<string> m_hsRutasDeCadaArchivo = new HashSet<string>();

        string m_strRutaDeDestino = "";
        string m_strNombresDeArchivosParaMostrarEnElMessageBox = "";
        

        List<FileInfo> m_lstElementosDeLaRutaDeDestino = new List<FileInfo>();
        List<string> m_lstElementosRepetidos = new List<string>();

        public HashSet<Archivo> ArchivosSeleccionados 
        { 
            get => m_hsArchivosSeleccionados; 
            set => m_hsArchivosSeleccionados = value; 
        }

        public HashSet<string> RutasDeCadaArchivo 
        { 
            get => m_hsRutasDeCadaArchivo; 
            set => m_hsRutasDeCadaArchivo = value; 
        }

        public string RutaDeDestino 
        { 
            get => m_strRutaDeDestino; 
            set => m_strRutaDeDestino = value; 
        }

        public List<FileInfo> ElementosDeLaRutaDeDestino 
        { 
            get => m_lstElementosDeLaRutaDeDestino; 
            set => m_lstElementosDeLaRutaDeDestino = value; 
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public HashSet<Archivo> ObtenerLaInformacionDeLosArchivos(HashSet<string> hsRutasObtenidas)
        {
            HashSet<Archivo> hsArchivosParaMostrar = new HashSet<Archivo>();

            if (hsRutasObtenidas == null || hsRutasObtenidas.Count == 0) return null;

            foreach (string strRutaDeArchivo in hsRutasObtenidas)
            {
                if (string.IsNullOrEmpty(strRutaDeArchivo) || 
                    string.IsNullOrWhiteSpace(strRutaDeArchivo)) continue;

                DirectoryInfo directoryInfo = new DirectoryInfo(strRutaDeArchivo);

                Archivo archivo = new Archivo()
                {
                    NombreDelArchivo = directoryInfo.Name,
                    RutaDeOrigen = directoryInfo.FullName,
                    FechaDeUltimaModificacion = directoryInfo.LastAccessTime
                };

                if (ArchivosSeleccionados.Contains(archivo))
                {
                    MessageBox.Show("El archivo" + archivo.NombreDelArchivo + "ya fue ingresado",
                        "Gestor de DOCXs", MessageBoxButton.OK, MessageBoxImage.Information);
                    continue;
                }
                else
                    hsArchivosParaMostrar.Add(archivo);
            }

            return hsArchivosParaMostrar;
        }

        private void btnFijarRutaDeDestino_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.IsFolderPicker = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Seleccione el lugar donde quiera dejar sus archivos";

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok &&
                openFileDialog.FileName != null)
                RutaDeDestino = openFileDialog.FileName;
            else
                return;

            txtRutaDeDestino.Text = RutaDeDestino;
        }

        private void btnExaminar_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.IsFolderPicker = false;
            openFileDialog.Multiselect = true;
            openFileDialog.Title = "Seleccione los archivos con los que quiera trabajar";

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok &&
                openFileDialog.FileNames != null)
                RutasDeCadaArchivo = openFileDialog.FileNames.ToHashSet();
            else
                return;

            ArchivosSeleccionados = ObtenerLaInformacionDeLosArchivos(RutasDeCadaArchivo);
            lvFiles.ItemsSource = ArchivosSeleccionados;
            lvFiles.Items.Refresh();
        }

        //Aqui van los eventos de copiado o movimiento

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            ArchivosSeleccionados.Clear();
            RutasDeCadaArchivo.Clear();
            lvFiles.ItemsSource = null;
            txtRutaDeDestino.Text = "Seleccione el lugar donde quiera copiar o mover los archivos";
            lvFiles.Items.Refresh();
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCopiar_Click(object sender, RoutedEventArgs e)
        {
            if (ArchivosSeleccionados.Count == 0 || ArchivosSeleccionados == null)
            {
                MessageBox.Show("No hay elementos para trabajar", "Gestor de DOCXs",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (Archivo aArchivosParaCopiar in ArchivosSeleccionados)
            {
                if (aArchivosParaCopiar == null) continue;

                DirectoryInfo directoryInfo = new DirectoryInfo(RutaDeDestino);

                ElementosDeLaRutaDeDestino = directoryInfo.GetFiles(
                    aArchivosParaCopiar.NombreDelArchivo).ToList();

                if (ElementosDeLaRutaDeDestino.Count != 0)
                {
                    if (m_lstElementosRepetidos.Contains(aArchivosParaCopiar.NombreDelArchivo))
                        continue;
                    else
                        m_lstElementosRepetidos.Add(aArchivosParaCopiar.NombreDelArchivo);

                    continue;
                }

                File.Copy(aArchivosParaCopiar.RutaDeOrigen,
                    RutaDeDestino + "\\" + aArchivosParaCopiar.NombreDelArchivo);
            }

            m_strNombresDeArchivosParaMostrarEnElMessageBox = string.Join(
                Environment.NewLine, m_lstElementosRepetidos);

            if (!string.IsNullOrEmpty(m_strNombresDeArchivosParaMostrarEnElMessageBox) || 
                !string.IsNullOrWhiteSpace(m_strNombresDeArchivosParaMostrarEnElMessageBox))
            {
                MessageBox.Show("Los archivos: " + m_strNombresDeArchivosParaMostrarEnElMessageBox +
                " ya existen en el directorio asignado", "Gestor de DOCXs",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            MessageBox.Show("Proceso terminado exitosamente", "Gestor de DOCXs",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnMover_Click(object sender, RoutedEventArgs e)
        {
            if (ArchivosSeleccionados.Count == 0 || ArchivosSeleccionados == null)
            {
                MessageBox.Show("No hay elementos para trabajar", "Gestor de DOCXs", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (Archivo aArchivosParaMover in ArchivosSeleccionados)
            {
                if (aArchivosParaMover == null) continue;

                DirectoryInfo directoryInfo = new DirectoryInfo(RutaDeDestino);

                ElementosDeLaRutaDeDestino = directoryInfo.GetFiles(
                    aArchivosParaMover.NombreDelArchivo).ToList();

                if (ElementosDeLaRutaDeDestino.Count != 0)
                {
                    if (m_lstElementosRepetidos.Contains(aArchivosParaMover.NombreDelArchivo))
                        continue;
                    else
                        m_lstElementosRepetidos.Add(aArchivosParaMover.NombreDelArchivo);

                    continue;
                }

                File.Move(aArchivosParaMover.RutaDeOrigen,
                RutaDeDestino + "\\" + aArchivosParaMover.NombreDelArchivo);
            }

            m_strNombresDeArchivosParaMostrarEnElMessageBox = string.Join(
                Environment.NewLine, m_lstElementosRepetidos);

            if (!string.IsNullOrEmpty(m_strNombresDeArchivosParaMostrarEnElMessageBox) ||
                !string.IsNullOrWhiteSpace(m_strNombresDeArchivosParaMostrarEnElMessageBox))
            {
                MessageBox.Show("Los archivos: " + m_strNombresDeArchivosParaMostrarEnElMessageBox +
                " ya existen en el directorio asignado", "Gestor de DOCXs",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            MessageBox.Show("Proceso terminado exitosamente", "Gestor de DOCXs", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
