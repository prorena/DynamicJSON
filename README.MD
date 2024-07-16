# Prorena Dynamic JSON
Plattforms: All | Frameworks: .NET 6.0, .NET 8.0 | [GitHub repository](https://github.com/prorena/DynamicJSON) 

Prorena Dynamic JSON library to convert JSON-objects into real dynamic ExpandoObjects.
You can significantly shorten your code since you no longer need type conversions. So it becomes much easier to write, read and maintain.

### Example 1
``` csharp
foreach (JObject link in ((JArray)item["links"]!).Cast<JObject>())
{}

// becomes
foreach (JObject link in item.links)
{}
 ```

### Example 2
``` csharp
var ids = (JArray)((JObject)((JArray)((JObject)responseData["bookingResponse"]!)["idInformation"]!).First())["ids"]!;

// becomes
var ids = responseData.bookingResponse.idInformation[0].ids;
 ```

## Usage
### Linq on String
``` csharp
string json = "{\"name\":\"Florian\",\"age\":25}";
dynamic? result = json.ToDynamicJson();
string name = result.name;      // "Florian"
int age = result.age;           // 25
```
### Linq on JToken
``` csharp
JObject json = JObject.Parse("{\"name\":\"Florian\",\"age\":25}");
dynamic? result = json.ToDynamic();
string name = result.name;      // "Florian"
int age = result.age;           // 25
```