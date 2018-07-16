using System;
using System.Collections.Generic;
using System.Text;


namespace Facade.Json.Tests.Models
{
    public enum Category
    {
        Retail,
        Electronic,
        Realstate,
        Vehicle,

    }

    public enum Rate
    {
        Horrible,
        Bad,
        Good,
        Great,
        Awsome,

    }

    
    public class Product
    {


        public Product()
        {
            this.Labels = new List<string>();
            this.Details = new Dictionary<string, string>();
            this.Created = DateTime.UtcNow;
        }

        public long Id { get; set; }

        public byte[] Image { get; set; }

        public string Seller { get; set; }

        public string Name  { get; set; }

        public Rate Rating { get; set; } 

        public Category Category { get; set; }
                

        public decimal? Price { get; set; }

        public string Currency { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }


        public String Description { get; set; }

        public Uri Location { get; set; }
        
        public ICollection<string> Labels { get; set; }
        
        public Dictionary<string, string> Details { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Product))
                return false;

            return Equals(obj as Product);

        }

        public bool Equals(Product obj)
        {
            return this.Id.Equals(obj.Id) &&
                   this.Name.Equals(obj.Name) &&
                   this.Price.Equals(obj.Price) &&
                   this.Category.Equals(obj.Category) &&
                   this.Created.Equals(obj.Created);
                  
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
