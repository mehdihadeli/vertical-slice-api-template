namespace Vertical.Slice.Template.EndToEndTests;

public class Constants
{
    public static class Routes
    {
        public static class Products
        {
            private const string RootBaseAddress = "api/v1";

            private const string ProductsBaseAddress = $"{RootBaseAddress}/products";

            public static string GetByPage => $"{ProductsBaseAddress}/";

            public static string GetById(Guid id) => $"{ProductsBaseAddress}/{id}";

            public static string Delete(long id) => $"{ProductsBaseAddress}/{id}";

            public static string Put(long id) => $"{ProductsBaseAddress}/{id}";

            public static string Create => $"{ProductsBaseAddress}/";
        }
    }
}
