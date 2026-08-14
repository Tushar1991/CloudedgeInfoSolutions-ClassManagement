namespace ClassManagement.Api.Configuration;

public class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "tusharugalein_db_user:hamcnuhA1FWS1tJ3@cluster0.pgp1szq.mongodb.net/";
    public string DatabaseName { get; set; } = "ClassManagementDb";
}
