using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Venues;
using Sabio.Models.Requests.VenueRequests;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using Stripe.Forwarding;
using System;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/venues")]
    [ApiController]
    public class VenueApiController : BaseApiController
    {
        private IVenuesService _service = null;
        private IAuthenticationService<int> _authService = null;

        public VenueApiController(IVenuesService service,
            IAuthenticationService<int> authService,
            ILogger<VenueApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        #region Post Venue
        [HttpPost]
        public ActionResult<ItemResponse<int>> AddVenue(VenueAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int createdBy = _authService.GetCurrentUserId();
                int id = _service.AddVenue(model, createdBy);

                ItemResponse<int> response = new ItemResponse<int> { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, response);

            }

            return result;

        }
        #endregion

        #region Put Venue
        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> UpdateVenue(VenueUpdateRequest model)
        {
            ObjectResult result = null;

            try
            {
                int modifiedBy = _authService.GetCurrentUserId();
                _service.UpdateVenue(model, modifiedBy);

                SuccessResponse response = new SuccessResponse();
                result = Ok(response);
            }

            catch (Exception ex)
            {
                ErrorResponse response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
                result = StatusCode(500, response);
            }

            return result;
        }

        #endregion

        #region Paginated
        [HttpGet("paginate")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<Venue>>> GetAllPaginated(int pageIndex, int pageSize)
        {
            ActionResult result = null;

            try
            {
                Paged<Venue> paged = _service.GetAllPaginated(pageIndex, pageSize);

                if(paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found"));
                }
                else
                {
                    ItemResponse<Paged<Venue>> response = new ItemResponse<Paged<Venue>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.ToString()));
            }

            return result;
        }

        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<Venue>>> VenueSearchByNamePaginated(int pageIndex, int pageSize, string query)
        {
            ActionResult result = null;

            try
            {
                Paged<Venue> paged = _service.VenueSearchByNamePaginated(pageIndex, pageSize, query);

                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found"));
                }
                else
                {
                    ItemResponse<Paged<Venue>> response = new ItemResponse<Paged<Venue>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.ToString()));
            }

            return result;
        }

        [HttpGet("createdby/paginate")]
        public ActionResult<ItemResponse<Paged<Venue>>> GetByCreatedByPaginated(int createdBy, int pageIndex, int pageSize)
        {
            ActionResult result = null;

            try
            {
                Paged<Venue> paged = _service.GetVenueByCreatedByPaginated(createdBy, pageIndex, pageSize);

                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found"));
                }
                else
                {
                    ItemResponse<Paged<Venue>> response = new ItemResponse<Paged<Venue>>();
                    response.Item = paged;
                    result = Ok200(response);
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.ToString()));
            }

            return result;
        }
        #endregion

        #region Get
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Venue>> GetByVenueId(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Venue venue = _service.GetByVenueId(id);

                if(venue == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemResponse<Venue> { Item = venue };
                }
            }

            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }
        #endregion

        #region Delete
        [HttpDelete("{id:int}")] 
        public ActionResult<SuccessResponse> DeleteById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.DeleteById(id);
                response = new SuccessResponse();
            }
            catch(Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.ToString());
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }
        #endregion
    }
}
