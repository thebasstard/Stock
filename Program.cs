using System;
using System.Collections.Generic;
using System.IO;

namespace Stock
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Saisissez le chemin complet du fichier :");
			string chemin = Console.ReadLine();
			Console.WriteLine();

			// Création d'un stock avec seuil d'alerte à 50;
			var stock = new Stock();
			stock.SeuilAlerte = 50m;

			// Abonnement à l'événement
			stock.AlerteStockBas += Stock_AlerteStockBas;

			// Chargement des mouvements de stock depuis le fichier
			Console.WriteLine();
			string ligne = String.Empty;
			int cpt = 0;
			try
			{
				// Lecture du fichier ligne à ligne
				using (StreamReader str = new StreamReader(chemin))
				{
					DateTime jour;
					decimal qté;
					bool ligne1 = true;

					Console.WriteLine("Création des mouvements de stocks");
					while ((ligne = str.ReadLine()) != null)
					{
						if (ligne1)
						{
							ligne1 = false;
							continue; // On écarte la première ligne car elle contient les en-têtes
						}

						jour = DateTime.Parse(ligne.Substring(0, 10));
						qté = Decimal.Parse(ligne.Substring(11, ligne.Length - 11));

						try
						{
							if (qté < 0)
								stock.Retirer(jour, -qté);
							else
								stock.Ajouter(jour, qté);
							cpt++;
						}
						catch (ArgumentException)
						{
							Console.WriteLine("Erreur : il y a déjà un mouvement de stock au {0}", jour.ToShortDateString());
						}
					}
					Console.WriteLine("{0} mouvements de stocks créés", cpt);
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Le fichier spécifié n'existe pas :\r\n{0}", chemin);
			}

			Console.WriteLine();

			// Affichage de l'état du stock au 1er jour de chaque mois
			for (var m = 1; m <= 12; m++)
			{
				var date = new DateTime(2016, m, 1);
				Console.WriteLine("Etat du stock au {0} : {1} kg", date.ToShortDateString(), stock.GetEtatStock(date));
			}

			Console.ReadKey();
		}

		// Gestionnaire de l'évènement AlerteStockBas
		private static void Stock_AlerteStockBas(object sender, DateAndDecimalEventArgs e)
		{
			Console.WriteLine("Attention, au {0}, il ne reste que {1} kg en stock !",
				 e.Date.ToShortDateString(), e.DecimalValue);
		}
	}
}
