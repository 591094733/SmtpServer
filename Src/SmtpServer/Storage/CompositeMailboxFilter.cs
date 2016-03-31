﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SmtpServer.Mail;

namespace SmtpServer.Storage
{
    public class CompositeMailboxFilter : IMailboxFilter
    {
        readonly IMailboxFilter[] _filters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filters">The list of filters to run in order.</param>
        public CompositeMailboxFilter(params IMailboxFilter[] filters)
        {
            _filters = filters;
        }

        /// <summary>
        /// Returns a value indicating whether the given mailbox can be accepted as a sender.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point of the client making the connection.</param>
        /// <param name="from">The mailbox to test.</param>
        /// <param name="size">The estimated message size to accept.</param>
        /// <returns>The acceptance state of the mailbox.</returns>
        public async Task<MailboxFilterResult> CanAcceptFromAsync(EndPoint remoteEndPoint, IMailbox @from, int size = 0)
        {
            if (_filters == null || _filters.Any() == false)
            {
                return MailboxFilterResult.Yes;
            }

            var results = await Task.WhenAll(_filters.Select(f => f.CanAcceptFromAsync(remoteEndPoint, @from, size)));

            return results.Max();
        }

        /// <summary>
        /// Returns a value indicating whether the given mailbox can be accepted as a recipient to the given sender.
        /// </summary>
        /// <param name="to">The mailbox to test.</param>
        /// <param name="from">The sender's mailbox.</param>
        /// <returns>The acceptance state of the mailbox.</returns>
        public async Task<MailboxFilterResult> CanDeliverToAsync(IMailbox to, IMailbox @from)
        {
            if (_filters == null || _filters.Any() == false)
            {
                return MailboxFilterResult.Yes;
            }

            var results = await Task.WhenAll(_filters.Select(f => f.CanDeliverToAsync(to, @from)));

            return results.Max();
        }
    }
}
