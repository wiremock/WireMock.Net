# 1.25.0 (25 January 2026)
- [#1389](https://github.com/wiremock/WireMock.Net/pull/1389) - Fix MimePartMatcher and add more tests [bug] contributed by [StefH](https://github.com/StefH)
- [#1371](https://github.com/wiremock/WireMock.Net/issues/1371) - MimePartMatcher does not match multipart/form-data request body (possible bug?) [bug]

# 1.24.0 (18 January 2026)
- [#1417](https://github.com/wiremock/WireMock.Net/pull/1417) - Update aspire to 13.1 (examples + code) [feature] contributed by [petrroll](https://github.com/petrroll)
- [#1418](https://github.com/wiremock/WireMock.Net/pull/1418) - Add OTEL tracing support for Wiremock + automatic OTEL for Aspire integration [feature] contributed by [petrroll](https://github.com/petrroll)
- [#1214](https://github.com/wiremock/WireMock.Net/issues/1214) - OpenTelemetry Support for .NET Aspire [feature]

# 1.23.0 (05 January 2026)
- [#1414](https://github.com/wiremock/WireMock.Net/pull/1414) - Pass the parameter matchOperator in Request.WithPath to its inner calls [bug] contributed by [gbamqzkdyg](https://github.com/gbamqzkdyg)
- [#1416](https://github.com/wiremock/WireMock.Net/pull/1416) - Fix: Pass AllowedHandlebarsHelpers configuration to Handlebars.Net.Helpers library contributed by [samlatham](https://github.com/samlatham)
- [#1413](https://github.com/wiremock/WireMock.Net/issues/1413) - Parameter `matchOperator` is not respected in the method Request.WithPath [bug]
- [#1415](https://github.com/wiremock/WireMock.Net/issues/1415) - HandlebarsSettings AllowedHandlebarsHelpers Configuration Not Applied [bug]

# 1.22.0 (02 January 2026)
- [#1412](https://github.com/wiremock/WireMock.Net/pull/1412) - chore(testcontainers): bump up Testcontainers to version 4.10.0 [feature] contributed by [vhatsura](https://github.com/vhatsura)
- [#1411](https://github.com/wiremock/WireMock.Net/issues/1411) - WireMock.Net.Testcontainers isn't compatible with Testcontainers 4.10.0 [bug]

# 1.21.0 (25 December 2025)
- [#1408](https://github.com/wiremock/WireMock.Net/pull/1408) - Fix readyness-check for Testcontainers [bug] contributed by [StefH](https://github.com/StefH)

# 1.20.0 (24 December 2025)
- [#1399](https://github.com/wiremock/WireMock.Net/pull/1399) - Upgrade RamlToOpenApiConverter and YamlDotNet [feature] contributed by [StefH](https://github.com/StefH)
- [#1400](https://github.com/wiremock/WireMock.Net/pull/1400) - Add WireMock.Net.NUnit project [feature] contributed by [StefH](https://github.com/StefH)
- [#1405](https://github.com/wiremock/WireMock.Net/pull/1405) - Fix Testcontainers AddProtoDefinition [bug] contributed by [StefH](https://github.com/StefH)
- [#1398](https://github.com/wiremock/WireMock.Net/issues/1398) - Upgrade YamlDotNet dependency [feature]
- [#1404](https://github.com/wiremock/WireMock.Net/issues/1404) - An exception occurs when adding multiple proto definitions in the TestContainer. [bug]

# 1.19.0 (12 December 2025)
- [#1391](https://github.com/wiremock/WireMock.Net/pull/1391) - Update WireMockContainerBuilder (WithImage and WithCustomImage) [feature] contributed by [StefH](https://github.com/StefH)
- [#1392](https://github.com/wiremock/WireMock.Net/pull/1392) - WireMockContainerBuilder: allow all docker images named wiremock [feature] contributed by [StefH](https://github.com/StefH)
- [#1390](https://github.com/wiremock/WireMock.Net/issues/1390) - Unable to build WireMockContainerBuilder with custom image [feature]

# 1.18.0 (09 December 2025)
- [#1388](https://github.com/wiremock/WireMock.Net/pull/1388) - Add WithBodyAsType to RequestMatcher [feature] contributed by [StefH](https://github.com/StefH)

# 1.17.0 (07 December 2025)
- [#1383](https://github.com/wiremock/WireMock.Net/pull/1383) - Aspire: Add WithProtoDefinition to support proto definition at server level [feature] contributed by [StefH](https://github.com/StefH)
- [#1386](https://github.com/wiremock/WireMock.Net/pull/1386) - Fix random delay in mapping json file [bug] contributed by [StefH](https://github.com/StefH)
- [#1274](https://github.com/wiremock/WireMock.Net/issues/1274) - .WithMappings to mount volume is not working for GRPC [bug]
- [#1381](https://github.com/wiremock/WireMock.Net/issues/1381) - Downstream dependencies missing after 1.16.0 release [bug]
- [#1382](https://github.com/wiremock/WireMock.Net/issues/1382) - Does Aspire support enabling HTTP/2? [feature]
- [#1385](https://github.com/wiremock/WireMock.Net/issues/1385) - Do delays and probabilities show in saved static mappings? [bug]
- [#1387](https://github.com/wiremock/WireMock.Net/issues/1387) - Tests failing with TaskCanceledException on Windows Server 2025 Build 7171 [bug]

# 1.16.0 (18 November 2025)
- [#1366](https://github.com/wiremock/WireMock.Net/pull/1366) - WireMock.Net.OpenApiParser : support Examples [feature] contributed by [StefH](https://github.com/StefH)
- [#1375](https://github.com/wiremock/WireMock.Net/pull/1375) - Add WireMockHealthCheck in WireMock.Net.Aspire [feature] contributed by [Zguy](https://github.com/Zguy)
- [#1377](https://github.com/wiremock/WireMock.Net/pull/1377) - Check if the path is valid when using WithPath(...) [feature] contributed by [StefH](https://github.com/StefH)
- [#1380](https://github.com/wiremock/WireMock.Net/pull/1380) - Add WireMock.Net.xUnit.v3 project [feature] contributed by [StefH](https://github.com/StefH)
- [#1364](https://github.com/wiremock/WireMock.Net/issues/1364) - Choosing examples from open api specification for responses. [feature]
- [#1376](https://github.com/wiremock/WireMock.Net/issues/1376) - AdminApiMappingBuilder `WithPath` should add the starting `/` if missing [feature]
- [#1379](https://github.com/wiremock/WireMock.Net/issues/1379) - xUnit v3 [feature]

# 1.15.0 (22 October 2025)
- [#1367](https://github.com/wiremock/WireMock.Net/pull/1367) - Fix WithProbability logic [bug] contributed by [StefH](https://github.com/StefH)
- [#1370](https://github.com/wiremock/WireMock.Net/pull/1370) - Support Testcontainers 4.8.0 [bug] contributed by [MD-V](https://github.com/MD-V)
- [#1126](https://github.com/wiremock/WireMock.Net/issues/1126) - Request matching WithProbability strange behaviour [bug]

# 1.14.0 (06 October 2025)
- [#1362](https://github.com/wiremock/WireMock.Net/pull/1362) - Update ProxyUrlReplaceSettingsModel with TransformTemplate property [feature] contributed by [StefH](https://github.com/StefH)
- [#1363](https://github.com/wiremock/WireMock.Net/pull/1363) - Add Tls13 [bug] contributed by [StefH](https://github.com/StefH)
- [#1342](https://github.com/wiremock/WireMock.Net/issues/1342) - Facing SSL certificate validation error when using .NET 5 and above framework for some http services during recording [bug]
- [#1360](https://github.com/wiremock/WireMock.Net/issues/1360) - ProxyURL path configuration [feature]

# 1.13.0 (28 September 2025)
- [#1358](https://github.com/wiremock/WireMock.Net/pull/1358) - TypeLoader: implement Try methods [feature] contributed by [StefH](https://github.com/StefH)
- [#1361](https://github.com/wiremock/WireMock.Net/pull/1361) - ProxyUrlTransformer [feature] contributed by [StefH](https://github.com/StefH)

# 1.12.0 (30 August 2025)
- [#1357](https://github.com/wiremock/WireMock.Net/pull/1357) - Upgrade Testcontainers to 4.7.0 [feature] contributed by [StefH](https://github.com/StefH)
- [#1356](https://github.com/wiremock/WireMock.Net/issues/1356) - WireMock.Net 1.11.2 is not compatible with TestContainers 4.7.0 [bug]

# 1.11.2 (27 August 2025)
- [#1352](https://github.com/wiremock/WireMock.Net/pull/1352) - Add additional try-catch to TypeLoader logic [feature] contributed by [StefH](https://github.com/StefH)
- [#1354](https://github.com/wiremock/WireMock.Net/pull/1354) - Add System.Net.Http again to solve CVE warning [bug] contributed by [StefH](https://github.com/StefH)
- [#1355](https://github.com/wiremock/WireMock.Net/pull/1355) - Revert JetBrains.Annotations and add System.Text.RegularExpressions t&#8230; [bug] contributed by [StefH](https://github.com/StefH)
- [#1071](https://github.com/wiremock/WireMock.Net/issues/1071) - Split WireMock into multiple nuget packages depending on features [feature]
- [#1351](https://github.com/wiremock/WireMock.Net/issues/1351) - Version 1.11.0 seems to have a dependency to non-existing version of JetBrans.Annotations [bug]
- [#1353](https://github.com/wiremock/WireMock.Net/issues/1353) - 1.11.0: NU1903: Warning As Error: Package 'System.Net.Http' 4.3.0 has a known high severity vulnerability, https://github.com/advisories/GHSA-7jgj-8wvc-jh57 [bug]

# 1.11.0 (26 August 2025)
- [#1350](https://github.com/wiremock/WireMock.Net/pull/1350) - Create WireMock.Net.ProtoBuf project [feature] contributed by [StefH](https://github.com/StefH)

# 1.10.1 (22 August 2025)
- [#1349](https://github.com/wiremock/WireMock.Net/pull/1349) - Add AtPath and AtAbsolutePath to Assertions projects [feature] contributed by [StefH](https://github.com/StefH)
- [#1348](https://github.com/wiremock/WireMock.Net/issues/1348) - Support AtPath and AtAbsolutePath in WireMockAssertions [feature]

# 1.10.0 (18 August 2025)
- [#1344](https://github.com/wiremock/WireMock.Net/pull/1344) - Add new package WireMock.Net.Extensions.Routing which provides minimal-API-style routing for WireMock.Net [feature] contributed by [GennadyGS](https://github.com/GennadyGS)
- [#1340](https://github.com/wiremock/WireMock.Net/issues/1340) - Feature Request: Add minimal-API-style routing as Extension Package [feature]

# 1.9.1 (17 August 2025)
- [#1345](https://github.com/wiremock/WireMock.Net/pull/1345) - Add TimesInSameState to MappingModel [feature] contributed by [StefH](https://github.com/StefH)
- [#1347](https://github.com/wiremock/WireMock.Net/pull/1347) - Fix generating source code for Scenario and State [bug] contributed by [StefH](https://github.com/StefH)
- [#1343](https://github.com/wiremock/WireMock.Net/issues/1343) - MappingModel allows to configure the times for WillSetStateTo [feature]
- [#1346](https://github.com/wiremock/WireMock.Net/issues/1346) - Mapping: generated C# code is missing InScenario and WillSetStateTo [bug]

# 1.9.0 (10 August 2025)
- [#1334](https://github.com/wiremock/WireMock.Net/pull/1334) - Create GraphQL project [feature] contributed by [StefH](https://github.com/StefH)

# 1.8.18 (04 August 2025)
- [#1339](https://github.com/wiremock/WireMock.Net/pull/1339) - Fixes an issue with matching JSON bodies as bytes [bug] contributed by [smfields](https://github.com/smfields)
- [#1338](https://github.com/wiremock/WireMock.Net/issues/1338) - Specifying .WithBody(byte[]) fails to match for JSON bodies [bug]

# 1.8.17 (23 July 2025)
- [#1337](https://github.com/wiremock/WireMock.Net/pull/1337) - Make CSharpCodeMatcher public [bug] contributed by [StefH](https://github.com/StefH)
- [#1336](https://github.com/wiremock/WireMock.Net/issues/1336) - Is CSharpCodeMatcher actually usable? [bug]

# 1.8.16 (19 July 2025)
- [#1332](https://github.com/wiremock/WireMock.Net/pull/1332) - Use correct Handlebars.Net.Helpers.Xslt (2.5.2) [bug] contributed by [StefH](https://github.com/StefH)
- [#1328](https://github.com/wiremock/WireMock.Net/issues/1328) - ReflectionTypeLoadException occurs when running multiple unit tests (Nunit 3) [bug]

# 1.8.15 (18 July 2025)
- [#1331](https://github.com/wiremock/WireMock.Net/pull/1331) - Correctly map the Pact Interaction Description property [bug] contributed by [StefH](https://github.com/StefH)
- [#1330](https://github.com/wiremock/WireMock.Net/issues/1330) - Generated Pact-compatible consumer contract does not contain description [bug]

# 1.8.14 (13 July 2025)
- [#1325](https://github.com/wiremock/WireMock.Net/pull/1325) - Add method CreateHttpClientFactory [feature] contributed by [StefH](https://github.com/StefH)
- [#1326](https://github.com/wiremock/WireMock.Net/pull/1326) - Implement IMimeMessageData [feature] contributed by [StefH](https://github.com/StefH)
- [#1329](https://github.com/wiremock/WireMock.Net/pull/1329) - Fix HandlebarsContext.ParseAndEvaluate [bug] contributed by [StefH](https://github.com/StefH)
- [#1327](https://github.com/wiremock/WireMock.Net/issues/1327) - Response Body Does Not Evaluate Multiple Handlebars Templates [bug]

# 1.8.13 (23 June 2025)
- [#1322](https://github.com/wiremock/WireMock.Net/pull/1322) - Add Scenario set State method [feature] contributed by [StefH](https://github.com/StefH)
- [#1321](https://github.com/wiremock/WireMock.Net/issues/1321) - Feature: Setting individual scenario state via Admin API [feature]

# 1.8.12 (15 June 2025)
- [#1317](https://github.com/wiremock/WireMock.Net/pull/1317) - Set description when converting MappingModel to IRespondWithAProvider [feature] contributed by [BodrickLight](https://github.com/BodrickLight)
- [#1320](https://github.com/wiremock/WireMock.Net/pull/1320) - Fix TypeLoader [bug] contributed by [StefH](https://github.com/StefH)
- [#1319](https://github.com/wiremock/WireMock.Net/issues/1319) - Why is IRequestMessage.BodyAsMimeMessage null in new versions (&gt; 1.8.7) [bug]

# 1.8.11 (11 June 2025)
- [#1311](https://github.com/wiremock/WireMock.Net/pull/1311) - Bump Testcontainers version to 4.5.0 [feature] contributed by [scrocquesel](https://github.com/scrocquesel)
- [#1313](https://github.com/wiremock/WireMock.Net/pull/1313) - Update RandomDataGenerator.Net to 1.0.19 [feature] contributed by [StefH](https://github.com/StefH)
- [#1315](https://github.com/wiremock/WireMock.Net/pull/1315) - Fix for WithTransformer and JsonBody as list [bug] contributed by [StefH](https://github.com/StefH)
- [#1310](https://github.com/wiremock/WireMock.Net/issues/1310) - Binary compat issue with testcontainers 4.5.0 [bug]
- [#1312](https://github.com/wiremock/WireMock.Net/issues/1312) - WithTransformer breaks when the response BodyAsJson is a List [bug]

# 1.8.10 (10 June 2025)
- [#1306](https://github.com/wiremock/WireMock.Net/pull/1306) - Update RequestModelBuilder (add WithHeader) [feature] contributed by [StefH](https://github.com/StefH)
- [#1308](https://github.com/wiremock/WireMock.Net/pull/1308) - For some projects, change dependency to WireMock.Net.Minimal [feature] contributed by [StefH](https://github.com/StefH)
- [#1309](https://github.com/wiremock/WireMock.Net/pull/1309) - Update AwesomeAssertions to version 9 [feature] contributed by [StefH](https://github.com/StefH)
- [#1305](https://github.com/wiremock/WireMock.Net/issues/1305) - AdminApiMappingBuilder do not expose WithHeader for Request [feature]
- [#1307](https://github.com/wiremock/WireMock.Net/issues/1307) - Update WireMock.Net.AwesomeAssertions to use latest version from AwesomeAssertions [feature]

# 1.8.9 (28 May 2025)
- [#1303](https://github.com/wiremock/WireMock.Net/pull/1303) - Add option to provide X509Certificate [feature] contributed by [StefH](https://github.com/StefH)
- [#1302](https://github.com/wiremock/WireMock.Net/issues/1302) - Support In-Memory SSL Certificate [feature]

# 1.8.8 (24 May 2025)
- [#1300](https://github.com/wiremock/WireMock.Net/pull/1300) - Create WireMock.Net.MimePart project [feature] contributed by [StefH](https://github.com/StefH)

# 1.8.7 (22 May 2025)
- [#1297](https://github.com/wiremock/WireMock.Net/pull/1297) - Fix BodyParser to correctly check for json [bug] contributed by [StefH](https://github.com/StefH)
- [#1299](https://github.com/wiremock/WireMock.Net/pull/1299) - Use TinyMapper.Signed [bug] contributed by [StefH](https://github.com/StefH)
- [#1301](https://github.com/wiremock/WireMock.Net/pull/1301) - Fix errors when converting response bodies with top-level JSON arrays to C# code [bug] contributed by [asaf92](https://github.com/asaf92)
- [#973](https://github.com/wiremock/WireMock.Net/issues/973) - TinyMapper error [bug]
- [#1272](https://github.com/wiremock/WireMock.Net/issues/1272) - WithMappingFromOpenApiFile fails with exception when example item present in text-plain response in OAS contract
- [#1295](https://github.com/wiremock/WireMock.Net/issues/1295) - Content-Type `multipart/form-data` header not proxied [bug]
- [#1298](https://github.com/wiremock/WireMock.Net/issues/1298) - TinyMapper error redux [bug]

# 1.8.6 (15 May 2025)
- [#1296](https://github.com/wiremock/WireMock.Net/pull/1296) - Content-Type multipart/form-data header should also be proxied [bug] contributed by [StefH](https://github.com/StefH)

# 1.8.5 (14 May 2025)
- [#1290](https://github.com/wiremock/WireMock.Net/pull/1290) - Use ILRepack to include Microsoft.OpenApi as internal [feature] contributed by [StefH](https://github.com/StefH)
- [#1293](https://github.com/wiremock/WireMock.Net/pull/1293) - Grpc: Fix parsing null value for google.protobuf.Timestamp [bug] contributed by [StefH](https://github.com/StefH)
- [#1275](https://github.com/wiremock/WireMock.Net/issues/1275) - WithMappingFromOpenApiFile - Support for OpenAPI 3.1.0 [feature]
- [#1280](https://github.com/wiremock/WireMock.Net/issues/1280) - Grpc: breaking changes from 1.6.11 to 1.6.12 [bug]

# 1.8.3 (06 May 2025)
- [#1288](https://github.com/wiremock/WireMock.Net/pull/1288) - Update AzureADAuthenticationMatcher to support V2 Azure AAD tokens [feature] contributed by [StefH](https://github.com/StefH)
- [#1286](https://github.com/wiremock/WireMock.Net/issues/1286) - Unauthorized (401) error when calling the admin API with version 2 Azure AAD tokens. [bug]

# 1.8.2 (05 May 2025)
- [#1291](https://github.com/wiremock/WireMock.Net/pull/1291) - Update ProtoBufJsonConverter to fix conflict for 'MessageOptions' [bug] contributed by [StefH](https://github.com/StefH)
- [#1083](https://github.com/wiremock/WireMock.Net/issues/1083) - Compilation Error due to 'MessageOptions' Type Conflict between 'Google.Protobuf' and 'WireMock.Net' [bug]
- [#1287](https://github.com/wiremock/WireMock.Net/issues/1287) - 1.8.0 - issues with dependency on Microsoft.OpenApi version 2.0.0-preview.xx [bug]

# 1.8.1 (02 May 2025)
- [#1289](https://github.com/wiremock/WireMock.Net/pull/1289) - Revert changes to WireMock.Net.OpenApiParser [bug] contributed by [StefH](https://github.com/StefH)

# 1.8.0 (28 April 2025)
- [#1268](https://github.com/wiremock/WireMock.Net/pull/1268) - 1.7.x [bug] contributed by [StefH](https://github.com/StefH)
- [#1271](https://github.com/wiremock/WireMock.Net/pull/1271) - Add HandlebarsSettings [feature] contributed by [StefH](https://github.com/StefH)
- [#1273](https://github.com/wiremock/WireMock.Net/pull/1273) - feat(awesome-assertions): Added new project WireMock.Net.AwesomeAssertions [feature] contributed by [Crowbar90](https://github.com/Crowbar90)
- [#1278](https://github.com/wiremock/WireMock.Net/pull/1278) - changed null check in JSONPathMatcher and JmesPathMatcher to ensure t&#8230; [bug] contributed by [jollyjoyce1995](https://github.com/jollyjoyce1995)
- [#1283](https://github.com/wiremock/WireMock.Net/pull/1283) - Add an launch inspector command to Aspire Dashboard [feature] contributed by [jmezach](https://github.com/jmezach)
- [#1285](https://github.com/wiremock/WireMock.Net/pull/1285) - Enable support for WireMock Middleware in Hosted Services [bug] contributed by [etkr](https://github.com/etkr)
- [#1243](https://github.com/wiremock/WireMock.Net/issues/1243) - Support for AwesomeAssertions [feature]
- [#1264](https://github.com/wiremock/WireMock.Net/issues/1264) - OpenApiParser - Construction of path possibly incorrect [bug]
- [#1276](https://github.com/wiremock/WireMock.Net/issues/1276) - Add WireMock Inspector command to Aspire integration [feature]
- [#1277](https://github.com/wiremock/WireMock.Net/issues/1277) - Newtonsoft.Json.JsonReaderException: 'Unable to read the JSON string.' when using IStringMatcher with an empty body [bug]
- [#1284](https://github.com/wiremock/WireMock.Net/issues/1284) - WireMock.Net.AspNetCore.Middleware does not work in hosted services [bug]

# 1.7.4 (27 February 2025)
- [#1256](https://github.com/wiremock/WireMock.Net/pull/1256) - Add ToArray() to ConcurrentObservableCollection [bug] contributed by [StefH](https://github.com/StefH)
- [#1254](https://github.com/wiremock/WireMock.Net/issues/1254) - FindLogEntries exception 'Destination array was not long enough' [bug]

# 1.7.3 (24 February 2025)
- [#1253](https://github.com/wiremock/WireMock.Net/pull/1253) - Update QueryStringParser to support param with equal but no value [bug] contributed by [StefH](https://github.com/StefH)
- [#1247](https://github.com/wiremock/WireMock.Net/issues/1247) - API call isn't matched when using an empty query string parameter [bug]

# 1.7.2 (12 February 2025)
- [#1246](https://github.com/wiremock/WireMock.Net/pull/1246) - Add &quot;AddUrl&quot; to WireMockContainerBuilder to support grpc [feature] contributed by [StefH](https://github.com/StefH)
- [#1248](https://github.com/wiremock/WireMock.Net/pull/1248) - Add exception message to logging when mapping fails due to an exception. [feature] contributed by [JvE-iO](https://github.com/JvE-iO)
- [#1250](https://github.com/wiremock/WireMock.Net/pull/1250) - Add ProtoDefinition to WireMockContainer [feature] contributed by [StefH](https://github.com/StefH)
- [#1239](https://github.com/wiremock/WireMock.Net/issues/1239) - How to use WiremockContainerBuilder for grpc using http2 [feature]
- [#1249](https://github.com/wiremock/WireMock.Net/issues/1249) - Add protodefinition and refer it from mapping [feature]

# 1.7.1 (26 January 2025)
- [#1236](https://github.com/wiremock/WireMock.Net/pull/1236) - Fix ProtoBuf mapping.json [bug] contributed by [StefH](https://github.com/StefH)
- [#1245](https://github.com/wiremock/WireMock.Net/pull/1245) - Use Handlebars.Net.Helpers to version 2.4.10 [feature] contributed by [StefH](https://github.com/StefH)
- [#1233](https://github.com/wiremock/WireMock.Net/issues/1233) - GRPC mappings are not created correctly when created through Admin API [bug]

# 1.7.0 (22 January 2025)
- [#1242](https://github.com/wiremock/WireMock.Net/pull/1242) - Disable DynamicLinq to fix CVE [bug] contributed by [StefH](https://github.com/StefH)

# 1.6.12 (21 January 2025)
- [#1231](https://github.com/wiremock/WireMock.Net/pull/1231) - Fix google protobuf WellKnownTypes: Empty, Duration and Timestamp [bug] contributed by [StefH](https://github.com/StefH)
- [#1235](https://github.com/wiremock/WireMock.Net/pull/1235) - Fix ArgumentException in FindLogEntries [bug] contributed by [StefH](https://github.com/StefH)
- [#1241](https://github.com/wiremock/WireMock.Net/pull/1241) - Upgrade to Handlebars.Net.Helpers 2.4.9 [bug] contributed by [StefH](https://github.com/StefH)
- [#1227](https://github.com/wiremock/WireMock.Net/issues/1227) - unable to call grpc method with namespace [bug]
- [#1228](https://github.com/wiremock/WireMock.Net/issues/1228) - how to set datetime for grpc field [bug]
- [#1234](https://github.com/wiremock/WireMock.Net/issues/1234) - FindLogEntries regression [bug]
- [#1240](https://github.com/wiremock/WireMock.Net/issues/1240) - Method 'get_Category' in type 'WireMock.Transformers.Handlebars.FileHelpers' [bug]

# 1.6.11 (02 January 2025)
- [#1221](https://github.com/wiremock/WireMock.Net/pull/1221) - Add overloads to AtUrl and AtAbsoluteUrl which can use a IStringMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#1222](https://github.com/wiremock/WireMock.Net/pull/1222) - Fix WireMockContainerBuilder (duplicate entries) [bug] contributed by [StefH](https://github.com/StefH)
- [#1223](https://github.com/wiremock/WireMock.Net/pull/1223) - Add functionality to call a PostTransform method after the Webhook request has been transformed [feature] contributed by [StefH](https://github.com/StefH)
- [#1224](https://github.com/wiremock/WireMock.Net/pull/1224) - Add FindLogEntries to IWireMockServer [feature] contributed by [StefH](https://github.com/StefH)
- [#1092](https://github.com/wiremock/WireMock.Net/issues/1092) - FindLogEntries present in WireMockServer but not IWireMockServer [feature]
- [#1192](https://github.com/wiremock/WireMock.Net/issues/1192) - Feature: add URL assertion excluding query parameters [feature]
- [#1204](https://github.com/wiremock/WireMock.Net/issues/1204) - Supplying Values From Request for Webhook Body and Headers [feature]
- [#1217](https://github.com/wiremock/WireMock.Net/issues/1217) - Order of WireMockContainerBuilder WithX calls caused duplicate Networks in Configuration [bug]

# 1.6.10 (15 December 2024)
- [#1189](https://github.com/wiremock/WireMock.Net/pull/1189) - WireMock.Net.Testcontainers: implement watching the static mapping folder for changes [bug] contributed by [StefH](https://github.com/StefH)
- [#1188](https://github.com/wiremock/WireMock.Net/issues/1188) - WithWatchStaticMappings doesn't respect new files [bug]

# 1.6.9 (06 December 2024)
- [#1216](https://github.com/wiremock/WireMock.Net/pull/1216) - Fix JsonPartialMatcher when using property names with dot [bug] contributed by [StefH](https://github.com/StefH)
- [#1210](https://github.com/wiremock/WireMock.Net/issues/1210) - JsonPartialMatcher fails to match on property name that JsonMatcher matches [bug]

# 1.6.8 (24 November 2024)
- [#1202](https://github.com/wiremock/WireMock.Net/pull/1202) - Log exception when (static) mapping file cannot be read [feature] contributed by [StefH](https://github.com/StefH)
- [#1206](https://github.com/wiremock/WireMock.Net/pull/1206) - Fix security issues [bug] contributed by [StefH](https://github.com/StefH)
- [#1211](https://github.com/wiremock/WireMock.Net/pull/1211) - Use GraphQL 8.2.1 [feature] contributed by [StefH](https://github.com/StefH)
- [#1213](https://github.com/wiremock/WireMock.Net/pull/1213) - Fix HandlebarsContext ParseAndEvaluate method [bug] contributed by [StefH](https://github.com/StefH)
- [#1201](https://github.com/wiremock/WireMock.Net/issues/1201) - Mapping file parse errors are not logged to the console [feature]
- [#1209](https://github.com/wiremock/WireMock.Net/issues/1209) - Upgrade of GraphQL libs to the latest [feature]
- [#1212](https://github.com/wiremock/WireMock.Net/issues/1212) - Response Body Does Not Include Text After Path Segment [bug]

# 1.6.7 (17 October 2024)
- [#1161](https://github.com/wiremock/WireMock.Net/pull/1161) -  Use latest ProtoBufJsonConverter to support WellKnownTypes  [bug] contributed by [StefH](https://github.com/StefH)
- [#1190](https://github.com/wiremock/WireMock.Net/pull/1190) - Bump System.Text.Json from 8.0.4 to 8.0.5 in /examples/WireMock.Net.Console.Net472.Classic [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#1194](https://github.com/wiremock/WireMock.Net/pull/1194) - Upgrade System.Text.RegularExpressions to 4.3.1 to solve CVE-2019-0820 [bug] contributed by [StefH](https://github.com/StefH)
- [#1197](https://github.com/wiremock/WireMock.Net/pull/1197) - Bump System.Text.Json from 8.0.4 to 8.0.5 in /src/dotnet-WireMock.Net [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#1198](https://github.com/wiremock/WireMock.Net/pull/1198) - Fix Google.Protobuf.WellKnownTypes.Value [bug] contributed by [StefH](https://github.com/StefH)
- [#1144](https://github.com/wiremock/WireMock.Net/issues/1144) - Using google.protobuf.Empty as response results in a bad gRPC response [bug]
- [#1153](https://github.com/wiremock/WireMock.Net/issues/1153) - Grpc support for multiple proto files [feature]
- [#1193](https://github.com/wiremock/WireMock.Net/issues/1193) - Snyk issue : Regular Expression Denial of Service  [bug]

# 1.6.6 (01 October 2024)
- [#1185](https://github.com/wiremock/WireMock.Net/pull/1185) - Throw exception in case WithTransformer is used after WithBodyFromFile [bug] contributed by [StefH](https://github.com/StefH)
- [#1187](https://github.com/wiremock/WireMock.Net/pull/1187) - Fix StaticMappingsPath in WireMockContainerBuilder [bug] contributed by [StefH](https://github.com/StefH)
- [#1184](https://github.com/wiremock/WireMock.Net/issues/1184) - .WithBodyFromFile() + .WithTransformer(transformContentFromBodyAsFile: true) = empty string [bug]
- [#1186](https://github.com/wiremock/WireMock.Net/issues/1186) - WithMappings path is null on Build() call [bug]

# 1.6.5 (28 September 2024)
- [#1175](https://github.com/wiremock/WireMock.Net/pull/1175) - Add WireMock.Net.AspNetCore.Middleware [feature] contributed by [StefH](https://github.com/StefH)
- [#1181](https://github.com/wiremock/WireMock.Net/pull/1181) - WireMock.Net.Testcontainers: Use 'sheyenrath/wiremock.net-alpine' image as default for Linux [feature] contributed by [StefH](https://github.com/StefH)
- [#1182](https://github.com/wiremock/WireMock.Net/pull/1182) - pass in the request when no matching is found to the warn logger [feature] contributed by [JasonLandbridge](https://github.com/JasonLandbridge)

# 1.6.4 (25 September 2024)
- [#1169](https://github.com/wiremock/WireMock.Net/pull/1169) - Allow mapping without Path or Url [bug] contributed by [StefH](https://github.com/StefH)
- [#1170](https://github.com/wiremock/WireMock.Net/pull/1170) - Update the .NET Aspire tests [feature] contributed by [StefH](https://github.com/StefH)
- [#1172](https://github.com/wiremock/WireMock.Net/pull/1172) - Fix JSON parsing of text/plain content type [bug] contributed by [ruxo](https://github.com/ruxo)
- [#1177](https://github.com/wiremock/WireMock.Net/pull/1177) - Unpin Testcontainers version and upgrade to version 3.10.0 [bug] contributed by [StefH](https://github.com/StefH)
- [#1178](https://github.com/wiremock/WireMock.Net/pull/1178) - Upgrade CS-Script to version 4.8.17 [feature] contributed by [StefH](https://github.com/StefH)
- [#1179](https://github.com/wiremock/WireMock.Net/pull/1179) - Add WireMock.Net.TUnit project [feature] contributed by [StefH](https://github.com/StefH)
- [#1146](https://github.com/wiremock/WireMock.Net/issues/1146) - Bump Request CS-Script 4.8.13 to 4.8.17 [feature]
- [#1167](https://github.com/wiremock/WireMock.Net/issues/1167) - Admin API fails to create a mapping with Request Header matching using WildCardMatcher [bug]
- [#1168](https://github.com/wiremock/WireMock.Net/issues/1168) - Numbers in text/plain content is parsed as JSON. [bug]
- [#1176](https://github.com/wiremock/WireMock.Net/issues/1176) - WireMock.NET TestContainer Dependency Constraint Issue [bug]

# 1.6.3 (07 September 2024)
- [#1165](https://github.com/wiremock/WireMock.Net/pull/1165) - Fix listen on AnyIP for url 0.0.0.0 contributed by [cocoon](https://github.com/cocoon)
- [#1154](https://github.com/wiremock/WireMock.Net/issues/1154) - Listen on all ips [bug]

# 1.6.2 (04 September 2024)
- [#1152](https://github.com/wiremock/WireMock.Net/pull/1152) - Update MappingConverter to correctly write the Matcher as C# code [bug] contributed by [StefH](https://github.com/StefH)
- [#1163](https://github.com/wiremock/WireMock.Net/pull/1163) - Upgrade Aspire to version 8.2.0 [feature] contributed by [StefH](https://github.com/StefH)
- [#1166](https://github.com/wiremock/WireMock.Net/pull/1166) - Also update IWireMockMiddlewareOptions when settings are updated via admin interface [bug] contributed by [StefH](https://github.com/StefH)
- [#1151](https://github.com/wiremock/WireMock.Net/issues/1151) - MappingsToCSharpCode should use RegexMatcher when specified [bug]
- [#1164](https://github.com/wiremock/WireMock.Net/issues/1164) - WithParam not working. [bug]

# 1.6.1 (22 August 2024)
- [#1160](https://github.com/wiremock/WireMock.Net/pull/1160) - Use default timeout for Regex [bug] contributed by [StefH](https://github.com/StefH)
- [#1159](https://github.com/wiremock/WireMock.Net/issues/1159) - RegexMatchTimeoutException when trying to parse HTTP version [bug]

# 1.6.0 (16 August 2024)
- [#1042](https://github.com/wiremock/WireMock.Net/pull/1042) - Update + add fluent builder methods [feature] contributed by [StefH](https://github.com/StefH)
- [#1109](https://github.com/wiremock/WireMock.Net/pull/1109) - Add Aspire Extension [feature] contributed by [StefH](https://github.com/StefH)
- [#1148](https://github.com/wiremock/WireMock.Net/pull/1148) - Use Guid.TryParseExact with format &quot;D&quot; contributed by [StefH](https://github.com/StefH)
- [#1157](https://github.com/wiremock/WireMock.Net/pull/1157) - Fix FormUrlEncodedMatcher (MatchOperator.And) [bug] contributed by [StefH](https://github.com/StefH)
- [#1158](https://github.com/wiremock/WireMock.Net/pull/1158) - Allow setting Content-Length header on the response [feature] contributed by [StefH](https://github.com/StefH)
- [#720](https://github.com/wiremock/WireMock.Net/issues/720) - Response Header Content-Length not available when call HEAD Method [feature]
- [#1145](https://github.com/wiremock/WireMock.Net/issues/1145) - Response is auto converting string to guid [bug]
- [#1156](https://github.com/wiremock/WireMock.Net/issues/1156) - FormUrlEncodedMatcher is not requiring to match all properties when MatchOperator.And  [bug]

# 1.5.62 (27 July 2024)
- [#1147](https://github.com/wiremock/WireMock.Net/pull/1147) - Add FormUrlEncodedMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#1143](https://github.com/wiremock/WireMock.Net/issues/1143) - FormEncoded Request fails (404 Not Found) if key value pairs order in mapping is different from request body order [bug]

# 1.5.61 (22 July 2024)
- [#1122](https://github.com/wiremock/WireMock.Net/pull/1122) - Fix OpenApiPathsMapper [bug] contributed by [StefH](https://github.com/StefH)
- [#1135](https://github.com/wiremock/WireMock.Net/pull/1135) - Bump System.Text.Json from 4.7.2 to 8.0.4 in /examples/WireMock.Net.Console.Net472.Classic [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#1136](https://github.com/wiremock/WireMock.Net/pull/1136) - Bump System.Text.Json from 8.0.0 to 8.0.4 in /src/dotnet-WireMock.Net [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#1137](https://github.com/wiremock/WireMock.Net/pull/1137) - Add link to TIOBE Index on main page + fix issues [refactor] contributed by [StefH](https://github.com/StefH)
- [#1138](https://github.com/wiremock/WireMock.Net/pull/1138) - Fix some SonarCloud warnings [refactor] contributed by [StefH](https://github.com/StefH)
- [#1141](https://github.com/wiremock/WireMock.Net/pull/1141) - Update WireMockContainerBuilder.WithMappings for &quot;includeSubDirectories&quot; [feature] contributed by [StefH](https://github.com/StefH)
- [#1142](https://github.com/wiremock/WireMock.Net/pull/1142) - Make property FromConfiguredStub nullable (for WireMock.Org) [bug] contributed by [StefH](https://github.com/StefH)
- [#1118](https://github.com/wiremock/WireMock.Net/issues/1118) - Generating mappings from Payroc open-api file gives ArgumentException: Property with the same name already exists on object [bug]
- [#1139](https://github.com/wiremock/WireMock.Net/issues/1139) - Allow WithMappings to support scanning SubDirectories when building a WireMockContainer [feature]
- [#1140](https://github.com/wiremock/WireMock.Net/issues/1140) - WireMock.Org nullable properties and defaults [bug]

# 1.5.60 (09 July 2024)
- [#1128](https://github.com/wiremock/WireMock.Net/pull/1128) - Add Handlebars.Net.Helpers.Xslt [feature] contributed by [StefH](https://github.com/StefH)
- [#1130](https://github.com/wiremock/WireMock.Net/pull/1130) - Add AdminPath to WireMockServerSettings [feature] contributed by [StefH](https://github.com/StefH)
- [#1132](https://github.com/wiremock/WireMock.Net/pull/1132) - Multipart Matcher Fix [bug] contributed by [rmeshksar](https://github.com/rmeshksar)
- [#1134](https://github.com/wiremock/WireMock.Net/pull/1134) - Remove some files and folders [refactor] contributed by [StefH](https://github.com/StefH)
- [#1119](https://github.com/wiremock/WireMock.Net/issues/1119) - Error in RequestMessageMultiPartMatcher [bug]
- [#1121](https://github.com/wiremock/WireMock.Net/issues/1121) - XML transformation [feature]

# 1.5.59 (26 June 2024)
- [#1127](https://github.com/wiremock/WireMock.Net/pull/1127) - Made changes to accommodate breaking change in testcontainers-dotnet 3.9 [feature] contributed by [epDugas](https://github.com/epDugas)

# 1.5.58 (08 June 2024)
- [#1116](https://github.com/wiremock/WireMock.Net/pull/1116) - Add some methods to the BodyModelBuilder [feature] contributed by [StefH](https://github.com/StefH)
- [#1117](https://github.com/wiremock/WireMock.Net/issues/1117) - AbstractJsonPartialMatcher: Regex Value is Uppercased when IgnoreCase is set to true [bug]

# 1.5.57 (04 June 2024)
- [#1113](https://github.com/wiremock/WireMock.Net/pull/1113) - Add some Extension methods to IWireMockAdminApi [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.56 (03 June 2024)
- [#1111](https://github.com/wiremock/WireMock.Net/pull/1111) - Fix Request.Create().WithBodyAsJson(...) [bug] contributed by [StefH](https://github.com/StefH)
- [#1112](https://github.com/wiremock/WireMock.Net/pull/1112) - Add &quot;/__admin/health&quot; endpoint [feature] contributed by [StefH](https://github.com/StefH)
- [#1110](https://github.com/wiremock/WireMock.Net/issues/1110) - Connection prematurely closed BEFORE response  [bug]

# 1.5.55 (22 May 2024)
- [#1107](https://github.com/wiremock/WireMock.Net/pull/1107) - When only Port is provided, bind to * (Fixes #1100) [bug] contributed by [StefH](https://github.com/StefH)

# 1.5.54 (18 May 2024)
- [#1100](https://github.com/wiremock/WireMock.Net/pull/1100) - Add support to bind to ip-address instead of only localhost [feature] contributed by [StefH](https://github.com/StefH)
- [#1104](https://github.com/wiremock/WireMock.Net/pull/1104) - Use try..catch to set encoding in WireMockConsoleLogger [feature] contributed by [asherber](https://github.com/asherber)

# 1.5.53 (08 May 2024)
- [#1093](https://github.com/wiremock/WireMock.Net/pull/1093) - Update Handlebars.Net [feature] contributed by [StefH](https://github.com/StefH)
- [#1101](https://github.com/wiremock/WireMock.Net/pull/1101) - Fix MappingConverter to support Body with JsonMatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#1095](https://github.com/wiremock/WireMock.Net/issues/1095) - When using C# code generation WithBody() matcher is not generated for POST Request [bug]

# 1.5.52 (06 April 2024)
- [#1091](https://github.com/wiremock/WireMock.Net/pull/1091) - Add RegEx support to JsonMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#1088](https://github.com/wiremock/WireMock.Net/issues/1088) - Regex support for JsonMatcher  [feature]

# 1.5.51 (20 March 2024)
- [#1085](https://github.com/wiremock/WireMock.Net/pull/1085) - Fix FluentAssertions (actual body is not displayed in error message) [bug] contributed by [StefH](https://github.com/StefH)
- [#1084](https://github.com/wiremock/WireMock.Net/issues/1084) - FluentAssertions - Actual body is not displayed in error message when using Json Body [bug]

# 1.5.50 (12 March 2024)
- [#1080](https://github.com/wiremock/WireMock.Net/pull/1080) - Fix FluentAssertions on Header(s) [bug] contributed by [StefH](https://github.com/StefH)
- [#1082](https://github.com/wiremock/WireMock.Net/pull/1082) - Make WireMockAssertions extendable [feature] contributed by [StefH](https://github.com/StefH)
- [#1074](https://github.com/wiremock/WireMock.Net/issues/1074) - FluentAssertions extensions do not filter headers correctly [bug]
- [#1075](https://github.com/wiremock/WireMock.Net/issues/1075) - FluentAssertions extensions are not open for extension [feature]

# 1.5.49 (06 March 2024)
- [#1069](https://github.com/wiremock/WireMock.Net/pull/1069) - Extend TypeLoader [feature] contributed by [StefH](https://github.com/StefH)
- [#1078](https://github.com/wiremock/WireMock.Net/pull/1078) - Upgrade ProtoBufJsonConverter to fix issue with dot(s) in package name [bug] contributed by [StefH](https://github.com/StefH)
- [#1077](https://github.com/wiremock/WireMock.Net/issues/1077) - ProtoBufMatcher not working when proto package name contains dots [bug]

# 1.5.48 (22 February 2024)
- [#1047](https://github.com/wiremock/WireMock.Net/pull/1047) - Add Grpc ProtoBuf support (request-response) [feature] contributed by [StefH](https://github.com/StefH)
- [#1058](https://github.com/wiremock/WireMock.Net/pull/1058) - Fix some SonarCloud issues [refactor] contributed by [StefH](https://github.com/StefH)

# 1.5.47 (25 January 2024)
- [#1049](https://github.com/wiremock/WireMock.Net/pull/1049) - Add WithoutHeader to WireMock.FluentAssertions [feature] contributed by [StefH](https://github.com/StefH)
- [#1053](https://github.com/wiremock/WireMock.Net/pull/1053) - [Snyk] Security upgrade Microsoft.IdentityModel.Protocols.OpenIdConnect from 6.12.2 to 6.34.0 [security] contributed by [StefH](https://github.com/StefH)
- [#1057](https://github.com/wiremock/WireMock.Net/pull/1057) - Pin the version from Testcontainers to 3.7.0 in WireMock.Net.Testcontainers [bug] contributed by [StefH](https://github.com/StefH)
- [#1048](https://github.com/wiremock/WireMock.Net/issues/1048) - WithoutHeader fluent assertion [feature]
- [#1054](https://github.com/wiremock/WireMock.Net/issues/1054) - WireMock.Net 1.5.46 is incompatible with TestContainers 3.7.0 (issue 1) [bug]
- [#1059](https://github.com/wiremock/WireMock.Net/issues/1059) - WireMock.Net 1.5.46 is incompatible with TestContainers 3.7.0 (issue 2) [bug]

# 1.5.46 (23 December 2023)
- [#1044](https://github.com/wiremock/WireMock.Net/pull/1044) - WireMockServerSettingsParser [refactor] contributed by [StefH](https://github.com/StefH)
- [#1046](https://github.com/wiremock/WireMock.Net/pull/1046) - Change FindRequestByMappingGuidAsync to return a collection of entries contributed by [tlevesque-ueat](https://github.com/tlevesque-ueat)

# 1.5.45 (21 December 2023)
- [#1036](https://github.com/wiremock/WireMock.Net/pull/1036) - Update Handlebars Transformer logic (ReplaceNodeOptions) [feature] contributed by [StefH](https://github.com/StefH)
- [#1043](https://github.com/wiremock/WireMock.Net/pull/1043) - FindRequestByMappingGuidAsync [feature] contributed by [StefH](https://github.com/StefH)
- [#1039](https://github.com/wiremock/WireMock.Net/issues/1039) - [Admin API] Find a request that matched a given mapping [feature]

# 1.5.44 (14 December 2023)
- [#1040](https://github.com/wiremock/WireMock.Net/pull/1040) - Implement prefix for saved mapping file [feature] contributed by [MindaugasLaganeckas](https://github.com/MindaugasLaganeckas)
- [#1033](https://github.com/wiremock/WireMock.Net/issues/1033) - How to get a Random Long? [bug]
- [#1037](https://github.com/wiremock/WireMock.Net/issues/1037) - Make mapping filenames more user friendly [feature]

# 1.5.43 (11 December 2023)
- [#1026](https://github.com/wiremock/WireMock.Net/pull/1026) - Add ProxyUrlReplaceSettings to Response [feature] contributed by [StefH](https://github.com/StefH)
- [#1038](https://github.com/wiremock/WireMock.Net/pull/1038) - Proxy all requests - even a repeated one [feature] contributed by [sameena-ops](https://github.com/sameena-ops)
- [#592](https://github.com/wiremock/WireMock.Net/issues/592) - Proxy all requests - even a repeated one [feature]
- [#1024](https://github.com/wiremock/WireMock.Net/issues/1024) - Scenario with proxy not removing route prefix [feature]

# 1.5.42 (09 December 2023)
- [#1023](https://github.com/wiremock/WireMock.Net/pull/1023) - Fix Mapping[] for WireMock.Org REST API [bug] contributed by [StefH](https://github.com/StefH)
- [#1031](https://github.com/wiremock/WireMock.Net/pull/1031) - Calling Reset also resets the scenarios [bug] contributed by [StefH](https://github.com/StefH)
- [#1034](https://github.com/wiremock/WireMock.Net/pull/1034) - Workaround for: Random.Generate Type=&quot;Long&quot; [bug] contributed by [StefH](https://github.com/StefH)
- [#1021](https://github.com/wiremock/WireMock.Net/issues/1021) - GetAdminMappingsResult in WireMock.Org.Abstractions should contain list of mappings [bug]
- [#1030](https://github.com/wiremock/WireMock.Net/issues/1030) - Reset resets only mappings and logentries, not scenarios. [bug]

# 1.5.41 (04 December 2023)
- [#1012](https://github.com/wiremock/WireMock.Net/pull/1012) - GraphQL - custom scalar support [feature] contributed by [StefH](https://github.com/StefH)
- [#1018](https://github.com/wiremock/WireMock.Net/pull/1018) - Add .NET 8 [feature] contributed by [StefH](https://github.com/StefH)
- [#984](https://github.com/wiremock/WireMock.Net/issues/984) - GraphQL Schema validation with custom scalars [feature]

# 1.5.40 (07 November 2023)
- [#1011](https://github.com/wiremock/WireMock.Net/pull/1011) - GraphQL - add support for standard scalar types in the schema [feature] contributed by [StefH](https://github.com/StefH)
- [#1014](https://github.com/wiremock/WireMock.Net/pull/1014) - FluentAssertions - WithBody and WithBodyAsJson and WithBodyAsBytes contributed by [StefH](https://github.com/StefH)

# 1.5.39 (09 October 2023)
- [#1006](https://github.com/wiremock/WireMock.Net/pull/1006) - Fix RequestMessageParamMatcher : RejectOnMatch [bug] contributed by [StefH](https://github.com/StefH)
- [#997](https://github.com/wiremock/WireMock.Net/issues/997) - JmesPathMatcher or and MatchOperator working in version 1.5.34 but not 1.5.35 [bug]

# 1.5.38 (02 October 2023)
- [#1005](https://github.com/wiremock/WireMock.Net/pull/1005) - Support for xml namespaces in XPathMatcher [feature] contributed by [cal-schleupen](https://github.com/cal-schleupen)

# 1.5.37 (27 September 2023)
- [#1004](https://github.com/wiremock/WireMock.Net/pull/1004) - Fix MappingModel to map IgnoreCase and RejectOnMatch for Headers, Cookies and Parameters [bug] contributed by [StefH](https://github.com/StefH)
- [#1003](https://github.com/wiremock/WireMock.Net/issues/1003) - Store Mapping per POST request ignores &quot;IgnoreCase&quot; property of HeaderModel [bug]

# 1.5.36 (21 September 2023)
- [#986](https://github.com/wiremock/WireMock.Net/pull/986) - Write logging in case a Matcher throws an exception [feature] contributed by [StefH](https://github.com/StefH)
- [#996](https://github.com/wiremock/WireMock.Net/pull/996) - Remove dependency on Microsoft.AspNet.WebApi.Client [feature] contributed by [StefH](https://github.com/StefH)
- [#1002](https://github.com/wiremock/WireMock.Net/pull/1002) - Fixed logic for SaveUnmatchedRequests [bug] contributed by [StefH](https://github.com/StefH)
- [#974](https://github.com/wiremock/WireMock.Net/issues/974) - HttpClient extension methods causes ambiguous invocations in .NET 7 [bug]
- [#1001](https://github.com/wiremock/WireMock.Net/issues/1001) - SaveUnmatchedRequests stopped working [bug]

# 1.5.35 (19 August 2023)
- [#993](https://github.com/wiremock/WireMock.Net/pull/993) - Update JSONPathMatcher.cs to cover the string path selection to a child contributed by [DayLightDancer](https://github.com/DayLightDancer)

# 1.5.34 (04 August 2023)
- [#989](https://github.com/wiremock/WireMock.Net/pull/989) - Fix MimeKitLite NuGet include [bug] contributed by [StefH](https://github.com/StefH)
- [#988](https://github.com/wiremock/WireMock.Net/issues/988) - v1.5.33  Returns always StatusCode 500 [bug]

# 1.5.33 (03 August 2023)
- [#972](https://github.com/wiremock/WireMock.Net/pull/972) - JsonPartialMatcher - match guid and string contributed by [timurnes](https://github.com/timurnes)
- [#976](https://github.com/wiremock/WireMock.Net/pull/976) - Upgrade to Handlebars.Net.Helpers 2.4.0 to update XPath.SelectTokens and XPath.EvaluateToString [feature] contributed by [StefH](https://github.com/StefH)
- [#981](https://github.com/wiremock/WireMock.Net/pull/981) - Add MultiPart/MimePart Request Matcher [feature] contributed by [StefH](https://github.com/StefH)
- [#968](https://github.com/wiremock/WireMock.Net/issues/968) - Using request multipart in response template [feature]
- [#969](https://github.com/wiremock/WireMock.Net/issues/969) - Multipart validation [feature]
- [#970](https://github.com/wiremock/WireMock.Net/issues/970) - Loop through xml elements in handlebars template [feature]
- [#971](https://github.com/wiremock/WireMock.Net/issues/971) - JsonPartialMatcher - match guid and string [feature]

# 1.5.32 (15 July 2023)
- [#966](https://github.com/wiremock/WireMock.Net/pull/966) - Fixed JsonPathMatcher to match nested objects [bug] contributed by [StefH](https://github.com/StefH)
- [#965](https://github.com/wiremock/WireMock.Net/issues/965) - JsonPathMatcher does not match json body nested objects [bug]
- [#967](https://github.com/wiremock/WireMock.Net/issues/967) - &#11088;10 million downloads ! &#11088; [feature]

# 1.5.31 (08 July 2023)
- [#964](https://github.com/wiremock/WireMock.Net/pull/964) - Add GraphQL Schema matching [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.30 (28 June 2023)
- [#959](https://github.com/wiremock/WireMock.Net/pull/959) - Fixed logic for FluentAssertions WithHeader [bug] contributed by [StefH](https://github.com/StefH)
- [#962](https://github.com/wiremock/WireMock.Net/pull/962) - Bump System.Linq.Dynamic.Core from 1.2.23 to 1.3.0 in /examples/WireMock.Net.Console.Net472.Classic [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#963](https://github.com/wiremock/WireMock.Net/pull/963) - Bump System.Linq.Dynamic.Core from 1.2.23 to 1.3.0 in /examples/WireMock.Net.StandAlone.Net461 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#958](https://github.com/wiremock/WireMock.Net/issues/958) - [FluentAssertions] Should().HaveReceivedACall().WithHeader() only checks the first header with the matching key. [bug]

# 1.5.29 (22 June 2023)
- [#954](https://github.com/wiremock/WireMock.Net/pull/954) - Support setting WireMockServerSettings via Environment [feature] contributed by [StefH](https://github.com/StefH)
- [#955](https://github.com/wiremock/WireMock.Net/pull/955) - Fix some SonarCloud issues [refactor] contributed by [StefH](https://github.com/StefH)
- [#953](https://github.com/wiremock/WireMock.Net/issues/953) - How to use environment variable [feature]

# 1.5.28 (11 June 2023)
- [#948](https://github.com/wiremock/WireMock.Net/pull/948) - WireMock.Net.Testcontainers [feature] contributed by [StefH](https://github.com/StefH)
- [#951](https://github.com/wiremock/WireMock.Net/pull/951) - Allow setting the Content-Length header for a HTTP method HEAD [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.27 (03 June 2023)
- [#946](https://github.com/wiremock/WireMock.Net/pull/946) - Add warning logging when sending a request to a Webhook does not return status 200 [feature] contributed by [StefH](https://github.com/StefH)
- [#949](https://github.com/wiremock/WireMock.Net/pull/949) - Add &quot;.NET Framework 4.7&quot; to WireMock.Net.FluentAssertions [feature] contributed by [StefH](https://github.com/StefH)
- [#928](https://github.com/wiremock/WireMock.Net/issues/928) - TypeLoadException when using WithHeader method. [bug]
- [#945](https://github.com/wiremock/WireMock.Net/issues/945) - Webhook logging [feature]

# 1.5.26 (25 May 2023)
- [#939](https://github.com/wiremock/WireMock.Net/pull/939) - WireMockMiddleware should use HandleRequestsSynchronously correctly [bug] contributed by [StefH](https://github.com/StefH)
- [#940](https://github.com/wiremock/WireMock.Net/pull/940) - Code generator improvements contributed by [cezarypiatek](https://github.com/cezarypiatek)
- [#942](https://github.com/wiremock/WireMock.Net/pull/942) - Add GetParameter method to IRequestMessage [feature] contributed by [StefH](https://github.com/StefH)
- [#941](https://github.com/wiremock/WireMock.Net/issues/941) - RequestMessage.GetParameter method missing from IRequestMessage interface [feature]

# 1.5.25 (13 May 2023)
- [#934](https://github.com/wiremock/WireMock.Net/pull/934) - Code generator improvements [feature] contributed by [cezarypiatek](https://github.com/cezarypiatek)

# 1.5.24 (07 May 2023)
- [#926](https://github.com/wiremock/WireMock.Net/pull/926) - Fix C# mapping code generator for header names [bug] contributed by [cezarypiatek](https://github.com/cezarypiatek)
- [#927](https://github.com/wiremock/WireMock.Net/pull/927) - Enrich generated code with status code [feature] contributed by [cezarypiatek](https://github.com/cezarypiatek)
- [#930](https://github.com/wiremock/WireMock.Net/pull/930) - Update C# mapping code generator for WithStatusCode [feature] contributed by [StefH](https://github.com/StefH)
- [#931](https://github.com/wiremock/WireMock.Net/pull/931) - Add property 'IsStartedWithAdminInterface' to 'IWireMockServer' [feature] contributed by [StefH](https://github.com/StefH)
- [#933](https://github.com/wiremock/WireMock.Net/pull/933) - C# code generator improvements [feature] contributed by [cezarypiatek](https://github.com/cezarypiatek)

# 1.5.23 (23 April 2023)
- [#922](https://github.com/wiremock/WireMock.Net/pull/922) - Add WithProbability [feature] contributed by [StefH](https://github.com/StefH)
- [#924](https://github.com/wiremock/WireMock.Net/pull/924) - Allow removal of prefix when proxying to another server (#630) [feature] contributed by [nudejustin](https://github.com/nudejustin)
- [#925](https://github.com/wiremock/WireMock.Net/pull/925) - Add IgnoreCase option to ProxyUrlReplaceSettings [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.22 (08 April 2023)
- [#914](https://github.com/wiremock/WireMock.Net/pull/914) - #912 add excluded params to proxy mapping [feature] contributed by [walidhaidarii](https://github.com/walidhaidarii)
- [#916](https://github.com/wiremock/WireMock.Net/pull/916) - Include WireMockOpenApiParser project [feature] contributed by [StefH](https://github.com/StefH)
- [#912](https://github.com/wiremock/WireMock.Net/issues/912) - Feature: adding excluded params to proxy and records settings [feature]

# 1.5.21 (22 March 2023)
- [#908](https://github.com/wiremock/WireMock.Net/pull/908) - RequestBuilder : add WithBodyAsJson and WithBody (with IJsonConverter) [feature] contributed by [StefH](https://github.com/StefH)
- [#911](https://github.com/wiremock/WireMock.Net/pull/911) - Fixed QueryStringParser for UrlEncoded values [bug] contributed by [StefH](https://github.com/StefH)
- [#901](https://github.com/wiremock/WireMock.Net/issues/901) - Matching one form-urlencoded value [feature]

# 1.5.20 (19 March 2023)
- [#905](https://github.com/wiremock/WireMock.Net/pull/905) - Add DeserializeFormUrl Encoded to the settings [feature] contributed by [StefH](https://github.com/StefH)
- [#907](https://github.com/wiremock/WireMock.Net/pull/907) - Fix issue with application/x-www-form-urlencoded and ExactMatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#906](https://github.com/wiremock/WireMock.Net/issues/906) - Upgrade to 1.5.19 breaks a form data test [bug]

# 1.5.19 (17 March 2023)
- [#903](https://github.com/wiremock/WireMock.Net/pull/903) - Add WithBody with IDictionary (form-urlencoded values) [feature] contributed by [StefH](https://github.com/StefH)
- [#904](https://github.com/wiremock/WireMock.Net/pull/904) - Update Handlebars.Net.Helpers to 2.3.15 [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.18 (09 March 2023)
- [#893](https://github.com/wiremock/WireMock.Net/pull/893) - Add 'Data' to response which can be used during transforming the response [feature] contributed by [StefH](https://github.com/StefH)
- [#896](https://github.com/wiremock/WireMock.Net/pull/896) - Bump Microsoft.Owin from 2.0.2 to 4.2.2 in /examples/WireMock.Net.Service [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#900](https://github.com/wiremock/WireMock.Net/pull/900) - ProxySettings : Add logic to not save some requests depending on HttpMethods  [feature] contributed by [StefH](https://github.com/StefH)
- [#897](https://github.com/wiremock/WireMock.Net/issues/897) - WebHostBuilder.ConfigureServices method not found when using nunit3testadapter 4.4.0 [bug]
- [#899](https://github.com/wiremock/WireMock.Net/issues/899) - Ignore OPTIONS request when using proxyandrecord [feature]

# 1.5.17 (25 February 2023)
- [#881](https://github.com/wiremock/WireMock.Net/pull/881) - Add WithBodyAsJson builder method with accepts a Func [feature] contributed by [StefH](https://github.com/StefH)
- [#890](https://github.com/wiremock/WireMock.Net/pull/890) - AdminApiMappingBuilder [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.16 (01 February 2023)
- [#880](https://github.com/wiremock/WireMock.Net/pull/880) - Add `WithProxy(string proxyUrl, X509Certificate2 certificate)` [feature] contributed by [StefH](https://github.com/StefH)
- [#879](https://github.com/wiremock/WireMock.Net/issues/879) - Possibility to pass a X509Certificate2 to WithProxy() or specifiy certificate loading options [feature]

# 1.5.15 (29 January 2023)
- [#878](https://github.com/wiremock/WireMock.Net/pull/878) - Update REST Admin interface to support &quot;Get Mapping(s) as C# Code&quot; [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.14 (24 January 2023)
- [#842](https://github.com/wiremock/WireMock.Net/pull/842) - Generate C# code from Mapping [feature] contributed by [StefH](https://github.com/StefH)
- [#869](https://github.com/wiremock/WireMock.Net/pull/869) - Add MappingBuilder to build mappings in code and export to Models or JSON [feature] contributed by [StefH](https://github.com/StefH)
- [#871](https://github.com/wiremock/WireMock.Net/pull/871) - Add UseWebhooksFireAndForget to Server ConvertMapping [bug] contributed by [ggradnig](https://github.com/ggradnig)
- [#872](https://github.com/wiremock/WireMock.Net/pull/872) - Fix unsubscribe from LogEntriesChanged event handler [bug] contributed by [StefH](https://github.com/StefH)
- [#875](https://github.com/wiremock/WireMock.Net/pull/875) - Fix Self referencing loop detected for property [bug] contributed by [eseneckiy](https://github.com/eseneckiy)
- [#701](https://github.com/wiremock/WireMock.Net/issues/701) - Allow to create MappingModel from c# to be able to configure local and remote mocks similarly [feature]
- [#867](https://github.com/wiremock/WireMock.Net/issues/867) - Can I build mappings with code and save them to JSON-file without starting server [feature]
- [#870](https://github.com/wiremock/WireMock.Net/issues/870) - Can not unsubscribe from LogEntriesChanged event. [bug]

# 1.5.13 (11 December 2022)
- [#858](https://github.com/wiremock/WireMock.Net/pull/858) - Update Transformer functionality to return value instead of string [feature] contributed by [StefH](https://github.com/StefH)
- [#859](https://github.com/wiremock/WireMock.Net/pull/859) - Add UpdatedAt property to Mapping [feature] contributed by [StefH](https://github.com/StefH)
- [#862](https://github.com/wiremock/WireMock.Net/pull/862) - Add client certificate support [feature] contributed by [billybraga](https://github.com/billybraga)
- [#863](https://github.com/wiremock/WireMock.Net/pull/863) - Update WireMockServer.CreateClient/CreateClients to include handlers [feature] contributed by [StefH](https://github.com/StefH)
- [#856](https://github.com/wiremock/WireMock.Net/issues/856) - Inconsistent result with overlapping (duplicate) request [bug]

# 1.5.12 (03 December 2022)
- [#851](https://github.com/wiremock/WireMock.Net/pull/851) - Fix Linux CI build + Fix opencover [feature] contributed by [StefH](https://github.com/StefH)
- [#853](https://github.com/wiremock/WireMock.Net/pull/853) - Add .Net 7 [feature] contributed by [StefH](https://github.com/StefH)
- [#854](https://github.com/wiremock/WireMock.Net/pull/854) - Fix logic for QueryParameterMultipleValueSupport [bug] contributed by [StefH](https://github.com/StefH)
- [#857](https://github.com/wiremock/WireMock.Net/pull/857) - Update some dependencies [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.11 (24 November 2022)
- [#836](https://github.com/wiremock/WireMock.Net/pull/836) - Add Settings.QueryParameterMultipleValueSupport [feature] contributed by [StefH](https://github.com/StefH)
- [#848](https://github.com/wiremock/WireMock.Net/pull/848) - Use try-catch when adding or removing logEntry [bug] contributed by [StefH](https://github.com/StefH)
- [#846](https://github.com/wiremock/WireMock.Net/issues/846) - Exception ArgumentOutOfRangeException [bug]

# 1.5.10 (06 November 2022)
- [#843](https://github.com/wiremock/WireMock.Net/pull/843) - Webhook Templating: Use the transformed URL to create the HttpRequestMessage contributed by [ggradnig](https://github.com/ggradnig)
- [#845](https://github.com/wiremock/WireMock.Net/pull/845) - Add WireMockNullLogger as valid commandline logger option [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.9 (29 October 2022)
- [#828](https://github.com/wiremock/WireMock.Net/pull/828) - Add setting to skip saving the string-response in the logging when using WithBody(Func...) [feature] contributed by [StefH](https://github.com/StefH)
- [#832](https://github.com/wiremock/WireMock.Net/pull/832) - Fixes for WireMock.Net.FluentAssertions (callcount behaviour) [feature] contributed by [StefH](https://github.com/StefH)
- [#834](https://github.com/wiremock/WireMock.Net/pull/834) - Support deleting / resetting a single scenario [feature] contributed by [StefH](https://github.com/StefH)
- [#837](https://github.com/wiremock/WireMock.Net/pull/837) - Bump Microsoft.AspNetCore.Server.Kestrel.Core from 2.1.7 to 2.1.25 in /examples/WireMock.Net.StandAlone.Net461 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#838](https://github.com/wiremock/WireMock.Net/pull/838) - Add option to ProxySettings to append guid to mapping file contributed by [StefH](https://github.com/StefH)
- [#826](https://github.com/wiremock/WireMock.Net/issues/826) - Dynamic Body not to be cached when a Func is used to created the body [feature]

# 1.5.8 (16 October 2022)
- [#816](https://github.com/wiremock/WireMock.Net/pull/816) - Some fixes to WireMock.Net.Assertions [feature] contributed by [StefH](https://github.com/StefH)
- [#817](https://github.com/wiremock/WireMock.Net/pull/817) - ExactMatcher : IgnoreCase [feature] contributed by [StefH](https://github.com/StefH)
- [#824](https://github.com/wiremock/WireMock.Net/pull/824) - WebHook - Transform Url [feature] contributed by [StefH](https://github.com/StefH)
- [#814](https://github.com/wiremock/WireMock.Net/issues/814) - WithHeader cannot handle multiple requests with the same header key values [bug]
- [#815](https://github.com/wiremock/WireMock.Net/issues/815) - Why does UsingMethod check _callscount? [bug]
- [#822](https://github.com/wiremock/WireMock.Net/issues/822) - Webhook with generic url, body and custom header values [feature]

# 1.5.7 (11 October 2022)
- [#818](https://github.com/wiremock/WireMock.Net/pull/818) - Add option to run the server on http &amp; https [feature] contributed by [StefH](https://github.com/StefH)
- [#821](https://github.com/wiremock/WireMock.Net/pull/821) - Add UseDefinedRequestMatchers to ProxyAndRecordSettings [feature] contributed by [StefH](https://github.com/StefH)
- [#823](https://github.com/wiremock/WireMock.Net/pull/823) - Add implicit operators to WireMockList contributed by [StefH](https://github.com/StefH)
- [#819](https://github.com/wiremock/WireMock.Net/issues/819) - Can I preserve Mapping title and matchers for proxy response? [feature]

# 1.5.6 (12 September 2022)
- [#803](https://github.com/wiremock/WireMock.Net/pull/803) - WebHook : UseFireAndForget + Delay [feature] contributed by [StefH](https://github.com/StefH)
- [#801](https://github.com/wiremock/WireMock.Net/issues/801) - Webhook Delays [feature]

# 1.5.5 (03 September 2022)
- [#798](https://github.com/wiremock/WireMock.Net/pull/798) - Add support to use 'mapping' object in in reponse templating [feature] contributed by [StefH](https://github.com/StefH)
- [#800](https://github.com/wiremock/WireMock.Net/pull/800) - Bump Microsoft.Owin from 4.1.1 to 4.2.2 in /src/WireMock.Net (net46) [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#802](https://github.com/wiremock/WireMock.Net/pull/802) - Add assertions for request methods contributed by [rafaelmfonseca](https://github.com/rafaelmfonseca)
- [#772](https://github.com/wiremock/WireMock.Net/issues/772) - How to get matched mapping by HttpRequest or HttpRequestMessage [feature]

# 1.5.4 (24 August 2022)
- [#778](https://github.com/wiremock/WireMock.Net/pull/778) - Fix Proxying when StartAdminInterface=true [bug] contributed by [StefH](https://github.com/StefH)
- [#781](https://github.com/wiremock/WireMock.Net/pull/781) - Update some NuGet packages [feature] contributed by [StefH](https://github.com/StefH)
- [#783](https://github.com/wiremock/WireMock.Net/pull/783) - Fix WithBody when using Pact and added more nullable annotations [feature] contributed by [StefH](https://github.com/StefH)
- [#787](https://github.com/wiremock/WireMock.Net/pull/787) - Add support for PEM certificates contributed by [StefH](https://github.com/StefH)
- [#789](https://github.com/wiremock/WireMock.Net/pull/789) - Add support for Matcher.Pattern in Pact Body mapping [feature] contributed by [StefH](https://github.com/StefH)
- [#790](https://github.com/wiremock/WireMock.Net/pull/790) - Add Response.WithBody with IJsonConverter [feature] contributed by [StefH](https://github.com/StefH)
- [#795](https://github.com/wiremock/WireMock.Net/pull/795) - Add check for duplicate Guids when posting multiple mappings in one request contributed by [StefH](https://github.com/StefH)
- [#797](https://github.com/wiremock/WireMock.Net/pull/797) - Fix WithHeader when using RejectOnMatch [bug] contributed by [flts](https://github.com/flts)
- [#775](https://github.com/wiremock/WireMock.Net/issues/775) - When &quot;StartAdminInterface&quot; is true then each time is generated new mapping from the proxy [bug]
- [#784](https://github.com/wiremock/WireMock.Net/issues/784) - Response body is missing in generated pact file when IBodyResponseBuilder.WithBody is used [bug]
- [#785](https://github.com/wiremock/WireMock.Net/issues/785) - Support for PEM certificates when using ssl [feature]
- [#788](https://github.com/wiremock/WireMock.Net/issues/788) - Request body is missing in generated pact file for requests that include matching on request body [bug]
- [#796](https://github.com/wiremock/WireMock.Net/issues/796) - RequestMessageHeaderMatcher with MatchBehaviour.RejectOnMatch reverses match results twice [bug]

# 1.5.3 (29 July 2022)
- [#777](https://github.com/wiremock/WireMock.Net/pull/777) - Update Scriban.Signed to version 5.5.0 [feature] contributed by [StefH](https://github.com/StefH)
- [#776](https://github.com/wiremock/WireMock.Net/issues/776) - Update Scriban.Signed to support more functions, e.g math.random [feature]

# 1.5.2 (24 July 2022)
- [#769](https://github.com/wiremock/WireMock.Net/pull/769) - Bump Microsoft.AspNetCore.Server.Kestrel.Core from 2.1.3 to 2.1.7 in /examples/WireMock.Net.StandAlone.Net461 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#771](https://github.com/wiremock/WireMock.Net/pull/771) - JsonPartialMatcher - support Regex [feature] contributed by [StefH](https://github.com/StefH)

# 1.5.1 (08 July 2022)
- [#762](https://github.com/wiremock/WireMock.Net/pull/762) - Bump Newtonsoft.Json from 11.0.2 to 13.0.1 in /examples/WireMock.Net.WebApplication.NETCore2 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#763](https://github.com/wiremock/WireMock.Net/pull/763) - Bump Newtonsoft.Json from 6.0.1 to 13.0.1 in /examples/WireMock.Net.Client.Net472 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#765](https://github.com/wiremock/WireMock.Net/pull/765) - Update WireMock.Org.Abstractions and WireMock.Org.RestClient [feature] contributed by [StefH](https://github.com/StefH)
- [#766](https://github.com/wiremock/WireMock.Net/pull/766) - Bump Microsoft.AspNetCore.Http from 2.1.1 to 2.1.22 in /examples/WireMock.Net.StandAlone.Net461 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#767](https://github.com/wiremock/WireMock.Net/pull/767) - Rename (WireMock.Pact.Models.V2)-Request to PactRequest and -Response to PactResponse [feature] contributed by [StefH](https://github.com/StefH)
- [#764](https://github.com/wiremock/WireMock.Net/issues/764) - Wrong mapping of method GetAdminMappingsAsync from IWireMockOrgApi [bug]

# 1.5.0 (09 June 2022)
- [#755](https://github.com/wiremock/WireMock.Net/pull/755) - Add MatchOperator &quot;Or&quot;, &quot;And&quot; and &quot;Average&quot; for patterns [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.43 (21 May 2022)
- [#757](https://github.com/wiremock/WireMock.Net/pull/757) - Log correct exception message when handling aggregate exceptions contributed by [siewers](https://github.com/siewers)
- [#759](https://github.com/wiremock/WireMock.Net/pull/759) - Add WireMock.Net.xUnit project [feature] contributed by [StefH](https://github.com/StefH)
- [#756](https://github.com/wiremock/WireMock.Net/issues/756) - WireMockConsoleLogger aggregate exception handling bug? [bug]
- [#758](https://github.com/wiremock/WireMock.Net/issues/758) - Add support for logging to an xUnit ITestOutputHelper [feature]

# 1.4.42 (13 May 2022)
- [#748](https://github.com/wiremock/WireMock.Net/pull/748) - Initial support for converting the mappings to a Pact(flow) json file [feature] contributed by [StefH](https://github.com/StefH)
- [#749](https://github.com/wiremock/WireMock.Net/pull/749) - Swagger support [feature] contributed by [StefH](https://github.com/StefH)
- [#750](https://github.com/wiremock/WireMock.Net/pull/750) - [Snyk] Security upgrade Newtonsoft.Json from 11.0.2 to 13.0.1 contributed by [snyk-bot](https://github.com/snyk-bot)
- [#751](https://github.com/wiremock/WireMock.Net/pull/751) - Update NuGets packages [feature] contributed by [StefH](https://github.com/StefH)
- [#741](https://github.com/wiremock/WireMock.Net/issues/741) - Integrate with Pact [feature]
- [#753](https://github.com/wiremock/WireMock.Net/issues/753) - FluentAssertions - assert the server has not received a call  [feature]

# 1.4.41 (21 April 2022)
- [#746](https://github.com/wiremock/WireMock.Net/pull/746) - Allow Timeout.InfiniteTimeSpan for WithDelay [feature] contributed by [StefH](https://github.com/StefH)
- [#747](https://github.com/wiremock/WireMock.Net/pull/747) - Update the logic for ProxyAndRecord contributed by [StefH](https://github.com/StefH)
- [#744](https://github.com/wiremock/WireMock.Net/issues/744) - System.ArgumentOutOfRangeException when Timeout.InfiniteTimeSpan used as an argument for IResponseBuilder.WithDelay() [bug]

# 1.4.40 (26 March 2022)
- [#740](https://github.com/wiremock/WireMock.Net/pull/740) - Add Port and Url property to WireMockServer + upgrade System.Linq.Dynamic.Core [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.39 (25 March 2022)
- [#739](https://github.com/wiremock/WireMock.Net/pull/739) - Upgrade NuGet for RandomDataGenerator.Net to 1.0.14 contributed by [StefH](https://github.com/StefH)

# 1.4.38 (11 March 2022)
- [#736](https://github.com/wiremock/WireMock.Net/pull/736) - Remove interface for all Setting classes [feature] contributed by [StefH](https://github.com/StefH)
- [#737](https://github.com/wiremock/WireMock.Net/pull/737) - Add WireMock.Net.WebApplication.NET6 example contributed by [StefH](https://github.com/StefH)

# 1.4.37 (02 March 2022)
- [#730](https://github.com/wiremock/WireMock.Net/pull/730) - Fixed bug &quot;dotnet nuget push -n&quot; [bug] contributed by [StefH](https://github.com/StefH)
- [#732](https://github.com/wiremock/WireMock.Net/pull/732) - Make X509CertificatePassword optional [feature] contributed by [StefH](https://github.com/StefH)
- [#733](https://github.com/wiremock/WireMock.Net/pull/733) - Fix FileSystemWatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#726](https://github.com/wiremock/WireMock.Net/issues/726) - Wiremock -  WatchStaticMappings only works until the first request is made  [bug]

# 1.4.36 (25 February 2022)
- [#728](https://github.com/wiremock/WireMock.Net/pull/728) - Update NuGet packages [feature] contributed by [StefH](https://github.com/StefH)
- [#729](https://github.com/wiremock/WireMock.Net/pull/729) - BodyAsFile should use BodyAsFileIsCached value [bug] contributed by [StefH](https://github.com/StefH)

# 1.4.35 (09 February 2022)
- [#722](https://github.com/wiremock/WireMock.Net/pull/722) - Fixed 'Response BodyAsJson with JArray does not work' [bug] contributed by [StefH](https://github.com/StefH)
- [#721](https://github.com/wiremock/WireMock.Net/issues/721) - Response BodyAsJson with array does not work [bug]

# 1.4.34 (27 January 2022)
- [#716](https://github.com/wiremock/WireMock.Net/pull/716) - MatcherMapper : Always use Pattern [bug] contributed by [StefH](https://github.com/StefH)
- [#715](https://github.com/wiremock/WireMock.Net/issues/715) - Record request mapping outputs JsonMatcher with Patterns instead of Pattern [bug]

# 1.4.33 (24 January 2022)
- [#714](https://github.com/wiremock/WireMock.Net/pull/714) - Add support for Cors [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.32 (17 January 2022)
- [#713](https://github.com/wiremock/WireMock.Net/pull/713) - Added support of custom matchers in static mappings contributed by [levanoz](https://github.com/levanoz)

# 1.4.31 (06 January 2022)
- [#706](https://github.com/wiremock/WireMock.Net/pull/706) - Provide open api schema to dynamic examples generator so you can generate accurate data [feature] contributed by [brunotarghetta](https://github.com/brunotarghetta)
- [#707](https://github.com/wiremock/WireMock.Net/pull/707) - Use NuGet &quot;Stef.Validation&quot; [feature] contributed by [StefH](https://github.com/StefH)
- [#710](https://github.com/wiremock/WireMock.Net/pull/710) - Add ReplaceNodeOption flag [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.30 (25 December 2021)
- [#703](https://github.com/wiremock/WireMock.Net/pull/703) - SaveUnmatchedRequests [feature] contributed by [StefH](https://github.com/StefH)
- [#704](https://github.com/wiremock/WireMock.Net/pull/704) - Add .ConfigureAwait(false); to the await Task calls [bug] contributed by [StefH](https://github.com/StefH)
- [#534](https://github.com/wiremock/WireMock.Net/issues/534) - Mock server not answer if integrated in Xamarin UITest project [bug]
- [#567](https://github.com/wiremock/WireMock.Net/issues/567) - Can't start WireMock.Net server in Xamarin.UITest project (.NET Framework 4.7.2) on MacOS [bug]
- [#685](https://github.com/wiremock/WireMock.Net/issues/685) - GuidWildcardMatcher to match on GUIDs [feature]

# 1.4.29 (12 December 2021)
- [#699](https://github.com/wiremock/WireMock.Net/pull/699) - GUID Pattern support in RegexMatcher contributed by [brogdogg](https://github.com/brogdogg)
- [#700](https://github.com/wiremock/WireMock.Net/pull/700) - RegexExtended in settings [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.28 (01 December 2021)
- [#686](https://github.com/wiremock/WireMock.Net/pull/686) - [Snyk] Security upgrade Microsoft.Owin from 4.0.0 to 4.1.1 [dependencies] contributed by [snyk-bot](https://github.com/snyk-bot)
- [#688](https://github.com/wiremock/WireMock.Net/pull/688) - Bump System.Text.Encodings.Web from 4.5.0 to 4.5.1 in /examples/WireMock.Net.Console.Net472.Classic [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#689](https://github.com/wiremock/WireMock.Net/pull/689) - Upgrade some NuGet's (Codecov, coverlet, Moq and NFluent) [dependencies] contributed by [StefH](https://github.com/StefH)
- [#691](https://github.com/wiremock/WireMock.Net/pull/691) - Update the OpenApiPathsMapper to handle Value/Wildcard [feature] contributed by [StefH](https://github.com/StefH)
- [#694](https://github.com/wiremock/WireMock.Net/pull/694) - RamlToOpenAPI updated to 0.5.0 [feature] contributed by [mcheguini](https://github.com/mcheguini)
- [#695](https://github.com/wiremock/WireMock.Net/pull/695) - Allow configure IgnoreCase in settings [feature] contributed by [leolplex](https://github.com/leolplex)
- [#696](https://github.com/wiremock/WireMock.Net/pull/696) - Filter required property in headers, query params, request body [feature] contributed by [leolplex](https://github.com/leolplex)
- [#666](https://github.com/wiremock/WireMock.Net/issues/666) - Example is not working as expected [bug]
- [#692](https://github.com/wiremock/WireMock.Net/issues/692) - Case insensitive and ignoring optional path and header parameters in OpenApiPathsMapper [feature]

# 1.4.27 (17 November 2021)
- [#678](https://github.com/wiremock/WireMock.Net/pull/678) - Support RequestBody [feature] contributed by [leolplex](https://github.com/leolplex)
- [#680](https://github.com/wiremock/WireMock.Net/pull/680) - Support examples in properties [feature] contributed by [leolplex](https://github.com/leolplex)
- [#681](https://github.com/wiremock/WireMock.Net/pull/681) - Support enums in properties [feature] contributed by [leolplex](https://github.com/leolplex)

# 1.4.26 (04 November 2021)
- [#670](https://github.com/wiremock/WireMock.Net/pull/670) - Improve method MapSchemaToObject to support array and object [feature] contributed by [leolplex](https://github.com/leolplex)
- [#673](https://github.com/wiremock/WireMock.Net/pull/673) - Support examples random data generation contributed by [leolplex](https://github.com/leolplex)
- [#675](https://github.com/wiremock/WireMock.Net/pull/675) - Support basepath from servers contributed by [leolplex](https://github.com/leolplex)
- [#676](https://github.com/wiremock/WireMock.Net/pull/676) - Fix random generate data in url no spaces [feature] contributed by [leolplex](https://github.com/leolplex)

# 1.4.25 (27 October 2021)
- [#661](https://github.com/wiremock/WireMock.Net/pull/661) - Add TimeSettings (Start, End and TTL) [feature] contributed by [StefH](https://github.com/StefH)
- [#664](https://github.com/wiremock/WireMock.Net/pull/664) - Support Array in OpenApiParser [feature] contributed by [leolplex](https://github.com/leolplex)
- [#667](https://github.com/wiremock/WireMock.Net/pull/667) - Add JsonPartialWildcardMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#669](https://github.com/wiremock/WireMock.Net/pull/669) - Support Schema Example and Support AllOf in definitions  [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.24 (20 October 2021)
- [#637](https://github.com/wiremock/WireMock.Net/pull/637) - Add support for AzureAD authentication for REST admin interface [feature] contributed by [StefH](https://github.com/StefH)
- [#643](https://github.com/wiremock/WireMock.Net/pull/643) - Support edge case: first object, next an array. [feature] contributed by [leolplex](https://github.com/leolplex)
- [#644](https://github.com/wiremock/WireMock.Net/pull/644) - Mapping headers in OpenAPI [feature] contributed by [leolplex](https://github.com/leolplex)
- [#649](https://github.com/wiremock/WireMock.Net/pull/649) - Refactor method name MapHeaders and httpStatusCode  contributed by [leolplex](https://github.com/leolplex)
- [#651](https://github.com/wiremock/WireMock.Net/pull/651) - Implement PatternAsFile for StringMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#654](https://github.com/wiremock/WireMock.Net/pull/654) - Update NotNullOrEmptyMatcher to also implement IStringMatcher [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.23 (27 September 2021)
- [#635](https://github.com/wiremock/WireMock.Net/pull/635) - WireMock.Net.FluentAssertions : upgrade to latest FluentAssertions [feature] contributed by [StefH](https://github.com/StefH)
- [#634](https://github.com/wiremock/WireMock.Net/issues/634) - Upgrade to latest FluentAssertions [bug]

# 1.4.22 (22 September 2021)
- [#633](https://github.com/wiremock/WireMock.Net/pull/633) - Implement Random Delay [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.21 (16 September 2021)
- [#631](https://github.com/wiremock/WireMock.Net/pull/631) - Add WireMock.org RestClient [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.20 (06 August 2021)
- [#628](https://github.com/wiremock/WireMock.Net/pull/628) - Fix issue with FluentBuilder [bug] contributed by [StefH](https://github.com/StefH)
- [#626](https://github.com/wiremock/WireMock.Net/issues/626) - version 1.4.19 throws a lot of analyzer errors related to the BaseBuilder.cs [bug]

# 1.4.19 (04 August 2021)
- [#622](https://github.com/wiremock/WireMock.Net/pull/622) - Add FluentBuilder for client models [feature] contributed by [StefH](https://github.com/StefH)
- [#625](https://github.com/wiremock/WireMock.Net/pull/625) - Add NotNullOrEmptyMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#621](https://github.com/wiremock/WireMock.Net/issues/621) - Fluent API for RestClient MappingModel creation [feature]
- [#624](https://github.com/wiremock/WireMock.Net/issues/624) - Post request with &quot;BodyAsBytes&quot; is not matched by RegexMatcher [bug]

# 1.4.18 (10 July 2021)
- [#619](https://github.com/wiremock/WireMock.Net/pull/619) - Update Handlebars.Net.Helpers.XPath to fix issue with 'xml version' contributed by [StefH](https://github.com/StefH)
- [#618](https://github.com/wiremock/WireMock.Net/issues/618) - Trying to use attribute of the request object while creating response while mocking a soap service [bug]

# 1.4.17 (07 July 2021)
- [#617](https://github.com/wiremock/WireMock.Net/pull/617) - Add support for Handlebars.Net.Helpers.Humanizer [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.16 (05 June 2021)
- [#616](https://github.com/wiremock/WireMock.Net/pull/616) - Upgrade Handlebars.Net.Helpers to 2.19 [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.15 (19 May 2021)
- [#615](https://github.com/wiremock/WireMock.Net/pull/615) - Add support for multiple webhooks [feature] contributed by [StefH](https://github.com/StefH)
- [#614](https://github.com/wiremock/WireMock.Net/issues/614) - Is it possible to some how send multiple webhooks? [feature]

# 1.4.14 (11 May 2021)
- [#610](https://github.com/wiremock/WireMock.Net/pull/610) - Fix some SonarCloud issues in UnitTests contributed by [StefH](https://github.com/StefH)
- [#611](https://github.com/wiremock/WireMock.Net/pull/611) - Allow to add custom service registrations when using ASP.NET Core [feature] contributed by [starkpl](https://github.com/starkpl)
- [#612](https://github.com/wiremock/WireMock.Net/pull/612) - Don't run SonarCloud tasks for PullRequests [feature] contributed by [StefH](https://github.com/StefH)

# 1.4.13 (26 April 2021)
- [#607](https://github.com/wiremock/WireMock.Net/pull/607) - Bump System.Text.Encodings.Web from 4.5.0 to 4.5.1 in /examples/WireMock.Net.StandAlone.Net461 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#609](https://github.com/wiremock/WireMock.Net/pull/609) - Add possibility to use settings to generate MappingModel models with wildcard path parameters. [feature] contributed by [StefH](https://github.com/StefH)
- [#608](https://github.com/wiremock/WireMock.Net/issues/608) - Import from OpenApi generates model with path parameter narrowed in range (example value=42 instead of '*') [feature]

# 1.4.12 (22 April 2021)
- [#605](https://github.com/wiremock/WireMock.Net/pull/605) - Bump System.Net.Http from 4.3.3 to 4.3.4 in /src/WireMock.Net [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#606](https://github.com/wiremock/WireMock.Net/pull/606) - Bump System.Net.Http from 4.3.3 to 4.3.4 in /examples/WireMock.Net.Service [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)

# 1.4.11 (18 April 2021)
- [#604](https://github.com/wiremock/WireMock.Net/pull/604) - Fix match logic for exact bytearray contributed by [StefH](https://github.com/StefH)
- [#601](https://github.com/wiremock/WireMock.Net/issues/601) - Exact byte array request matching fails on specific byte arrays [bug]

# 1.4.10 (15 April 2021)
- [#603](https://github.com/wiremock/WireMock.Net/pull/603) - Fix callback with Headers [bug] contributed by [StefH](https://github.com/StefH)
- [#602](https://github.com/wiremock/WireMock.Net/issues/602) - Header not being returned when set in WithCallback [bug]

# 1.4.9 (31 March 2021)
- [#600](https://github.com/wiremock/WireMock.Net/pull/600) - WithProxy() should save the new mapping [bug] contributed by [StefH](https://github.com/StefH)

# 1.4.8 (24 March 2021)
- [#591](https://github.com/wiremock/WireMock.Net/pull/591) - Webhook [feature] contributed by [StefH](https://github.com/StefH)
- [#589](https://github.com/wiremock/WireMock.Net/issues/589) - How to send a request to a specific URL after sending response [feature]

# 1.4.7 (21 March 2021)
- [#594](https://github.com/wiremock/WireMock.Net/pull/594) - Add possibility to the WithBody() to use IBodyData [feature] contributed by [StefH](https://github.com/StefH)
- [#595](https://github.com/wiremock/WireMock.Net/pull/595) - Use Handlebars.Net.Helpers Version=&quot;2.1.2&quot; [feature] contributed by [StefH](https://github.com/StefH)
- [#597](https://github.com/wiremock/WireMock.Net/pull/597) - Remove 2 second delay from first response and add IPv6 address support [bug, feature] contributed by [benagain](https://github.com/benagain)

# 1.4.6 (26 February 2021)
- [#587](https://github.com/wiremock/WireMock.Net/pull/587) - Fix WithCallback logic when using other fluent builder statements [bug] contributed by [StefH](https://github.com/StefH)
- [#569](https://github.com/wiremock/WireMock.Net/issues/569) - WithCallback circumvent the rest of the builder [bug]

# 1.4.5 (11 February 2021)
- [#585](https://github.com/wiremock/WireMock.Net/pull/585) - Fix response date header [bug] contributed by [wolf8196](https://github.com/wolf8196)

# 1.4.4 (09 February 2021)
- [#581](https://github.com/wiremock/WireMock.Net/pull/581) - Use new Handlebars.Net.Helpers [feature] contributed by [StefH](https://github.com/StefH)
- [#582](https://github.com/wiremock/WireMock.Net/pull/582) - Add Xamarin UI tests [feature] contributed by [StefH](https://github.com/StefH)
- [#568](https://github.com/wiremock/WireMock.Net/issues/568) - [Question] Dates in response templates [feature]

# 1.4.3 (05 February 2021)
- [#570](https://github.com/wiremock/WireMock.Net/pull/570) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.StandAlone.NETCoreApp [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#571](https://github.com/wiremock/WireMock.Net/pull/571) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.Console.NETCoreApp2 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#572](https://github.com/wiremock/WireMock.Net/pull/572) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.Console.NETCoreApp [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#573](https://github.com/wiremock/WireMock.Net/pull/573) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.Console.Net461.Classic [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#574](https://github.com/wiremock/WireMock.Net/pull/574) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.Console.Net452.Classic [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#575](https://github.com/wiremock/WireMock.Net/pull/575) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.StandAlone.Net452 [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#576](https://github.com/wiremock/WireMock.Net/pull/576) - Bump log4net from 2.0.8 to 2.0.10 in /examples/WireMock.Net.Service [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#579](https://github.com/wiremock/WireMock.Net/pull/579) - Net5 issue [bug] contributed by [StefH](https://github.com/StefH)
- [#577](https://github.com/wiremock/WireMock.Net/issues/577) - WireMock.Net will not run with certain .net5 dependencies installed in the project [bug]

# 1.4.2 (24 January 2021)
- [#566](https://github.com/wiremock/WireMock.Net/pull/566) - Do not save Mappings when SaveMappingForStatusCodePattern does not match [bug] contributed by [StefH](https://github.com/StefH)
- [#565](https://github.com/wiremock/WireMock.Net/issues/565) - NullReferenceException [bug]

# 1.4.1 (19 January 2021)
- [#562](https://github.com/wiremock/WireMock.Net/pull/562) - Refactor Transformer (add Scriban) [feature] contributed by [StefH](https://github.com/StefH)
- [#214](https://github.com/wiremock/WireMock.Net/issues/214) - Feature: Add support for template language DotLiquid [feature]

# 1.4.0 (12 January 2021)
- [#548](https://github.com/wiremock/WireMock.Net/pull/548) - Move CSharpCodeMatcher to a new project [feature] contributed by [StefH](https://github.com/StefH)

# 1.3.10 (23 December 2020)
- [#555](https://github.com/wiremock/WireMock.Net/pull/555) - Add more tests for Proxy with Authorization [feature] contributed by [StefH](https://github.com/StefH)
- [#561](https://github.com/wiremock/WireMock.Net/pull/561) - Do not save &quot;admin&quot; mappings when running in Proxy - mode contributed by [StefH](https://github.com/StefH)
- [#559](https://github.com/wiremock/WireMock.Net/issues/559) - WireMock Setting 'SaveMappingToFile' raising cast object to type error [bug]

# 1.3.9 (08 December 2020)
- [#550](https://github.com/wiremock/WireMock.Net/pull/550) - WithProxy(...) also use all proxy settings [bug] contributed by [StefH](https://github.com/StefH)
- [#551](https://github.com/wiremock/WireMock.Net/pull/551) - Add obsolete warning: CSharpCodeMatcher will be moved to a separate NuGet package 'WireMock.Net.Matchers.CSharpCode' [feature] contributed by [StefH](https://github.com/StefH)
- [#549](https://github.com/wiremock/WireMock.Net/issues/549) - WithProxy(...) does not save the mappings to file [bug]

# 1.3.8 (03 December 2020)
- [#539](https://github.com/wiremock/WireMock.Net/pull/539) - Support for partial JSON matching contributed by [gleb-osokin](https://github.com/gleb-osokin)
- [#542](https://github.com/wiremock/WireMock.Net/pull/542) - Create dotnet-wiremock tool [feature] contributed by [StefH](https://github.com/StefH)
- [#543](https://github.com/wiremock/WireMock.Net/pull/543) - Add support for .NET 5 [feature] contributed by [StefH](https://github.com/StefH)
- [#544](https://github.com/wiremock/WireMock.Net/pull/544) - Use Java 11 in Azure Pipelines (needed for SonarCloud) [feature] contributed by [StefH](https://github.com/StefH)
- [#545](https://github.com/wiremock/WireMock.Net/pull/545) - Fix SonarCloud OpenCover (coverlet-coverage) [bug] contributed by [StefH](https://github.com/StefH)
- [#547](https://github.com/wiremock/WireMock.Net/pull/547) - Fix Proxying with SSL and NetCoreApp3.1 [bug] contributed by [StefH](https://github.com/StefH)
- [#524](https://github.com/wiremock/WireMock.Net/issues/524) - Proxying with SSL Not Working in .NET Core 3.1 [bug]

# 1.3.6 (10 November 2020)
- [#529](https://github.com/wiremock/WireMock.Net/pull/529) - Add assertions for ClientIP, Url and ProxyUrl [feature] contributed by [akamud](https://github.com/akamud)
- [#535](https://github.com/wiremock/WireMock.Net/pull/535) - WithCallback should use also use enum HttpStatusCode [bug] contributed by [StefH](https://github.com/StefH)
- [#537](https://github.com/wiremock/WireMock.Net/pull/537) - Add Custom Certificate settings [feature] contributed by [StefH](https://github.com/StefH)
- [#533](https://github.com/wiremock/WireMock.Net/issues/533) - Stubbed response with only callback returns unexpected status code. [bug]
- [#536](https://github.com/wiremock/WireMock.Net/issues/536) - Overriding the default ssl certificate via file. [feature]

# 1.3.5 (04 November 2020)
- [#530](https://github.com/wiremock/WireMock.Net/pull/530) - Fix dotnet-sonarscanner [bug] contributed by [StefH](https://github.com/StefH)
- [#531](https://github.com/wiremock/WireMock.Net/pull/531) - Add WithCallback-Async [feature] contributed by [StefH](https://github.com/StefH)

# 1.3.4 (17 October 2020)
- [#522](https://github.com/wiremock/WireMock.Net/pull/522) - Add ContinuousIntegrationBuild property [feature] contributed by [StefH](https://github.com/StefH)
- [#525](https://github.com/wiremock/WireMock.Net/pull/525) - Handlebars.Net.Helpers Version=&quot;1.1.0&quot; [feature] contributed by [StefH](https://github.com/StefH)

# 1.3.3 (15 October 2020)
- [#520](https://github.com/wiremock/WireMock.Net/pull/520) - Make kestrel limits configurable contributed by [eduherminio](https://github.com/eduherminio)
- [#521](https://github.com/wiremock/WireMock.Net/issues/521) - Make Kestrel limits configurable [feature]

# 1.3.2 (14 October 2020)
- [#505](https://github.com/wiremock/WireMock.Net/pull/505) - Fix reading JsonMatcher-mapping with object as pattern [bug] contributed by [StefH](https://github.com/StefH)
- [#514](https://github.com/wiremock/WireMock.Net/pull/514) - Update .NET Core 3.1 example contributed by [Crossbow78](https://github.com/Crossbow78)
- [#504](https://github.com/wiremock/WireMock.Net/issues/504) - Loading mapping models with `JsonMatcher`  is not working correctly [bug]
- [#513](https://github.com/wiremock/WireMock.Net/issues/513) - Static mapping break from 1.2.17 to 1.2.18 and higher [bug]

# 1.3.1 (30 September 2020)
- [#509](https://github.com/wiremock/WireMock.Net/pull/509) - Adding netcoreapp3.1 as a target framework [feature] contributed by [APIWT](https://github.com/APIWT)

# 1.3.0 (29 September 2020)
- [#508](https://github.com/wiremock/WireMock.Net/pull/508) - Fix vulnerability in NuGet dependencies contributed by [StefH](https://github.com/StefH)
- [#327](https://github.com/wiremock/WireMock.Net/issues/327) - Index must be within the bounds of the List - Bug [bug]
- [#507](https://github.com/wiremock/WireMock.Net/issues/507) - Fix vulnerability found in Microsoft.AspNetCore dependency [feature]

# 1.2.18 (13 August 2020)
- [#496](https://github.com/wiremock/WireMock.Net/pull/496) - Add setting to handle requests synchronously [feature] contributed by [StefH](https://github.com/StefH)
- [#500](https://github.com/wiremock/WireMock.Net/pull/500) - Add ThrowExceptionWhenMatcherFails option to all Matchers [feature] contributed by [StefH](https://github.com/StefH)
- [#478](https://github.com/wiremock/WireMock.Net/issues/478) - Sometimes returns status code 0 in unit tests with xunit test fixture (flaky test) [bug]

# 1.2.17 (01 August 2020)
- [#495](https://github.com/wiremock/WireMock.Net/pull/495) - Scenario : stay on current state for a number of times contributed by [StefH](https://github.com/StefH)
- [#494](https://github.com/wiremock/WireMock.Net/issues/494) - Stay in Current State for specified number of requests [feature]

# 1.2.16 (27 July 2020)
- [#492](https://github.com/wiremock/WireMock.Net/pull/492) - Mark FluentMockServer, FluentMockServerSettings, BlacklistedHeaders and BlacklistedCookies as obsolete [feature] contributed by [StefH](https://github.com/StefH)
- [#489](https://github.com/wiremock/WireMock.Net/issues/489) - Change &quot;blacklist&quot; and &quot;whitelist&quot; terms [feature]

# 1.2.14 (09 July 2020)
- [#479](https://github.com/wiremock/WireMock.Net/pull/479) - An OpenApi (swagger) parser to generate MappingModel or mapping.json file [feature] contributed by [StefH](https://github.com/StefH)
- [#482](https://github.com/wiremock/WireMock.Net/pull/482) - Add PartialMatch to logging / logentries [feature] contributed by [StefH](https://github.com/StefH)
- [#483](https://github.com/wiremock/WireMock.Net/pull/483) - Bring in the WireMock.Net.FluentAssertions tests contributed by [akamud](https://github.com/akamud)
- [#484](https://github.com/wiremock/WireMock.Net/pull/484) - Refactor: extract interfaces [feature] contributed by [StefH](https://github.com/StefH)
- [#487](https://github.com/wiremock/WireMock.Net/pull/487) - Fixed MappingConverter when methods are null [bug] contributed by [StefH](https://github.com/StefH)
- [#486](https://github.com/wiremock/WireMock.Net/issues/486) - Admin API fails to create a mapping with Request Body matching [bug]

# 1.2.13 (24 May 2020)
- [#475](https://github.com/wiremock/WireMock.Net/pull/475) - Fix Limits.KeepAliveTimeout &amp; Limits.RequestHeadersTimeout [bug] contributed by [StefH](https://github.com/StefH)
- [#474](https://github.com/wiremock/WireMock.Net/issues/474) - Performance issue with multiple httpclients (since version 1.2.10) [bug]

# 1.2.12 (23 May 2020)
- [#472](https://github.com/wiremock/WireMock.Net/pull/472) - Create new .sln contributed by [StefH](https://github.com/StefH)
- [#473](https://github.com/wiremock/WireMock.Net/pull/473) - Fixed Proxy when using MultipartForm with byte[] [bug] contributed by [StefH](https://github.com/StefH)
- [#468](https://github.com/wiremock/WireMock.Net/issues/468) - Proxy mode: Incorrect handling of multipart requests [bug]

# 1.2.11.0 (18 May 2020)
- [#469](https://github.com/wiremock/WireMock.Net/pull/469) - Fix unhandled exception when target is unavailable [bug] contributed by [StefH](https://github.com/StefH)
- [#467](https://github.com/wiremock/WireMock.Net/issues/467) - Proxy mode: Unhandled exception when target is not working [bug]

# 1.2.10 (17 May 2020)
- [#456](https://github.com/wiremock/WireMock.Net/pull/456) - Include Handlebars.Net.Helpers project [feature] contributed by [StefH](https://github.com/StefH)
- [#457](https://github.com/wiremock/WireMock.Net/pull/457) - Kestrel Options Limits [bug] contributed by [StefH](https://github.com/StefH)
- [#455](https://github.com/wiremock/WireMock.Net/issues/455) - There is no option to increase body size while proxying [bug]

# 1.2.9.0 (14 May 2020)
- [#465](https://github.com/wiremock/WireMock.Net/pull/465) - Fix method ResetMappingsAsync in the RestEase-AdminApi [bug] contributed by [StefH](https://github.com/StefH)
- [#464](https://github.com/wiremock/WireMock.Net/issues/464) - RestClient Admin API Metadata Base Path Duplication [bug]

# 1.2.8.0 (04 May 2020)
- [#463](https://github.com/wiremock/WireMock.Net/pull/463) - GH161: Fix SourceLink support [bug] contributed by [gitfool](https://github.com/gitfool)

# 1.2.7.0 (30 April 2020)
- [#461](https://github.com/wiremock/WireMock.Net/pull/461) - Support Path in ProxyUrl contributed by [StefH](https://github.com/StefH)
- [#459](https://github.com/wiremock/WireMock.Net/issues/459) - When respond with proxy requestMessage.Url is used, not AbsoluteUrl [bug]

# 1.2.6.0 (29 April 2020)
- [#460](https://github.com/wiremock/WireMock.Net/pull/460) - When using ResponseMessageTransformer : keep BodyEncoding [bug] contributed by [StefH](https://github.com/StefH)
- [#458](https://github.com/wiremock/WireMock.Net/issues/458) - Response BodyAsString loses BodyData.Encoding when UseTransformer = true [bug]

# 1.2.5.0 (17 April 2020)
- [#454](https://github.com/wiremock/WireMock.Net/pull/454) - Fix port = 0 for net452 [bug] contributed by [StefH](https://github.com/StefH)
- [#453](https://github.com/wiremock/WireMock.Net/issues/453) - MockServer not starting  [bug]

# 1.2.4.0 (10 April 2020)
- [#439](https://github.com/wiremock/WireMock.Net/pull/439) - Add support for GZip and Deflate [feature] contributed by [StefH](https://github.com/StefH)
- [#444](https://github.com/wiremock/WireMock.Net/pull/444) - Add readme.md + license from mock4net [feature] contributed by [StefH](https://github.com/StefH)
- [#451](https://github.com/wiremock/WireMock.Net/pull/451) - Update NuGet dependencies (e.g. coverage related) to fix CI-build [feature] contributed by [StefH](https://github.com/StefH)
- [#452](https://github.com/wiremock/WireMock.Net/pull/452) - Add ValidatedNotNullAttribute (for SonarQube) [refactor] contributed by [StefH](https://github.com/StefH)
- [#426](https://github.com/wiremock/WireMock.Net/issues/426) - Add support for compressed requests, such as GZIP or DEFLATE [feature]

# 1.2.3.0 (01 April 2020)
- [#449](https://github.com/wiremock/WireMock.Net/pull/449) - Netstandard21 [feature] contributed by [StefH](https://github.com/StefH)
- [#447](https://github.com/wiremock/WireMock.Net/issues/447) - Add support for .NET Standard 2.1 / .NET Core 3.1 [feature]
- [#448](https://github.com/wiremock/WireMock.Net/issues/448) - WireMock.Net is not compatible with Microsoft.VisualStudio.Web.CodeGeneration.Design 3.1.1 [bug]

# 1.2.2.0 (25 March 2020)
- [#446](https://github.com/wiremock/WireMock.Net/pull/446) - When port is provided: WireMockServer still takes a random port [bug] contributed by [StefH](https://github.com/StefH)
- [#445](https://github.com/wiremock/WireMock.Net/issues/445) - Port where WireMockServer listens to - v1.2x [bug]

# 1.2.1.0 (17 March 2020)
- [#442](https://github.com/wiremock/WireMock.Net/pull/442) - Fix Null body in handlebars transformation [bug] contributed by [StefH](https://github.com/StefH)

# 1.2.0.0 (14 March 2020)
- [#417](https://github.com/wiremock/WireMock.Net/pull/417) - Let the .NET core/standard WebHostBuilder use a random port [bug] contributed by [StefH](https://github.com/StefH)
- [#422](https://github.com/wiremock/WireMock.Net/pull/422) - AllowOnlyDefinedHttpStatusCodeInResponse [bug] contributed by [StefH](https://github.com/StefH)
- [#379](https://github.com/wiremock/WireMock.Net/issues/379) - Trusting the self signed certificate to enable SSL on dotnet core [bug]
- [#420](https://github.com/wiremock/WireMock.Net/issues/420) - Updating to 1.1.6+ breaks tests because new AllowAnyHttpStatusCodeInResponse option defaults to false [bug]

# 1.1.10 (05 March 2020)
- [#427](https://github.com/wiremock/WireMock.Net/pull/427) - Add UsingOptions, UsingConnect and UsingTrace [feature] contributed by [StefH](https://github.com/StefH)
- [#434](https://github.com/wiremock/WireMock.Net/pull/434) - Option to disable JSON deserialization [feature] contributed by [sebastianmattar](https://github.com/sebastianmattar)
- [#435](https://github.com/wiremock/WireMock.Net/pull/435) - Also call HandlebarsRegistrationCallback when using WithCallback(..) [feature] contributed by [StefH](https://github.com/StefH)
- [#408](https://github.com/wiremock/WireMock.Net/issues/408) - Intermittent threading errors with FindLogEntries [bug]
- [#433](https://github.com/wiremock/WireMock.Net/issues/433) - HandlebarsRegistrationCallback not fired [feature]

# 1.1.9.0 (25 February 2020)
- [#431](https://github.com/wiremock/WireMock.Net/pull/431) - Fix LinqMatcher for JSON int64 [bug] contributed by [StefH](https://github.com/StefH)
- [#425](https://github.com/wiremock/WireMock.Net/issues/425) - Allow 64 bit numbers in JSON [bug]

# 1.1.8.0 (22 February 2020)
- [#419](https://github.com/wiremock/WireMock.Net/pull/419) - Support multi line wild card matching [bug] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#421](https://github.com/wiremock/WireMock.Net/pull/421) - Fix: do not return empty matchers array when Func has been used [bug] contributed by [StefH](https://github.com/StefH)
- [#423](https://github.com/wiremock/WireMock.Net/pull/423) - Fixes for Cookie and Header Reject on Match [bug] contributed by [StefH](https://github.com/StefH)
- [#424](https://github.com/wiremock/WireMock.Net/pull/424) - Don't return empty dictionary object for response headers in JSON mapping [feature] contributed by [StefH](https://github.com/StefH)
- [#418](https://github.com/wiremock/WireMock.Net/issues/418) - Body matching fails if body has newline [bug]

# 1.1.7.0 (06 February 2020)
- [#409](https://github.com/wiremock/WireMock.Net/pull/409) - Admin Delete with mappings in body [feature] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#411](https://github.com/wiremock/WireMock.Net/pull/411) - Improved relative path checking based on file existence [feature] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#413](https://github.com/wiremock/WireMock.Net/pull/413) - Fix new Delete with body missing from IWireMockAdminApi interface contributed by [NoahLerner](https://github.com/NoahLerner)
- [#414](https://github.com/wiremock/WireMock.Net/pull/414) - Fix logger in StandAlone [bug] contributed by [StefH](https://github.com/StefH)
- [#412](https://github.com/wiremock/WireMock.Net/issues/412) - WireMock Standalone - null reference exception since settings.Logger [bug]

# 1.1.6.0 (27 January 2020)
- [#407](https://github.com/wiremock/WireMock.Net/pull/407) - AllowAnyHttpStatusCodeInResponse [feature] contributed by [StefH](https://github.com/StefH)

# 1.1.5.0 (25 January 2020)
- [#405](https://github.com/wiremock/WireMock.Net/pull/405) - Fix logging an Exception Message (linux docker on azure) [bug] contributed by [StefH](https://github.com/StefH)
- [#406](https://github.com/wiremock/WireMock.Net/pull/406) - Fixed StatusCode = null or &lt; 0 [bug] contributed by [StefH](https://github.com/StefH)

# 1.1.3.0 (22 January 2020)
- [#403](https://github.com/wiremock/WireMock.Net/pull/403) - Fix for invalid cast exception contributed by [kashifsoofi](https://github.com/kashifsoofi)
- [#402](https://github.com/wiremock/WireMock.Net/issues/402) - Invalid Cast Exception  [bug]

# 1.1.2.0 (09 January 2020)
- [#399](https://github.com/wiremock/WireMock.Net/pull/399) - ResponseModel.StatusCode is deserialized as either string or long. [bug] contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#400](https://github.com/wiremock/WireMock.Net/issues/400) - StatusCode not built correctly when loaded from mapping file. [bug]

# 1.1.1.0 (09 January 2020)
- [#398](https://github.com/wiremock/WireMock.Net/pull/398) - Feature/xpath transformer [feature] contributed by [kashifsoofi](https://github.com/kashifsoofi)
- [#397](https://github.com/wiremock/WireMock.Net/issues/397) - Question/Feature: Add support for selecting XPath in response template [feature]

# 1.1.0.0 (27 December 2019)
- [#363](https://github.com/wiremock/WireMock.Net/pull/363) - WireMock.Net version 1.1.x contributed by [StefH](https://github.com/StefH)

# 1.0.43.0 (26 December 2019)
- [#385](https://github.com/wiremock/WireMock.Net/pull/385) - StatusCode as string [feature] contributed by [StefH](https://github.com/StefH)
- [#380](https://github.com/wiremock/WireMock.Net/issues/380) - StatusCode is defined as integer (string is not possible) [bug]
- [#382](https://github.com/wiremock/WireMock.Net/issues/382) - Return same request body [feature]

# 1.0.42.0 (15 December 2019)
- [#391](https://github.com/wiremock/WireMock.Net/pull/391) - Correctly support DateTime pattern as string in ExactMatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#383](https://github.com/wiremock/WireMock.Net/issues/383) - ExactMatcher does not accept ISO8601 DateTime? [bug]

# 1.0.41.0 (14 December 2019)
- [#392](https://github.com/wiremock/WireMock.Net/pull/392) - Fix array in JsonMatcher [bug] contributed by [StefH](https://github.com/StefH)
- [#390](https://github.com/wiremock/WireMock.Net/issues/390) - JsonMatcher does not match a body containing an array of strings [bug]

# 1.0.40.0 (09 December 2019)
- [#389](https://github.com/wiremock/WireMock.Net/pull/389) - Fix QueryStringParser [bug] contributed by [StefH](https://github.com/StefH)
- [#387](https://github.com/wiremock/WireMock.Net/issues/387) - Query string parameter value which contains %26 does not work with ExactMatcher [bug]

# 1.0.39.0 (07 December 2019)
- [#370](https://github.com/wiremock/WireMock.Net/pull/370) - Add WebProxySettings (use when proxying requests) [feature] contributed by [StefH](https://github.com/StefH)
- [#388](https://github.com/wiremock/WireMock.Net/pull/388) - Transform body as file [bug] contributed by [StefH](https://github.com/StefH)
- [#369](https://github.com/wiremock/WireMock.Net/issues/369) - Question: Is there a way to provide a corporate proxy configuration? [feature]
- [#375](https://github.com/wiremock/WireMock.Net/issues/375) - Proxying does not follow redirects : make this configurable [feature]
- [#386](https://github.com/wiremock/WireMock.Net/issues/386) - Is transforming contents of XML file supported.? [bug]

# 1.0.38.0 (30 November 2019)
- [#376](https://github.com/wiremock/WireMock.Net/pull/376) - Support int values for states and scenario naming [feature] contributed by [NoahLerner](https://github.com/NoahLerner)
- [#378](https://github.com/wiremock/WireMock.Net/pull/378) - Set handlebars dependency for .net 4.5.1 to fixed value [bug] contributed by [StefH](https://github.com/StefH)
- [#381](https://github.com/wiremock/WireMock.Net/pull/381) - Use dotnet default development certificate for .NET Core 2.x [feature] contributed by [StefH](https://github.com/StefH)
- [#377](https://github.com/wiremock/WireMock.Net/issues/377) - Unable to build against .NET 4.5.1 because of Handlebars [bug]

# 1.0.37.0 (08 November 2019)
- [#373](https://github.com/wiremock/WireMock.Net/pull/373) - Make Sonar and WhiteSource optional in the Azure pipelines build [feature] contributed by [StefH](https://github.com/StefH)
- [#374](https://github.com/wiremock/WireMock.Net/pull/374) - WatchStaticMappingsInSubdirectories [feature] contributed by [StefH](https://github.com/StefH)
- [#372](https://github.com/wiremock/WireMock.Net/issues/372) - Reset in WireMock admin API not working fine. [feature]

# 1.0.36.0 (26 October 2019)
- [#360](https://github.com/wiremock/WireMock.Net/pull/360) - Add support for Faults [feature] contributed by [StefH](https://github.com/StefH)
- [#343](https://github.com/wiremock/WireMock.Net/issues/343) - Feature: Please provide support for Bad responses. [feature]

# 1.0.35.0 (25 October 2019)
- [#367](https://github.com/wiremock/WireMock.Net/pull/367) - No symbol NuGets [feature] contributed by [StefH](https://github.com/StefH)
- [#368](https://github.com/wiremock/WireMock.Net/pull/368) - Remove Obsolete annotations [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.34.0 (22 October 2019)
- [#354](https://github.com/wiremock/WireMock.Net/pull/354) - AllowBodyForAllHttpMethods [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#365](https://github.com/wiremock/WireMock.Net/pull/365) - Bump Microsoft.AspNetCore.All from 2.0.8 to 2.0.9 in /examples/WireMock.Net.WebApplication [dependencies] contributed by [dependabot[bot]](https://github.com/apps/dependabot)
- [#366](https://github.com/wiremock/WireMock.Net/pull/366) - Update ObsoleteAnnotations [feature] contributed by [StefH](https://github.com/StefH)
- [#352](https://github.com/wiremock/WireMock.Net/issues/352) - DELETE request drops the body [feature]

# 1.0.33.0 (12 October 2019)
- [#311](https://github.com/wiremock/WireMock.Net/pull/311) - fix jsonpath matcher [bug] contributed by [StefH](https://github.com/StefH)
- [#324](https://github.com/wiremock/WireMock.Net/pull/324) - Add CSharpCodeMatcher [feature] contributed by [StefH](https://github.com/StefH)
- [#353](https://github.com/wiremock/WireMock.Net/pull/353) - Fixed failing admin requests when content type includes a charset (based on idea from Paul Roub) [bug] contributed by [StefH](https://github.com/StefH)
- [#355](https://github.com/wiremock/WireMock.Net/pull/355) - Add Try-Catch to the event LogEntriesChanged [feature] contributed by [StefH](https://github.com/StefH)
- [#357](https://github.com/wiremock/WireMock.Net/pull/357) - Add Proxy Setting for: SaveMappingForStatusCodePattern to only save the mapping when the status code matches the pattern [feature] contributed by [StefH](https://github.com/StefH)
- [#358](https://github.com/wiremock/WireMock.Net/pull/358) - Fix JsonMatcher (parsing DateTimeOffset) contributed by [StefH](https://github.com/StefH)
- [#306](https://github.com/wiremock/WireMock.Net/issues/306) - Writing to the response body is invalid for responses with status code 204 [bug]
- [#307](https://github.com/wiremock/WireMock.Net/issues/307) - JsonPathMatcher always convert to JArray before matching [bug]
- [#329](https://github.com/wiremock/WireMock.Net/issues/329) - Feature: Add support for CSharpCodeMatcher [feature]
- [#350](https://github.com/wiremock/WireMock.Net/issues/350) - Admin requests fail when content type includes a charset [bug]
- [#356](https://github.com/wiremock/WireMock.Net/issues/356) - JsonMatcher not working when JSON contains a DateTimeOffset [bug]

# 1.0.32.0 (20 September 2019)
- [#348](https://github.com/wiremock/WireMock.Net/pull/348) - When posting new mapping, use DateParseHandling.None [bug] contributed by [StefH](https://github.com/StefH)
- [#347](https://github.com/wiremock/WireMock.Net/issues/347) - Query string match on DateTimeOffset is not working [bug]

# 1.0.31.0 (19 September 2019)
- [#334](https://github.com/wiremock/WireMock.Net/pull/334) - Fix issues with Proxy mode and Binary Request Bodies [bug] contributed by [andi0b](https://github.com/andi0b)
- [#338](https://github.com/wiremock/WireMock.Net/pull/338) - Fix ContentType with parameters in Proxy Mode [bug] contributed by [StefH](https://github.com/StefH)
- [#339](https://github.com/wiremock/WireMock.Net/pull/339) - Fix ConcurrentObservableCollection [bug] contributed by [StefH](https://github.com/StefH)
- [#345](https://github.com/wiremock/WireMock.Net/pull/345) - Fix CompareTo in RequestMatchResult [bug] contributed by [StefH](https://github.com/StefH)
- [#346](https://github.com/wiremock/WireMock.Net/pull/346) - Fix recorded requests skipped by request logger contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#337](https://github.com/wiremock/WireMock.Net/issues/337) - Proxy Missing header Content-Type - tried with Recording [bug]
- [#344](https://github.com/wiremock/WireMock.Net/issues/344) - Mapping adding order matters for multiple mappings? [bug]

# 1.0.29.0 (29 August 2019)
- [#328](https://github.com/wiremock/WireMock.Net/pull/328) - Fix LogRequest : Index Out Of Bounds [bug] contributed by [StefH](https://github.com/StefH)
- [#331](https://github.com/wiremock/WireMock.Net/pull/331) - Fix: Collection was modified exception [bug] contributed by [theramis](https://github.com/theramis)
- [#333](https://github.com/wiremock/WireMock.Net/pull/333) - JsonMatcher support IgnoreCase [feature] contributed by [StefH](https://github.com/StefH)
- [#332](https://github.com/wiremock/WireMock.Net/issues/332) - Case sensitive true is ignored for JsonMatcher [feature]

# 1.0.28.0 (20 August 2019)
- [#309](https://github.com/wiremock/WireMock.Net/pull/309) - Fix LogEntries: collection was modified exception [bug] contributed by [StefH](https://github.com/StefH)
- [#314](https://github.com/wiremock/WireMock.Net/pull/314) - RequestLogExpirationDuration : use DateTime.UtcNow [bug] contributed by [StefH](https://github.com/StefH)
- [#316](https://github.com/wiremock/WireMock.Net/pull/316) - Handles case where parameter value contains == [feature] contributed by [lobsteropteryx](https://github.com/lobsteropteryx)
- [#317](https://github.com/wiremock/WireMock.Net/pull/317) - Make SaveMapping and SaveMappingToFile settings independent. [feature] contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#319](https://github.com/wiremock/WireMock.Net/pull/319) - Add blacklist for Request Cookies. contributed by [vitaliydavydiak](https://github.com/vitaliydavydiak)
- [#322](https://github.com/wiremock/WireMock.Net/pull/322) - Fix MappingMatcher in case of an exception in LinqMatcher. [bug] contributed by [StefH](https://github.com/StefH)
- [#323](https://github.com/wiremock/WireMock.Net/pull/323) - Refactor MappingConverter &amp; MatcherMapper [refactor] contributed by [StefH](https://github.com/StefH)
- [#326](https://github.com/wiremock/WireMock.Net/pull/326) - Fix Parsing Guid in PUT Mapping [bug] contributed by [StefH](https://github.com/StefH)
- [#252](https://github.com/wiremock/WireMock.Net/issues/252) - Proxy with Transform
- [#308](https://github.com/wiremock/WireMock.Net/issues/308) - __admin/requests - &quot;Collection was modified&quot; exception [bug]
- [#313](https://github.com/wiremock/WireMock.Net/issues/313) - RequestLogExpirationDuration - bug [bug]
- [#325](https://github.com/wiremock/WireMock.Net/issues/325) - Admin API: PUT Mapping, FormatException because of wrong parsing of the Query  [bug]

# 1.0.25.0 (23 July 2019)
- [#304](https://github.com/wiremock/WireMock.Net/pull/304) - Support WithBody with multiple matchers [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.24.0 (22 July 2019)
- [#302](https://github.com/wiremock/WireMock.Net/pull/302) - Fixed bug 301 by not setting BodyAsFile to null after first use [bug] contributed by [rwwilden](https://github.com/rwwilden)
- [#301](https://github.com/wiremock/WireMock.Net/issues/301) - Error thrown when calling mocked endpoint second time when using file-based response body [bug]

# 1.0.23.0 (16 July 2019)
- [#298](https://github.com/wiremock/WireMock.Net/pull/298) - MappingModels [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.22.0 (15 July 2019)
- [#297](https://github.com/wiremock/WireMock.Net/pull/297) - FixNullRef (#295) contributed by [StefH](https://github.com/StefH)
- [#295](https://github.com/wiremock/WireMock.Net/issues/295) - NullRef in 1.0.21 [bug]

# 1.0.21.0 (03 July 2019)
- [#286](https://github.com/wiremock/WireMock.Net/pull/286) - Handlebars Extension [feature] contributed by [StefH](https://github.com/StefH)
- [#293](https://github.com/wiremock/WireMock.Net/pull/293) - workaround for AppContext.BaseDirectory being null on some platforms contributed by [eli-darkly](https://github.com/eli-darkly)
- [#294](https://github.com/wiremock/WireMock.Net/pull/294) - don't strip request body if we don't recognize the request method contributed by [eli-darkly](https://github.com/eli-darkly)
- [#289](https://github.com/wiremock/WireMock.Net/issues/289) - Bug:  When WatchStaticMappings=true throws exceptions on updating the mapping files [bug]
- [#290](https://github.com/wiremock/WireMock.Net/issues/290) - Request body is dropped if verb is REPORT [bug]
- [#292](https://github.com/wiremock/WireMock.Net/issues/292) - Can't start server in Xamarin Android [bug]

# 1.0.20.0 (17 June 2019)
- [#284](https://github.com/wiremock/WireMock.Net/pull/284) - Add SaveToFile in the mapping [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.19.0 (15 June 2019)
- [#283](https://github.com/wiremock/WireMock.Net/issues/283) - Support equal-sign in query [bug]

# 1.0.18.0 (10 June 2019)
- [#282](https://github.com/wiremock/WireMock.Net/pull/282) - WireMock.Net.Standalone : Add --WireMockLogger commandline argument [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.17.0 (05 June 2019)
- [#278](https://github.com/wiremock/WireMock.Net/pull/278) - Add support for HandleBars File (to read a file) [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.16.0 (16 May 2019)
- [#274](https://github.com/wiremock/WireMock.Net/pull/274) - Sign Assembly [feature] contributed by [StefH](https://github.com/StefH)
- [#160](https://github.com/wiremock/WireMock.Net/issues/160) - Feature: Sign 'WireMock.Net' [feature]
- [#267](https://github.com/wiremock/WireMock.Net/issues/267) - Assembly does not have strong name

# 1.0.15.0 (04 May 2019)
- [#271](https://github.com/wiremock/WireMock.Net/pull/271) - Support Dynamic response files using Handlebars templating [bug, feature] contributed by [StefH](https://github.com/StefH)
- [#273](https://github.com/wiremock/WireMock.Net/pull/273) - Dynamic response handlebars templating (2) [bug, feature] contributed by [StefH](https://github.com/StefH)

# 1.0.14.0 (20 April 2019)
- [#269](https://github.com/wiremock/WireMock.Net/pull/269) - Add JmesPath matcher [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.13.0 (11 April 2019)
- [#266](https://github.com/wiremock/WireMock.Net/pull/266) - [265] Add file upload to allow mocking of file operations contributed by [JackCreativeCrew](https://github.com/JackCreativeCrew)
- [#265](https://github.com/wiremock/WireMock.Net/issues/265) - File Upload [feature]

# 1.0.12.0 (05 April 2019)
- [#264](https://github.com/wiremock/WireMock.Net/pull/264) - Proxy : also save multipart as string in mapping file contributed by [StefH](https://github.com/StefH)
- [#263](https://github.com/wiremock/WireMock.Net/issues/263) - Content-Type multipart/form-data is not serialized in proxy and recording mode [bug]

# 1.0.11.0 (30 March 2019)
- [#261](https://github.com/wiremock/WireMock.Net/pull/261) - Fix BodyAsJson transform bug in ResponseMessageTransformer [bug] contributed by [ghost](https://github.com/ghost)

# 1.0.10.0 (27 March 2019)
- [#260](https://github.com/wiremock/WireMock.Net/pull/260) - Fix Response.Delay property serialization [bug] contributed by [StefH](https://github.com/StefH)

# 1.0.9.0 (25 March 2019)
- [#256](https://github.com/wiremock/WireMock.Net/pull/256) - Fixed Multi Param Match logic contributed by [StefH](https://github.com/StefH)
- [#255](https://github.com/wiremock/WireMock.Net/issues/255) - ExactMatcher with array pattern not working? [bug]

# 1.0.8.0 (12 March 2019)
- [#254](https://github.com/wiremock/WireMock.Net/pull/254) - RequestMessageParamMatcher supports Ignore Case for the key [feature] contributed by [StefH](https://github.com/StefH)
- [#253](https://github.com/wiremock/WireMock.Net/issues/253) - Request Path and query parameter keys are case-sensitive

# 1.0.7.0 (19 January 2019)
- [#244](https://github.com/wiremock/WireMock.Net/pull/244) - Fix BodyAsFile to also allow relative paths [feature] contributed by [StefH](https://github.com/StefH)
- [#240](https://github.com/wiremock/WireMock.Net/issues/240) - How to submit mappings for multiple request, responses [feature]
- [#243](https://github.com/wiremock/WireMock.Net/issues/243) - Not able to read response from file [bug]

# 1.0.6.1 (10 January 2019)
- [#247](https://github.com/wiremock/WireMock.Net/pull/247) - Issue 225 improve logging in example for wire mock as windows service contributed by [paulssn](https://github.com/paulssn)
- [#249](https://github.com/wiremock/WireMock.Net/pull/249) - Fixed &quot;Content-Type multipart/form-data&quot; [bug] contributed by [StefH](https://github.com/StefH)
- [#225](https://github.com/wiremock/WireMock.Net/issues/225) - Feature: Improve logging in example for WireMock as Windows Service [feature]
- [#248](https://github.com/wiremock/WireMock.Net/issues/248) - Content-Type multipart/form-data is not seen as byte[] anymore

# 1.0.6 (15 December 2018)
- [#242](https://github.com/wiremock/WireMock.Net/pull/242) - Post multiple Mappings [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.5 (07 December 2018)
- [#236](https://github.com/wiremock/WireMock.Net/pull/236) - Add Random Regex (using Fare) [feature] contributed by [StefH](https://github.com/StefH)

# 1.0.4.21 (30 November 2018)
- [#221](https://github.com/wiremock/WireMock.Net/pull/221) - Update dependencies [feature] contributed by [StefH](https://github.com/StefH)
- [#230](https://github.com/wiremock/WireMock.Net/pull/230) - Add HandleBars Random functionality (#219) [feature] contributed by [StefH](https://github.com/StefH)
- [#231](https://github.com/wiremock/WireMock.Net/pull/231) - Use RandomDataGenerator.Net 1.0.3.0 contributed by [StefH](https://github.com/StefH)
- [#232](https://github.com/wiremock/WireMock.Net/pull/232) - Add SonarLint checks [feature] contributed by [StefH](https://github.com/StefH)
- [#233](https://github.com/wiremock/WireMock.Net/pull/233) - RandomDataGenerator.Net 1.0.4 [feature] contributed by [StefH](https://github.com/StefH)
- [#235](https://github.com/wiremock/WireMock.Net/pull/235) - Check aggregate exception during startup [bug] contributed by [StefH](https://github.com/StefH)
- [#219](https://github.com/wiremock/WireMock.Net/issues/219) - Feature: random value helper [feature]
- [#234](https://github.com/wiremock/WireMock.Net/issues/234) - Timeout Exception on VSTS Test Platform (Azure DevOps), with private build agent

# 1.0.4.20 (07 November 2018)
- [#222](https://github.com/wiremock/WireMock.Net/pull/222) - Codecov contributed by [StefH](https://github.com/StefH)
- [#224](https://github.com/wiremock/WireMock.Net/pull/224) - Fixed issue 223: Example for WireMock as Windows Service throws Exception because of WireMockConsoleLogger contributed by [paulssn](https://github.com/paulssn)
- [#228](https://github.com/wiremock/WireMock.Net/pull/228) - Fixed logic for IsRestrictedResponseHeader [bug] contributed by [StefH](https://github.com/StefH)
- [#223](https://github.com/wiremock/WireMock.Net/issues/223) - Bug: Example for WireMock as Windows Service throws Exception because of WireMockConsoleLogger [bug]

# 1.0.4.19 (31 October 2018)
- [#220](https://github.com/wiremock/WireMock.Net/pull/220) - Update SimpleCommandLineParser to handle arguments with key and value contributed by [StefH](https://github.com/StefH)

# 1.0.4.18 (27 October 2018)
- [#207](https://github.com/wiremock/WireMock.Net/pull/207) - Rewrite some unit-integration-tests to unit-tests (#206) contributed by [StefH](https://github.com/StefH)
- [#208](https://github.com/wiremock/WireMock.Net/pull/208) - Refactor contributed by [StefH](https://github.com/StefH)
- [#209](https://github.com/wiremock/WireMock.Net/pull/209) - NET Core 2.1 + support for Service Fabric commandline parameters contributed by [StefH](https://github.com/StefH)
- [#212](https://github.com/wiremock/WireMock.Net/pull/212) - Update BodyParser logic contributed by [StefH](https://github.com/StefH)
- [#217](https://github.com/wiremock/WireMock.Net/pull/217) - Enable Source Link contributed by [kashifsoofi](https://github.com/kashifsoofi)
- [#218](https://github.com/wiremock/WireMock.Net/pull/218) - remove appveyor contributed by [StefH](https://github.com/StefH)
- [#107](https://github.com/wiremock/WireMock.Net/issues/107) - Feature: increase code coverage [feature]
- [#161](https://github.com/wiremock/WireMock.Net/issues/161) - Feature: Implement SourceLink [feature]
- [#179](https://github.com/wiremock/WireMock.Net/issues/179) - BodyAsFile .json files interferes with WatchStaticMappings
- [#194](https://github.com/wiremock/WireMock.Net/issues/194) - Could not load file or assembly 'netstandard, Version=2.0.0.0 On Build Server
- [#206](https://github.com/wiremock/WireMock.Net/issues/206) - Rewrite some unit-integration-tests to unit-tests
- [#210](https://github.com/wiremock/WireMock.Net/issues/210) - When proxying, the Content-Type headers get dropped from the request
- [#211](https://github.com/wiremock/WireMock.Net/issues/211) - Feature: Add support to recognise custom json media-types
- [#213](https://github.com/wiremock/WireMock.Net/issues/213) - Question: Unable get response from wiremock.net server in c#
- [#215](https://github.com/wiremock/WireMock.Net/issues/215) - Issue: upgrade Microsoft.AspNetCore / Microsoft.AspNetCore.All to 2.1.5

# 1.0.4.17 (22 September 2018)
- [#203](https://github.com/wiremock/WireMock.Net/pull/203) - Set up CI with Azure Pipelines contributed by [azure-pipelines[bot]](https://github.com/apps/azure-pipelines)
- [#204](https://github.com/wiremock/WireMock.Net/pull/204) - Lower priority from Proxy mappings in favor of Admin Mappings [feature] contributed by [StefH](https://github.com/StefH)
- [#200](https://github.com/wiremock/WireMock.Net/issues/200) - Issue: Incorrect port matching
- [#205](https://github.com/wiremock/WireMock.Net/issues/205) - Issue: DELETE method is proxied as lowercase [bug]

# 1.0.4.16 (11 September 2018)
- [#202](https://github.com/wiremock/WireMock.Net/pull/202) - Update handlebars code to support Regex.Match (#201) contributed by [StefH](https://github.com/StefH)
- [#201](https://github.com/wiremock/WireMock.Net/issues/201) - Question : Extracting text from a request.body that is not json

# 1.0.4.15 (04 September 2018)
- [#199](https://github.com/wiremock/WireMock.Net/pull/199) - Fix for .WithBody(Func&lt;RequestMessage, string&gt;...) contributed by [StefH](https://github.com/StefH)
- [#198](https://github.com/wiremock/WireMock.Net/issues/198) - Issue : creating response using .WithBody(Func&lt;RequestMessage, string&gt;...) and .WithStatusCode [bug]

# 1.0.4.14 (02 September 2018)
- [#197](https://github.com/wiremock/WireMock.Net/pull/197) - Set IsStarted = true in a IApplicationLifetime.ApplicationStarted listener [bug] contributed by [davide-romanini](https://github.com/davide-romanini)
- [#196](https://github.com/wiremock/WireMock.Net/issues/196) - Issue: AspNetCoreSelfHost.IsStarted set before the server actually started for real [bug]

# 1.0.4.13 (31 August 2018)
- [#195](https://github.com/wiremock/WireMock.Net/pull/195) - Add LinqMatcher contributed by [StefH](https://github.com/StefH)
- [#192](https://github.com/wiremock/WireMock.Net/issues/192) - Cannot upgrade from 1.0.4.10 to 1.0.4.12 without upgrading to .net core 2.1 [bug]

# 1.0.4.12 (23 August 2018)
- [#190](https://github.com/wiremock/WireMock.Net/pull/190) - Fix ResponseMessageTransformer (#188) contributed by [StefH](https://github.com/StefH)
- [#191](https://github.com/wiremock/WireMock.Net/pull/191) - Fix ignore case logic for header-name and cookie-name  contributed by [StefH](https://github.com/StefH)
- [#188](https://github.com/wiremock/WireMock.Net/issues/188) - Bug: ResponseMessageTransformer : 
- [#189](https://github.com/wiremock/WireMock.Net/issues/189) - Issue: Case of header key/name not ignored in RequestBuilder when ignoreCase == true

# 1.0.4.11 (20 August 2018)
- [#183](https://github.com/wiremock/WireMock.Net/pull/183) - Set Content-Type header for PutMappingAsync in the client contributed by [seanamosw](https://github.com/seanamosw)
- [#185](https://github.com/wiremock/WireMock.Net/pull/185) - Support Microsoft.AspNetCore for net 4.6.1 and up [feature] contributed by [StefH](https://github.com/StefH)
- [#186](https://github.com/wiremock/WireMock.Net/pull/186) - ContentType &quot;application/vnd.api+json&quot; is not recognized as json contributed by [steveland83](https://github.com/steveland83)
- [#182](https://github.com/wiremock/WireMock.Net/issues/182) - Bug: IFluentMockServerAdmin::PutMappingAsync does not set Content-Type
- [#184](https://github.com/wiremock/WireMock.Net/issues/184) - Bug: Fix AppVeyor PR build process
- [#187](https://github.com/wiremock/WireMock.Net/issues/187) - Bug: Admin GetRequestAsync does not populate request body for JsonApi (&quot;application/vnd.api+json&quot;) content

# 1.0.4.10 (14 August 2018)
- [#180](https://github.com/wiremock/WireMock.Net/pull/180) - Add IFileSystemHandler to support Azure for StaticMapping location contributed by [StefH](https://github.com/StefH)
- [#173](https://github.com/wiremock/WireMock.Net/issues/173) - Feature: Mapping files lost when restarting an Azure app service [feature]

# 1.0.4.9 (08 August 2018)
- [#172](https://github.com/wiremock/WireMock.Net/issues/172) - Question: Same/similar fluent interface for in process and admin client API
- [#174](https://github.com/wiremock/WireMock.Net/issues/174) - Bug: JsonMatcher and JsonPathMatcher throws when posting byte[] [bug]
- [#175](https://github.com/wiremock/WireMock.Net/issues/175) - Bug: Don't allow adding a mapping with no URL or PATH [bug]
- [#176](https://github.com/wiremock/WireMock.Net/issues/176) - Question: Saving mapping with relative (not found) file fails
- [#177](https://github.com/wiremock/WireMock.Net/issues/177) - Feature: Skip invalid static mapping files [feature]

# 1.0.4.8 (23 July 2018)
- [#170](https://github.com/wiremock/WireMock.Net/pull/170) - Support json path in the response contributed by [StefH](https://github.com/StefH)
- [#167](https://github.com/wiremock/WireMock.Net/issues/167) - Feature: Support for JsonPath in the response (with HandleBars) [feature]

# 1.0.4.7 (19 July 2018)
- [#169](https://github.com/wiremock/WireMock.Net/pull/169) - Fix for Restricted Response headers [bug] contributed by [StefH](https://github.com/StefH)
- [#148](https://github.com/wiremock/WireMock.Net/issues/148) - Question: proxy passthrough when no match?

# 1.0.4.6 (18 July 2018)
- [#168](https://github.com/wiremock/WireMock.Net/pull/168) - Expose scenario states [feature] contributed by [StefH](https://github.com/StefH)
- [#163](https://github.com/wiremock/WireMock.Net/issues/163) - Feature: Expose scenario states

# 1.0.4.5 (17 July 2018)
- [#164](https://github.com/wiremock/WireMock.Net/pull/164) - Support running WireMock.Net as a sub-app in IIS [feature] contributed by [StefH](https://github.com/StefH)
- [#165](https://github.com/wiremock/WireMock.Net/pull/165) - Add SonarCloud contributed by [StefH](https://github.com/StefH)
- [#166](https://github.com/wiremock/WireMock.Net/pull/166) - Fix Sonar issues contributed by [StefH](https://github.com/StefH)
- [#120](https://github.com/wiremock/WireMock.Net/issues/120) - Question: JsonPathMatcher - not matching? Correct syntax?
- [#123](https://github.com/wiremock/WireMock.Net/issues/123) - Fix for DateTime Header causing null value in ResponseBuilder

# 1.0.4.4 (01 July 2018)
- [#156](https://github.com/wiremock/WireMock.Net/issues/156) - Feature: when adding / updating a mapping : return more details

# 1.0.4.3 (30 June 2018)
- [#159](https://github.com/wiremock/WireMock.Net/issues/159) - Bug: IRequestBuilder.WithParam broken for key-only matching [bug]

# 1.0.4.2 (26 June 2018)
- [#157](https://github.com/wiremock/WireMock.Net/pull/157) - Support for string and object in JsonMatcher. [feature] contributed by [StefH](https://github.com/StefH)
- [#150](https://github.com/wiremock/WireMock.Net/issues/150) - Add support for .NET Core 2.1 (.NET Core 2.0 will reach end of life on september 2018)
- [#154](https://github.com/wiremock/WireMock.Net/issues/154) - Feature: support BodyAsJson for Request in static mapping files. [feature]

# 1.0.4.1 (25 June 2018)
- [#153](https://github.com/wiremock/WireMock.Net/issues/153) - Feature: Add JsonMatcher to support Json mapping

# 1.0.4.0 (23 June 2018)
- [#131](https://github.com/wiremock/WireMock.Net/issues/131) - Bug: CurlException Couldn't connect to Server when running multiple tests
- [#149](https://github.com/wiremock/WireMock.Net/issues/149) - Question: Transformer and Delay in Static Mappings?
- [#151](https://github.com/wiremock/WireMock.Net/issues/151) - Feature: Add logging of incoming request and body for tracability

# 1.0.3.20 (29 May 2018)
- [#147](https://github.com/wiremock/WireMock.Net/pull/147) - Revert PortUtil.cs changes contributed by [StefH](https://github.com/StefH)
- [#129](https://github.com/wiremock/WireMock.Net/issues/129) - Random test failures between WireMock.Net 1.0.3.1 and 1.0.3.2
- [#146](https://github.com/wiremock/WireMock.Net/issues/146) - Hang possibly due to Windows firewall prompt

# 1.0.3.19 (28 May 2018)
- [#144](https://github.com/wiremock/WireMock.Net/pull/144) - Fix ConcurrentDictionary (#129) contributed by [StefH](https://github.com/StefH)
- [#145](https://github.com/wiremock/WireMock.Net/pull/145) - Cancellation token not passed to server instance in .NET Core 2 [bug] contributed by [Bob11327](https://github.com/Bob11327)

# 1.0.3.18 (25 May 2018)
- [#142](https://github.com/wiremock/WireMock.Net/pull/142) - Allow all headers to be set as Response headers contributed by [StefH](https://github.com/StefH)
- [#122](https://github.com/wiremock/WireMock.Net/issues/122) - WireMock.Net not responding in unit tests - same works in console application
- [#126](https://github.com/wiremock/WireMock.Net/issues/126) - Question: UsingHead always returns 0 for Content-Length header even when explicitly specified
- [#132](https://github.com/wiremock/WireMock.Net/issues/132) - LogEntries not being recorded on subsequent tests
- [#136](https://github.com/wiremock/WireMock.Net/issues/136) - Question: Does the WireMock send Content-Length response header
- [#137](https://github.com/wiremock/WireMock.Net/issues/137) - Question: How to specify Transfer-Encoding response header?
- [#139](https://github.com/wiremock/WireMock.Net/issues/139) - Wiki link https://github.com/StefH/WireMock.Net/wiki/Record-(via-proxy)-and-Save is dead

# 1.0.3.17 (16 May 2018)
- [#134](https://github.com/wiremock/WireMock.Net/pull/134) - Stef negate matcher contributed by [alastairtree](https://github.com/alastairtree)
- [#138](https://github.com/wiremock/WireMock.Net/pull/138) - Added Negate matcher logic contributed by [StefH](https://github.com/StefH)
- [#130](https://github.com/wiremock/WireMock.Net/issues/130) - ...

# 1.0.3.16 (17 April 2018)
- [#121](https://github.com/wiremock/WireMock.Net/pull/121) - Fix for issue #118 [bug] contributed by [raghavendrabankapur](https://github.com/raghavendrabankapur)
- [#125](https://github.com/wiremock/WireMock.Net/pull/125) - Change listen from loopback to any ip address for dotnetcore2.0 apps contributed by [SubjectiveReality](https://github.com/SubjectiveReality)
- [#118](https://github.com/wiremock/WireMock.Net/issues/118) - Not reading the response from a file when mappings are placed in json file
- [#124](https://github.com/wiremock/WireMock.Net/issues/124) - Issue: Unable to get host to listen on ips other than 127.0.0.1 using StandAloneApp

# 1.0.3.15 (05 April 2018)
- [#117](https://github.com/wiremock/WireMock.Net/pull/117) - Respect start timeout setting and expose exception from server startup contributed by [evanlwj](https://github.com/evanlwj)

# 1.0.3.14 (01 April 2018)
- [#111](https://github.com/wiremock/WireMock.Net/issues/111) - Question: Adding wiki documentation on how to use WireMock.Net.WebApplication project
- [#112](https://github.com/wiremock/WireMock.Net/issues/112) - Question: Request.Create().WithBody() not able to match with custom class which implements IMatcher
- [#113](https://github.com/wiremock/WireMock.Net/issues/113) - Feature: Add BodyAsJsonIndented for response message [feature]
- [#114](https://github.com/wiremock/WireMock.Net/issues/114) - Feature: Add PathSegments in Transform [feature]

# 1.0.3.11 (20 March 2018)
- [#110](https://github.com/wiremock/WireMock.Net/issues/110) - Fix: remove `Func[]` from MappingModel

# 1.0.3.10 (17 March 2018)
- [#109](https://github.com/wiremock/WireMock.Net/issues/109) - Issue: When proxying, MimeType is wrong for StringContent

# 1.0.3.9 (15 March 2018)
- [#108](https://github.com/wiremock/WireMock.Net/issues/108) - Issue: provide correct contentTypeHeader value for the bodyparser [bug]

# 1.0.3.8 (10 March 2018)
- [#106](https://github.com/wiremock/WireMock.Net/issues/106) - Issue: Params does not work, when there are multiple values for a key

# 1.0.3.4 (04 March 2018)
- [#95](https://github.com/wiremock/WireMock.Net/pull/95) - Unittest fix contributed by [StefH](https://github.com/StefH)
- [#96](https://github.com/wiremock/WireMock.Net/pull/96) - Replace log4net by custom logger (#94) contributed by [StefH](https://github.com/StefH)
- [#101](https://github.com/wiremock/WireMock.Net/pull/101) - ICallbackResponseBuilder + added more unit-tests [bug] contributed by [StefH](https://github.com/StefH)
- [#102](https://github.com/wiremock/WireMock.Net/pull/102) - Feature: add WithBody(req =&gt; dostuff) style callback [feature] contributed by [alastairtree](https://github.com/alastairtree)
- [#66](https://github.com/wiremock/WireMock.Net/issues/66) - Interested in callbacks?
- [#93](https://github.com/wiremock/WireMock.Net/issues/93) - Bug: FluentMockServer IsStarted after calling Start()
- [#94](https://github.com/wiremock/WireMock.Net/issues/94) - Issue: Introduced dependency on log4net
- [#98](https://github.com/wiremock/WireMock.Net/issues/98) - IBodyResponseBuilder.WithBody* should receive the request as a parameter

# 1.0.3.3 (24 February 2018)
- [#92](https://github.com/wiremock/WireMock.Net/pull/92) - Json fixes (#91) contributed by [StefH](https://github.com/StefH)
- [#91](https://github.com/wiremock/WireMock.Net/issues/91) - Bug: WireMock.Net is not matching application/json http requests using JSONPathMatcher [bug]

# 1.0.3.2 (14 February 2018)
- [#90](https://github.com/wiremock/WireMock.Net/pull/90) - Concurrent issue (#88) contributed by [StefH](https://github.com/StefH)
- [#88](https://github.com/wiremock/WireMock.Net/issues/88) - Bug: Standalone server throws 500 error when receiving concurrent requests [bug]

# 1.0.3.1 (14 February 2018)
- [#89](https://github.com/wiremock/WireMock.Net/pull/89) - Add log4net logging contributed by [StefH](https://github.com/StefH)
- [#87](https://github.com/wiremock/WireMock.Net/issues/87) - Feature: Add logging

# 1.0.3.0 (05 February 2018)
- [#80](https://github.com/wiremock/WireMock.Net/issues/80) - Feature: When using proxy, in case Content-Type is JSON, use BodyAsJson in Response
- [#81](https://github.com/wiremock/WireMock.Net/issues/81) - Feature: When using proxy, only BodyAsBytes in case of binary data?
- [#82](https://github.com/wiremock/WireMock.Net/issues/82) - Feature: make it possible to ignore some headers when proxying [feature]
- [#83](https://github.com/wiremock/WireMock.Net/issues/83) - Feature : Add also a method in IProxyResponseBuilder to provide proxy-settings [feature]
- [#85](https://github.com/wiremock/WireMock.Net/issues/85) - Bug: https for netstandard does not work ? [bug]
- [#86](https://github.com/wiremock/WireMock.Net/issues/86) - Feature : Add FileSystemWatcher logic for watching static mapping files [feature]

# 1.0.2.13 (23 January 2018)
- [#79](https://github.com/wiremock/WireMock.Net/pull/79) - Fix missed content headers contributed by [volodymyr-fed](https://github.com/volodymyr-fed)
- [#57](https://github.com/wiremock/WireMock.Net/issues/57) - ProxyAndRecord does not save query-parameters, headers and body [bug]
- [#78](https://github.com/wiremock/WireMock.Net/issues/78) - WireMock not working when attempting to access from anything other than localhost.

# 1.0.2.12 (16 January 2018)
- [#75](https://github.com/wiremock/WireMock.Net/pull/75) - Add WireMock.Net.WebApplication example contributed by [StefH](https://github.com/StefH)
- [#77](https://github.com/wiremock/WireMock.Net/pull/77) - Fixed issue #76 contributed by [StefH](https://github.com/StefH)
- [#73](https://github.com/wiremock/WireMock.Net/issues/73) - Updated mapping is not being picked and responded with the response
- [#76](https://github.com/wiremock/WireMock.Net/issues/76) - Bug: IFluentMockServerAdmin is missing content-type for some POST/PUT calls

# 1.0.2.11 (20 December 2017)
- [#72](https://github.com/wiremock/WireMock.Net/issues/72) - Matching WithParam on OData End Points

# 1.0.2.10 (12 December 2017)
- [#70](https://github.com/wiremock/WireMock.Net/issues/70) - Proxy/Intercept pattern is throwing a keep alive header error with net461

# 1.0.2.9 (07 December 2017)
- [#71](https://github.com/wiremock/WireMock.Net/pull/71) - Fixed restricted headers on response contributed by [StefH](https://github.com/StefH)
- [#69](https://github.com/wiremock/WireMock.Net/issues/69) - Instructions are incorrect (?)

# 1.0.2.8 (23 November 2017)
- [#65](https://github.com/wiremock/WireMock.Net/pull/65) - bug: Fix admin api client definition returning the wrong types contributed by [alastairtree](https://github.com/alastairtree)
- [#67](https://github.com/wiremock/WireMock.Net/pull/67) - bug: fix supporting the Patch method and logging the body contributed by [alastairtree](https://github.com/alastairtree)
- [#64](https://github.com/wiremock/WireMock.Net/issues/64) - Pull Requests do not trigger test + codecoverage ?

# 1.0.2.7 (18 November 2017)
- [#62](https://github.com/wiremock/WireMock.Net/pull/62) - Add the Host, Protocol, Port and Origin to the Request message so they can be used in templating contributed by [alastairtree](https://github.com/alastairtree)
- [#63](https://github.com/wiremock/WireMock.Net/pull/63) - Fix issue with concurrent logging contributed by [volodymyr-fed](https://github.com/volodymyr-fed)
- [#27](https://github.com/wiremock/WireMock.Net/issues/27) - New feature: Record and Save
- [#42](https://github.com/wiremock/WireMock.Net/issues/42) - Enhancement - Save/load request logs to/from disk [feature]
- [#53](https://github.com/wiremock/WireMock.Net/issues/53) - New feature request: Access to Owin pipeline

# 1.0.2.6 (30 October 2017)
- [#59](https://github.com/wiremock/WireMock.Net/pull/59) - Add ability to provide multiple values for headers in response contributed by [Dreamescaper](https://github.com/Dreamescaper)
- [#60](https://github.com/wiremock/WireMock.Net/pull/60) - Fix proxy headers handling contributed by [Dreamescaper](https://github.com/Dreamescaper)
- [#54](https://github.com/wiremock/WireMock.Net/issues/54) - Proxy for AWS: Error unmarshalling response back from AWS [bug]
- [#56](https://github.com/wiremock/WireMock.Net/issues/56) - WithBodyFromFile Support [feature]
- [#58](https://github.com/wiremock/WireMock.Net/issues/58) - Multiple headers with same name [feature]

# 1.0.2.5 (24 October 2017)
- [#55](https://github.com/wiremock/WireMock.Net/pull/55) - Fix the problem with headers passthrough [bug] contributed by [dmtrrk](https://github.com/dmtrrk)
- [#44](https://github.com/wiremock/WireMock.Net/issues/44) - Bug: Server not listening after Start() returns (on macOS) [bug]
- [#48](https://github.com/wiremock/WireMock.Net/issues/48) - Stateful support [feature]
- [#52](https://github.com/wiremock/WireMock.Net/issues/52) - SimMetrics.NET error when trying to install NuGet Package

# 1.0.2.4 (10 October 2017)
- [#32](https://github.com/wiremock/WireMock.Net/pull/32) - [Feature] Add support for client certificate password and test with real services that require client certificate auth [feature] contributed by [phillee007](https://github.com/phillee007)
- [#35](https://github.com/wiremock/WireMock.Net/pull/35) - Revert changes that were made by mistake in prior PR contributed by [phillee007](https://github.com/phillee007)
- [#39](https://github.com/wiremock/WireMock.Net/pull/39) - Listen on http://*:9090 contributed by [StefH](https://github.com/StefH)
- [#40](https://github.com/wiremock/WireMock.Net/pull/40) - Expose more settings to stand-alone app contributed by [StefH](https://github.com/StefH)
- [#41](https://github.com/wiremock/WireMock.Net/pull/41) - Dotnet 20 preview final [feature] contributed by [StefH](https://github.com/StefH)
- [#45](https://github.com/wiremock/WireMock.Net/pull/45) - Add RequestLogExpirationDuration and MaxRequestLogCount (#43) contributed by [StefH](https://github.com/StefH)
- [#51](https://github.com/wiremock/WireMock.Net/pull/51) - Observable logs contributed by [dmtrrk](https://github.com/dmtrrk)
- [#15](https://github.com/wiremock/WireMock.Net/issues/15) - New feature: Proxying [feature]
- [#20](https://github.com/wiremock/WireMock.Net/issues/20) - Add client certificate authentication [feature]
- [#31](https://github.com/wiremock/WireMock.Net/issues/31) - Feature request: Nuget package for standalone version [feature]
- [#33](https://github.com/wiremock/WireMock.Net/issues/33) - Issue with launching sample code (StandAlone server) [bug]
- [#38](https://github.com/wiremock/WireMock.Net/issues/38) - Bug: support also listening on *:{port}
- [#43](https://github.com/wiremock/WireMock.Net/issues/43) - Feature: Add RequestLogExpirationDuration and MaxRequestLogCount
- [#46](https://github.com/wiremock/WireMock.Net/issues/46) - Log the ip-address from the client/caller also in the RequestLog [feature]
- [#47](https://github.com/wiremock/WireMock.Net/issues/47) - Feature: add matcher details to Request to see which matchers match/not match [feature]
- [#50](https://github.com/wiremock/WireMock.Net/issues/50) - New Feature: Callbacks

# 1.0.2.1 (14 June 2017)
- [#30](https://github.com/wiremock/WireMock.Net/issues/30) - [Feature] Disable partial mappings by default in standalone version [bug, feature]

# 1.0.2.0 (05 May 2017)
- [#26](https://github.com/wiremock/WireMock.Net/pull/26) - merge netstandard into main contributed by [StefH](https://github.com/StefH)
- [#21](https://github.com/wiremock/WireMock.Net/issues/21) - Admin static json mappings [feature]
- [#23](https://github.com/wiremock/WireMock.Net/issues/23) - Consider port to .Net Core
- [#25](https://github.com/wiremock/WireMock.Net/issues/25) - Upgrade to vs2017 [feature]

# 1.0.1.2 (27 February 2017)
- [#24](https://github.com/wiremock/WireMock.Net/pull/24) - Body Encoding contributed by [sbebrys](https://github.com/sbebrys)
- [#8](https://github.com/wiremock/WireMock.Net/issues/8) - admin rest api
- [#22](https://github.com/wiremock/WireMock.Net/issues/22) - Add basic-authentication for accessing admin-interface [feature]

# 1.0.1.1 (10 February 2017)
- [#1](https://github.com/wiremock/WireMock.Net/issues/1) - Replace WildcardPatternMatcher by RegEx [feature]
- [#2](https://github.com/wiremock/WireMock.Net/issues/2) - Func&lt;string&gt; matching [feature]
- [#3](https://github.com/wiremock/WireMock.Net/issues/3) - WithUrls and WithHeaders and ...
- [#4](https://github.com/wiremock/WireMock.Net/issues/4) - Handlebar support
- [#5](https://github.com/wiremock/WireMock.Net/issues/5) - Xml(2)Path matching
- [#6](https://github.com/wiremock/WireMock.Net/issues/6) - JsonPath support matching
- [#9](https://github.com/wiremock/WireMock.Net/issues/9) - Cookie matching
- [#10](https://github.com/wiremock/WireMock.Net/issues/10) - Add usingDelete [feature]
- [#11](https://github.com/wiremock/WireMock.Net/issues/11) - Add response body in binary format  [feature]
- [#12](https://github.com/wiremock/WireMock.Net/issues/12) - Getting all currently registered stub mappings [feature]
- [#13](https://github.com/wiremock/WireMock.Net/issues/13) - Handle Exception
- [#14](https://github.com/wiremock/WireMock.Net/issues/14) - Allow Body as Base64
- [#16](https://github.com/wiremock/WireMock.Net/issues/16) - Stub priority [feature]
- [#17](https://github.com/wiremock/WireMock.Net/issues/17) - Add JsonBody to response [feature]
- [#18](https://github.com/wiremock/WireMock.Net/issues/18) - Listen on more ip-address/ports [feature]

