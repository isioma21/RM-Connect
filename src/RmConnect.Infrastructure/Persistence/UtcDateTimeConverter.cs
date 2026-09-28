using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RmConnect.Infrastructure.Persistence;

public class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    value => value,
    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
