using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookStore.Models;

namespace BookStore.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // بنضيف شرط (Check Constraint) في الداتابيز يمنع إن السعر يكون أقل من صفر
            builder.ToTable(t => t.HasCheckConstraint("CK_Book_Price", "Price >= 0"));
        }
    }
}