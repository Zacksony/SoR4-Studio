using System;

namespace SoR4_Studio.Modules;

internal static class Exceptions
{
    internal static class OperationExceptions
    {
        // IByteable

        public static Exception SetBytesNotSupportedException
            => new("IByteable.SetBytes() is not supported.");

        // IProtoType

        public static Exception SetValueNotSupportedException
            => new("IProtoContent.Set() is not supported here.");
    }

    internal static class ProtobufExceptions
    {
        // Message

        public static Exception ProtoMessageParseFailedException
            => new("Failed to parse protobuf message.");

        // Field

        public static Exception InvalidFieldAddressException
            => new("Invalid field address.");

        public static Exception OperationNotSupportedException
            => new("The field operation is not supported.");

        public static Exception OperationFailedException
            => new("Field operation failed.");

        // Content

        public static Exception FieldTypeMismatchException
            => new("Invalid field type.");
    }
}
