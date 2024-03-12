using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reactive.Subjects;
using System.Collections.Generic;

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

        public async Task<IActionResult> OnGet(string? order)
        {
            var url = _configuration["SUPABASE_URL"];
            var key = _configuration["SUPABASE_KEY"];
            var email = _configuration["SUPABASE_USER_EMAIL"];
            var password = _configuration["SUPABASE_USER_PASSWORD"];

            var supabase = new Supabase.Client(url, key);
            await supabase.InitializeAsync();

            var session = await supabase.Auth.SignIn(email, password);

            var table = supabase.From<Models.Ingestion>();
            if (order != null)
            {
                var cases = order.Split(',');
                foreach (var c in cases)
                {
                    var criteria = c.Split('.');
                    var orderDirection = criteria[1] == "desc" ? Postgrest.Constants.Ordering.Descending : Postgrest.Constants.Ordering.Ascending;
                    table.Order(criteria[0], orderDirection);
                }
            }

            var results = await table.Get();

            Ingestions = results.Models;

            return Page();
        }
    }
}
