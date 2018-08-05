using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ModCore.Logic.Table
{
    public class RowNotFoundException : KeyNotFoundException
    {
        public object RowObject { get; set; }

        protected RowNotFoundException()
            : base("row not found in table")
        {
        }

        public RowNotFoundException(string message)
            : base(message)
        {
        }

        public RowNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RowNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public RowNotFoundException(object row)
            : this(row?.ToString() ?? "null")
        {
            RowObject = row;
        }
    }
}