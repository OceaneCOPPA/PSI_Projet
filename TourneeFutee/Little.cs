namespace TourneeFutee
{
    // Résout le problème de voyageur de commerce défini par le graphe `graph`
    // en utilisant l'algorithme de Little
    public class Little
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 
        Graph graph;

        // Instancie le planificateur en spécifiant le graphe modélisant un problème de voyageur de commerce
        public Little(Graph graph)
        {
            // TODO : implémenter
            this.graph = graph;
        }

        // Trouve la tournée optimale dans le graphe `this.graph`
        // (c'est à dire le cycle hamiltonien de plus faible coût)
        public Tour ComputeOptimalTour()
        {
            // TODO : implémenter
            return new Tour();
        }

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
            return (0, 0, 0.0f);

        }

        /* Renvoie vrai si le segment `segment` est un trajet parasite, c'est-à-dire s'il ferme prématurément la tournée incluant les trajets contenus dans `includedSegments`
         * Une tournée est incomplète si elle visite un nombre de villes inférieur à `nbCities`
         */
        public static bool IsForbiddenSegment((string source, string destination) segment, List<(string source, string destination)> includedSegments, int nbCities)
        {

            // TODO : implémenter
            return false;   
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}
