// Copyright © WireMock.Net

namespace WireMock.Net.Tests.Serialization
{
    public class CustomPathParamMatcherModel
    {
        public string Path { get; set; }
        public Dictionary<string, string> PathParams { get; set; }

        public CustomPathParamMatcherModel()
        {
        }

        public CustomPathParamMatcherModel(string path, Dictionary<string, string> pathParams)
        {
            Path = path;
            PathParams = pathParams;
        }
    }
}