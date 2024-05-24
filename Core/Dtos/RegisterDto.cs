using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProduct.Core.Dtos
{
    public class RegisterDto
    {
        [Required] public string FirstName { get; init; }
        [Required] public string LastName { get; init; }
        [Required][EmailAddress] public string Email { get; init; }
        [Required] public string Password { get; init; }
    }
}