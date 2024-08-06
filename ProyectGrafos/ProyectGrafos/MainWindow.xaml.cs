using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32; // Añade esta línea



namespace ProyectGrafos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grafo grafo;

        public MainWindow()
        {
            InitializeComponent();
            grafo = new Grafo();
        }

        private void btnCrearNodos_Click(object sender, RoutedEventArgs e)
        {
            int numeroNodos = int.Parse(txtNumeroNodos.Text);
            grafo.Nodos.Clear();  // Limpiar nodos previos para evitar duplicados
            for (int i = 1; i <= numeroNodos; i++)
            {
                grafo.AgregarNodo(new Nodo(i, $"Nodo {i}"));
            }
            MessageBox.Show("Nodos creados");
        }

        private void btnAgregarAdyacencia_Click(object sender, RoutedEventArgs e)
        {
            string[] datos = txtAdyacencia.Text.Split(',');
            int origenId = int.Parse(datos[0]);
            int destinoId = int.Parse(datos[1]);
            int peso = int.Parse(datos[2]);

            Nodo origen = grafo.Nodos.Find(n => n.Id == origenId);
            Nodo destino = grafo.Nodos.Find(n => n.Id == destinoId);

            if (origen != null && destino != null)
            {
                grafo.AgregarArista(new Arista(origen, destino, peso));
                MessageBox.Show("Adyacencia agregada");
            }
            else
            {
                MessageBox.Show("Nodo origen o destino no encontrado para la adyacencia.");
            }
        }

        private void btnCalcularDijkstra_Click(object sender, RoutedEventArgs e)
        {
            int origenId = int.Parse(txtNodoOrigen.Text);
            int destinoId = int.Parse(txtNodoDestino.Text);

            Nodo origen = grafo.Nodos.Find(n => n.Id == origenId);
            Nodo destino = grafo.Nodos.Find(n => n.Id == destinoId);

            if (origen == null || destino == null)
            {
                MessageBox.Show("Nodo origen o destino no encontrado.");
                return;
            }

            var resultado = Dijkstra(grafo, origen, destino);
            if (resultado.Item2 != int.MaxValue)
            {
                string camino = string.Join(" -> ", resultado.Item1.Select(n => n.Nombre));
                txtResultado.Text = $"Camino más corto: {camino} con peso {resultado.Item2}";
            }
            else
            {
                txtResultado.Text = "No se encontró un camino.";
            }
        }

        private Tuple<List<Nodo>, int> Dijkstra(Grafo grafo, Nodo origen, Nodo destino)
        {
            // Diccionario para almacenar la distancia mínima desde el nodo origen a cada nodo
            var distancias = new Dictionary<Nodo, int>();

            // Diccionario para almacenar el nodo anterior en el camino más corto
            var anteriores = new Dictionary<Nodo, Nodo>();

            // Lista de nodos no visitados
            var nodos = new List<Nodo>(grafo.Nodos);

            // Inicializar las distancias y los nodos anteriores
            foreach (var nodo in nodos)
            {
                distancias[nodo] = int.MaxValue; // Establecer la distancia inicial a cada nodo como infinita
                anteriores[nodo] = null; // No hay nodos anteriores en el camino inicial
            }

            // La distancia desde el nodo origen a sí mismo es 0
            distancias[origen] = 0;

            // Mientras haya nodos no visitados
            while (nodos.Count > 0)
            {
                // Ordenar los nodos por la distancia acumulada desde el nodo origen
                nodos.Sort((x, y) => distancias[x] - distancias[y]);

                // Seleccionar el nodo con la menor distancia acumulada
                Nodo menor = nodos[0];
                nodos.Remove(menor); // Eliminar el nodo de la lista de no visitados

                // Si el nodo menor es el nodo destino, se ha encontrado el camino más corto
                if (menor == destino)
                {
                    var camino = new List<Nodo>();
                    var actual = destino;

                    // Reconstruir el camino más corto desde el nodo destino al nodo origen
                    while (actual != null)
                    {
                        camino.Insert(0, actual); // Insertar el nodo al principio de la lista
                        actual = anteriores[actual]; // Ir al nodo anterior en el camino
                    }

                    return Tuple.Create(camino, distancias[destino]); // Devolver el camino y la distancia total
                }

                // Recorrer las aristas salientes del nodo menor
                foreach (var arista in grafo.Aristas.Where(a => a.Origen == menor))
                {
                    // Calcular la distancia total desde el nodo origen al nodo destino de la arista actual
                    int distancia = distancias[menor] + arista.Peso;

                    // Si se encuentra una distancia menor, actualizar la distancia y el nodo anterior
                    if (distancia < distancias[arista.Destino])
                    {
                        distancias[arista.Destino] = distancia; // Actualizar la distancia menor
                        anteriores[arista.Destino] = menor; // Establecer el nodo menor como el nodo anterior
                    }
                }
            }

            // Si no se encuentra un camino al nodo destino, devolver una lista vacía y una distancia infinita
            return Tuple.Create(new List<Nodo>(), int.MaxValue);
        }


        // Código para guardar el grafo
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (var nodo in grafo.Nodos)
                    {
                        writer.WriteLine($"N,{nodo.Id},{nodo.Nombre}");
                    }

                    foreach (var arista in grafo.Aristas)
                    {
                        writer.WriteLine($"A,{arista.Origen.Id},{arista.Destino.Id},{arista.Peso}");
                    }
                }
                MessageBox.Show("Grafo guardado.");
            }
        }
        //Cargar Archivo Txt
        private void btnCargar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                grafo.Nodos.Clear();
                grafo.Aristas.Clear();
                txtGrafoCargado.Clear();

                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] datos = line.Split(',');

                        if (datos[0] == "N")
                        {
                            int id = int.Parse(datos[1]);
                            string nombre = datos[2];
                            grafo.AgregarNodo(new Nodo(id, nombre));
                            txtGrafoCargado.AppendText($"Nodo: Id = {id}, Nombre = {nombre}\n");
                        }
                        else if (datos[0] == "A")
                        {
                            int origenId = int.Parse(datos[1]);
                            int destinoId = int.Parse(datos[2]);
                            int peso = int.Parse(datos[3]);

                            Nodo origen = grafo.Nodos.Find(n => n.Id == origenId);
                            Nodo destino = grafo.Nodos.Find(n => n.Id == destinoId);

                            if (origen != null && destino != null)
                            {
                                grafo.AgregarArista(new Arista(origen, destino, peso));
                                txtGrafoCargado.AppendText($"Arista: Origen = {origenId}, Destino = {destinoId}, Peso = {peso}\n");
                            }
                        }
                    }
                }
                MessageBox.Show("Grafo cargado.");
            }
        }


    }
}