using FakeItEasy;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Toastify.DI;
using Toastify.Model;
using ToastifyAPI.Helpers;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Tests.Model
{
    [TestFixture, TestOf(typeof(HotkeyJsonConverter))]
    public class HotkeyJsonConverterTest
    {
        private HotkeyJsonConverter hotkeyJsonConverter;
        private JsonSerializerSettings jsonSerializerSettings;
        private JsonSerializer serializer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.hotkeyJsonConverter = new HotkeyJsonConverter();

            this.jsonSerializerSettings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.Default,
                FloatParseHandling = FloatParseHandling.Decimal,
                FloatFormatHandling = FloatFormatHandling.String,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Formatting = Formatting.Indented,
                MaxDepth = null,
                Culture = CultureInfo.InvariantCulture,
                ConstructorHandling = ConstructorHandling.Default,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                MetadataPropertyHandling = MetadataPropertyHandling.Default,
                TypeNameHandling = TypeNameHandling.None,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            this.serializer = JsonSerializer.Create(this.jsonSerializerSettings);
        }

        [Test(Author = "aleab")]
        public void CanWriteTest()
        {
            Assert.That(this.hotkeyJsonConverter.CanWrite, Is.False);
        }

        [Test(Author = "aleab")]
        public void CanReadTest()
        {
            Assert.That(this.hotkeyJsonConverter.CanRead, Is.True);
        }

        [Test(Author = "aleab")]
        public void WriteJsonTest()
        {
            Hotkey hotkey = new FakeHotkey();

            Assert.That(() =>
            {
                using (var writer = new StringWriter())
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        this.hotkeyJsonConverter.WriteJson(jsonWriter, hotkey, this.serializer);
                    }
                }
            }, Throws.TypeOf<NotImplementedException>());
        }

        [Test(Author = "aleab"), Apartment(ApartmentState.STA)]
        [TestCaseSource(typeof(HotkeyJsonConverterData), nameof(HotkeyJsonConverterData.ReadJsonTestCases))]
        public void ReadJsonTest(string json, Hotkey existingValue, [NotNull, ItemNotNull] List<Func<Hotkey, bool>> conditions)
        {
            Hotkey result;

            using (TextReader textReader = new StringReader(json))
            {
                using (JsonReader reader = new JsonTextReader(textReader))
                {
                    result = this.hotkeyJsonConverter.ReadJson(reader, typeof(Hotkey), existingValue, this.serializer) as Hotkey;
                }
            }

            Assert.Multiple(() =>
            {
                for (int i = 0; i < conditions.Count; i++)
                {
                    var condition = conditions[i];
                    Assert.That(condition.Invoke(result), $"Condition {i} not met!");
                }
            });
        }

        [Test(Author = "aleab")]
        [TestCaseSource(typeof(HotkeyJsonConverterData), nameof(HotkeyJsonConverterData.CanConvertTestCases))]
        public bool CanConvertTest(Type type)
        {
            return this.hotkeyJsonConverter.CanConvert(type);
        }

        public class HotkeyJsonConverterData
        {
            public static IEnumerable<TestCaseData> CanConvertTestCases
            {
                get
                {
                    yield return new TestCaseData(typeof(Hotkey)).Returns(true).SetName("Hotkey");
                    yield return new TestCaseData(typeof(IHotkey)).Returns(false).SetName("IHotkey");
                    yield return new TestCaseData(typeof(FakeHotkey)).Returns(false).SetName("Concrete type implementing IHotkey");
                    yield return new TestCaseData(typeof(object)).Returns(false).SetName("object");
                    yield return new TestCaseData(typeof(string)).Returns(false).SetName("string");
                }
            }

            public static IEnumerable<TestCaseData> ReadJsonTestCases
            {
                get
                {
                    IHotkey fakeHotkey = A.Fake<IHotkey>();

                    Hotkey emptyHotkey = new FakeHotkey();
                    Hotkey hotkey = new FakeHotkey(fakeHotkey);
                    Hotkey keyboardHotkey = new KeyboardHotkey(fakeHotkey);
                    Hotkey mouseHookHotkey = new MouseHookHotkey(fakeHotkey);

                    // Empty json
                    yield return new TestCaseData("{}", null, new List<Func<Hotkey, bool>> { h => h == null }).SetName("Empty json, null existing hotkey");
                    yield return new TestCaseData("{}", emptyHotkey, new List<Func<Hotkey, bool>> { h => h.Equals(emptyHotkey) }).SetName("Empty json, empty existing hotkey");
                    yield return new TestCaseData("{}", hotkey, new List<Func<Hotkey, bool>> { h => h.Equals(hotkey) }).SetName("Empty json, actual hotkey");

                    // Non-empty json; null existing hotkey
                    yield return new TestCaseData($"{{Enabled: true, {nameof(KeyboardHotkey.Key)}: null}}", null,
                            new List<Func<Hotkey, bool>> { h => h is KeyboardHotkey, h => h.Enabled })
                       .SetName("Non-empty json, null existing hotkey; KeyboardHotkey");
                    yield return new TestCaseData($"{{Enabled: true, {nameof(MouseHookHotkey.MouseButton)}: null}}", null,
                            new List<Func<Hotkey, bool>> { h => h is MouseHookHotkey, h => h.Enabled })
                       .SetName("Non-empty json, null existing hotkey; MouseHookHotkey");
                    yield return new TestCaseData("{Enabled: true}", null, new List<Func<Hotkey, bool>> { h => h == null })
                       .SetName("Non-empty json, null existing hotkey; no hotkey type");

                    // Non-empty json; empty existing hotkey
                    yield return new TestCaseData($"{{Enabled: true, {nameof(KeyboardHotkey.Key)}: null}}", emptyHotkey,
                            new List<Func<Hotkey, bool>> { h => h is KeyboardHotkey, h => h.Enabled })
                       .SetName("Non-empty json, empty existing hotkey; KeyboardHotkey");
                    yield return new TestCaseData($"{{Enabled: true, {nameof(MouseHookHotkey.MouseButton)}: null}}", emptyHotkey,
                            new List<Func<Hotkey, bool>> { h => h is MouseHookHotkey, h => h.Enabled })
                       .SetName("Non-empty json, empty existing hotkey; MouseHookHotkey");
                    yield return new TestCaseData("{Enabled: true}", emptyHotkey, new List<Func<Hotkey, bool>> { h => h.Enabled })
                       .SetName("Non-empty json, empty existing hotkey; no hotkey type");

                    // Non-empty json; same-type existing hotkey
                    yield return new TestCaseData($"{{Enabled: true, {nameof(KeyboardHotkey.Key)}: null}}", keyboardHotkey,
                            new List<Func<Hotkey, bool>> { h => h is KeyboardHotkey, h => h.Enabled })
                       .SetName("Non-empty json, same-type existing hotkey; KeyboardHotkey");
                    yield return new TestCaseData($"{{Enabled: true, {nameof(MouseHookHotkey.MouseButton)}: null}}", mouseHookHotkey,
                            new List<Func<Hotkey, bool>> { h => h is MouseHookHotkey, h => h.Enabled })
                       .SetName("Non-empty json, same-type existing hotkey; MouseHookHotkey");

                    // Non-empty json; different-type existing hotkey
                    yield return new TestCaseData($"{{Enabled: true, {nameof(KeyboardHotkey.Key)}: null}}", mouseHookHotkey,
                            new List<Func<Hotkey, bool>> { h => h is KeyboardHotkey, h => h.Enabled })
                       .SetName("Non-empty json, different-type existing hotkey; KeyboardHotkey & MouseHookHotkey (serialized object prevails)");
                    yield return new TestCaseData($"{{Enabled: true, {nameof(MouseHookHotkey.MouseButton)}: null}}", keyboardHotkey,
                            new List<Func<Hotkey, bool>> { h => h is MouseHookHotkey, h => h.Enabled, })
                       .SetName("Non-empty json, different-type existing hotkey; MouseHookHotkey & KeyboardHotkey (serialized object prevails)");

                    // Test property dependency injection
                    yield return new TestCaseData("{}", emptyHotkey, new List<Func<Hotkey, bool>>
                    {
                        h =>
                        {
                            var properties = typeof(Hotkey).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                           .Where(prop => prop.GetCustomAttributes(typeof(PropertyDependencyAttribute)).Any());
                            return properties.All(prop =>
                            {
                                var type = prop.PropertyType;
                                var @default = type.GetDefault();
                                var value = prop.GetValue(h);

                                bool result = value != @default;
                                if (!result)
                                    Console.WriteLine($@"Faulty property: {prop.Name}");
                                return result;
                            });
                        }
                    }).SetName("Test property dependency injection");
                }
            }
        }

        private sealed class FakeHotkey : Hotkey
        {
            private readonly bool _isValid;

            public FakeHotkey()
            {
            }

            public FakeHotkey([NotNull] IHotkey hotkey) : base(hotkey)
            {
            }

            public FakeHotkey(bool isValid, bool active)
            {
                this._isValid = isValid;
                this.Active = active;
            }

            /// <inheritdoc />
            public override string HumanReadableKey { get; }

            /// <inheritdoc />
            protected override void InitInternal()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override bool IsValid()
            {
                return this._isValid;
            }

            internal override void SetIsValid(bool isValid, string invalidReason)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public override IHotkeyVisitor GetVisitor()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            protected override void DispatchInternal(IHotkeyVisitor hotkeyVisitor)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            protected override void DisposeInternal()
            {
                throw new NotImplementedException();
            }
        }
    }
}