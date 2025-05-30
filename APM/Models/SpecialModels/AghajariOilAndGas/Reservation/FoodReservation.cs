using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.SpecialModels.AghajariOilAndGas.Reservation
{
    public class FoodReservation
    {
        public int Food { get; set; }
        public int Meal { get; set; }
        public int Restaurant { get; set; }

        public FoodReservation()
        {
            Food = 0;
            Meal = 0;
            Restaurant = 0;
        }

    }
}