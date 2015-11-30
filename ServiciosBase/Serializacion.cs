using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace ServiciosBase
{
    public class Serializacion<TipoGenerico>
    {
        //De JSON a objeto. El objeto NO tiene que tener constructor (o ser vacío)
        public static TipoGenerico Deserializar(String objeto)
        {
            var ser = new JavaScriptSerializer();
            var data = ser.Deserialize<TipoGenerico>(objeto);
            return data;
        }
        //De objeto a JSON
        public static String Serializar(TipoGenerico objeto)
        {
            var ser = new JavaScriptSerializer();
            var data = ser.Serialize(objeto);
            return data;
        }
    }
}
