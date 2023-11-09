using Core.Divdados.Domain.UserContext.Entities;

namespace Core.Divdados.Domain.UserContext.Services;

public sealed class AwsService
{
    public readonly string _awsAccessKeyId;
    public readonly string _awsSecretAccessKey;
    public readonly Amazon.RegionEndpoint _awsRegion;

    public AwsService(AWS aws)
    {
        _awsAccessKeyId = aws.AccessKeyId;
        _awsSecretAccessKey = aws.SecretAccessKeyId;
        _awsRegion = Amazon.RegionEndpoint.SAEast1;
    }
}
