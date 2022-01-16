using System.Collections.Generic;

namespace CarWarehouse.Models
{
    public class Car
    {
        public string location { get; set; }
        public List<Vehicle> vehicles { get; set; }
    }
}
