// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
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
    public MimeMessageDataWrapper(IMimeMessage message)
    {
        Message = Guard.NotNull(message);
    }

    /// <inheritdoc/>
    public IEnumerable<string> Headers => Message.Headers.Select(h => h.ToString());

    /// <inheritdoc/>
    public int Importance => (int)Message.Importance;

    /// <inheritdoc/>
    public int Priority => (int)Message.Priority;

    /// <inheritdoc/>
    public int XPriority => (int)Message.XPriority;

    /// <inheritdoc/>
    public string Sender => Message.Sender.Address;

    /// <inheritdoc/>
    public string ResentSender => Message.ResentSender.ToString();

    /// <inheritdoc/>
    public IEnumerable<string> From => Message.From.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> ResentFrom => Message.ResentFrom.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> ReplyTo => Message.ReplyTo.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> ResentReplyTo => Message.ResentReplyTo.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> To => Message.To.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> ResentTo => Message.ResentTo.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> Cc => Message.Cc.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> ResentCc => Message.ResentCc.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> Bcc => Message.Bcc.Select(h => h.ToString());

    /// <inheritdoc/>
    public IEnumerable<string> ResentBcc => Message.ResentBcc.Select(h => h.ToString());

    /// <inheritdoc/>
    public string Subject => Message.Subject;

    /// <inheritdoc/>
    public DateTimeOffset Date => Message.Date;

    /// <inheritdoc/>
    public DateTimeOffset ResentDate => Message.ResentDate;

    /// <inheritdoc/>
    public IEnumerable<string> References => Message.References;

    /// <inheritdoc/>
    public string InReplyTo => Message.InReplyTo;

    /// <inheritdoc/>
    public string MessageId => Message.MessageId;

    /// <inheritdoc/>
    public string ResentMessageId => Message.ResentMessageId;

    /// <inheritdoc/>
    public Version MimeVersion => Message.MimeVersion;

    /// <inheritdoc/>
    public IMimeEntityData Body => new MimeEntityDataWrapper(Message.Body);

    /// <inheritdoc/>
    public string TextBody => Message.TextBody;

    /// <inheritdoc/>
    public string HtmlBody => Message.HtmlBody;

    /// <inheritdoc/>
    public IEnumerable<IMimePartData> BodyParts => Message.BodyParts.OfType<MimePart>().Select(mp => new MimePartDataWrapper(mp));

    /// <inheritdoc/>
    public IEnumerable<IMimeEntityData> Attachments => Message.Attachments.Select(me => new MimeEntityDataWrapper(me));
}