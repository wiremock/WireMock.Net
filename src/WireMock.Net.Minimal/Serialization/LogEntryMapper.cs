// Copyright © WireMock.Net

using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.ResponseBuilders;
using WireMock.Types;

namespace WireMock.Serialization;

internal class LogEntryMapper(IWireMockMiddlewareOptions options)
{
    public LogEntryModel Map(ILogEntry logEntry)
    {
        var logRequestModel = logEntry.RequestMessage != null ? MapRequest(logEntry.RequestMessage) : null;

        LogResponseModel? logResponseModel = null;
        if (logEntry.ResponseMessage != null)
        {
            logResponseModel = new LogResponseModel
            {
                DateTime = logEntry.ResponseMessage.DateTime,
                Method = logEntry.ResponseMessage.Method,
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

                logResponseModel.DetectedBodyType = logEntry.ResponseMessage.BodyData.DetectedBodyType?.ToString();
                logResponseModel.DetectedBodyTypeFromContentType = logEntry.ResponseMessage.BodyData.DetectedBodyTypeFromContentType?.ToString();

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

    public NearMissModel Map(NearMiss nearMiss)
    {
        return new NearMissModel
        {
            LogEntryGuid = nearMiss.LogEntryGuid,
            Request = MapRequest(nearMiss.RequestMessage),
            MappingGuid = nearMiss.MappingGuid,
            MappingTitle = nearMiss.MappingTitle,
            RequestMatchResult = Map(nearMiss.RequestMatchResult)
        };
    }

    private static LogRequestModel MapRequest(IRequestMessage requestMessage)
    {
        var logRequestModel = new LogRequestModel
        {
            DateTime = requestMessage.DateTime,
            ClientIP = requestMessage.ClientIP,
            Path = requestMessage.Path,
            AbsolutePath = requestMessage.AbsolutePath,
            Url = requestMessage.Url,
            AbsoluteUrl = requestMessage.AbsoluteUrl,
            ProxyUrl = requestMessage.ProxyUrl,
            Query = requestMessage.Query,
            Method = requestMessage.Method,
            HttpVersion = requestMessage.HttpVersion,
            Headers = requestMessage.Headers,
            Cookies = requestMessage.Cookies
        };

        if (requestMessage.BodyData != null)
        {
            logRequestModel.DetectedBodyType = requestMessage.BodyData.DetectedBodyType?.ToString();
            logRequestModel.DetectedBodyTypeFromContentType = requestMessage.BodyData.DetectedBodyTypeFromContentType?.ToString();

            switch (requestMessage.BodyData.DetectedBodyType)
            {
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                case BodyType.WebSocketText:
                case BodyType.WebSocketClose:
                    logRequestModel.Body = requestMessage.BodyData.BodyAsString;
                    break;

                case BodyType.Json:
                    logRequestModel.Body = requestMessage.BodyData.BodyAsString; // In case of Json, do also save the Body as string (backwards compatible)
                    logRequestModel.BodyAsJson = requestMessage.BodyData.BodyAsJson;
                    break;

                case BodyType.Bytes:
                case BodyType.WebSocketBinary:
                    logRequestModel.BodyAsBytes = requestMessage.BodyData.BodyAsBytes;
                    break;
            }

            logRequestModel.BodyEncoding = requestMessage.BodyData.Encoding != null
                ? new EncodingModel
                {
                    EncodingName = requestMessage.BodyData.Encoding.EncodingName,
                    CodePage = requestMessage.BodyData.Encoding.CodePage,
                    WebName = requestMessage.BodyData.Encoding.WebName
                }
                : null;
        }

        return logRequestModel;
    }

    private void MapBody(ILogEntry logEntry, LogResponseModel logResponseModel)
    {
        switch (logEntry.ResponseMessage?.BodyData?.DetectedBodyType)
        {
            case BodyType.String:
            case BodyType.FormUrlEncoded:
            case BodyType.WebSocketText:
            case BodyType.WebSocketClose:
                if (!string.IsNullOrEmpty(logEntry.ResponseMessage.BodyData.IsFuncUsed) && options.DoNotSaveDynamicResponseInLogEntry == true)
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
            case BodyType.WebSocketBinary:
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