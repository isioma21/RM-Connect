namespace RmConnect.Application.Common.Exceptions;

/// <summary>The request clashes with existing data (e.g. an email that is already registered).</summary>
public class ConflictException(string message) : Exception(message);
