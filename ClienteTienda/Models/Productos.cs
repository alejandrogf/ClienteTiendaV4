using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteTienda.Models
{
    public class Productos
    {
        public int idProducto { get; set; }
        public string Nombre { get; set; }
        public int Precio { get; set; }
        public int PrecioOferta { get; set; }
        public string Descripcion { get; set; }
    }
}
