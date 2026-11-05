using System.Text;

namespace SavorHub.Utilities
{
    // Minimal RFC 4180 CSV helpers shared by the CSV export and upload endpoints.
    // Values containing commas, quotes, or line breaks (e.g. TinyMCE HTML in
    // menu item descriptions) are wrapped in quotes so columns never shift.
    public static class CsvUtility
    {
        public static string Escape(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            bool mustQuote = value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
            if (!mustQuote)
            {
                return value;
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        public static string BuildRow(params string?[] values)
        {
            return string.Join(",", values.Select(Escape));
        }

        // Parses CSV text into rows of fields. Supports quoted fields, escaped
        // quotes ("") and line breaks inside quoted fields. Blank lines are skipped.
        public static List<List<string>> Parse(string csv)
        {
            var rows = new List<List<string>>();
            var row = new List<string>();
            var field = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < csv.Length; i++)
            {
                char c = csv[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < csv.Length && csv[i + 1] == '"')
                        {
                            field.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        field.Append(c);
                    }
                }
                else if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    row.Add(field.ToString());
                    field.Clear();
                }
                else if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && i + 1 < csv.Length && csv[i + 1] == '\n')
                    {
                        i++;
                    }
                    EndRow(rows, row, field);
                    row = new List<string>();
                }
                else
                {
                    field.Append(c);
                }
            }

            EndRow(rows, row, field);
            return rows;
        }

        private static void EndRow(List<List<string>> rows, List<string> row, StringBuilder field)
        {
            row.Add(field.ToString());
            field.Clear();

            bool isBlank = row.Count == 1 && row[0].Length == 0;
            if (!isBlank)
            {
                rows.Add(row);
            }
        }
    }
}
