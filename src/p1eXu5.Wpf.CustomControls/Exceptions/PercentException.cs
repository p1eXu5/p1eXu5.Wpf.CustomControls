using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace p1eXu5.Wpf.CustomControls.Exceptions
{
    [Serializable]
    public class PercentException : ArgumentException, ISerializable
    {
        public PercentException() : base() { }
        public PercentException( string message ) : base( message ) { }
        public PercentException( string message, string paramName ) : base( message, paramName ) { }
        protected PercentException( SerializationInfo info, StreamingContext context ) : base( info, context) { }
    }
}
