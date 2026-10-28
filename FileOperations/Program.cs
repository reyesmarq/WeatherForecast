using System.Text;

string baseDirectory = AppContext.BaseDirectory;
string salesDataDirectory = Path.Combine(baseDirectory, "SalesData");
string reportFilePath = Path.Combine(baseDirectory, "sales-summary-report.txt");

Console.WriteLine($"Reading sales data from: {salesDataDirectory}");

string report = GenerateSalesSummaryReport(salesDataDirectory, reportFilePath);

Console.WriteLine();
Console.WriteLine(report);
Console.WriteLine($"Report written to: {reportFilePath}");

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
