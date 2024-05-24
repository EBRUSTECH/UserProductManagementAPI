

using Microsoft.AspNetCore.Identity;

namespace UserProduct.Domain.Entities
{
    public class User : IdentityUser, IAuditable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
