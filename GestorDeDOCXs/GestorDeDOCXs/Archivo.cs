using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDeDOCXs
{
    public class Archivo
    {
        private string m_strNombreDelArchivo = "";
        private string m_strRutaDeOrigen = "";
        private DateTime m_dtFechaDeUltimaModificacion = new DateTime();

        public string NombreDelArchivo
        {
            get => m_strNombreDelArchivo;
            set => m_strNombreDelArchivo = value;
        }

        public string RutaDeOrigen
        {
            get => m_strRutaDeOrigen;
            set => m_strRutaDeOrigen = value;
        }

        public DateTime FechaDeUltimaModificacion
        {
            get => m_dtFechaDeUltimaModificacion;
            set => m_dtFechaDeUltimaModificacion = value;
        }
    }
}
