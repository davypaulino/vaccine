using DotNet.Testcontainers.Configurations;

namespace vaccine.integration.tests.Helpers;

public static class DockerHelper
{
    public static async Task<bool> IsDockerRunningAsync()
    {
        try
        {
            var client = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration().CreateClient();
            var info = await client.System.GetSystemInfoAsync();
            return info != null;
        }
        catch
        {
            return false;
        }
    }
}