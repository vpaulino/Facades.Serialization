using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facade.Bson.Tests.Models
{
    public class Car
    {
        public string Brand { get; set; }

        public string Model { get; set; }

        public int Doors { get; set; }
        
        public int HorsePower { get; set; }
    }
}
