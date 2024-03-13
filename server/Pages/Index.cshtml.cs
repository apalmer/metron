using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reactive.Subjects;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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

        public async Task<IActionResult> OnGet(string? id, string? created_at, string? user_id, string? data_schema_name, string? data_schema_version, string? order)
        {
            var url = _configuration["SUPABASE_URL"];
            var key = _configuration["SUPABASE_KEY"];
            var email = _configuration["SUPABASE_USER_EMAIL"];
            var password = _configuration["SUPABASE_USER_PASSWORD"];

            var supabase = new Supabase.Client(url, key);
            await supabase.InitializeAsync();

            var session = await supabase.Auth.SignIn(email, password);

            var filters = new Dictionary<string, string>();

            if(id != null)
            {
                filters.Add("id", id);
            }

            if (created_at != null)
            {
                filters.Add("created_at", created_at);
            }

            if (user_id != null)
            {
                filters.Add("user_id", user_id);
            }

            if (data_schema_name != null)
            {
                filters.Add("data_schema_name", data_schema_name);
            }

            if (data_schema_version != null)
            {
                filters.Add("data_schema_version", data_schema_version);
            }

            var table = supabase.From<Models.Ingestion>();

            foreach (var filterKey in filters.Keys)
            {
                var critera = filters[filterKey].Split(".");
                var operation = critera[0].ToUpper();
                var operand = critera[1];

                if (operation == "LIKE" &&  !String.IsNullOrWhiteSpace(operand))
                {
                    table.Filter(filterKey, Postgrest.Constants.Operator.Like, operand);
                }
            }

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
