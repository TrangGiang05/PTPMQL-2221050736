using Bogus;
using FirstWebMVC.Models.Entities;
namespace FirstWebMVC.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Books.Any())
            {
                return;
            }
            var categories = new[]
            {
                "Programming",
                "Database",
                "AI",
                "Networking",
                "DevOps",
                "Cyber Security",
                "Cloud Computing"
            };
            var faker = new Faker<Book>()
                .RuleFor(x => x.ISBN,
                    f => $"978-{f.Random.Number(1000, 9999)}")

                .RuleFor(x => x.Title,
                    f => f.Commerce.ProductName())

                .RuleFor(x => x.Author,
                    f => f.Name.FullName())

                .RuleFor(x => x.Publisher,
                    f => f.Company.CompanyName())

                .RuleFor(x => x.PublishYear,
                    f => f.Random.Int(2010, 2025))

                .RuleFor(x => x.Price,
                    f => f.Random.Decimal(100000, 1000000))

                .RuleFor(x => x.Quantity,
                    f => f.Random.Int(1, 100))

                .RuleFor(x => x.Category,
                    f => f.PickRandom(categories))

                .RuleFor(x => x.Description,
                    f => f.Lorem.Paragraph())

                .RuleFor(x => x.CreatedDate,
                    f => f.Date.Recent(365))

                .RuleFor(x => x.IsAvailable,
                    f => f.Random.Bool());

            // Sinh 500 bản ghi
            var books = faker.Generate(500);

            context.Books.AddRange(books);

            context.SaveChanges();
        }
    }
}