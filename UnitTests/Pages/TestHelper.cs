using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using Moq;

using RamenRatings.WebSite.Services;
using System.IO;
using System.Text;

namespace UnitTests.Pages
{
    /// <summary>
    /// Helper to configure context and services for Razor Page unit tests.
    /// </summary>
    public static class TestHelper
    {
        public static Mock<IWebHostEnvironment> MockWebHostEnvironment;
        public static IUrlHelperFactory UrlHelperFactory;
        public static DefaultHttpContext HttpContextDefault;
        public static IWebHostEnvironment WebHostEnvironment;
        public static ModelStateDictionary ModelState;
        public static ActionContext ActionContext;
        public static EmptyModelMetadataProvider ModelMetadataProvider;
        public static ViewDataDictionary ViewData;
        public static TempDataDictionary TempData;
        public static PageContext PageContext;
        public static JsonFileProductService ProductService;

        public static IUrlHelper UrlHelper { get; set; }

        /// <summary>
        /// Static constructor to initialize test helpers.
        /// </summary>
        static TestHelper()
        {
            // Setup mock web host environment
            MockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            MockWebHostEnvironment.Setup(m => m.EnvironmentName).Returns("Hosting:UnitTestEnvironment");
            MockWebHostEnvironment.Setup(m => m.WebRootPath).Returns(TestFixture.DataWebRootPath);
            MockWebHostEnvironment.Setup(m => m.ContentRootPath).Returns(TestFixture.DataContentRootPath);

            // Setup HttpContext
            HttpContextDefault = new DefaultHttpContext
            {
                TraceIdentifier = "trace"
            };

            // ModelState and ActionContext
            ModelState = new ModelStateDictionary();
            ActionContext = new ActionContext(HttpContextDefault, HttpContextDefault.GetRouteData(), new PageActionDescriptor(), ModelState);

            // ViewData and TempData
            ModelMetadataProvider = new EmptyModelMetadataProvider();
            ViewData = new ViewDataDictionary(ModelMetadataProvider, ModelState);
            TempData = new TempDataDictionary(HttpContextDefault, Mock.Of<ITempDataProvider>());

            // PageContext
            PageContext = new PageContext(ActionContext)
            {
                ViewData = ViewData,
                HttpContext = HttpContextDefault
            };

            // ProductService
            ProductService = new JsonFileProductService(MockWebHostEnvironment.Object);

            // URL Helper
            UrlHelper = new UrlHelper(ActionContext);
        }

        /// <summary>
        /// Generates a mock IFormFile for simulating image uploads in tests.
        /// </summary>
        public static IFormFile GetMockImageFile(int number, string fileName = null)
        {
            var content = "Fake image content for test";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileName ??= $"{number}.jpg";

            return new FormFile(stream, 0, stream.Length, "Image", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }
}
