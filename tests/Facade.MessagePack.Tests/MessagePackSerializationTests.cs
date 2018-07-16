using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Facade.MessagePack.Tests.Models;
using Xunit;

using System.Linq;
using Facade.Serialization.MessagePack;
using Facade.Serialization.Extensions;
using System.Buffers;


namespace Facade.MessagePack.Tests.Tests
{
    public class MessagePackSerializationTests
    {
        [Fact]
        public async Task Serialize_Null()
        {
            DefaultMessagePackSerializer serializer = new DefaultMessagePackSerializer(DefaultFormatterResolver.Instance);

            ReadOnlySequence<byte> bytes = await serializer.Serialize<object>(null);

            Assert.True(bytes.IsEmpty);

            

        }

        [Fact]
        public async Task Serialize_TypeWithNoAttributes()
        {
            DefaultMessagePackSerializer serializer = new DefaultMessagePackSerializer(DefaultFormatterResolver.Instance);

            
            await Assert.ThrowsAsync<FormatterNotRegisteredException>(async ()=> 
            {
                await serializer.Serialize(new Car() { Brand = "Tesla", Doors = 2, HorsePower = 2000, Model = "S" });
                
            });

        }

        [Fact]
        public async Task Serialize_TypeWithAttribute()
        {
            DefaultMessagePackSerializer serializer = new DefaultMessagePackSerializer(DefaultFormatterResolver.Instance);

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
          

            int numberOfFields = typeof(Product).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Count();
                        
            int numberOfSerializedFields = ((byte)0x0f) & bytesSequence.First.ToArray().FirstOrDefault();
            
            Assert.Equal(numberOfFields, numberOfSerializedFields);

            Product result = await serializer.DeSerialize<Product>(bytesSequence);

            Assert.Equal(result, product);

        }

    }
}
