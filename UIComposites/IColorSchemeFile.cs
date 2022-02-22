using System.Text.Json;
using System.Text.Json.Serialization;

namespace UIComposites
{
    public class ColorBindingConverter<M, I> : JsonConverter<I> where M : class, I
    {
        public override I? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<M>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, I value, JsonSerializerOptions options) { }
    }
    public class ListConverter<M> : JsonConverter<IList<M>>
    {
        public override IList<M> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<List<M>>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, IList<M> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class IListInterfaceConverterFactory : JsonConverterFactory
    {
        public IListInterfaceConverterFactory(Type interfaceType)
        {
            this.InterfaceType = interfaceType;
        }

        public Type InterfaceType { get; }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.Equals(typeof(IList<>).MakeGenericType(this.InterfaceType))
             && typeToConvert.GenericTypeArguments[0].Equals(this.InterfaceType))
            {
                return true;
            }

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(ListConverter<>).MakeGenericType(this.InterfaceType));
        }
    }
    public class InterfaceConverterFactory : JsonConverterFactory
    {
        public InterfaceConverterFactory(Type concrete, Type interfaceType)
        {
            this.ConcreteType = concrete;
            this.InterfaceType = interfaceType;
        }

        public Type ConcreteType { get; }
        public Type InterfaceType { get; }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == this.InterfaceType;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(ColorBindingConverter<,>).MakeGenericType(this.ConcreteType, this.InterfaceType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
    public interface IColorSchemeFile
    {
        [JsonConverter(typeof(ColorBindingConverter<ColorBinding<Control>, IColorBinding>))]
        public IColorBinding Primary { get; set; }
        [JsonConverter(typeof(ColorBindingConverter<ColorBinding<Control>, IColorBinding>))]
        public IColorBinding Secondary { get; set; }
        public List<IColorBinding> Components { get; set; }
    }
}
