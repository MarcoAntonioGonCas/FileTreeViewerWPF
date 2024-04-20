using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeViewerWPF
{
    [Serializable]
    public class Configuration
    {
        public string Ruta { get; set; }
        public string Ignorar { get; set; }
        public string Resultado { get; set; }



        public bool ConfiguracionVacia 
        {

            get {
                return string.IsNullOrWhiteSpace(Ruta) &&  
                        string.IsNullOrWhiteSpace(Ignorar) && 
                        string.IsNullOrEmpty(Resultado);
            }
        }

        public bool ConfiguracionCorrecta
        {
            get
            {
                return string.IsNullOrWhiteSpace(Ruta);
            }
        }
    }
    public class ConfigHelper
    {

        public const string ruta = "./config.bin";

        public static void BorraCache()
        {
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }
        }
        public static bool GuardarCache(Configuration config)
        {
            return GuardarConfig(config,ruta);
        }
        public static Configuration LeerCache()
        {
           return LeerConfig(ruta);
        }

        public static Configuration LeerConfig(string ruta)
        {
            try
            {
                // Si la ruta no existe
                if (!File.Exists(ruta))
                {
                    return null;
                }
                // Creamos el formateador
                BinaryFormatter formatter = new BinaryFormatter();

                // Leemos el archivo
                using (var stream = new System.IO.FileStream(ruta, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    Configuration config=(Configuration)formatter.Deserialize(stream);

                    return config;
                }

            // Si no se puede leer el archivo
            }catch(Exception e)
            {
                return null;
            }
        }

        public static bool GuardarConfig(Configuration config,string ruta)
        {
            try
            {
                // Creamos el formateador
                BinaryFormatter formatter = new BinaryFormatter();

                // Leemos el archivo
                using (var stream = new System.IO.FileStream(ruta, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                
                    formatter.Serialize(stream,config);

                }
                return true;

            // Si no se puede leer el archivo
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
