using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.WebApi;
using StartMeUp.App_Code;
using Newtonsoft.Json;
using Umbraco.Core.Logging;
using System.Configuration;

namespace StartMeUp.App_Code
{
    public class LocationsController : UmbracoApiController
    {
        /// <summary>
        /// Gets the root document type alias from appSettings
        /// </summary>
        /// <value>
        /// The root document type alias.
        /// </value>
        public string RootDocTypeAlias
        {
            get
            {
                return ConfigurationManager.AppSettings["PostcodesRootDocTypeAlias"].ToString() ?? "";
            }
        }

        /// <summary>
        /// Builds the all locations and returns as JSON to be consumed client side.
        /// </summary>
        /// <returns></returns>
        public string GetLocations()
        {
            try
            {
                var foo = Umbraco.TypedContentAtRoot().Where(x => x.DocumentTypeAlias == this.RootDocTypeAlias).First();

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

        public string GetDefaultLocation()
        {
            try
            {
                var nodeId = Umbraco.TypedContentAtRoot().Where(x => x.DocumentTypeAlias == RootDocTypeAlias).First().GetProperty("defaultLocation").Value;
                var def = Umbraco.TypedContent(nodeId);

                Location defaultLocation = new Location();
                defaultLocation.Name = def.GetProperty("city").Value.ToString();
                defaultLocation.Longitude = def.GetProperty("longitude").Value.ToString() ?? "";
                defaultLocation.Postcode = def.GetProperty("postcode").Value.ToString() ?? "";
                defaultLocation.Latitude = def.GetProperty("latitude").Value.ToString() ?? "";
                defaultLocation.PhoneNumber = def.GetProperty("PhoneNumber").Value.ToString() ?? "";
                
                return JsonConvert.SerializeObject(defaultLocation);
            }
            catch(Exception e)
            {
                LogHelper.Error(typeof(LocationsController), "Error getting default office", e);
                return "Error!";
            }
        }
    }
}