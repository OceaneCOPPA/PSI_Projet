namespace TourneeFutee
{
    public class Matrix
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 

        List<List<float>> matrice;
        int nbRows;
        int nbColumns;
        float defaultValue;

        /* Crée une matrice de dimensions `nbRows` x `nbColums`.
         * Toutes les cases de cette matrice sont remplies avec `defaultValue`.
         * Lève une ArgumentOutOfRangeException si une des dimensions est négative
         */
        public Matrix(int nbRows = 0, int nbColumns = 0, float defaultValue = 0)
        {
            // TODO : implémenter

            if(nbRows < 0|| nbColumns<0)
            {
                throw new ArgumentOutOfRangeException("Erreur : nb de colonnes ou de lignes négatif");
            }
            this.nbRows = nbRows;
            this.nbColumns = nbColumns;
            this.defaultValue = defaultValue;
            this.matrice = new List<List<float>>();

            for (int i = 0; i < nbRows; i++)
            {
                List<float> colonne = new List<float>();
                {
                    for (int j = 0; j < nbColumns; j++)
                    {
                        colonne.Add(defaultValue);
                    }
                    matrice.Add(colonne);
                }
            }
        }
        
        public float DefaultValue
        {
            get { return this.defaultValue; } // TODO pas de set
        }
        public int NbRows
        {
            get { return this.nbRows; }  //TODO pas de set
        }
        public int NbColumns
        {
            get { return this.nbColumns; } // TODO pas de set
        }

        /* Insère une ligne à l'indice `i`. Décale les lignes suivantes vers le bas.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `i` = NbRows, insère une ligne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
         */
        public void AddRow(int i)
        {
            if( (i<0) || (i > nbRows) )
            {
                throw new ArgumentOutOfRangeException("l'indice n'est pas dans les bornes");
            }
            List<float> newRow = new List<float>();
            for (int j = 0; j < nbColumns; j++)
            {
                newRow.Add(defaultValue);
            }
            this.nbRows++;
            this.matrice.Insert(i, newRow);
            
            // TODO : implémenter
        }

        /* Insère une colonne à l'indice `j`. Décale les colonnes suivantes vers la droite.
         * Toutes les cases de la nouvelle ligne contiennent DefaultValue.
         * Si `j` = NbColums, insère une colonne en fin de matrice
         * Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
         */
        public void AddColumn(int j)
        {
            // TODO : implémenter
        }

        // Supprime la ligne à l'indice `i`. Décale les lignes suivantes vers le haut.
        // Lève une ArgumentOutOfRangeException si `i` est en dehors des indices valides
        public void RemoveRow(int i)
        {
            // TODO : implémenter
        }

        // Supprime la colonne à l'indice `j`. Décale les colonnes suivantes vers la gauche.
        // Lève une ArgumentOutOfRangeException si `j` est en dehors des indices valides
        public void RemoveColumn(int j)
        {
            // TODO : implémenter
        }

        // Renvoie la valeur à la ligne `i` et colonne `j`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public float GetValue(int i, int j)
        {
            // TODO : implémenter
            return 0.0f;
        }

        // Affecte la valeur à la ligne `i` et colonne `j` à `v`
        // Lève une ArgumentOutOfRangeException si `i` ou `j` est en dehors des indices valides
        public void SetValue(int i, int j, float v)
        {
            // TODO : implémenter
        }

        // Affiche la matrice
        public void Print()
        {
            // TODO : implémenter
        }


        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }


}
