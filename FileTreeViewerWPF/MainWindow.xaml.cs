using Microsoft.Win32;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.Forms.MessageBox;

namespace FileTreeViewerWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtIgnorar_TextChanged(object sender, TextChangedEventArgs e)
        {

            string texto = txtIgnorar.Text.Trim();

            // Si el texto esta vacio no hacemos nada
            if(string.IsNullOrWhiteSpace(texto))
            {
                txtPalabrasUnidas.Text = string.Empty;
                return;
            }

            // Separamos las palabras por el salto de linea
            string[] palabras = CortaTextoSaltoLinea(texto);

            
            string palabrasUnidas = string.Join(" , ", palabras);

            txtPalabrasUnidas.Text = palabrasUnidas.Replace("\n","");


        }

        private string[] CortaTextoSaltoLinea(string texto)
        {

            if (string.IsNullOrWhiteSpace(texto))
            {
                txtPalabrasUnidas.Text = string.Empty;
                return new string[0];
            }

            // Quitamos los retornos de carro
            texto = texto.Replace("\r", "");

            // Separamos las palabras por el salto de linea
            string[] palabras = texto.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Recorremos las palabras y las limpiamos quitamos los espacios en blanco
            palabras = palabras.Select(p => p.Trim()).ToArray();

            return palabras;
        }

        private void btnCopiar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string textoResultado = txtResultado.Text;


                if (string.IsNullOrWhiteSpace(textoResultado))
                {
                    MessageBox.Show("No hay texto para copiar");
                    return;
                }

                Clipboard.SetText(textoResultado);
            }
            catch(Exception exception)
            {
                MessageBox.Show("Ocurrio un error al copiar el texto");
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtRuta.Text))
                {
                    MessageBox.Show("No hay ruta");
                    return;
                }

                string ruta = txtRuta.Text;



                if (!Directory.Exists(ruta))
                {
                    System.Windows.Forms.MessageBox.Show("El directorio no existe");
                    return;
                }


                string[] archivosCarpetasIgnoradas = CortaTextoSaltoLinea(txtIgnorar.Text);
                txtResultado.Text = TreeViewerFile.DirectoriosTreeEvil(ruta, archivosCarpetasIgnoradas);


            }
            catch(Exception exception)
            {
                System.Windows.Forms.MessageBox.Show("Ocurrio un error inesperado comprueba el directorio o los permisos");
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtRuta.Text = openFileDialog.SelectedPath;
            }
        }

        private void Menu_guardar_Click(object sender, RoutedEventArgs e)
        {
            Configuration config = ObtenerConfigApp();

            if(!config.ConfiguracionCorrecta)
            {
                MessageBox.Show("No hay ruta para guardar la configuracion");
                return;
            }


            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                

            saveFileDialog.Filter = "Archivo de configuracion (*.bin)|*.bin";
            saveFileDialog.Title = "Guardar configuracion";
            saveFileDialog.FileName = "config.bin";

            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            if(ConfigHelper.GuardarConfig(config, saveFileDialog.FileName))
            {
                MessageBox.Show("Guardado correcrtamente");
            }

        }

        private void Menu_cargar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                openFileDialog.Filter = "Archivo de configuracion (*.bin)|*.bin";
                openFileDialog.Title = "Cargar configuracion";


                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Configuration config = ConfigHelper.LeerConfig(openFileDialog.FileName);

                    if (config == null)
                    {
                        MessageBox.Show("No se pudo cargar la configuracion");
                        return;
                    }

                    CargarConfig(config);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ocurrio un error al cargar la configuracion");
            }
            

        }

        private void Menu_Limpiar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

      

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Configuration configuration = ObtenerConfigApp();

            if (configuration.ConfiguracionVacia)
            {
                ConfigHelper.BorraCache();
            }
            else
            {
                ConfigHelper.GuardarCache(configuration);

            }
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Configuration configuration = ConfigHelper.LeerCache();


            if(configuration != null)
            {
                CargarConfig(configuration);
            }
        }


        #region Helpers
        private void Limpiar()
        {
            txtIgnorar.Text = string.Empty;
            txtRuta.Text = string.Empty;
            txtResultado.Text = string.Empty;
        }
        private Configuration ObtenerConfigApp()
        {
            return new Configuration()
            {
                Ruta = txtRuta.Text,
                Ignorar = txtIgnorar.Text,
                Resultado = txtResultado.Text
            };
        }


        private void CargarConfig(Configuration config)
        {
            if (config == null)
            {
                return;
            }

            txtResultado.Text = config.Resultado;
            txtIgnorar.Text = config.Ignorar;
            txtRuta.Text = config.Ruta;
        }
        #endregion
    }
}
