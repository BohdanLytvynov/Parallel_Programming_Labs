namespace LabWork2.Base
{
    internal interface IFoodSupply
    {
        int FoodQuantity { get; set; }
        string Name { get; }

        bool Feed<THungryEntity>(THungryEntity hungryEntity)
            where THungryEntity : IHungryEntity;

        void AddToFeeding(Task task);

        Task ConfigureWait();
    }
}
