using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TourneeFutee
{
    /// <summary>
    /// Service de persistance permettant de sauvegarder et charger
    /// des graphes et des tournées dans une base de données MySQL.
    /// </summary>
    public class ServicePersistance
    {
        // ─────────────────────────────────────────────────────────────────────
        // Attributs privés
        // ─────────────────────────────────────────────────────────────────────

        private readonly string _connectionString;

        // TODO : si vous avez besoin de maintenir une connexion ouverte,
        //        ajoutez un attribut MySqlConnection ici.

        // ─────────────────────────────────────────────────────────────────────
        // Constructeur
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Instancie un service de persistance et se connecte automatiquement
        /// à la base de données <paramref name="dbname"/> sur le serveur
        /// à l'adresse IP <paramref name="serverIp"/>.
        /// Les identifiants sont définis par <paramref name="user"/> (utilisateur)
        /// et <paramref name="pwd"/> (mot de passe).
        /// </summary>
        /// <param name="serverIp">Adresse IP du serveur MySQL.</param>
        /// <param name="dbname">Nom de la base de données.</param>
        /// <param name="user">Nom d'utilisateur.</param>
        /// <param name="pwd">Mot de passe.</param>
        /// <exception cref="Exception">Levée si la connexion échoue.</exception>
        public ServicePersistance(string serverIp, string dbname, string user, string pwd)
        {
          // TODO : initialiser et ouvrir la connexion à la base de données
        // Exemple :
            _connectionString = $"server={serverIp};database={dbname};uid={user};pwd={pwd};";

            // TODO : tester la connexion dès la construction
            //        (ouvrir puis fermer une connexion pour valider les paramètres)
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la connexion à la base de données : " + ex.Message, ex);
            }

        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes publiques
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sauvegarde le graphe <paramref name="g"/> en base de données
        /// (sommets et arcs inclus) et renvoie son identifiant.
        /// </summary>
        /// <param name="g">Le graphe à sauvegarder.</param>
        /// <returns>Identifiant du graphe en base de données (AUTO_INCREMENT).</returns>
        public uint SaveGraph(Graph g)
        {
            // TODO : implémenter la sauvegarde du graphe
            //
            // Ordre recommandé :
            //   1. INSERT dans la table Graphe -> récupérer l'id avec LAST_INSERT_ID()
            //   2. Pour chaque sommet de g : INSERT dans Sommet (valeur + graphe_id)
            //      -> conserver la correspondance sommet C# <-> id BdD
            //   3. Pour chaque arc de la matrice d'adjacence (poids != +inf) :
            //      INSERT dans Arc (sommet_source_id, sommet_dest_id, poids, graphe_id)
            //
            // Exemple pour récupérer l'id généré :
            //   uint id = Convert.ToUInt32(cmd.ExecuteScalar());
            using (var conn = OpenConnection())
            {
                string insertGraphQuery = @"
                INSERT INTO Graphe (nom, nb_sommets, est_oriente)
                VALUES (@nom, @nbSommets,@estOriente);";
                uint graphId;
                using (var cmd = new MySqlCommand(insertGraphQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@nom", "Graphe");
                    cmd.Parameters.AddWithValue("@nbSommets", g.Order);
                    cmd.Parameters.AddWithValue("@estOriente", g.Directed);
                    cmd.ExecuteNonQuery();
                    graphId = Convert.ToUInt32(cmd.LastInsertedId);
                }

                Dictionary<string, uint> vertexDbIds = new Dictionary<string, uint>();
                List<string> vertexNames = g.GetVertexNames();

                string insertVertexQuery = @"
                    INSERT INTO Sommet (graphe_id, nom, valeur, indice)
                    VALUES (@grapheId, @nom,@valeur,@indice);";

                for (int i = 0; i < vertexNames.Count; i++)
                {
                    string vertexName = vertexNames[i];
                    float vertexValue = g.GetVertexValue(vertexName);
                    using (var cmd = new MySqlCommand(insertVertexQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@grapheId", graphId);
                        cmd.Parameters.AddWithValue("@nom", vertexName);
                        cmd.Parameters.AddWithValue("@valeur", vertexValue);
                        cmd.Parameters.AddWithValue("@indice", i);
                        cmd.ExecuteNonQuery();
                        uint vertexId = Convert.ToUInt32(cmd.LastInsertedId);
                        vertexDbIds[vertexName] = vertexId;
                    }

                }

                string insertEdgeQuery = @"
                INSERT INTO Arc (graphe_id, sommet_source,sommet_dest,poids)
                VALUES (@grapheId, @sourceId, @destId,@poids);";

                for (int i = 0; i < vertexNames.Count; i++)
                {
                    int jStart = g.Directed ? 0 : i + 1;
                    for (int j = jStart; j < vertexNames.Count; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }
                        float value = g.GetMatrixValue(i, j);
                        if (value != g.GetNoEdgeValue())
                        {
                            string sourceName = vertexNames[i];
                            string destName = vertexNames[j];
                            using (var cmd = new MySqlCommand(insertEdgeQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@grapheId", graphId);
                                cmd.Parameters.AddWithValue("@sourceId", vertexDbIds[sourceName]);
                                cmd.Parameters.AddWithValue("@destId", vertexDbIds[destName]);
                                cmd.Parameters.AddWithValue("@poids", value);
                                cmd.ExecuteNonQuery();

                            }
                        }
                    }
                }
                return graphId;
            }
        }

        /// <summary>
        /// Charge depuis la base de données le graphe identifié par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Graph"/>.
        /// </summary>
        /// <param name="id">Identifiant du graphe à charger.</param>
        /// <returns>Instance de <see cref="Graph"/> reconstituée.</returns>
        public Graph LoadGraph(uint id)
        {
            // TODO : implémenter le chargement du graphe
            //
            // Ordre recommandé :
            //   1. SELECT dans Graphe WHERE id = @id -> récupérer IsOriented, etc.
            //   2. SELECT dans Sommet WHERE graphe_id = @id -> reconstruire les sommets
            //      (respecter l'ordre d'insertion pour que les indices de la matrice
            //       correspondent à ceux sauvegardés)
            //   3. SELECT dans Arc WHERE graphe_id = @id -> reconstruire la matrice
            //      d'adjacence en utilisant les correspondances sommet_id <-> indice
            using (var conn = OpenConnection())
            {
                bool isDirected;

                string selectGraphQuery = @"
                SELECT est_oriente
                FROM Graphe 
                WHERE id = @id;";

                using (var cmd = new MySqlCommand(selectGraphQuery,conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new ArgumentException("Aucun graphe trouvé avec cet identifiant");

                        }
                        isDirected = reader.GetBoolean("est_oriente");
                    }
                }

                Graph g = new Graph(isDirected);

                Dictionary<uint, string> dbIdToVertexName = new Dictionary<uint, string>();
                string selectVerticesQuery = @"
                SELECT id, nom, valeur, indice
                FROM Sommet
                WHERE graphe_id = @id
                ORDER BY indice;";

                using (var cmd = new MySqlCommand(selectVerticesQuery,conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint vertexId = Convert.ToUInt32(reader["id"]);
                            string vertexName = reader.GetString("nom");
                            float vertexValue = reader.IsDBNull(reader.GetOrdinal("valeur"))
                                ? 0
                                : reader.GetFloat("valeur");
                            g.AddVertex(vertexName, vertexValue);
                            dbIdToVertexName[vertexId]= vertexName;
                        }
                    }
                }

                string selectEdgesQuery = @"
                SELECT sommet_source, sommet_dest, poids
                FROM Arc
                WHERE graphe_id = @id ;";

                using (var cmd = new MySqlCommand(selectEdgesQuery,conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint sourceId = Convert.ToUInt32(reader["sommet_source"]);
                            uint destId = Convert.ToUInt32(reader["sommet_dest"]);
                            float weight = reader.GetFloat("poids");
                            string sourceName = dbIdToVertexName[sourceId];
                            string destName = dbIdToVertexName[destId];
                            g.AddEdge(sourceName, destName, weight);
                        }
                    }
                } return g;
            }
           
        }

        /// <summary>
        /// Sauvegarde la tournée <paramref name="t"/> (effectuée dans le graphe
        /// identifié par <paramref name="graphId"/>) en base de données
        /// et renvoie son identifiant.
        /// </summary>
        /// <param name="graphId">Identifiant BdD du graphe dans lequel la tournée a été calculée.</param>
        /// <param name="t">La tournée à sauvegarder.</param>
        /// <returns>Identifiant de la tournée en base de données (AUTO_INCREMENT).</returns>
        public uint SaveTour(uint graphId, Tour t)
        {
            // TODO : implémenter la sauvegarde de la tournée
            //
            // Ordre recommandé :
            //   1. INSERT dans Tournee (cout_total, graphe_id) -> récupérer l'id
            //   2. Pour chaque sommet de la séquence (avec son numéro d'ordre) :
            //      INSERT dans EtapeTournee (tournee_id, numero_ordre, sommet_id)
            //
            // Attention : conserver l'ordre des étapes est essentiel pour
            //             pouvoir reconstruire la tournée fidèlement au chargement.

            throw new NotImplementedException("SaveTour non implémenté.");
        }

        /// <summary>
        /// Charge depuis la base de données la tournée identifiée par <paramref name="id"/>
        /// et renvoie une instance de la classe <see cref="Tour"/>.
        /// </summary>
        /// <param name="id">Identifiant de la tournée à charger.</param>
        /// <returns>Instance de <see cref="Tour"/> reconstituée.</returns>
        public Tour LoadTour(uint id)
        {
            // TODO : implémenter le chargement de la tournée
            //
            // Ordre recommandé :
            //   1. SELECT dans Tournee WHERE id = @id -> récupérer cout_total et graphe_id
            //   2. SELECT dans EtapeTournee JOIN Sommet WHERE tournee_id = @id
            //      ORDER BY numero_ordre -> reconstruire la séquence ordonnée de sommets
            //   3. Construire et retourner l'instance Tour

            throw new NotImplementedException("LoadTour non implémenté.");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Méthodes utilitaires privées (à compléter selon vos besoins)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crée et retourne une nouvelle connexion MySQL ouverte.
        /// Encadrez toujours l'appel dans un bloc using pour garantir la fermeture.
        /// </summary>
        private MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}
