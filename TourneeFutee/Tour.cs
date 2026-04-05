namespace TourneeFutee
{
    // Modélise une tournée dans le cadre du problème du voyageur de commerce
    public class Tour
    {
        // TODO : ajouter tous les attributs que vous jugerez pertinents 

        // propriétés

        private List<(string source, string destination)> segments;
        private float cost;

        public Tour()
        {
            segments = new List<(string source, string destination)>();
            cost = 0f;
        }

        // Coût total de la tournée
        public float Cost
        {
            get { return cost; }
            set { cost = value; }
            // TODO : implémenter
        }

        // Nombre de trajets dans la tournée
        public int NbSegments
        {
            get { return segments.Count; }    // TODO : implémenter
        }


        // Renvoie vrai si la tournée contient le trajet `source`->`destination`
        public bool ContainsSegment((string source, string destination) segment)
        {
            return segments.Contains(segment);   // TODO : implémenter 
        }


        // Affiche les informations sur la tournée : coût total et trajets
        public void Print()
        {
            Console.WriteLine($"Coût total : {cost}");
            Console.WriteLine("Trajets :");
            foreach (var seg in segments)
            {
                Console.WriteLine($" {seg.source} - {seg.destination}");
            }

            // TODO : implémenter 
        }

        public void AddSegment((string source, string destination) segment)
        {
            segments.Add(segment);
        }

        // TODO : ajouter toutes les méthodes que vous jugerez pertinentes 

    }
}
