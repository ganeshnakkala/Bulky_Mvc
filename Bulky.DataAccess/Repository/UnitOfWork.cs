using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _db;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }

    public ICompanyRepository Company { get; private set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        //  ProductRepository productRepository = new ProductRepository(_db);
        Product = new ProductRepository(_db);
        Company = new CompanyRepository(_db);

    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
