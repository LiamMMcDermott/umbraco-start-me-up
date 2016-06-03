using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Core.Services;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Runtime.Serialization.Json;
using RestSharp;
using MarkEmbling.PostcodesIO;

namespace StartMeUp.App_Code
{
    public class CustomEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Saved += ContentService_Saved;
            ContentService.Saving += ContentService_Saving;
            ContentService.Deleted += ContentService_Deleted;
        }

        private void ContentService_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            foreach (var content in e.SavedEntities.Where(c => c.ContentType.Alias.Equals("BlogPost")))
            {
                try
                {
                    var newRef = content.GetValue("referenceCode").ToString();
                    var versionToUpdate = sender.GetByVersion(content.Version);
                    UpdateCustomColumn(versionToUpdate, newRef);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(CustomEventHandler), "1: Error updating JobReference in DB: ", ex);
                }
            }

            //maybe move this to it's own 
            foreach (var content in e.SavedEntities.Where(c => c.ContentType.Alias.Equals("office")))
            {
                var postcode = content.GetValue("postcode").ToString();
                try
                {
                    PostcodesIOClient client = new PostcodesIOClient();
                    var result = client.Lookup(postcode);

                    //now save the LonLat on the node.
                    content.SetValue("longitude", result.Longitude);
                    content.SetValue("latitude", result.Latitude);
                    
                }
                catch (Exception ex)
                {
                    LogHelper.Error(typeof(CustomEventHandler), "Error Converting LonLat from postcode", ex);
                    e.Cancel = true;
                }
            }
            //sender.Save(content);
        }
        
        private void ContentService_Saved(IContentService sender, SaveEventArgs<IContent> e)
        {
            
        }

        private void ContentService_Deleted(IContentService sender, DeleteEventArgs<IContent> e)
        {
            foreach (var content in e.DeletedEntities.Where(c => c.ContentType.Alias.Equals("BlogPost")))
            {
                if (!content.HasIdentity)
                {
                    var newRef = content.GetValue("referenceCode");
                }
            }
        }

        private void UpdateCustomColumn(IContent versionToUpdate, string newRef)
        {
            try
            {
                StringBuilder userAddress = new StringBuilder();

                SqlCeConnection conn = new SqlCeConnection(System.Configuration.ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString);

                conn.Open();

                string sql = "select count(*) numRows from cmsDocument where versionId = @versionId";

                using (SqlCeCommand comm = new SqlCeCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@versionId", versionToUpdate.Version);
                    SqlCeDataReader reader = comm.ExecuteReader();
                    int numRows = 0;

                    while (reader.Read())
                    {
                        numRows = reader.GetInt32(0);
                    }

                    reader.Close();

                    //should only be one Version 
                    if (numRows == 1)
                    {
                        using (SqlCeCommand commUpdate = new SqlCeCommand("UPDATE cmsDocument SET JobReference = @newRef where versionId = @versionId", conn))
                        {
                            commUpdate.Parameters.AddWithValue("@versionId", versionToUpdate.Version);
                            commUpdate.Parameters.AddWithValue("@newRef", newRef);
                            commUpdate.ExecuteNonQuery();
                        }
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(CustomEventHandler), "2: Error updating JobReference in DB: ", ex);
            }


        }

        
    }
}