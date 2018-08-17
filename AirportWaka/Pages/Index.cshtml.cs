using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace AirportWaka.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public string MapboxAccessToken { get; set; }
        public IndexModel(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            MapboxAccessToken = configuration["Mapbox:AccessToken"];
            _hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {

        }
        public IActionResult OnGetAirports()
        {
            var configuration = new Configuration
            {
                BadDataFound = context => { }
            };

            using (var sr = new StreamReader(Path.Combine(_hostingEnvironment.WebRootPath, "data/airports.dat")))
            using (var reader = new CsvReader(sr))
            {
                FeatureCollection featureCollection = new FeatureCollection();
                try
                {
                    while (reader.Read())
                    {
                        string name = reader.GetField<string>(1);
                        string iataCode = reader.GetField<string>(4);
                        double latitude = reader.GetField<double>(6);
                        double longitude = reader.GetField<double>(7);

                        featureCollection.Features.Add(new Feature(
                            new Point(new Position(latitude, longitude)),
                            new Dictionary<string, object>
                            {
                                {"name", name},
                                {"iataCode", iataCode}
                            }));
                    }
                }
                catch (Exception ex)
                {

                   
                }

               

                return new JsonResult(featureCollection);
            }
            
        }
    }
}
