using System.Text.Json;
using System.Text.Json.Serialization;

namespace UIComposites.ColorSchemes
{
    public class ControlThemeConverter<M, I> : JsonConverter<I> where M : class, I
    {
        public override I? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize<M>(ref reader, options);

        public override void Write(Utf8JsonWriter writer, I value, JsonSerializerOptions options) { }
    }
    public class ListConverter<M> : JsonConverter<IList<M>>
    {
        public override IList<M> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize<List<M>>(ref reader, options) ?? new List<M>();

        public override void Write(Utf8JsonWriter writer, IList<M> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
    public class IListInterfaceConverterFactory : JsonConverterFactory
    {
        public IListInterfaceConverterFactory(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        public Type InterfaceType { get; }

        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.Equals(typeof(IList<>).MakeGenericType(InterfaceType)) && typeToConvert.GenericTypeArguments[0].Equals(InterfaceType);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => (JsonConverter)Activator.CreateInstance(typeof(ListConverter<>).MakeGenericType(InterfaceType))!;

    }
    public class InterfaceConverterFactory : JsonConverterFactory
    {
        public InterfaceConverterFactory(Type concrete, Type interfaceType)
        {
            ConcreteType = concrete;
            InterfaceType = interfaceType;
        }

        public Type ConcreteType { get; }
        public Type InterfaceType { get; }

        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == InterfaceType;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => (JsonConverter)Activator.CreateInstance(typeof(ControlThemeConverter<,>).MakeGenericType(ConcreteType, InterfaceType))!;
    }
    public interface IColorSchemeFile
    {
        [JsonConverter(typeof(ControlThemeConverter<ControlTheme<Control>, IControlTheme>))]
        public IControlTheme Primary { get; set; }
        [JsonConverter(typeof(ControlThemeConverter<ControlTheme<Control>, IControlTheme>))]
        public IControlTheme Secondary { get; set; }
        [JsonConverter(typeof(IListInterfaceConverterFactory))]
        public List<IControlTheme> Components { get; set; }
    }
}
