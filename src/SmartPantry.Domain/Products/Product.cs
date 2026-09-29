using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace SmartPantry.Products;

public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    // Private constructor required by Entity Framework Core
    private Product() { }

    public Product(Guid id, string name) : base(id)
    {
        SetName(name);
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            maxLength: ProductConsts.MaxNameLength
        ).Trim();
    }
}