using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ####
{
    public interface IVenuesService
    {
        public int AddVenue(VenueAddRequest request, int createdBy);

        public void UpdateVenue(VenueUpdateRequest request, int modifiedBy);

        public Paged<Venue> GetAllPaginated(int pageIndex, int pageSize);

        public Paged<Venue> GetVenueByCreatedByPaginated(int createdBy, int pageIndex, int pageSize);

        public Paged<Venue> VenueSearchByNamePaginated(int pageIndex, int pageSize, string query);

        public Venue GetByVenueId(int id);

        public void DeleteById(int id);
    }
}
