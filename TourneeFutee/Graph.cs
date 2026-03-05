using System.Xml.Linq;

namespace TourneeFutee
{
    public class Graph
    {
        private bool directed;// graphe orienté ou non ?
        private float noEdgeValue; // absence d'un arc 

        private Matrix matrix; //matrice d'adj qui stocke les poids des arcs entre sommets

        private Dictionary<string, int> vertexIndex = new Dictionary<string, int>(); // associe le nom d'un sommet a son index dans la matrice
        List<string> vertexNames = new List<string>(); // liste qui stocke le nom des sommets
        List<float> vertexValues = new List<float>(); // liste qui stocke la valeur associée a chaque sommet

        // TODO : ajouter tous les attributs que vous jugerez pertinents 


        // --- Construction du graphe ---

        // Contruit un graphe (`directed`=true => orienté)
        // La valeur `noEdgeValue` est le poids modélisant l'absence d'un arc (0 par défaut)
        public Graph(bool directed, float noEdgeValue = 0)
        {
            this.directed = directed;
            this.noEdgeValue = noEdgeValue;
            matrix = new Matrix(0, 0, noEdgeValue); // création de matrice vide
            vertexIndex = new Dictionary<string, int>(); // dico vide pour associer nom à index
            vertexNames = new List<string>(); // Liste vide pour stocker noms sommets
            vertexValues = new List<float>(); // liste vide pour stock des valeurs des sommets
        }


        // --- Propriétés ---

        // Propriété : ordre du graphe
        // Lecture seule
        public int Order
        {
            get { return vertexNames.Count; } // nb de sommmets = taille de la liste
        }

        // Propriété : graphe orienté ou non
        // Lecture seule
        public bool Directed
        {
            get { return directed; } // renvoie si le graphe est orienté ou no 
            // TODO : implémenter
                    // pas de set
        }


        // --- Gestion des sommets ---

        // Ajoute le sommet de nom `name` et de valeur `value` (0 par défaut) dans le graphe
        // Lève une ArgumentException s'il existe déjà un sommet avec le même nom dans le graphe
        public void AddVertex(string name, float value = 0)
        {
            if (vertexIndex.ContainsKey(name))
            {
                throw new ArgumentException("Erreur : sommet déjà existant");
            }

            int index = vertexNames.Count; //nouvel index = taille actuele de la liste
            vertexIndex.Add(name, index); // ajout du couple nom index 
            vertexNames.Add(name);
            vertexValues.Add(value); // ajout de la valeur associé a ce sommet
            matrix.AddRow(matrix.NbRows); //ajout dune nouvelel ligne à la fin de la matrice
            matrix.AddColumn(matrix.NbColumns); // ajout nvl colonne à la fin de la matrice 
            // TODO : implémenter
        }


        // Supprime le sommet de nom `name` du graphe (et tous les arcs associés)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void RemoveVertex(string name)
        {
            // TODO : implémenter
            if (!vertexIndex.ContainsKey(name))
            { 
                throw new ArgumentException("Erreur : sommet non trouvé");
            }
            int index = vertexIndex[name]; // on recup index du sommet dans la matrice
            matrix.RemoveColumn(index); //  on suppp colonne
            matrix.RemoveRow(index);// on supp ligne correspondante au sommet dans la matrice
            vertexIndex.Remove(name); //on supp le sommet du dico 
            vertexNames.RemoveAt(index); // supp le nom du sommet dans la liste
            vertexValues.RemoveAt(index); // supp valeur associée au sommet

            for (int i = index; i < vertexNames.Count; i++)
            {
                vertexIndex[vertexNames[i]] = i; // mise a jour des index 
            }
        }

        // Renvoie la valeur du sommet de nom `name`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public float GetVertexValue(string name)
        {
            // TODO : implémenter

            if(!vertexIndex.ContainsKey(name))
            {
                throw new ArgumentException("Erreur : sommet non trouvé");
            }

            int index = vertexIndex[name]; // recup index sommer
            return vertexValues[index]; // on renvoie la valeur associée a ce sommet
            
        }

        // Affecte la valeur du sommet de nom `name` à `value`
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public void SetVertexValue(string name, float value)
        {
            // TODO : implémenter
            if (!vertexIndex.ContainsKey(name))
            {
                throw new ArgumentException("Erreur : sommet non trouvé");
            }
            int index = vertexIndex[name]; // o, recup index du sommet dans les listes
            vertexValues[index] = value; // on remplace ancienne valeur du sommet par la nouvelle

        }


