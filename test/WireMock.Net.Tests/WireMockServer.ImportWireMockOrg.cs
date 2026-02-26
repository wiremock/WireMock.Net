// Copyright © WireMock.Net

using WireMock.Server;

namespace WireMock.Net.Tests
{
    public class WireMockServerImportWireMockOrgTests
    {
        // For for AppVeyor + OpenCover
        private string GetCurrentFolder()
        {
            return Directory.GetCurrentDirectory();
        }

        [Fact]
        public void WireMockServer_Admin_ReadStaticWireMockOrgMappingAndAddOrUpdate()
        {
            var server = WireMockServer.Start();
            string path = Path.Combine(GetCurrentFolder(), "__admin", "mappings.org", "mapping1.json");
            server.ReadStaticWireMockOrgMappingAndAddOrUpdate(path);

            var mappings = server.Mappings.ToArray();

            mappings.Should().HaveCount(1);

            server.Stop();
        }
    }
}