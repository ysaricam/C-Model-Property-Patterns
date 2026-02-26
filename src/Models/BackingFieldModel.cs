namespace PropertyPatterns.Models;
/// <summary>
/// Geleneksel backing field yaklaşımı.
/// Bir property'ye erişimden önce/sonra validasyon veya yan etki uygulamak istediğinde kullanılır.
/// C# 1.0+ ile kullanılabilir; günümüzde hala en yaygın validasyon yöntemidir.
/// </summary>
public class BackingFieldModel
{
    
    private int _age;
    private string _name = string.Empty;
    private decimal _salary;

    public int Age
    {
        get => _age;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Yaş negatif olamaz");
            _age = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("İsim alanı boş olamaz.", nameof(value));
            _name = value.Trim();
        }
    }

    public decimal Salary
    {
        get => _salary;
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Maaş negatif değer alamaz.");
            
            decimal old = _salary;
            _salary = value;

            if(old != value)
                SalaryChanged?.Invoke(this, new SalaryChangedEventArgs(old, value));
        }
    }

    /// <summary>
    /// Maaş değiştiğinde tetiklenen event
    /// </summary>
    public event EventHandler<SalaryChangedEventArgs>? SalaryChanged;

    public string DisplayName => $"{Name} (Yaş: {Age}) ";

    public BackingFieldModel(string name, int age, decimal salary)
    {
        Name = name;
        Age = age;
        Salary = salary;
    }
}

public sealed class SalaryChangedEventArgs(decimal oldSalary, decimal newSalary) : EventArgs
{
    public decimal OldSalary {get; } = oldSalary;
    public decimal NewSalary {get; } = newSalary;
}