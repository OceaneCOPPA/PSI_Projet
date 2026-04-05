namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        private Graph graph;
        private List<string> cities;
        private Matrix initialMatrix;

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            // TODO : implémenter
            this.graph = graph;
            this.cities = graph.GetVertexNames();
            this.initialMatrix = BuildInitialMatrix(); 
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
       

        // --- Méthodes utilitaires réalisant des étapes de l'algorithme de Little


        // Réduit la matrice `m` et revoie la valeur totale de la réduction
        // Après appel à cette méthode, la matrice `m` est *modifiée*.
        public static float ReduceMatrix(Matrix m)
        {
            // TODO : implémenter
            float valeurReduction = 0f;
            for(int i =0; i<m.NbRows; i++)
            {
                float minRow = float.PositiveInfinity;
                for(int j=0; j<m.NbColumns;j++)
                {
                    float val = m.GetValue(i, j);
                    if(val < minRow)
                    {
                        minRow = val;  
                    }
                }

                if(minRow > 0 && minRow!= float.PositiveInfinity)
                {
                    for(int j = 0; j<m.NbColumns; j++)
                    {
                        float val = m.GetValue(i, j);
                        if(val!= float.PositiveInfinity)
                        { 
                            m.SetValue(i, j, val-minRow);
                        }
                    }
                    valeurReduction += minRow;
                }
            }
            for (int j = 0; j<m.NbColumns; j++)
            {
                float minColumn = float.PositiveInfinity;
                for (int i =0; i<m.NbRows;i++)
                {
                    float val = m.GetValue(i, j);
                    if(val<minColumn)
                    {
                        minColumn = val;
                    }
                }
                if(minColumn > 0 && minColumn!= float.PositiveInfinity)
                {
                    for(int i=0; i<m.NbColumns;i++)
                    {
                        float val = m.GetValue(i, j);
                        if(val!= float.PositiveInfinity)
                        {
                            m.SetValue(i,j,val-minColumn);
                        }
                    }
                    valeurReduction += minColumn;
                }
            }
            return valeurReduction;
        }

        // Renvoie le regret de valeur maximale dans la matrice de coûts `m` sous la forme d'un tuple `(int i, int j, float value)`
        // où `i`, `j`, et `value` contiennent respectivement la ligne, la colonne et la valeur du regret maximale
        public static (int i, int j, float value) GetMaxRegret(Matrix m)
        {
            // TODO : implémenter
            int ligneRegret = -1;
            int colRegret = -1;
            
            float maxRegret = -1f;
            for(int i = 0; i<m.NbRows;i++)
            {
                for(int j = 0; j<m.NbColumns;j++)
                {
                    if(m.GetValue(i, j) ==0)
                    {
                        float ligneMin = float.PositiveInfinity;
                        for(int col = 0; col<m.NbColumns;col++)
                        {
                            if(col != j)
                            {
                                float val = m.GetValue(i, col);
                                if(val<ligneMin)
                                {
                                    ligneMin = val;
                                }
                            }
                        }
                        float colMin = float.PositiveInfinity;
                        for (int row = 0; row<m.NbRows;row++)
                        {
                            if(row!=i)
                            {
                                float val = m.GetValue(row, j);
                                if(val<colMin)
                                {
                                    colMin = val;
                                }
                            }
                        }
                        float regret = ligneMin + colMin;
                        if(regret > maxRegret)
                        {
                            maxRegret = regret;
                            ligneRegret = i;
                            colRegret = j;
                        }
                    }
                }
            }
            return (ligneRegret, colRegret, maxRegret);

        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {
            var prochain = new Dictionary<string, string >();
            foreach (var seg in includedSegments)
            {
                prochain[seg.source] = seg.destination;
            }

            string actuel = segment.destination;
            int saut = 0;

            while (prochain.ContainsKey(actuel) && saut < nbCities)
            {
                actuel = prochain[actuel];
                saut++;

                if (actuel == segment.source)
                {
                    if (saut == nbCities - 1)
                    {
                        return false;
                    }
                    return true;
                }
            }
            
            return false;   
        }

        private Matrix CopyMatrix(Matrix m)
        {
            Matrix copy = new Matrix(m.NbRows, m.NbColumns) ;
            for (int i = 0; i< m.NbRows; i++)
            {
                for (int j = 0; j < m.NbColumns; j++)
                {
                    copy.SetValue(i,j,m.GetValue(i,j))  ;
                }
            }
            return copy;
        }

        private Matrix BuildInitialMatrix()
        {
            int n = cities.Count;
            Matrix m = new Matrix(n, n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        m.SetValue(i, j, float.PositiveInfinity);
                    }
                    else
                    {
                        try
                        {
                            m.SetValue(i, j, graph.GetEdgeWeight(cities[i], cities[j]));
                        }
                        catch
                        {
                            m.SetValue(i, j, float.PositiveInfinity);
                        }
                    }
                }
            }

            return m;
        }

        private bool CanInclude((string source, string destination) seg, List<(string source, string destination)> included)
        {
            foreach (var s in included)
            {
                if (s.source == seg.source) return false;
                if (s.destination == seg.destination) return false;

            }

            if (IsForbiddenSegment(seg,included, cities.Count))
            {  return false; }

            return true;
        }

        private void ApplyInclusion(Matrix m, int i, int j)
        {
            int n = cities.Count;

            for (int col = 0; col < n; col++)
                m.SetValue(i, col, float.PositiveInfinity);

            for (int row = 0; row < n; row++)
                m.SetValue(row, j, float.PositiveInfinity);

            m.SetValue(j, i, float.PositiveInfinity);
        }

        private float RealCost(List<(string,string)> segments)
        {
            float cost = 0;
            foreach (var s in segments)
            {
                cost += graph.GetEdgeWeight(s.Item1, s.Item2);

            }
            return cost;
        }

        private void Solve (
            Matrix m,
            float bound, 
            List<(string,string)> included,
            ref float bestCost,
            ref List<(string, string)> bestSol)
        {
            if (bound >= bestCost) return;
            if(included.Count==cities.Count)
            {
                float cost = RealCost(included);
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestSol = new List<(string, string)>(included);
                }
                return;
            }

            var regret = GetMaxRegret(m);
            int i = regret.i;
            int j = regret.j;
            if (i == -1) return;

            Matrix m1 = CopyMatrix(m);
            m1.SetValue(i, j, float.PositiveInfinity);
            float b1 = bound + ReduceMatrix(m1);

            Solve(m1, b1, new List<(string, string)>(included), ref bestCost, ref bestSol);

            var seg = (cities[i], cities[j]);
            if(CanInclude(seg, included))
            {
                List<(string, string)> newIncluded = new List<(string, string)>(included);
                newIncluded.Add(seg);

                Matrix m2 = CopyMatrix(m);
                ApplyInclusion(m2, i, j);
                float b2 = bound + ReduceMatrix(m2);
                Solve(m2, b2, newIncluded, ref bestCost, ref bestSol);
            }
        }

        public Tour ComputeOptimalTour()
        {
            Matrix m = CopyMatrix(initialMatrix);
            float bound = ReduceMatrix(m);

            float bestCost = float.PositiveInfinity;
            List<(string source, string destination)> bestSol = new List<(string source, string destination)>();

            Solve(m, bound, new List<(string source, string destination)>(), ref bestCost, ref bestSol);
            Tour t = new Tour();

            foreach (var seg in bestSol)
            {
                t.AddSegment(seg);

            }
            t.Cost = bestCost;
            return t;

        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}
