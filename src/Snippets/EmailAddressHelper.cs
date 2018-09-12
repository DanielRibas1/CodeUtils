using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Snippets
{
    public static class EmailAddressHelper
    {
        public const char DefaultEmailListSeparator = ',';

        public static List<MailAddress> ProcessSingleLineAddressList(string input, char separator = DefaultEmailListSeparator)
        {
            if (string.IsNullOrEmpty(input)) return null;

            return input.Split(separator).Select(rawEmail => new MailAddress(rawEmail)).ToList();
        }
    }
}
