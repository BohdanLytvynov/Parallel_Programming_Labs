using LabWork2.Base;

namespace LabWork2.Canteens
{
    internal class Canteen : FoodSupplyBase, IFoodSupply
    {
        #region Ctor

        public Canteen(int foodQuantity, string name)
        {
            FoodQuantity = foodQuantity;
            Name = name;
        }

        #endregion
    }
}
