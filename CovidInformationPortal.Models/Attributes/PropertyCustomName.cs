using System;

namespace CovidInformationPortal.Models.Attributes
{
    public class PropertyCustomName : Attribute
    {
        public string Name;

        public PropertyCustomName(string customerName)
        {
            this.Name = customerName;
        }
    }
}
