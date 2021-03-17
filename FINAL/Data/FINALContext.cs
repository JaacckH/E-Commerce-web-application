using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FINAL.Data
{
    public class FINALContext : DbContext
    {
        public FINALContext(DbContextOptions<FINALContext> options)
            : base(options)
        {
        }
    }
}
