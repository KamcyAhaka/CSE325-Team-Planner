namespace TeamProjectPlanner.Data;

public sealed class MongoDbSettings
{
  public const string SectionName = "MongoDB";

  public string ConnectionString { get; set; } = string.Empty;

  public string DatabaseName { get; set; } = "TeamProjectPlanner";
}
