using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;

using Facade.MessagePack.Tests.Models;
using Facade.Serialization.MessagePack;

namespace Facade.MessagePack.Tests.Formatters
{
    public class ProductMessagePackFormatter : TypeMessagePackFormatter<Product>
    {
        public ProductMessagePackFormatter()
        {
            Product product = new Product();
            this.DeserializationHandlers.Add(nameof(product.Id), (bytes, offset, formatterResolver, instance) =>
            {

                int contentsReadSize = 0;

                instance.Id = formatterResolver.GetFormatter<int>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);
                
                return contentsReadSize;

            });
            this.DeserializationHandlers.Add(nameof(product.Labels), (bytes, offset, formatterResolver, instance) => 
            {

                int contentsReadSize = 0;

                instance.Labels = formatterResolver.GetFormatter<List<string>>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });
            this.DeserializationHandlers.Add(nameof(product.Location), (bytes, offset, formatterResolver, instance) => 
            {
                int contentsReadSize = 0;

                instance.Labels = formatterResolver.GetFormatter<List<string>>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });
            this.DeserializationHandlers.Add(nameof(product.Name), (bytes, offset, formatterResolver, instance) => 
            {

                int contentsReadSize = 0;

                instance.Name = formatterResolver.GetFormatter<string>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });
            this.DeserializationHandlers.Add(nameof(product.Price), (bytes, offset, formatterResolver, instance) => 
            {
                int contentsReadSize = 0;
                string price = MessagePackBinary.ReadString(bytes, offset, out contentsReadSize);
                decimal priceResult;
                if (Decimal.TryParse(price, out priceResult))
                {
                    instance.Price = priceResult;
                };

                return contentsReadSize;

            });
            this.DeserializationHandlers.Add(nameof(product.Rating), (bytes, offset, formatterResolver, instance) => 
            {
                int contentsReadSize = 0;

                instance.Rating = formatterResolver.GetFormatter<Rate>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });
            this.DeserializationHandlers.Add(nameof(product.Seller), (bytes, offset, formatterResolver, instance) => 
            {
                int contentsReadSize = 0;

                instance.Seller = formatterResolver.GetFormatter<string>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Description), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Description = formatterResolver.GetFormatter<String>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Details), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Details = formatterResolver.GetFormatter<Dictionary<string, string>>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Currency), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Currency = formatterResolver.GetFormatter<string>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Location), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Location = formatterResolver.GetFormatter<Uri>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });


            this.DeserializationHandlers.Add(nameof(product.Location), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Category = formatterResolver.GetFormatter<Category>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Image), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Image = MessagePackBinary.ReadBytes(bytes, offset, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Updated), (bytes, offset, formatterResolver, instance) =>
            {
                int contentsReadSize = 0;

                instance.Updated = formatterResolver.GetFormatter<DateTime?>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });

            this.DeserializationHandlers.Add(nameof(product.Updated), (bytes, offset, formatterResolver, instance) => 
            {
                int contentsReadSize = 0;

                instance.Updated = formatterResolver.GetFormatter<DateTime?>().Deserialize(bytes, offset, formatterResolver, out contentsReadSize);

                return contentsReadSize;

            });
        }

        

        public override Product Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            int startOffset = offset;
          
            Product p = new Product();

            var initialSize = offset;
            if (bytes[offset] == 0xc0)
            {
                readSize = 1;
                return null;
            }

            int headSize = 0;
            _ = MessagePackBinary.ReadMapHeader(bytes, offset, out headSize);
            offset += headSize;
            // element name 
            var instance = new Product();
            int expectedFieldsDeserialized = 8;

            offset += DeserializeProperties(bytes, offset, formatterResolver, expectedFieldsDeserialized, instance);

            readSize = offset - initialSize;

            return instance;
               
        }

        public override int Serialize(ref byte[] bytes, int offset, Product value, IFormatterResolver formatterResolver)
        {
            int startOffset = offset;
            offset += MessagePackBinary.WriteMapHeader(ref bytes, offset, 14);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Id));
            offset += MessagePackBinary.WriteInt64(ref bytes, offset, value.Id);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Name));
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.Name);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Description));
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.Description);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Seller));
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.Seller);
             
            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Category));
            offset += formatterResolver.GetFormatter<Category>().Serialize(ref bytes, offset, value.Category, formatterResolver);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Rating));
            offset += formatterResolver.GetFormatter<Rate>().Serialize(ref bytes, offset, value.Rating, formatterResolver);
             
            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Price));
            if (!value.Price.HasValue)
                offset += MessagePackBinary.WriteNil(ref bytes, offset);
            else
            { 
                offset += MessagePackBinary.WriteString(ref bytes, offset, value.Price.Value.ToString());
            }
                


            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Currency));
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.Currency);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Created));
            offset += MessagePackBinary.WriteDateTime(ref bytes, offset, value.Created);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Updated));
            if (!value.Updated.HasValue)
                offset += MessagePackBinary.WriteNil(ref bytes, offset);
            else
                offset += MessagePackBinary.WriteDateTime(ref bytes, offset, value.Updated.Value);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Labels));
            offset += formatterResolver.GetFormatter<ICollection<string>>().Serialize(ref bytes, offset, value.Labels, formatterResolver);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Location));
            offset += formatterResolver.GetFormatter<Uri>().Serialize(ref bytes, offset, value.Location, formatterResolver);


            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Image));
            offset += MessagePackBinary.WriteBytes(ref bytes, offset, value.Image);

            offset += MessagePackBinary.WriteString(ref bytes, offset, nameof(value.Details));
            offset += formatterResolver.GetFormatter<Dictionary<string, string>>().Serialize(ref bytes, offset, value.Details, formatterResolver);
            
            return offset - startOffset;
        }
    }
}
