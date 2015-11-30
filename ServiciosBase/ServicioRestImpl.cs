using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosBase
{
    public class ServicioRestImpl<TModelo> : IServiciosRest<TModelo>
    {
        private String url;
        private bool auth;
        private String user;
        private String pass;


        public ServicioRestImpl(String url, bool auth = false, String user = null, String pass = null)
        {
            this.url = url;
            this.auth = auth;
            this.user = user;
            this.pass = pass;
        }

        public async Task<TModelo> Add(TModelo model)
        {
            var datos = Serializacion<TModelo>.Serializar(model);
            //No se usa webrequest porque puede/suele fallar en los metodos post/put/delete
            //Se usa el httppclienthandler en su lugar.
            using (var handler = new HttpClientHandler())
            {
                if (auth)
                {
                    handler.Credentials = new NetworkCredential(user, pass);
                }
                //Crea un client pasandole la información del handler(que tiene los datos de autenticacion)
                using (var client = new HttpClient(handler))
                {
                    //stringcontent manipula una cadena de texto plano para que la pueda interpretar
                    //una cabecera http
                    var contenido = new StringContent(datos);
                    //indicas en la cabecera el tipo de contenido que tiene.
                    contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //le indicas q espere(await) para que no se cierre la conexión del using
                    //mientras está haciendo el put
                    var respuesta = await client.PostAsync(new Uri(url), contenido);
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        throw new Exception("Error al grabar");
                    }
                    var objetoSerializado = await respuesta.Content.ReadAsStringAsync();
                    return Serializacion<TModelo>.Deserializar(objetoSerializado);
                }
            }
        }

        //Para que un metodo sea asincrono hay que marcarlo como Task y async
        //Para esperar y poder utilizar lo que devuelve una llamada asincrona
        //se usa await
        public async Task Update(TModelo model)
        {
            var datos = Serializacion<TModelo>.Serializar(model);
            //No se usa webrequest porque puede/suele fallar en los metodos post/put/delete
            //Se usa el httppclienthandler en su lugar.
            using (var handler = new HttpClientHandler())
            {
                if (auth)
                {
                    handler.Credentials = new NetworkCredential(user, pass);
                }
                //Crea un client pasandole la información del handler(que tiene los datos de autenticacion)
                using (var client = new HttpClient(handler))
                {
                    //stringcontent manipula una cadena de texto plano para que la pueda interpretar
                    //una cabecera http
                    var contenido = new StringContent(datos);
                    //indicas en la cabecera el tipo de contenido que tiene.
                    contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //le indicas q espere(await) para que no se cierre la conexión del using
                    //mientras está haciendo el put
                    var respuesta = await client.PutAsync(new Uri(url), contenido);
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        throw new Exception("Error al modificar");
                    }
                }
            }
        }

        public async Task Delete(int id)
        {
            //No se usa webrequest porque puede/suele fallar en los metodos post/put/delete
            //Se usa el httppclienthandler en su lugar.
            using (var handler = new HttpClientHandler())
            {
                if (auth)
                {
                    handler.Credentials = new NetworkCredential(user, pass);
                }
                //Crea un client pasandole la información del handler(que tiene los datos de autenticacion)
                using (var client = new HttpClient(handler))
                {
                    //le indicas q espere(await) para que no se cierre la conexión del using
                    //mientras está haciendo el delete
                    var respuesta = await client.DeleteAsync(new Uri(url + "/" + id));
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        throw new Exception("Error al eliminar");
                    }
                }
            }
        }

        public List<TModelo> Get(String paramUrl = null)
        {
            List<TModelo> lista;
            var urlDest = url;

            if (paramUrl != null)
            {
                urlDest += paramUrl;
            }
            //webrequest siempre es con método GET.
            var request = WebRequest.Create(urlDest);
            if (auth)
            {
                request.Credentials = new NetworkCredential(user, pass);
            }
            request.Method = "GET";
            var response = request.GetResponse();
            //stream es el canal de comunicacion/flujo de datos que se abre entre aplicaciones
            //que dentro lleva ya la respuesta. De base la información viaja como bytes
            using (var stream = response.GetResponseStream())
            {
                //reader permite leer datos del stream. 
                //Streamreader transforma él solo de bytes a texto
                using (var reader = new StreamReader(stream))
                {
                    var serializado = reader.ReadToEnd();
                    //transforma el texto en una lista de objetos para trabajar con ella.
                    lista = Serializacion<List<TModelo>>.Deserializar(serializado);
                }
            }
            return lista;
        }
        //Un diccionario es como una lista, que almacena el objeto y su valor.
        public List<TModelo> Get(Dictionary<string, string> parametros)
        {
            var parametrosurl = "";
            var primero = true;
            //Con Keys se busca por las claves del diccionario directamente
            //Al recorrerlo por las calves, se construye "a mano" una url codificada
            foreach (var key in parametros.Keys)
            {
                if (primero)
                {
                    parametrosurl += "?";
                    primero = false;
                }
                else
                {
                    parametrosurl += "&";
                    parametrosurl += key + "=" + parametros[key];
                }
            }
            return Get(parametrosurl);
        }

        public List<TModelo> Get(int id)
        {
            var parametrosurl = "";
            parametrosurl += "/" + id;
            return Get(parametrosurl);

            //TModelo objeto;
            ////webrequest siempre es con método GET.
            //var request = WebRequest.Create(url + "/" + id);
            //if (auth)
            //{
            //    request.Credentials = new NetworkCredential(user, pass);
            //}
            //request.Method = "GET";
            //var response = request.GetResponse();
            ////stream es el canal de comunicacion/flujo de datos que se abre entre aplicaciones
            ////que dentro lleva ya la respuesta. De base la información viaja como bytes
            //using (var stream = response.GetResponseStream())
            //{
            //    //reader permite leer datos del stream. 
            //    //Streamreader transforma él solo de bytes a texto
            //    using (var reader = new StreamReader(stream))
            //    {
            //        var serializado = reader.ReadToEnd();
            //        //transforma el texto en una lista de objetos para trabajar con ella.
            //        objeto = Serializacion<TModelo>.Deserializar(serializado);
            //    }
            //}
            //return objeto;
        }
    }
}
