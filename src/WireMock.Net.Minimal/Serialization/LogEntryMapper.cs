// Copyright © WireMock.Net

using System.Linq;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.ResponseBuilders;
using WireMock.Types;

namespace WireMock.Serialization;

internal class LogEntryMapper
{
    private readonly IWireMockMiddlewareOptions _options;

    public LogEntryMapper(IWireMockMiddlewareOptions options)
    {
        _options = Guard.NotNull(options);
    }

    public LogEntryModel Map(ILogEntry logEntry)
    {
        var logRequestModel = new LogRequestModel
        {
            DateTime = logEntry.RequestMessage.DateTime,
            ClientIP = logEntry.RequestMessage.ClientIP,
            Path = logEntry.RequestMessage.Path,
            AbsolutePath = logEntry.RequestMessage.AbsolutePath,
            Url = logEntry.RequestMessage.Url,
            AbsoluteUrl = logEntry.RequestMessage.AbsoluteUrl,
            ProxyUrl = logEntry.RequestMessage.ProxyUrl,
            Query = logEntry.RequestMessage.Query,
            Method = logEntry.RequestMessage.Method,
            HttpVersion = logEntry.RequestMessage.HttpVersion,
            Headers = logEntry.RequestMessage.Headers,
            Cookies = logEntry.RequestMessage.Cookies
        };

        if (logEntry.RequestMessage.BodyData != null)
        {
            logRequestModel.DetectedBodyType = logEntry.RequestMessage.BodyData.DetectedBodyType?.ToString();
            logRequestModel.DetectedBodyTypeFromContentType = logEntry.RequestMessage.BodyData.DetectedBodyTypeFromContentType?.ToString();

            switch (logEntry.RequestMessage.BodyData.DetectedBodyType)
            {
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                    logRequestModel.Body = logEntry.RequestMessage.BodyData.BodyAsString;
                    break;

                case BodyType.Json:
                    logRequestModel.Body = logEntry.RequestMessage.BodyData.BodyAsString; // In case of Json, do also save the Body as string (backwards compatible)
                    logRequestModel.BodyAsJson = logEntry.RequestMessage.BodyData.BodyAsJson;
                    break;

                case BodyType.Bytes:
                    logRequestModel.BodyAsBytes = logEntry.RequestMessage.BodyData.BodyAsBytes;
                    break;
            }

            logRequestModel.BodyEncoding = logEntry.RequestMessage.BodyData.Encoding != null
                ? new EncodingModel
                {
                    EncodingName = logEntry.RequestMessage.BodyData.Encoding.EncodingName,
                    CodePage = logEntry.RequestMessage.BodyData.Encoding.CodePage,
                    WebName = logEntry.RequestMessage.BodyData.Encoding.WebName
                }
                : null;
        }

        var logResponseModel = new LogResponseModel
        {
            StatusCode = logEntry.ResponseMessage.StatusCode,
            Headers = logEntry.ResponseMessage.Headers
        };

        if (logEntry.ResponseMessage.FaultType != FaultType.NONE)
        {
            logResponseModel.FaultType = logEntry.ResponseMessage.FaultType.ToString();
            logResponseModel.FaultPercentage = logEntry.ResponseMessage.FaultPercentage;
        }

        if (logEntry.ResponseMessage.BodyData != null)
        {
            logResponseModel.BodyOriginal = logEntry.ResponseMessage.BodyOriginal;
            logResponseModel.BodyDestination = logEntry.ResponseMessage.BodyDestination;

            logResponseModel.DetectedBodyType = logEntry.ResponseMessage.BodyData.DetectedBodyType;
            logResponseModel.DetectedBodyTypeFromContentType = logEntry.ResponseMessage.BodyData.DetectedBodyTypeFromContentType;

            MapBody(logEntry, logResponseModel);

            logResponseModel.BodyEncoding = logEntry.ResponseMessage.BodyData.Encoding != null
                ? new EncodingModel
                {
                    EncodingName = logEntry.ResponseMessage.BodyData.Encoding.EncodingName,
                    CodePage = logEntry.ResponseMessage.BodyData.Encoding.CodePage,
                    WebName = logEntry.ResponseMessage.BodyData.Encoding.WebName
                }
                : null;
        }

        return new LogEntryModel
        {
            Guid = logEntry.Guid,
            Request = logRequestModel,
            Response = logResponseModel,

            MappingGuid = logEntry.MappingGuid,
            MappingTitle = logEntry.MappingTitle,
            RequestMatchResult = Map(logEntry.RequestMatchResult),

            PartialMappingGuid = logEntry.PartialMappingGuid,
            PartialMappingTitle = logEntry.PartialMappingTitle,
            PartialRequestMatchResult = Map(logEntry.PartialMatchResult)
        };
    }

    private void MapBody(ILogEntry logEntry, LogResponseModel logResponseModel)
    {
        switch (logEntry.ResponseMessage.BodyData!.DetectedBodyType)
        {
            case BodyType.String:
            case BodyType.FormUrlEncoded:
                if (!string.IsNullOrEmpty(logEntry.ResponseMessage.BodyData.IsFuncUsed) && _options.DoNotSaveDynamicResponseInLogEntry == true)
                {
                    logResponseModel.Body = logEntry.ResponseMessage.BodyData.IsFuncUsed;
                }
                else
                {
                    logResponseModel.Body = logEntry.ResponseMessage.BodyData.BodyAsString;
                }
                break;

            case BodyType.Json:
                logResponseModel.BodyAsJson = logEntry.ResponseMessage.BodyData.BodyAsJson;
                break;

            case BodyType.Bytes:
                logResponseModel.BodyAsBytes = logEntry.ResponseMessage.BodyData.BodyAsBytes;
                break;

            case BodyType.File:
                logResponseModel.BodyAsFile = logEntry.ResponseMessage.BodyData.BodyAsFile;
                logResponseModel.BodyAsFileIsCached = logEntry.ResponseMessage.BodyData.BodyAsFileIsCached;
                break;

            default:
                break;
        }
    }

    private static LogRequestMatchModel? Map(IRequestMatchResult? matchResult)
    {
        if (matchResult == null)
        {
            return null;
        }

        return new LogRequestMatchModel
        {
            IsPerfectMatch = matchResult.IsPerfectMatch,
            TotalScore = matchResult.TotalScore,
            TotalNumber = matchResult.TotalNumber,
            AverageTotalScore = matchResult.AverageTotalScore,
            MatchDetails = matchResult.MatchDetails
            //MatchDetails = matchResult.MatchDetails.Select(md => new
            //{
            //    Name = md.MatcherType.Name.Replace("RequestMessage", string.Empty),
            //    md.Score
            //} as object).ToList()
        };
    }
}