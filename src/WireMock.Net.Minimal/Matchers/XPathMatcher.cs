// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using AnyOfTypes;
using WireMock.Extensions;
using WireMock.Models;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Util;
//#if !NETSTANDARD1_3
using Wmhelp.XPath2;
//#endif

namespace WireMock.Matchers;

/// <summary>
/// XPath2Matcher
/// </summary>
/// <seealso cref="IStringMatcher" />
public class XPathMatcher : IStringMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Array of namespace prefix and uri.
    /// </summary>
    public XmlNamespace[]? XmlNamespaceMap { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public XPathMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, null, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XPathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="xmlNamespaceMap">The xml namespaces of the xml document.</param>
    /// <param name="patterns">The patterns.</param>
    public XPathMatcher(
        MatchBehaviour matchBehaviour,
        MatchOperator matchOperator = MatchOperator.Or,
        XmlNamespace[]? xmlNamespaceMap = null,
        params AnyOf<string, StringPattern>[] patterns)
    {
        _patterns = Guard.NotNull(patterns);
        XmlNamespaceMap = xmlNamespaceMap;
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input)
    {
        var score = MatchScores.Mismatch;

        if (input == null)
        {
            return CreateMatchResult(score);
        }

        try
        {
            var xPathEvaluator = new XPathEvaluator();
            xPathEvaluator.Load(input);

            if (!xPathEvaluator.IsXmlDocumentLoaded)
            {
                return CreateMatchResult(score);
            }

            score = MatchScores.ToScore(xPathEvaluator.Evaluate(_patterns, XmlNamespaceMap), MatchOperator);
        }
        catch (Exception exception)
        {
            return CreateMatchResult(score, exception);
        }

        return CreateMatchResult(score);
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(XPathMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}, " +
               $"null, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}" +
               $")";
    }

    private MatchResult CreateMatchResult(double score, Exception? exception = null)
    {
        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    private sealed class XPathEvaluator
    {
        private XmlDocument? _xmlDocument;
        private XPathNavigator? _xpathNavigator;

        public bool IsXmlDocumentLoaded => _xmlDocument != null;

        public void Load(string input)
        {
            try
            {
                _xmlDocument = new XmlDocument { InnerXml = input };
                _xpathNavigator = _xmlDocument.CreateNavigator();
            }
            catch
            {
                _xmlDocument = default;
            }
        }

        public bool[] Evaluate(AnyOf<string, StringPattern>[] patterns, IEnumerable<XmlNamespace>? xmlNamespaceMap)
        {
            return _xpathNavigator == null ? [] : patterns.Select(pattern => true.Equals(Evaluate(_xpathNavigator, pattern, xmlNamespaceMap))).ToArray();
        }

        private object Evaluate(XPathNavigator navigator, AnyOf<string, StringPattern> pattern, IEnumerable<XmlNamespace>? xmlNamespaceMap)
        {
            var xpath = $"boolean({pattern.GetPattern()})";

            var xmlNamespaceManager = GetXmlNamespaceManager(xmlNamespaceMap);
            if (xmlNamespaceManager == null)
            {
//#if NETSTANDARD1_3
                //return navigator.Evaluate(xpath);
//#else
                return navigator.XPath2Evaluate(xpath);
//#endif
            }

//#if NETSTANDARD1_3
            //return navigator.Evaluate(xpath, xmlNamespaceManager);
//#else
            return navigator.XPath2Evaluate(xpath, xmlNamespaceManager);
//#endif
        }

        private XmlNamespaceManager? GetXmlNamespaceManager(IEnumerable<XmlNamespace>? xmlNamespaceMap)
        {
            if (_xpathNavigator == null || xmlNamespaceMap == null)
            {
                return default;
            }

            var nsManager = new XmlNamespaceManager(_xpathNavigator.NameTable);
            foreach (var xmlNamespace in xmlNamespaceMap)
            {
                nsManager.AddNamespace(xmlNamespace.Prefix, xmlNamespace.Uri);
            }

            return nsManager;
        }
    }
}