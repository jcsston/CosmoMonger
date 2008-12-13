//-----------------------------------------------------------------------
// <copyright file="Ship.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Linq;

    /// <summary>
    /// Extension of the partial LINQ class Ship
    /// </summary>
    public partial class Ship
    {
        /// <summary>
        /// Gets the current free cargo space for this ship.
        /// Calculated by taking the total cargo space and subtracting all upgrades, cargo, etc.
        /// </summary>
        public int CargoSpaceFree
        {
            get
            {
                int cargoSpace = this.CargoSpaceTotal;
                cargoSpace -= this.ShipGoods.Sum(x => x.Quantity);
                cargoSpace -= this.JumpDrive.CargoCost;
                cargoSpace -= this.Shield.CargoCost;
                cargoSpace -= this.Weapon.CargoCost;
                return cargoSpace;
            }
        }
        
        /// <summary>
        /// Gets to total amount of cargo space 
        /// Currently this is just the BaseShip model CargoSpace value. 
        /// But we have this to account for possible future ship upgrades that actually upgrade your cargo space.
        /// </summary>
        public int CargoSpaceTotal
        {
            get
            {
                return this.BaseShip.CargoSpace;
            }
        }

        /// <summary>
        /// Starts the ship traveling to the target system.
        /// If the ship is already in the target system an InvalidOperationException is thrown.
        /// If the ship is not already traveling an InvalidOperationException is thrown.
        /// If the target system is out of range of the ship an ArgumentOutOfRangeException is thrown.
        /// </summary>
        /// <param name="targetSystem">The target system to travel to.</param>
        /// <returns>The number of seconds before the ship arrives at the target system</returns>
        public int Travel(CosmoSystem targetSystem)
        {
            // Check if ship is in the target system
            if (this.CosmoSystem == targetSystem)
            {
                throw new InvalidOperationException("Ship is already in the target system");
            }

            // Check that the system is within range
            CosmoSystem[] inRangeSystems = GetInRangeSystems();
            if (!inRangeSystems.Contains(targetSystem))
            {
                throw new ArgumentOutOfRangeException("Target system is out of JumpDrive range", "targetSystem");
            }

            // Check that the ship is not already traveling
            if (this.TargetSystemId != null || this.TargetSystemArrivalTime != null)
            {
                throw new InvalidOperationException("Ship is already traveling");
            }

            // This formula is use to determine the time to travel
            // 5 / ln(engine power + 0.5) = number of seconds to charge
            int travelTime = (int)(5 / Math.Log(this.JumpDrive.Speed + 0.5));

            // Update the database
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            this.TargetSystemId = targetSystem.SystemId;
            this.TargetSystemArrivalTime = DateTime.Now.AddSeconds(travelTime);
            db.SubmitChanges();

            return travelTime;
        }

        /// <summary>
        /// Gets a list of Systems within traveling range of the Ship.
        /// </summary>
        /// <returns>Array of CosmoSystems within JumpDrive distance</returns>
        public CosmoSystem[] GetInRangeSystems()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            
            // Find all systems within range of the JumpDrive
            var systems = (from s in db.CosmoSystems
                           where s != this.CosmoSystem
                           && Math.Abs(this.CosmoSystem.PositionX - s.PositionX) < this.JumpDrive.Range
                           && Math.Abs(this.CosmoSystem.PositionY - s.PositionY) < this.JumpDrive.Range
                           select s);

            return systems.ToArray();
        }

        /// <summary>
        /// Gets the goods on this ship
        /// </summary>
        /// <returns>Array of ShipGoods</returns>
        public ShipGood[] GetGoods()
        {
            return this.ShipGoods.ToArray();
        }
    }
}
