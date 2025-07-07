// Copyright © WireMock.Net

using System;
using System.Collections.Generic;

namespace WireMock.Models;

/// <summary>
/// A simplified interface exposing the public, readable properties of a MIME message.
/// </summary>
public interface IMimeMessageData
{
    /// <summary>
    /// Get the list of headers.
    /// </summary>
    /// <value>The list of headers.</value>
    IEnumerable<object> Headers { get; }

    /// <summary>
    /// Get the value of the Importance header.
    /// </summary>
    /// <value>The importance, as an integer.</value>
    int Importance { get; }

    /// <summary>
    /// Get the value of the Priority header.
    /// </summary>
    /// <value>The priority, as an integer.</value>
    int Priority { get; }

    /// <summary>
    /// Get the value of the X-Priority header.
    /// </summary>
    /// <value>The X-priority, as an integer.</value>
    int XPriority { get; }

    /// <summary>
    /// Get the address in the Sender header.
    /// </summary>
    /// <value>The address in the Sender header.</value>
    object Sender { get; }

    /// <summary>
    /// Get the address in the Resent-Sender header.
    /// </summary>
    /// <value>The address in the Resent-Sender header.</value>
    object ResentSender { get; }

    /// <summary>
    /// Get the list of addresses in the From header.
    /// </summary>
    /// <value>The list of addresses in the From header.</value>
    object From { get; }

    /// <summary>
    /// Get the list of addresses in the Resent-From header.
    /// </summary>
    /// <value>The list of addresses in the Resent-From header.</value>
    object ResentFrom { get; }

    /// <summary>
    /// Get the list of addresses in the Reply-To header.
    /// </summary>
    /// <value>The list of addresses in the Reply-To header.</value>
    object ReplyTo { get; }

    /// <summary>
    /// Get the list of addresses in the Resent-Reply-To header.
    /// </summary>
    /// <value>The list of addresses in the Resent-Reply-To header.</value>
    object ResentReplyTo { get; }

    /// <summary>
    /// Get the list of addresses in the To header.
    /// </summary>
    /// <value>The list of addresses in the To header.</value>
    object To { get; }

    /// <summary>
    /// Get the list of addresses in the Resent-To header.
    /// </summary>
    /// <value>The list of addresses in the Resent-To header.</value>
    object ResentTo { get; }

    /// <summary>
    /// Get the list of addresses in the Cc header.
    /// </summary>
    /// <value>The list of addresses in the Cc header.</value>
    object Cc { get; }

    /// <summary>
    /// Get the list of addresses in the Resent-Cc header.
    /// </summary>
    /// <value>The list of addresses in the Resent-Cc header.</value>
    object ResentCc { get; }

    /// <summary>
    /// Get the list of addresses in the Bcc header.
    /// </summary>
    /// <value>The list of addresses in the Bcc header.</value>
    object Bcc { get; }

    /// <summary>
    /// Get the list of addresses in the Resent-Bcc header.
    /// </summary>
    /// <value>The list of addresses in the Resent-Bcc header.</value>
    object ResentBcc { get; }

    /// <summary>
    /// Get the subject of the message.
    /// </summary>
    /// <value>The subject of the message.</value>
    string Subject { get; }

    /// <summary>
    /// Get the date of the message.
    /// </summary>
    /// <value>The date of the message.</value>
    DateTimeOffset Date { get; }

    /// <summary>
    /// Get the Resent-Date of the message.
    /// </summary>
    /// <value>The Resent-Date of the message.</value>
    DateTimeOffset ResentDate { get; }

    /// <summary>
    /// Get the list of references to other messages.
    /// </summary>
    /// <value>The references.</value>
    IList<string> References { get; }

    /// <summary>
    /// Get the Message-Id that this message is replying to.
    /// </summary>
    /// <value>The message id that this message is in reply to.</value>
    string InReplyTo { get; }

    /// <summary>
    /// Get the message identifier.
    /// </summary>
    /// <value>The message identifier.</value>
    string MessageId { get; }

    /// <summary>
    /// Get the Resent-Message-Id header.
    /// </summary>
    /// <value>The Resent-Message-Id.</value>
    string ResentMessageId { get; }

    /// <summary>
    /// Get the MIME-Version.
    /// </summary>
    /// <value>The MIME version.</value>
    Version MimeVersion { get; }

    /// <summary>
    /// Get the body of the message.
    /// </summary>
    /// <value>The body of the message.</value>
    object Body { get; }

    /// <summary>
    /// Get the text body of the message if it exists.
    /// </summary>
    /// <value>The text body if it exists; otherwise, <see langword="null"/>.</value>
    string TextBody { get; }

    /// <summary>
    /// Get the html body of the message if it exists.
    /// </summary>
    /// <value>The html body if it exists; otherwise, <see langword="null"/>.</value>
    string HtmlBody { get; }

    /// <summary>
    /// Get the body parts of the message.
    /// </summary>
    /// <value>The body parts.</value>
    IEnumerable<object> BodyParts { get; }

    /// <summary>
    /// Get the attachments.
    /// </summary>
    /// <value>The attachments.</value>
    IEnumerable<object> Attachments { get; }
}