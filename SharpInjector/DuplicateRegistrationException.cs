using System;

namespace SharpInjector {
    public class DuplicateRegistrationException : Exception {
        public Type AlreadyRegisteredType { get; set; }

        public DuplicateRegistrationException(Type typeKey) : this(typeKey, (string) null, (Exception) null) { }
        public DuplicateRegistrationException(Type typeKey, string message) : this(typeKey, message, (Exception) null) { }

        public DuplicateRegistrationException(Type typeKey, string message = "Type already registered", Exception inner = null) : base(message, inner) {
            AlreadyRegisteredType = typeKey;
        }
    }
}