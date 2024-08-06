using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectGrafos
{
    public class Nodo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public Nodo(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
        

    }

}    
