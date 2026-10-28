# CSE 325 — Week 01 Assignment Notes

## Part 1 — Pizza API seed data

`Services/PizzaService.cs` seeds the `Pizzas` list with the original three pizzas plus one additional pizza (`BBQ Chicken`, id 4):

```json
[
  { "id": 1, "name": "Classic Italian", "isGlutenFree": false },
  { "id": 2, "name": "Veggie", "isGlutenFree": true },
  { "id": 3, "name": "Meat Lovers", "isGlutenFree": false },
  { "id": 4, "name": "BBQ Chicken", "isGlutenFree": false }
]
```

Verified with `GET /pizza` below.

## Part 2 — CRUD request/response examples with status codes

Server run locally with `dotnet run --urls http://localhost:5197`.

### GET all pizzas — `200 OK`

Request:
```
GET http://localhost:5197/pizza
```

Response:
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

[{"id":1,"name":"Classic Italian","isGlutenFree":false},{"id":2,"name":"Veggie","isGlutenFree":true},{"id":3,"name":"Meat Lovers","isGlutenFree":false},{"id":4,"name":"BBQ Chicken","isGlutenFree":false}]
```

### GET pizza by id — `200 OK`

Request:
```
GET http://localhost:5197/pizza/1
```

Response:
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{"id":1,"name":"Classic Italian","isGlutenFree":false}
```

### POST create a pizza — `201 Created`

Request:
```
POST http://localhost:5197/pizza
Content-Type: application/json

{"name": "Hawaiian", "isGlutenFree": false}
```

Response:
```http
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: http://localhost:5197/Pizza/5

{"id":5,"name":"Hawaiian","isGlutenFree":false}
```

### PUT update a pizza — `204 No Content`

Request:
```
PUT http://localhost:5197/pizza/5
Content-Type: application/json

{"id": 5, "name": "Hawaiian Deluxe", "isGlutenFree": true}
```

Response:
```http
HTTP/1.1 204 No Content
```

Confirmed persisted with a follow-up `GET /pizza/5`:
```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{"id":5,"name":"Hawaiian Deluxe","isGlutenFree":true}
```

### DELETE a pizza — `204 No Content`

Request:
```
DELETE http://localhost:5197/pizza/5
```

Response:
```http
HTTP/1.1 204 No Content
```

Confirmed removed with a follow-up `GET /pizza/5`:
```http
HTTP/1.1 404 Not Found
Content-Type: application/problem+json; charset=utf-8

{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"Not Found","status":404}
```

## Part 3 — Sales summary report function

Project: `FileOperations/Program.cs`. Reads every `.txt` file in `FileOperations/SalesData`, sums each file's lines into a per-file total and a grand total, and writes/returns a formatted report using a `StringBuilder` with `:C` currency formatting.

```csharp
static string GenerateSalesSummaryReport(string directoryPath, string outputFilePath)
{
    var report = new StringBuilder();
    decimal totalSales = 0m;

    report.AppendLine("Sales Summary");

    var fileTotals = new List<(string FileName, decimal Total)>();

    foreach (string filePath in Directory.GetFiles(directoryPath, "*.txt").OrderBy(f => f))
    {
        decimal fileTotal = File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Sum(line => decimal.Parse(line.Trim()));

        fileTotals.Add((Path.GetFileName(filePath), fileTotal));
        totalSales += fileTotal;
    }

    report.AppendLine($"Total Sales: {totalSales:C}");
    report.AppendLine("Details:");

    foreach (var (fileName, total) in fileTotals)
    {
        report.AppendLine($"{fileName}: {total:C}");
    }

    File.WriteAllText(outputFilePath, report.ToString());

    return report.ToString();
}
```

Sample run (`dotnet run` from `FileOperations/`) output:

```
Sales Summary
Total Sales: $12,044.44
Details:
store-east.txt: $3,412.44
store-north.txt: $4,336.50
store-south.txt: $4,295.50
```
