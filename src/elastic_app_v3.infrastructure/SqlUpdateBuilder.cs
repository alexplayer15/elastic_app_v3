namespace elastic_app_v3.infrastructure;
public class SqlUpdateBuilder(string tableName)
{
    private readonly string _tableName = tableName;
    private readonly List<string> _setClauses = [];
    private string _whereClause = string.Empty;
    public SqlUpdateBuilder SetIfNotNull(string column, object? value)
    {
        if (value is not null) //might have to do type coersion
        {
            _setClauses.Add($"{column} = @{column}");
        }

        return this;
    }
    public SqlUpdateBuilder Where(string clause)
    {
        _whereClause = clause;
        return this;
    }

    public string Build()
    {
        if (_setClauses.Count == 0)
            throw new InvalidOperationException("No fields to update");

        if (string.IsNullOrWhiteSpace(_whereClause))
            throw new InvalidOperationException("WHERE clause required");

        return $@"UPDATE {_tableName}
            SET {string.Join(",", _setClauses)}
            WHERE {_whereClause}";
    }
}
