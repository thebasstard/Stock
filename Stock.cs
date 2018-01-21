using System;
using System.Collections.Generic;

namespace Stock
{
    public enum TypesMouvements { Entrée, Sortie, RAZ }

    class Stock
    {
        private SortedList<DateTime, Mouvement> _mouvements;

        public decimal SeuilAlerte { get; set; }
        public event EventHandler<DateAndDecimalEventArgs> AlerteStockBas;
        public Stock()
        {
            _mouvements = new SortedList<DateTime, Mouvement>();
        }

        /// <summary>
        /// Ajoute la quantité spécifiée au stock, à la date spécifiée
        /// Lève une System.ArgumentException s'il y a déjà un mouvement de stock à la même date
        /// </summary>
        /// <param name="date"></param>
        /// <param name="quantité"></param>
        public void Ajouter(DateTime date, decimal quantité)
        {
            AjouterMvt(TypesMouvements.Entrée, date, quantité);
        }

        /// <summary>
        /// Retire la quantité spécifiée du stock à la date spécifiée
        /// Lève une System.ArgumentException s'il y a déjà un mouvement de stock à la même date
        /// Lève une InvalidOperationException si la quantité en stock est insuffisante
        /// </summary>
        /// <param name="date"></param>
        /// <param name="quantité"></param>
        public void Retirer(DateTime date, decimal quantité)
        {
            // On vérifie si la quantité en stock est suffisante
            // Si ce n'est pas le cas, on lève une exception
            decimal etatStock = GetEtatStock(date);
            if (quantité <= etatStock)
                AjouterMvt(TypesMouvements.Sortie, date, quantité);
            else
                throw new InvalidOperationException("Quantité en stock insuffisante");

            // Si la quantité en stock devient inférieure au seuil défini,
            // on lève un évènement
            if (etatStock < SeuilAlerte && AlerteStockBas != null)
                AlerteStockBas(this, new DateAndDecimalEventArgs(date, etatStock));
        }

        /// <summary>
        /// Remet le stock à zéro à la date spécifiée
        /// </summary>
        /// <param name="date"></param>
        public void RemettreAZéro(DateTime date)
        {
            AjouterMvt(TypesMouvements.RAZ, date, 0);
        }

        /// <summary>
        /// Obtient l'état du stock à une date donnée
        /// </summary>
        /// <param name="date">date de calcul du stock</param>
        /// <returns>quantité en stock</returns>
        public decimal GetEtatStock(DateTime date)
        {
            decimal qte = 0m;

            foreach (var mvt in _mouvements)
            {
                if (mvt.Value.DateMvt <= date)
                {
                    switch (mvt.Value.Type)
                    {
                        case TypesMouvements.Entrée:
                            qte += mvt.Value.Quantité;
                            break;
                        case TypesMouvements.Sortie:
                            qte -= mvt.Value.Quantité;
                            break;
                        default:
                            qte = 0;
                            break;
                    }
                }
                else
                    break;
            }

            return qte;
        }

        private void AjouterMvt(TypesMouvements type, DateTime date, decimal qté)
        {
            var mvt = new Mouvement
            {
                Type = type,
                DateMvt = date,
                Quantité = qté
            };

            _mouvements.Add(date, mvt);
        }

        class Mouvement
        {
            public DateTime DateMvt { get; set; }
            public decimal Quantité { get; set; }
            public TypesMouvements Type { get; set; }
        }
    }

    // Classe décrivant un argument d'évènement contenant une date et un décimal
    public class DateAndDecimalEventArgs : EventArgs
    {
        public DateTime Date { get; set; }
        public decimal DecimalValue { get; set; }
        public DateAndDecimalEventArgs(DateTime date, decimal value)
        {
            Date = date;
            DecimalValue = value;
        }
    }
}