        // Renvoie la liste des noms des voisins du sommet de nom `vertexName`
        // (si ce sommet n'a pas de voisins, la liste sera vide)
        // Lève une ArgumentException si le sommet n'a pas été trouvé dans le graphe
        public List<string> GetNeighbors(string vertexName)
        {
            List<string> neighborNames = new List<string>();
            if (!vertexIndex.ContainsKey(vertexName))
            {
                throw new ArgumentException("Erreur : sommet non trouvé");
            }
            int index = vertexIndex[vertexName];

            for (int j = 0; j<matrix.NbColumns;j++)
            {
                float value = matrix.GetValue(index, j); // recup de la valeur dans la matrice
                
                if (value != noEdgeValue)
                {
                    neighborNames.Add(vertexNames[j]);
                }
            }

            // TODO : implémenter

            return neighborNames;
        }

        // --- Gestion des arcs ---

        /* Ajoute un arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`, avec le poids `weight` (1 par défaut)
         * Si le graphe n'est pas orienté, ajoute aussi l'arc inverse, avec le même poids
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - il existe déjà un arc avec ces extrémités
         */
        public void AddEdge(string sourceName, string destinationName, float weight = 1)
        {
            // TODO : implémenter
            if (!vertexIndex.ContainsKey(sourceName))
            {
                throw new ArgumentException("Erreur : sommet source non trouvé");
            }

            if (!vertexIndex.ContainsKey(destinationName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            int sourceIndex = vertexIndex[sourceName];
            int destinationIndex = vertexIndex[destinationName];

            if (matrix.GetValue(sourceIndex, destinationIndex) != noEdgeValue)
            {
                throw new ArgumentException("Erreur : arc déjà existant");
            }

            matrix.SetValue(sourceIndex, destinationIndex, weight);

            if(!directed)
            {
                matrix.SetValue(destinationIndex, sourceIndex, weight);
            }


        }

        /* Supprime l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` du graphe
         * Si le graphe n'est pas orienté, supprime aussi l'arc inverse
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public void RemoveEdge(string sourceName, string destinationName)
        {
            // TODO : implémenter
            if (!vertexIndex.ContainsKey(sourceName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            if (!vertexIndex.ContainsKey(destinationName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            int sourceIndex = vertexIndex[sourceName];
            int destinationIndex = vertexIndex[destinationName];

            if (matrix.GetValue(sourceIndex, destinationIndex)==noEdgeValue)
            {
                throw new ArgumentException("Erreur : arc inexistant");
            }

            matrix.SetValue(sourceIndex, destinationIndex, noEdgeValue);

            if (!directed)
            {
                matrix.SetValue(destinationIndex, sourceIndex, noEdgeValue);
            }
        }

        /* Renvoie le poids de l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName`
         * Si le graphe n'est pas orienté, GetEdgeWeight(A, B) = GetEdgeWeight(B, A) 
         * Lève une ArgumentException dans les cas suivants :
         * - un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         * - l'arc n'existe pas
         */
        public float GetEdgeWeight(string sourceName, string destinationName)
        {
            // TODO : implémenter
            if (!vertexIndex.ContainsKey(sourceName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            if (!vertexIndex.ContainsKey(destinationName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            int sourceIndex = vertexIndex[sourceName];
            int destinationIndex = vertexIndex[destinationName];

            float value = matrix.GetValue(sourceIndex, destinationIndex);

            if (value == noEdgeValue)
            {
                throw new ArgumentException("Erreur : arc inexistant");
            }
            return value;
        }

        /* Affecte le poids l'arc allant du sommet nommé `sourceName` au sommet nommé `destinationName` à `weight` 
         * Si le graphe n'est pas orienté, affecte le même poids à l'arc inverse
         * Lève une ArgumentException si un des sommets n'a pas été trouvé dans le graphe (source et/ou destination)
         */
        public void SetEdgeWeight(string sourceName, string destinationName, float weight)
        {
            // TODO : implémenter
            if (!vertexIndex.ContainsKey(sourceName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            if (!vertexIndex.ContainsKey(destinationName))
            {
                throw new ArgumentException("Erreur : sommet destination non trouvé");
            }

            int sourceIndex = vertexIndex[sourceName];
            int destinationIndex = vertexIndex[destinationName];

            matrix.SetValue(sourceIndex, destinationIndex, weight); 

            if (!directed)
            {
                matrix.SetValue(destinationIndex, sourceIndex, weight);
            }



        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}
