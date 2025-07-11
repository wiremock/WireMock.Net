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
    private readonly IMimeMessage _message;

    /// <summary>
    /// Initializes a new instance of the <see cref="MimeMessageDataWrapper"/> class.
    /// </summary>
    /// <param name="message">The MIME message to wrap.</param>
    public MimeMessageDataWrapper(IMimeMessage message)
    {
        _message = Guard.NotNull(message);

        Bcc = _message.Bcc.Select(h => h.ToString()).ToList();
        Cc = _message.Cc.Select(h => h.ToString()).ToList();
        From = _message.From.Select(h => h.ToString()).ToList();
        Headers = _message.Headers.Select(h => h.ToString()).ToList();
        References = _message.References.ToList();
        ReplyTo = _message.ReplyTo.Select(h => h.ToString()).ToList();
        ResentBcc = _message.ResentBcc.Select(h => h.ToString()).ToList();
        ResentCc = _message.ResentCc.Select(h => h.ToString()).ToList();
        ResentFrom = _message.ResentFrom.Select(h => h.ToString()).ToList();
        ResentReplyTo = _message.ResentReplyTo.Select(h => h.ToString()).ToList();
        ResentTo = _message.ResentTo.Select(h => h.ToString()).ToList();
        To = _message.To.Select(h => h.ToString()).ToList();

        Body = new MimeEntityDataWrapper(_message.Body);
        BodyParts = _message.BodyParts.OfType<MimePart>().Select(mp => new MimePartDataWrapper(mp)).ToList<IMimePartData>();
        Attachments = _message.Attachments.Select(me => new MimeEntityDataWrapper(me)).ToList<IMimeEntityData>();
    }

    /// <inheritdoc/>
    public IList<string> Headers { get; private set; }

    /// <inheritdoc/>
    public int Importance => (int)_message.Importance;

    /// <inheritdoc/>
    public int Priority => (int)_message.Priority;

    /// <inheritdoc/>
    public int XPriority => (int)_message.XPriority;

    /// <inheritdoc/>
    public string Sender => _message.Sender.Address;

    /// <inheritdoc/>
    public string ResentSender => _message.ResentSender.ToString();

    /// <inheritdoc/>
    public IList<string> From { get; private set; }

    /// <inheritdoc/>
    public IList<string> ResentFrom { get; private set; }

    /// <inheritdoc/>
    public IList<string> ReplyTo { get; private set; }

    /// <inheritdoc/>
    public IList<string> ResentReplyTo { get; private set; }

    /// <inheritdoc/>
    public IList<string> To { get; private set; }

    /// <inheritdoc/>
    public IList<string> ResentTo { get; private set; }

    /// <inheritdoc/>
    public IList<string> Cc { get; private set; }

    /// <inheritdoc/>
    public IList<string> ResentCc { get; private set; }

    /// <inheritdoc/>
    public IList<string> Bcc { get; private set; }

    /// <inheritdoc/>
    public IList<string> ResentBcc { get; private set; }

    /// <inheritdoc/>
    public string Subject => _message.Subject;

    /// <inheritdoc/>
    public DateTimeOffset Date => _message.Date;

    /// <inheritdoc/>
    public DateTimeOffset ResentDate => _message.ResentDate;

    /// <inheritdoc/>
    public IList<string> References { get; private set; }

    /// <inheritdoc/>
    public string InReplyTo => _message.InReplyTo;

    /// <inheritdoc/>
    public string MessageId => _message.MessageId;

    /// <inheritdoc/>
    public string ResentMessageId => _message.ResentMessageId;

    /// <inheritdoc/>
    public Version MimeVersion => _message.MimeVersion;

    /// <inheritdoc/>
    public IMimeEntityData Body { get; private set; }

    /// <inheritdoc/>
    public string TextBody => _message.TextBody;

    /// <inheritdoc/>
    public string HtmlBody => _message.HtmlBody;

    /// <inheritdoc/>
    public IList<IMimePartData> BodyParts { get; private set; }

    /// <inheritdoc/>
    public IList<IMimeEntityData> Attachments { get; private set; }
}