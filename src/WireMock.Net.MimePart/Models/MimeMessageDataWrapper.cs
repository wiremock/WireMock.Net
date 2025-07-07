// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using MimeKit;
using Stef.Validation;

namespace WireMock.Models;

/// <summary>
/// A wrapper class that implements the <see cref="IMimeMessageData"/>> interface by wrapping an <see cref="IMimeMessage"/> interface.
/// </summary>
/// <remarks>
/// This class provides a simplified, read-only view of an <see cref="IMimeMessage"/>.
/// </remarks>
internal class MimeMessageDataWrapper : IMimeMessageData
{
    public IMimeMessage Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MimeMessageDataWrapper"/> class.
    /// </summary>
    /// <param name="message">The MIME message to wrap.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="message"/> is <see langword="null"/>.
    /// </exception>
    public MimeMessageDataWrapper(IMimeMessage message)
    {
        Message = Guard.NotNull(message);
    }

    /// <inheritdoc/>
    public IEnumerable<object> Headers => Message.Headers;

    /// <inheritdoc/>
    public int Importance => (int)Message.Importance;

    /// <inheritdoc/>
    public int Priority => (int)Message.Priority;

    /// <inheritdoc/>
    public int XPriority => (int)Message.XPriority;

    /// <inheritdoc/>
    public object Sender => Message.Sender;

    /// <inheritdoc/>
    public object ResentSender => Message.ResentSender;

    /// <inheritdoc/>
    public object From => Message.From;

    /// <inheritdoc/>
    public object ResentFrom => Message.ResentFrom;

    /// <inheritdoc/>
    public object ReplyTo => Message.ReplyTo;

    /// <inheritdoc/>
    public object ResentReplyTo => Message.ResentReplyTo;

    /// <inheritdoc/>
    public object To => Message.To;

    /// <inheritdoc/>
    public object ResentTo => Message.ResentTo;

    /// <inheritdoc/>
    public object Cc => Message.Cc;

    /// <inheritdoc/>
    public object ResentCc => Message.ResentCc;

    /// <inheritdoc/>
    public object Bcc => Message.Bcc;

    /// <inheritdoc/>
    public object ResentBcc => Message.ResentBcc;

    /// <inheritdoc/>
    public string Subject => Message.Subject;

    /// <inheritdoc/>
    public DateTimeOffset Date => Message.Date;

    /// <inheritdoc/>
    public DateTimeOffset ResentDate => Message.ResentDate;

    /// <inheritdoc/>
    public IList<string> References => Message.References;

    /// <inheritdoc/>
    public string InReplyTo => Message.InReplyTo;

    /// <inheritdoc/>
    public string MessageId => Message.MessageId;

    /// <inheritdoc/>
    public string ResentMessageId => Message.ResentMessageId;

    /// <inheritdoc/>
    public Version MimeVersion => Message.MimeVersion;

    /// <inheritdoc/>
    public object Body => Message.Body;

    /// <inheritdoc/>
    public string TextBody => Message.TextBody;

    /// <inheritdoc/>
    public string HtmlBody => Message.HtmlBody;

    /// <inheritdoc/>
    public IEnumerable<object> BodyParts => Message.BodyParts;

    /// <inheritdoc/>
    public IEnumerable<object> Attachments => Message.Attachments;
}