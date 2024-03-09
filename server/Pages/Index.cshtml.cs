using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reactive.Subjects;

namespace Metron.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IEnumerable<Models.Ingestion>? Ingestions { get; set; } 

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Ingestions = new List<Models.Ingestion>();
        }

        public async Task<IActionResult> OnGet(string? som)
        {
            var url = _configuration["SUPABASE_URL"];
            var key = _configuration["SUPABASE_KEY"];
            var email = _configuration["SUPABASE_USER_EMAIL"];
            var password = _configuration["SUPABASE_USER_PASSWORD"];

            var supabase = new Supabase.Client(url, key);
            await supabase.InitializeAsync();

            var session = await supabase.Auth.SignIn(email, password);

            var results = await supabase.From<Models.Ingestion>().Get();

            Ingestions = results.Models.ToList();

            return Page();
        }
    }
}
