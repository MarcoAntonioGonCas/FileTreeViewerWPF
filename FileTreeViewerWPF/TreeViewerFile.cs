using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeViewerWPF
{
    public static class TreeViewerFile
    {

        public static string DirectoriosTreeEvil(string ruta, params string[] archivosCarpetasIgnorar)
        {
            DirectoryInfo raiz = new DirectoryInfo(ruta);
            return DirectoriosTreeEvil(raiz, "", true,archivosCarpetasIgnorar);
        }
        private static string DirectoriosTreeEvil(DirectoryInfo raiz, string prefijo, bool esRaiz = false, params string[] igorarRegex)
        {

            StringBuilder sb = new StringBuilder();
            // Si es la raiz solo mostramos el nombre
            if (esRaiz)
            {

                sb.AppendLine($"{raiz.Name}");
            }
            // Si no es la raiz mostramos el nombre con el prefijo
            else
            {
                
                sb.AppendLine($"{prefijo}---{raiz.Name}");

            }


            // Recorremos los directorios
            string[] directorios = Directory.GetDirectories(raiz.FullName);
            string[] directoriosFiltrados = directorios.Where(
                // Solo filtramos los directorios que no coincidan con las expresiones regulares
                directorio => !igorarRegex.Any(
                    expresion => System.Text.RegularExpressions.Regex.IsMatch(new DirectoryInfo(directorio).Name, expresion)
                )
                
            ).ToArray();

            // Recorremos los directorios filtrados
            foreach (var rutaDir in directoriosFiltrados)
            {
                
                DirectoryInfo info = new DirectoryInfo(rutaDir);


                sb.Append( DirectoriosTreeEvil(info, prefijo + "  │", false, igorarRegex));
            }
            

            // Obtenemos los archivos de la carpeta
            string[] archivos = Directory.GetFiles(raiz.FullName);
            // Filtramos solo los que no cumplan con los filtros
            string[] archivosFiltrados = archivos.Where(
                //Solo filtramos los archivos que no coincidan con las expresiones regulares
                archivo => !igorarRegex.Any(
                                expresion => System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileName(archivo), expresion)
                )
            ).ToArray();


            // Recorremos los archivos filtrados
            for (int i = 0; i < archivosFiltrados.Length; i++)
            {
                // Obtenemos el directorio actual
                string rutaArchivo = archivosFiltrados[i];
                FileInfo info = new FileInfo(rutaArchivo);
                string nuevoPrefijo = prefijo;

                // Creamos el nuevo prefijo para el archivo
                // Si este es el ultimo archivo de la lista
                // cambiamos el simbolo de la rama
                if (i == archivosFiltrados.Length - 1)
                {
                    nuevoPrefijo += "  └---";
                }
                // Si no es el ultimo archivo de la lista
                // mantenemos el simbolo de la rama
                else
                {
                    nuevoPrefijo += "  │---";
                }

                sb.AppendLine($"{nuevoPrefijo} {Path.GetFileName(rutaArchivo)}");
            }

            // Retornamos el resultado
            return sb.ToString();
        }
    }
}
