using Amazon.S3.Model;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Locations;
using Sabio.Models.Domain.Venues;
using Sabio.Models.Requests.Licenses;
using Sabio.Models.Requests.VenueRequests;
using Sabio.Services.Interfaces;
using Stripe.Terminal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Sabio.Services
{
    public class VenuesService : IVenuesService
    {
        IDataProvider _data = null;
        ILookUpService _lookup = null;
        public VenuesService(IDataProvider data, ILookUpService lookup)
        {
            _data = data;
            _lookup = lookup;
        }

        #region Add Methods
        public int AddVenue(VenueAddRequest request, int createdBy)
        {
            int id = 0;

            string storedProc = "dbo.Venues_Insert";

            _data.ExecuteNonQuery(storedProc,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(request, collection);

                    collection.AddWithValue("@CreatedBy", createdBy);

                    collection.AddWithValue("@ModifiedBy", createdBy);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;

                    collection.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCol)
                {
                    object oId = returnCol["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                }
                );

            return id;
        }
        #endregion

        #region Update Methods
        public void UpdateVenue(VenueUpdateRequest request, int modifiedBy)
        {
            string storedProc = "dbo.Venues_Update";

            _data.ExecuteNonQuery(storedProc,
                inputParamMapper: delegate (SqlParameterCollection collection)
                {
                    AddCommonParams(request, collection);

                    collection.AddWithValue("@ModifiedBy", modifiedBy);
                    collection.AddWithValue("@Id", request.Id);

                },
                returnParameters: null);
        }
        #endregion

        #region Get Methods  
        public Paged<Venue> GetAllPaginated(int pageIndex, int pageSize)
        {
            string storedProc = "dbo.Venues_SelectAll";

            Paged<Venue> pagedList = null;
            List<Venue> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(storedProc,
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    Venue aVenue = new Venue();
                    int colIndex = 0;

                    colIndex = SingleVenueMapper(reader, aVenue, ref colIndex);

                    if (totalCount == 0) { 
                         totalCount = reader.GetSafeInt32(colIndex++); 
                    }

                    if (list == null)
                    {
                        list = new List<Venue>();
                    }

                    list.Add(aVenue);
                }
                );  

            if (list != null)
            {
                pagedList = new Paged<Venue>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Paged<Venue> GetVenueByCreatedByPaginated(int createdBy, int pageIndex, int pageSize)
        {
            string storedProc = "dbo.Venues_Select_ByCreatedBy";

            Paged<Venue> pagedList = null;
            List<Venue> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(storedProc,
               (param) =>
               {
                   param.AddWithValue("@CreatedBy", createdBy);
                   param.AddWithValue("@PageIndex", pageIndex);
                   param.AddWithValue("@PageSize", pageSize);
               },

                (reader, recordSetIndex) =>
                {
                    Venue aVenue = new Venue();
                    int colIndex = 0;

                    colIndex = SingleVenueMapper(reader, aVenue, ref colIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(colIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<Venue>();
                    }

                    list.Add(aVenue);
                });

            if (list != null)
            {
                pagedList = new Paged<Venue>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Venue GetByVenueId(int id)
        {
            Venue venue = null;

            string storedProc = "dbo.Venues_Select_ById";

            _data.ExecuteCmd(storedProc,
               (param) =>
               {
                   param.AddWithValue("@Id", id);
               },

                (reader, recordSetIndex) =>
                {
                    Venue aVenue = new Venue();
                    int colIndex = 0;

                    colIndex = SingleVenueMapper(reader, aVenue, ref colIndex);

                    venue = aVenue;
                });

            return venue;
        }

        public Paged<Venue> VenueSearchByNamePaginated( int pageIndex, int pageSize, string query)
        {
            string storedProc = "[dbo].[Venues_SearchByName_Pagination]";

            Paged<Venue> pagedList = null;
            List<Venue> list = null;

            int totalCount = 0;

            _data.ExecuteCmd(storedProc,
               (param) =>
               {
                   param.AddWithValue("@Query", query);
                   param.AddWithValue("@PageIndex", pageIndex);
                   param.AddWithValue("@PageSize", pageSize);
               },

                (reader, recordSetIndex) =>
                {
                    Venue aVenue = new Venue();
                    int colIndex = 0;

                    colIndex = SingleVenueMapper(reader, aVenue, ref colIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(colIndex++);
                    }

                    if (list == null)
                    {
                        list = new List<Venue>();
                    }

                    list.Add(aVenue);
                });

            if (list != null)
            {
                pagedList = new Paged<Venue>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }
        #endregion

        #region Delete Methods
        public void DeleteById(int id)
        {
            string storedProc = "dbo.Venues_Delete_ById";

            _data.ExecuteNonQuery(storedProc,
                (param) =>
                {
                    param.AddWithValue("@Id", id);
                });
        }

        #endregion

        private static void AddCommonParams(VenueAddRequest request, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Name", request.Name);
            collection.AddWithValue("@Description", request.Description);
            collection.AddWithValue("@LocationId", request.LocationId);
            collection.AddWithValue("@VenueTypeId", request.VenueTypeId);
            collection.AddWithValue("@FileId", request.FileId);
            collection.AddWithValue("@Url", request.Url);
           
        }

        private int SingleVenueMapper(IDataReader reader, Venue aVenue, ref int colIndex)
        {
            aVenue.Id = reader.GetSafeInt32(colIndex++);
            aVenue.Name = reader.GetSafeString(colIndex++);
            aVenue.Description = reader.GetSafeString(colIndex++);
            aVenue.LocationInfo = reader.DeserializeObject<BaseLocation>(colIndex++);
            aVenue.VenueType = _lookup.MapSingleLookUp(reader, ref colIndex);
            aVenue.FileImageUrl = reader.GetSafeString(colIndex++);
            aVenue.Url = reader.GetSafeString(colIndex++);
            aVenue.CreatedBy = reader.DeserializeObject<BaseUser>(colIndex++);
            aVenue.ModifiedBy = reader.DeserializeObject<BaseUser>(colIndex++);
            aVenue.DateCreated = reader.GetSafeDateTime(colIndex++);
            aVenue.DateModifed = reader.GetSafeDateTime(colIndex++);
            return colIndex;
        }

    }
}
