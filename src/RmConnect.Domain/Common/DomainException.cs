namespace RmConnect.Domain.Common;

/// <summary>A business rule was broken (e.g. accepting a request that isn't pending).</summary>
public class DomainException(string message) : Exception(message);
