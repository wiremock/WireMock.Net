// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// A structure defining an (optional) Id and a Text.
/// </summary>
public readonly struct IdOrTexts
{
    /// <summary>
    /// The Id [optional].
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// The Texts.
    /// </summary>
    public IReadOnlyList<string> Texts { get; }

    /// <summary>
    /// Create a IdOrText
    /// </summary>
    /// <param name="id">The Id [optional]</param>
    /// <param name="text">The Text.</param>
    public IdOrTexts(string? id, string text) : this(id, [text])
    {
    }

    /// <summary>
    /// Create a IdOrText
    /// </summary>
    /// <param name="id">The Id [optional]</param>
    /// <param name="texts">The Texts.</param>
    public IdOrTexts(string? id, IReadOnlyList<string> texts)
    {
        Id = id;
        Texts = texts;
    }

    /// <summary>
    /// When Id is defined, process the Id, else process the Texts.
    /// </summary>
    /// <param name="id">Callback to process the id.</param>
    /// <param name="texts">Callback to process the texts.</param>
    public void Value(Action<string> id, Action<IReadOnlyList<string>> texts)
    {
        if (Id != null)
        {
            id(Id);
        }
        else
        {
            texts(Texts);
        }
    }
}