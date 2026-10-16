namespace Imigrei.Application.Common.Relatorios;

/// <summary>
/// O banco guarda instantes em UTC; quem lê o relatório pensa em dias de
/// Brasília. Toda conversão entre os dois passa por aqui.
/// </summary>
public static class HorarioBrasilia
{
    private static readonly TimeZoneInfo Fuso = ResolverFuso();

    public static DateTime InicioDoDiaEmUtc(DateOnly dia)
        => TimeZoneInfo.ConvertTimeToUtc(dia.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified), Fuso);

    /// <summary>
    /// Converte um instante lido do banco. O provedor devolve
    /// <see cref="DateTimeKind.Unspecified"/>, mas o valor foi gravado em UTC.
    /// </summary>
    public static DateTime DeUtc(DateTime utc)
        => TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Fuso);

    public static DateOnly Hoje()
        => DateOnly.FromDateTime(DeUtc(DateTime.UtcNow));

    private static TimeZoneInfo ResolverFuso()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.CreateCustomTimeZone(
                "America/Sao_Paulo", TimeSpan.FromHours(-3), "Brasília", "Brasília");
        }
    }
}
