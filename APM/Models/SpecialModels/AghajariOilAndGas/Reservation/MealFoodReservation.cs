using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.SpecialModels.AghajariOilAndGas.Reservation
{
    public class MealFoodReservation
    {
        public int MealID { get; set; }
        public string MealTitle { get; set; }
        public int SuperiorMeal { get; set; }

        public MealFoodReservation()
        {
            MealID = 0;
            MealTitle = string.Empty;
            SuperiorMeal = 0;
        }
    }
}