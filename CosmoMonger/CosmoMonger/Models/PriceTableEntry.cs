namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;

    public class PriceTableEntry
    {
        private string systemName;
        private Dictionary<string, int> goodPrices = new Dictionary<string, int>();

        public string SystemName
        {
            get { return systemName; }
            set { systemName = value; }
        }
        public Dictionary<string, int> GoodPrices
        {
            get { return goodPrices; }
            set { goodPrices = value; }
        }
    }
}
