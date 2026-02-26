using System.Collections.ObjectModel;

namespace PropertyPatterns.Models;

/// <summary>
/// Immutability(değişmezlik) odaklı model.
/// 1- reguired - C# 11 - object initializer'da atanması zorunlu alanlar.
/// 2- init     - C# 9  -  nesne oluşturulurken atanır, sonradan değiştirilemez.
/// 3- readonly - C# 1  - alan seviyesinde immutabilty
/// 4- get-only prop   C# 6 - yalnızca constructor'da veya field initializer'da set
/// 
/// Ne zaman tercih edilmeli?
/// Thread-sade veri transferi, DTO/value object tasarımı,
/// side-effect'siz domain modelleri.
/// </summary>
public class ImmutableModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateOnly BirthDate {get; init; }

    public string? Notes {get; init; }

    // readonly field + get-only property;
    private readonly Guid _id = Guid.NewGuid();
    public Guid Id => _id;


    public string FullName => $"{FirstName} {LastName}";

    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - BirthDate.Year;
            if(BirthDate > today.AddYears(-age)) age--;
            return age;
        }
    }

    // - Defensive copy
    // Mutable koleksiyonlar için immutability sağlamak
    private readonly IReadOnlyList<string> _tags;
    public IReadOnlyList<string> Tags => _tags;

    public ImmutableModel(DateOnly birthDate, IEnumerable<string>? tags = null)
    {
        BirthDate = birthDate;
        // Defensive copy: dışarıdaki liste değişse bile iç kopyamız etkilenmez.
        _tags = tags?.ToList().AsReadOnly() ?? ReadOnlyCollection<string>.Empty;
    }
}

/// <summary>
/// readonly struct - stack-allocated, tamamen immutable değer tipi
/// </summary>
public readonly struct Money
{
    public decimal Amount {get; init; }
    public string Currency {get; init; }

    public Money(decimal amount, string currency)
    {
        if(amount < 0 ) throw new ArgumentOutOfRangeException(nameof(amount));
        if(string.IsNullOrWhiteSpace(currency)) throw new ArgumentException(nameof(currency));
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if(Currency != other.Currency)
            throw new InvalidOperationException("Para birimi uyuşmuyor");
        return new Money(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}