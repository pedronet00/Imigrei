using Imigrei.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Imigrei.Domain.ValueObjects;

public sealed class CPF
{
    public string Valor { get; } // formato normalizado: 000.000.000-00

    private CPF(string valor) => Valor = valor;

    public static CPF Criar(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        var digits = Regex.Replace(input, @"\D", "");

        if (digits.Length != 11)
            throw new DomainException($"CPF inválido: '{input}'. Deve conter 11 dígitos.");

        if (digits.Distinct().Count() == 1)
            throw new DomainException($"CPF inválido: '{input}'. Todos os dígitos são iguais.");

        if (!ValidarDigito(digits, 9) || !ValidarDigito(digits, 10))
            throw new DomainException($"CPF inválido: '{input}'. Dígitos verificadores incorretos.");

        var formatado = $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}";
        return new CPF(formatado);
    }

    private static bool ValidarDigito(string digits, int posicao)
    {
        // peso começa em (posicao + 1) e decrementa
        var soma = digits.Take(posicao)
                         .Select((c, i) => (c - '0') * (posicao + 1 - i))
                         .Sum();

        var resto = soma % 11;
        var digitoEsperado = resto < 2 ? 0 : 11 - resto;

        return (digits[posicao] - '0') == digitoEsperado;
    }

    public override bool Equals(object? obj) =>
        obj is CPF outro && Valor == outro.Valor;

    public override int GetHashCode() => HashCode.Combine(Valor);

    public override string ToString() => Valor;

    public static bool operator ==(CPF? a, CPF? b) => Equals(a, b);
    public static bool operator !=(CPF? a, CPF? b) => !Equals(a, b);
}