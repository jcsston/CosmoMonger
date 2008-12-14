namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension of the partial LINQ class SystemShip
    /// </summary>
    public partial class SystemShip
    {
        /// <summary>
        /// Gets the total price of the ship.
        /// Calcuated by taking the BaseShip.BasePrice and the SystemShip.PriceMultiplier
        /// </summary>
        public int Price
        {
            get
            {
                return (int)(this.BaseShip.BasePrice * this.PriceMultiplier);
            }
        }

        /// <summary>
        /// Buys this ship.
        /// </summary>
        public void Buy(GameManager manager)
        {
            
            // Check if the player has enough credits to buy the ship
            if (manager.CurrentPlayer.CashCredits < this.Price)
            {
                throw new InvalidOperationException("Not enough credits to buy ship");
            }

            // Check if the new ship has enough cargo space to transfer everything 
            Ship currentShip = manager.CurrentPlayer.Ship;
            int cargoSpaceNeeded = currentShip.CargoSpaceTotal - currentShip.CargoSpaceFree;
            if (cargoSpaceNeeded > this.BaseShip.CargoSpace)
            {
                throw new InvalidOperationException("Not enough cargo space on new ship to transfer over cargo");
            }

            
            // Commit changes to the database
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            db.SubmitChanges();
        }
    }
}
