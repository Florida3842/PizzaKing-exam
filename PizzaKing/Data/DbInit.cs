using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaKing.Models;
using PizzaKing.Models.Custom;

namespace PizzaKing.Data
{
    public class DbInit
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("Editor") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Editor"));
            }
            if (await roleManager.FindByNameAsync("Client") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Client"));
            }

            string adminEmail = "admin@gmail.com", adminPassword = "192837";
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    PhoneNumber = "380970601478",
                    Year = 1990,
                    City = "Днепр",
                    Address = "Титова 12, кв 33."
                };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                User alex = new User
                {
                    Email = "alex@gmail.com",
                    UserName = "alex@gmail.com",
                    PhoneNumber = "38096546798",
                    Year = 2001,
                    City = "Днепр",
                    Address = "Карла Маркса 121, кв 32."
                };
                result = await userManager.CreateAsync(alex, "qwerty");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(alex, "Editor");
                }

                User tom = new User
                {
                    Email = "tom@gmail.com",
                    UserName = "tom@gmail.com",
                    PhoneNumber = "380665459874",
                    Year = 1995,
                    City = "Днепр",
                    Address = "Тополь 3, дом 44 кв 7."
                };
                result = await userManager.CreateAsync(tom, "1234567AS");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(tom, "Client");
                }

                User marry = new User
                {
                    Email = "marry@in.ua",
                    UserName = "marry@in.ua",
                    PhoneNumber = "380964578796",
                    Year = 1981,
                    City = "Киев",
                    Address = "Шевеченко, дом 7 кв 14."
                };
                result = await userManager.CreateAsync(marry, "2S91lds");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(marry, "Client");
                }
            }
        }
        public static async Task InitializeContentAsync(ApplicationContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddRangeAsync(
                    new Category { Name = "Пицца" },
                    new Category { Name = "Напитки" },
                    new Category { Name = "Салаты" }
                );
                await context.SaveChangesAsync();
            }

            // Получаем категории из базы
            var pizzaCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Пицца");
            var drinksCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Напитки");
            var saladCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Салаты");

            // Добавляем товары, если таблица пустая
            if (!await context.Products.AnyAsync())
            {
                await context.Products.AddRangeAsync(
                    new Product
                    {
                        Name = "Пеперони",
                        Description = "Пепперони, сыр Моцарелла, соус фирменный томатный, специи.",
                        Weight = 460,
                        Calories = 1160,
                        Price = 140,
                        Category = pizzaCategory,
                        Type = ProductType.Dish,
                        DateOfPublication = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Чикен Чиз",
                        Description = "Kurka, sire mozzarella, sire cheddar, cucurudza, verticillium sauce.",
                        Weight = 520,
                        Calories = 1220,
                        Price = 155,
                        Category = pizzaCategory,
                        Type = ProductType.Dish,
                        DateOfPublication = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Спрайт",
                        Description = "Всегда освежает и бодрит!",
                        Weight = 500,
                        Calories = 225,
                        Price = 24,
                        Category = drinksCategory,
                        Type = ProductType.Drink,
                        DateOfPublication = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Мясная",
                        Description = "Ветчина, салями, бекон, сыр Моцарелла, помидор, маринованный лук, соус фирменный томатный.",
                        Weight = 570,
                        Calories = 1420,
                        Price = 179,
                        Category = pizzaCategory,
                        Type = ProductType.Dish,
                        DateOfPublication = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Цезарь",
                        Description = "Курица, бекон, сыр Моцарелла, помидор, листья салата, яйца, соус фирменный.",
                        Weight = 300,
                        Calories = 680,
                        Price = 95,
                        Category = saladCategory,
                        Type = ProductType.Dish,
                        DateOfPublication = DateTime.Now
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Ingredients.AnyAsync())
            {
                await context.Ingredients.AddRangeAsync(
                    // Crusts
                    new Ingredient { Name = "Тонкая основа", Type = IngredientType.Crust, Price = 0m },
                    new Ingredient { Name = "Традиционная основа", Type = IngredientType.Crust, Price = 20m },
                    new Ingredient { Name = "Цельнозерновая основа", Type = IngredientType.Crust, Price = 25m },
                    new Ingredient { Name = "Толстая основа (пан)", Type = IngredientType.Crust, Price = 30m },

                    // Sauces
                    new Ingredient { Name = "Томатный соус", Type = IngredientType.Sauce, Price = 0m },
                    new Ingredient { Name = "Чесночный соус", Type = IngredientType.Sauce, Price = 15m },
                    new Ingredient { Name = "Барбекю соус", Type = IngredientType.Sauce, Price = 20m },
                    new Ingredient { Name = "Белый сливочный соус", Type = IngredientType.Sauce, Price = 18m },

                    // Toppings
                    new Ingredient { Name = "Пепперони", Type = IngredientType.Topping, Price = 30m },
                    new Ingredient { Name = "Ветчина", Type = IngredientType.Topping, Price = 28m },
                    new Ingredient { Name = "Бекон", Type = IngredientType.Topping, Price = 32m },
                    new Ingredient { Name = "Курица", Type = IngredientType.Topping, Price = 30m },
                    new Ingredient { Name = "Грибы", Type = IngredientType.Topping, Price = 18m },
                    new Ingredient { Name = "Оливки", Type = IngredientType.Topping, Price = 15m },
                    new Ingredient { Name = "Помидоры", Type = IngredientType.Topping, Price = 12m },
                    new Ingredient { Name = "Ананас", Type = IngredientType.Topping, Price = 20m },
                    new Ingredient { Name = "Моцарелла", Type = IngredientType.Topping, Price = 35m },
                    new Ingredient { Name = "Чеддер", Type = IngredientType.Topping, Price = 35m },
                    new Ingredient { Name = "Перец халапеньо", Type = IngredientType.Topping, Price = 12m },
                    new Ingredient { Name = "Лук маринованный", Type = IngredientType.Topping, Price = 10m },
                    new Ingredient { Name = "Руккола (после выпечки)", Type = IngredientType.Topping, Price = 22m }
                );

                await context.SaveChangesAsync();
            }

            if (!await context.Reviews.AnyAsync())
            {
                await context.Reviews.AddRangeAsync(
                    new Review
                    {
                        Author = "Алексей",
                        Text = "Отличная пицца — корочка хрустящая, начинка свежая. Быстрая доставка, рекомендую!",
                        Rating = 5,
                        CreatedAt = DateTime.Now.AddDays(-10),
                    },
                    new Review
                    {
                        Author = "Ирина",
                        Text = "Вкусно, но немного не хватало соли в соусе. В целом — хорошее соотношение цены и качества.",
                        Rating = 4,
                        CreatedAt = DateTime.Now.AddDays(-7),
                    },
                    new Review
                    {
                        Author = "Михаил",
                        Text = "Очень понравился сервис — менеджер уточнил все детали заказа. Пицца горячая и ароматная.",
                        Rating = 5,
                        CreatedAt = DateTime.Now.AddDays(-3),
                    }
                );

                await context.SaveChangesAsync();
            }


        }
        public static async Task CreateSeedDataAsync(ApplicationContext context, int[] categories, int rowCount = 50, bool cleaData = true)
        {

            if (cleaData)
            {
                ClearData(context);
            }
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            context.Database.ExecuteSqlRaw("DROP PROCEDURE IF EXISTS CreateSeedData");
            context.Database.ExecuteSqlRaw($@"
        CREATE PROCEDURE CreateSeedData 
        @RowCount decimal,
        @pizzaCategoryId int,
        @saladCategoryId int,
        @drinkCategoryId int
        AS
        BEGIN
        SET NOCOUNT ON
        DECLARE @i INT = 0;

BEGIN TRANSACTION 

        WHILE @i < @RowCount
        BEGIN
        insert into Products (CategoryId, Image, Name, Description, Brand, Calories, Weight, Price, DateOfPublication, Type)
        values (@pizzaCategoryId, CONCAT('/productFiles/','pizza.png'), CONCAT('Pizza - ', @i), CONCAT('So tasty pizza - ', @i), 
        CONCAT('Brand - St', @i), RAND() * (500-50+1), RAND() * (200-50+1), RAND() * (300-70+1), 
        DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 3650), '2014-01-01'), 0);
        SET @i = @i + 1;
        END

        SET @i = 0;

        WHILE @i < @RowCount
        BEGIN
        insert into Products (CategoryId, Image, Name, Description, Brand, Calories, Weight, Price, DateOfPublication, Type)
        values (@saladCategoryId, CONCAT('/productFiles/','salad.png'), CONCAT('Salad - ', @i), CONCAT('Ooo`h so good salad - ', @i), 
        CONCAT('Brand - Ki', @i), RAND() * (500-50+1), RAND() * (200-50+1), RAND() * (300-70+1), 
        DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 3650), '2014-01-01'), 2);
        SET @i = @i + 1;
        END

        SET @i = 0;

        WHILE @i < @RowCount
        BEGIN
        insert into Products (CategoryId, Image, Name, Description, Brand, Calories, Weight, Price, DateOfPublication, Type)
        values (@drinkCategoryId, CONCAT('/productFiles/','drink.png'), CONCAT('Drink - ', @i), CONCAT('Drink fresh drinks - ', @i), 
        CONCAT('Brand - Jh', @i), RAND() * (500-50+1), RAND() * (200-50+1), RAND() * (300-70+1), 
        DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 3650), '2014-01-01'), 1);
        SET @i = @i + 1;
        END

        COMMIT
        END");
            context.Database.BeginTransaction();
            context.Database.ExecuteSqlRaw($"EXEC CreateSeedData @RowCount = {rowCount}," +
              $"@pizzaCategoryId = {categories[0]}, @saladCategoryId = {categories[1]}, @drinkCategoryId = {categories[2]}");
            context.Database.CommitTransaction();
        }
        private static void ClearData(ApplicationContext context)
        {
            context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            context.Database.BeginTransaction();
            context.Database.ExecuteSqlRaw("DELETE FROM Products");
            context.Database.CommitTransaction();
        }



    }
}
