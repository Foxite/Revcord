using Newtonsoft.Json;
using Revcord.Entities;

namespace Revcord.Tests;

public class EntityIdJsonConverterTests {
	public static object[][] Test1Cases = {
		new object[] { "hey" },
		new object[] { 123UL },
		new object[] { 123U },
		new object[] { 123 },
		new object[] { 123L },
		new object[] { 123F },
		new object[] { 123D },
	};
	
	[Test]
	[TestCaseSource(nameof(Test1Cases))]
	public void Test1(object value) {
		var entityId = EntityId.Of(value);
		string serialized = JsonConvert.SerializeObject(entityId);
		Console.WriteLine(serialized);
		var deserialized = JsonConvert.DeserializeObject<EntityId>(serialized);
		
		Assert.That(deserialized, Is.EqualTo(entityId));
	}
	
	[Test]
	public void Test2() {
		var entityId = EntityId.Of(123UL);
		// This project uses net7.0
		string serialized = """
{"TypeName":"System.UInt64","Value":123}
""";
		Console.WriteLine(serialized);
		var deserialized = JsonConvert.DeserializeObject<EntityId>(serialized);
		
		Assert.That(deserialized, Is.EqualTo(entityId));
	}
}
