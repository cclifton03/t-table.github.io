import React, { useEffect, useState } from "react";
import "../venues/venuesadmin.css";
import VenueSearch from "components/venues/VenueSearch";
import venuesService from "services/venuesService";
import VenueAdminTableRow from "./VenueAdminTableRow";
import Pagination from "rc-pagination";
import locale from "rc-pagination/lib/locale/en_US";
import toastr from "toastr";

const _logger = debug.extend("VenueAdminTable");

function VenueAdminTable() {
  const [venueData, setVenueData] = useState({
    venueArray: [],
    venueComponent: [],
  });

  const [pagination, setPagination] = useState({
    current: 1,
    pageIndex: 0,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
  });

  const [tracker, setTracker] = useState("");

  //Pagination, Search
  useEffect(() => {
    if (tracker === "") {
      venueGetter(pagination.pageIndex, pagination.pageSize);
    } else {
      venuesService
        .SearchByName(pagination.pageIndex, pagination.pageSize, tracker)
        .then(onSearchByNameSuccess)
        .catch(onSearchByNameError);
    }
  }, [pagination.pageSize, pagination.pageIndex]);

  const onSearchByNameSuccess = (response) => {
    const responseArray = response.item.pagedItems;

    setVenueData((prevState) => {
      const newState = { ...prevState };
      newState.venueArray = responseArray;
      newState.venueComponent = newState.venueArray.map(mapVenues);

      return newState;
    });
  };

  const onSearchByNameError = (error) => {
    _logger("onSearchByNameError error::::", error);
    toastr.error("User not found", error);
  };

  const venueGetter = (pageIndex, pageSize) => {
    venuesService
      .GetAllPaginated(pageIndex, pageSize)
      .then(onGetAllPaginatedSuccess)
      .catch(onGetAllPaginatedError);
  };

  const onGetAllPaginatedSuccess = (response) => {
    const totalCount = response.item.totalCount;
    const pageSize = response.item.pageSize;
    const totalPages = response.item.totalPages;

    setPagination((prevState) => {
      const paginState = { ...prevState };
      paginState.totalCount = totalCount;
      paginState.pageSize = pageSize;
      paginState.totalPages = totalPages;
      return paginState;
    });

    const responseArray = response.item.pagedItems;

    setVenueData((prevState) => {
      const newState = { ...prevState };
      newState.venueArray = responseArray;
      newState.venueComponent = newState.venueArray.map(mapVenues);

      return newState;
    });
  };

  const onGetAllPaginatedError = (error) => {
    _logger("onGetAllPaginatedError error::::", error);
    toastr.error(error);
  };

  const onPageChange = (page) => {
    setPagination((prevState) => {
      const newPaginationState = { ...prevState };
      newPaginationState.current = page;
      newPaginationState.pageIndex = page - 1;
      return newPaginationState;
    });

    if (tracker === "") {
      venueGetter(page - 1, pagination.pageSize);
    } else {
      venuesService
        .SearchByName(page - 1, pagination.pageSize, tracker)
        .then(onSearchByNameSuccess)
        .catch(onSearchByNameError);
    }
  };

  //End of Pagination, Search

  const onDeleteBtnClick = (venueObj) => {
    venuesService
      .deleteById(venueObj.id)
      .then(() => onDeleteByVenueIdSuccess(venueObj.id))
      .catch(onDeleteByVenueByIdError);
  };

  const onDeleteByVenueIdSuccess = (deletedVenueId) => {
    toastr.success("Item Deleted");

    setVenueData((prevState) => {
      const newState = { ...prevState };
      newState.venueArray = prevState.venueArray.filter(
        (venue) => venue.id !== deletedVenueId
      );
      newState.venueComponent = newState.venueArray.map(mapVenues);
      return newState;
    });

    setPagination((prevState) => {
      const newPaginationState = { ...prevState };
      newPaginationState.totalCount -= 1;
      newPaginationState.totalPages = Math.ceil(
        newPaginationState.totalCount / newPaginationState.pageSize
      );
      if (
        newPaginationState.totalCount > 0 &&
        newPaginationState.current > newPaginationState.totalPages
      ) {
        newPaginationState.current = newPaginationState.totalPages;
        newPaginationState.pageIndex = newPaginationState.totalPages - 1;
        venueGetter(newPaginationState.pageIndex, newPaginationState.pageSize);
      }
      return newPaginationState;
    });
  };

  const onDeleteByVenueByIdError = (error) => {
    _logger("onDeleteByVenueByIdError", error);
    toastr.error(error);
  };
  //End of Delete

  //Handlers passed as props

  const handleVenueDataSearchResults = (searchResultsArray) => {
    setVenueData((prevState) => {
      const newState = { ...prevState };
      newState.venueArray = searchResultsArray;
      newState.venueComponent = newState.venueArray.map(mapVenues);
      return newState;
    });
  };

  const handleSearchQueryReset = () => {
    setTracker("");

    setPagination((prevState) => {
      const paginState = { ...prevState };
      pagination.current = 0;
      return paginState;
    });

    venuesService
      .GetAllPaginated(0, 10)
      .then(onGetAllPaginatedSuccess)
      .catch(onGetAllPaginatedError);
  };

  const handlePaginationChange = (response, searchQuery) => {
    setTracker(searchQuery);

    setPagination((prevState) => {
      const newState = { ...prevState };
      newState.totalCount = response.item.totalCount;
      newState.pageSize = response.item.pageSize;
      newState.totalPages = response.item.totalPages;

      return newState;
    });
  };
  //Map, Pass Props
  const mapVenues = (venueObj) => {
    return (
      <VenueAdminTableRow
        onDeleteBtnClick={onDeleteBtnClick}
        venueObj={venueObj}
        key={"ListA-" + venueObj.id}
      ></VenueAdminTableRow>
    );
  };
  //End of Map, PassProps

  return (
    <React.Fragment>
      <div className="d-flex align-items-center justify-content-center">
        <h5 className="fs-6">Venues</h5>
      </div>

      <div className="venue-admin-table-container rounded container px-5 pb-5">
        {/* <div className="row d-flex venue-admin-table-row1"> */}
        <div className="venue-search-row row justify-content-center align-items-center">
          <div className="col d-flex py-2">
            <VenueSearch
              pagination={pagination}
              handlePaginationChange={handlePaginationChange}
              venueData={venueData}
              handleVenueDataSearchResults={handleVenueDataSearchResults}
              handleSearchQueryReset={handleSearchQueryReset}
              className="col-8"
            ></VenueSearch>

            <Pagination
              className="venue-admin-table-pagination m-0 col-4 d-flex"
              locale={locale}
              onChange={onPageChange}
              current={pagination.current}
              pageIndex={pagination.pageIndex}
              pageSize={pagination.pageSize}
              total={pagination.totalCount}
            />

            {/* </div> */}
          </div>
        </div>
        <div className="row">
          <table className="venue-admin-table">
            <thead className="venue-admin-table-thead">
              <tr className="venue-admin-table-tr-1">
                <th className="item1 border border-light p-2 text-white">
                  User
                </th>
                <th className="item2 border border-light p-2 text-white">
                  Venue Name
                </th>
                <th className="item3 border border-light p-2 text-white">
                  Venue Type
                </th>
                <th className="item4 border border-light p-2 text-white">
                  Location
                </th>
                <th className="item-last border border-light p-2 text-white"></th>
              </tr>
            </thead>
            <tbody className="venue-admin-table-tbody">
              {venueData.venueComponent}
            </tbody>
          </table>
        </div>
      </div>
    </React.Fragment>
  );
}

export default VenueAdminTable;
