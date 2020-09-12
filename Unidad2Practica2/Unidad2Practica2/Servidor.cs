using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;

namespace Unidad2Practica2
{
    public class Servidor : INotifyPropertyChanged
    {
        HttpListener listener;

        Dispatcher dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Mensaje { get; set; }
        public string Colores { get; set; }

        public Servidor()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:80/Actividad2/");
            listener.Start();
            listener.BeginGetContext(OnRequest, null);
        }

        private void OnRequest(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(OnRequest, null);

            if(context.Request.Url.LocalPath=="/Actividad2/"|| context.Request.Url.LocalPath == "/Actividad2" )
            {
                var buffer = File.ReadAllBytes("index.html");
                context.Response.ContentType = "text/html";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();

                context.Response.StatusCode = 200;
            }

            if (context.Request.Url.LocalPath== "/Actividad2/CambiarValores" && context.Request.HttpMethod == "GET" )
            {
                if (context.Request.QueryString["textoNuevo"] != null && context.Request.QueryString["color"] != null)
                {
                    var texto = context.Request.QueryString["textoNuevo"];
                    var color = context.Request.QueryString["color"];

                    if (color == "Rojo")
                        color = "Red";
                    else if (color == "Verde")
                        color = "Green";
                    else if (color == "Azul")
                        color = "Blue";
                    else color = "Purple";

                    CambiarTexto(texto, color);

                    context.Response.StatusCode = 200;
                    context.Response.Redirect("/Actividad2");

                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.StatusDescription = "No escribiste ningun mensaje";
                }

            }
           
            else
            {
                context.Response.StatusCode = 404;
            }
            context.Response.Close();

        }
        public void OnPropertyChanged(string property)
        {
            if(property!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public void CambiarTexto(string texto, string color)
        {
            this.dispatcher.Invoke(() => Mensaje = texto);
            this.dispatcher.Invoke(() => Colores = color);
            OnPropertyChanged("Mensaje");
            OnPropertyChanged("Colores");
        }
    }
}
