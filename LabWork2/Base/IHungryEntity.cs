namespace LabWork2.Base
{
    internal interface IHungryEntity
    {
        int RequiredFoodQuantity { get; set; }
        int FeedingTime { get; set; }
        IFoodSupply FoodSupply { get; }

        void GoToFoodSupply(IFoodSupply foodSupply);
        void EatFood();
        void GoOutFoodSupply();
    }
}
