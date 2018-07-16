﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facade.Json.Tests.Models;
using Facade.Serialization.Json;
using Xunit;

namespace Facade.Json.Tests
{
    public class NewtonSoftSerialzationTests
    {
        [Fact]
        public async Task Serialize_Null()
        {
            DefaultJsonSerializer serializer = new DefaultJsonSerializer();

            ReadOnlySequence<byte> bytes = await serializer.Serialize<object>(null);

            Assert.True(bytes.IsEmpty);

        } 


       

        [Fact]
        public async Task Serialize_Deserialize()
        {
            DefaultJsonSerializer serializer = new DefaultJsonSerializer();

            Product product = new Product()
            {
                Id = 1,
                Category = Models.Category.Electronic,
                Created = DateTime.UtcNow,
                Description = @"o beyond the traditional laptop with Surface Laptop. Backed by the best of Microsoft, including Windows3 and Office5. Enjoy a natural typing experience enhanced by our Signature Alcantara® fabric-covered keyboard. Thin, light, and powerful, it fits easily in your bag",
                Labels = new List<string> { "windows", "laptop", "gray" },
                Location = new Uri("http://microsoft.pt"),
                Name = "surface pro",
                Price = Decimal.Parse("959,32"),

                Rating = Models.Rate.Good,
                Details = new Dictionary<string, string>()
                  {
                      { "SO","windows 10" },
                      { "CPU","i5" },
                      { "RAM","4GB" },
                      { "Storage","128GB" },
                      { "BatteryLife","14.5h" },
                  }

            };


            ReadOnlySequence<byte> bytesSequence = await serializer.Serialize(product);

            Assert.True(!bytesSequence.IsEmpty);

 
            Product result = await serializer.DeSerialize<Product>(bytesSequence);

            Assert.Equal(result, product);

        }
    }
}
