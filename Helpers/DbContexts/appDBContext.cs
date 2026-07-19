using Microsoft.EntityFrameworkCore;
using hariloom.Models.Entity;

namespace hariloom.Helpers.DbContexts
{
    public class appDBContext : DbContext
    {
        public appDBContext(DbContextOptions<appDBContext> options) : base(options) { }

        public DbSet<mstUser> mstUser { get; set; }
        public DbSet<mstProductMainCategory> mstProductMainCategory { get; set; }
        public DbSet<mstProductSubCategory> mstProductSubCategory { get; set; }
        public DbSet<mstProduct> mstProduct { get; set; }
        public DbSet<trnProductTags> trnProductTags { get; set; }
        public DbSet<trnProductSpecification> trnProductSpecification { get; set; }
        public DbSet<trnProductColor> trnProductColor { get; set; }
        public DbSet<trnProductSize> trnProductSize { get; set; }
        public DbSet<trnOrder> trnOrder { get; set; }
        public DbSet<trnOrderItems> trnOrderItem { get; set; }
        public DbSet<trnWishlist> trnWishlist { get; set; }
        public DbSet<trnCart> trnCart { get; set; }
        public DbSet<trnPaymentLog> trnPaymentLog { get; set; }
        public DbSet<trnUserAddress> trnUserAddress { get; set; }
    }
}
