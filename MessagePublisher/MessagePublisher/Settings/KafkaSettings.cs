
public class KafkaSettings
{
    public string KeytabPath { get; set; } = string.Empty;

    public string LdapPrincipleName { get; set; } = string.Empty;

    public string EnvironmentType { get; set; } = string.Empty;

    public string Broker { get; set; } = string.Empty;

    public string SchemaRegistryServer { get; set; } = string.Empty;

    public string CertificatePath { get; set; } = string.Empty;

    public string HostingEnvironment { get; set; } = string.Empty;

    public string ApplicationName { get; set; } = string.Empty;

    public string ConsumerGroupName { get; set; } = string.Empty;

    public int LimitForConsume { get; set; }

    public int LimitForPublish { get; set; }

    public int PartitionsCount { get; set; }
}