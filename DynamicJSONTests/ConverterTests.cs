///////////////////////////////////////
// Part of Prorena dynamic JSON; an  //
// open-source converter for JObjects//
// into real dynamic ExpandoObjects. //
//                                   //
// Copyright © 2024 by Prorena GmbH. //
// Author: Florian Weihmann          //
// Licensed under GPL v.3            //
///////////////////////////////////////

using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace DynamicJSON.Tests
{
    [TestClass()]
    public class ConverterTests
    {
        [TestMethod()]
        public void ToDynamicJsonTest()
        {
            var json = "{\"name\":\"Max\",\"alter\":40,\"anschrift\":{\"strasse\":\"Waldweg\",\"nr\":4.8},\"single\":true,\"pets\":null,\"books\":[\"Harry Potter I\", \"Harry Potter II\"]}";
            var dyn = json.ToDynamicJson();
            Assert.IsNotNull(dyn);
            Assert.AreEqual("Max", dyn!.name);
            Assert.AreEqual(40, dyn.alter);
            Assert.AreEqual("Waldweg", dyn.anschrift.strasse);
            Assert.AreEqual(4.8, dyn.anschrift.nr);
            Assert.AreEqual(true, dyn.single);
            Assert.IsNull(dyn.pets);
            Assert.IsInstanceOfType(dyn.books, typeof(List<object>));
            Assert.AreEqual(2, ((List<object>)dyn.books).Count);
            Assert.AreEqual("Harry Potter I", dyn.books[0]);
        }

        [TestMethod()]
        public void ToDynamicTest()
        {
            JObject? j = null;
            Assert.IsNull(j.ToDynamic());
        }

        [TestMethod()]
        public void SpecialTest()
        {
            var json = new JObject
            {
                new JProperty("ts", TimeSpan.FromDays(5)),
                new JProperty("guid", new Guid()),
                new JProperty("uri", new Uri("https://prorena.de/")),
                new JProperty("date", DateTime.Now),
                new JProperty("bytes",new byte[] {9,6, 3 }),
            };
            var dyn = json.ToDynamic();

            Assert.IsNotNull(dyn);
            Assert.IsInstanceOfType<TimeSpan>(dyn!.ts);
            Assert.IsInstanceOfType<Guid>(dyn.guid);
            Assert.IsInstanceOfType<Uri>(dyn.uri);
            Assert.IsInstanceOfType<DateTime>(dyn.date);
            Assert.IsInstanceOfType<byte[]>(dyn.bytes);


            Assert.IsNull(new JConstructor("test").ToDynamic());

            Assert.AreEqual("test1", new JConstructor("name", "test1").ToDynamic());
            dyn = new JConstructor("name", "test1", "test2").ToDynamic();
            Assert.IsNotNull(dyn);
            Assert.AreEqual("test2", dyn!.test1);

            dyn = new JConstructor("name", 5, "test2").ToDynamic();
            Assert.IsInstanceOfType<List<object>>(dyn);
            Assert.AreEqual(5, dyn![0]);
            Assert.AreEqual("test2", dyn[1]);

#pragma warning disable CS8625
            dyn = new JConstructor("name", null, "test2").ToDynamic();
            Assert.IsInstanceOfType<List<object>>(dyn);
            Assert.IsNull(dyn![0]);
#pragma warning restore

            dyn = new JConstructor("name", "test1", "test2", "test3").ToDynamic();
            Assert.IsInstanceOfType<dynamic>(dyn);
            Assert.AreEqual("test2", dyn!.test1[0]);
            Assert.AreEqual("test3", dyn.test1[1]);

            dyn = new JConstructor("ctorExample",
                new JValue("value"),
                new JObject(new JProperty("prop1", new JValue(1)), new JProperty("prop2", new JValue(2)))
                ).ToDynamic();

            Assert.AreEqual(1, dyn!.value.prop1);
            Assert.AreEqual(2, dyn!.value.prop2);

            dyn = new JProperty("text", "value").ToDynamic();
            Assert.IsNotNull(dyn);
            Assert.AreEqual("value", dyn!.text);

            dyn = new JRaw(json).ToDynamic();
            Assert.IsNotNull(dyn);
            Assert.IsInstanceOfType<TimeSpan>(dyn!.ts);
            Assert.AreEqual(TimeSpan.FromDays(5), dyn.ts);

            Assert.IsNull(new JRaw((object?)null).ToDynamic());
        }

#if NET8_0
        [TestMethod()]
        public void MsTest()
        {
            var json = JsonObject.Parse("{\"name\":\"Max\",\"alter\":40,\"anschrift\":{\"strasse\":\"Waldweg\",\"nr\":4.8},\"single\":true,\"pets\":null,\"books\":[\"Harry Potter I\", \"Harry Potter II\"],\"happy\":false}");
            var dyn = json.ToDynamic();
            Assert.IsNotNull(dyn);
            Assert.AreEqual("Max", dyn!.name);
            Assert.AreEqual(40, dyn.alter);
            Assert.AreEqual("Waldweg", dyn.anschrift.strasse);
            Assert.AreEqual(4.8, dyn.anschrift.nr);
            Assert.AreEqual(true, dyn.single);
            Assert.AreEqual(false, dyn.happy);
            Assert.IsNull(dyn.pets);
            Assert.IsInstanceOfType(dyn.books, typeof(List<object>));
            Assert.AreEqual(2, ((List<object>)dyn.books).Count);
            Assert.AreEqual("Harry Potter I", dyn.books[0]);
        }
#endif
    }
}