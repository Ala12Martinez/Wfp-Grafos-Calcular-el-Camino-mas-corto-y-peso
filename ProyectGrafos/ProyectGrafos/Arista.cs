using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectGrafos
{
    public class Arista
    {
        public Nodo Origen { get; set; }
        public Nodo Destino { get; set; }
        public int Peso { get; set; }

        public Arista(Nodo origen, Nodo destino, int peso)
        {
            Origen = origen;
            Destino = destino;
            Peso = peso;
        }

    }

}
