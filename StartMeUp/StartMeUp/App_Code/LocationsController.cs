using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using StartMeUp.App_Code;
using Newtonsoft.Json;
using Umbraco.Core.Logging;

namespace StartMeUp.App_Code
{
    public class LocationsController : UmbracoApiController
    {
        public string GetLocations()
        {
            try
            {
                var foo = Umbraco.TypedContentAtRoot().Where(x => x.DocumentTypeAlias == "settings").First();

                List<Location> allOffices = new List<Location>();

                foreach (var location in foo.Children)
                {
                    allOffices.Add(new Location()
                    {
                        Name = location.GetProperty("city").Value.ToString(),
                        Longitude = location.GetProperty("longitude").Value.ToString(),
                        Latitude = location.GetProperty("latitude").Value.ToString(),
                        PhoneNumber = location.GetProperty("phoneNumber").Value.ToString(),
                        Postcode = location.GetProperty("postcode").Value.ToString()
                    });
                }

                var json = JsonConvert.SerializeObject(allOffices);
                return json;
            }
            catch(Exception e)
            {
                LogHelper.Error(typeof(LocationsController), "Error getting location information", e);
                return "Error!";
            }
        }
    }
}