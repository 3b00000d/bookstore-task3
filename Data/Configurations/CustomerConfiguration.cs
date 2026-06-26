using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookStore.Models;

namespace BookStore.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // بنعمل Index على عمود الإيميل ونقوله إنه لازم يكون Unique (فريد ميتكررش)
            builder.HasIndex(c => c.Email).IsUnique();
        }
    }
}