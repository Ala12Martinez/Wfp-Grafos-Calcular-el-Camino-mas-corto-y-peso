using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectGrafos
{
    public class Grafo
    {
        public List<Nodo> Nodos { get; set; }
        public List<Arista> Aristas { get; set; }

        public Grafo()
        {
            Nodos = new List<Nodo>();
            Aristas = new List<Arista>();
        }

        public void AgregarNodo(Nodo nodo)
        {
            Nodos.Add(nodo);
        }

        public void AgregarArista(Arista arista)
        {
            Aristas.Add(arista);
        }
    }

}
