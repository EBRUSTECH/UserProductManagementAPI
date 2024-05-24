using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProduct.Domain.Entities;

namespace UserProduct.Core.Abstractions
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
