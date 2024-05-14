import React, { useState } from "react";
import debug from "sabio-debug";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faSearch } from "@fortawesome/free-solid-svg-icons";
import venuesService from "services/venuesService";
import PropTypes from "prop-types";
import toastr from "toastr";

const _logger = debug.extend("VenueSearch");

toastr.options = {
  positionClass: "toast-top-right",
  hideDuration: 300,
  timeOut: 5000,
  closeButton: true,
};

function VenueSearch({
  handlePaginationChange,
  handleVenueDataSearchResults,
  handleSearchQueryReset,
}) {
  const [searchQuery, setSearchQuery] = useState({
    searchInput: "",
  });

  const onInputChange = (e) => {
    const searchInputVal = e.target.value;
    setSearchQuery(searchInputVal);
  };

  const onSearchClicked = () => {
    if (searchQuery.searchInput !== "") {
      venuesService
        .SearchByName(0, 10, searchQuery)
        .then(onSearchByNameSuccess)
        .catch(onSearchByNameError);
    }
  };

  const onSearchByNameSuccess = (response) => {
    _logger("onSearchByNameSuccess", response);
    toastr.success("Success");

    const searchResultsArray = response.item.pagedItems;
    handleVenueDataSearchResults(searchResultsArray);

    handlePaginationChange(response, searchQuery);
  };

  const onSearchByNameError = (error) => {
    _logger("onSearchByNameError", error);
    toastr.error("We could not find a user by that name");
  };

  const onResetClicked = () => {
    setSearchQuery((prevState) => {
      const newQueryState = { ...prevState };
      newQueryState.searchInput = "";
      return newQueryState;
    });
    handleSearchQueryReset();
  };

  const handleEnterPress = (e) => {
    if (e.key === "Enter") {
      _logger("enter pressed!");
      onSearchClicked();
    }
  };

  return (
    <React.Fragment>
      <div className="venue-search-container d-flex justify-content-center align-items-center">
        <input
          type="text"
          className="searchInput rounded"
          placeholder="Enter user name"
          aria-label="Search"
          onChange={onInputChange}
          value={searchQuery.searchInput}
          onKeyPress={handleEnterPress}
        />
        <FontAwesomeIcon
          icon={faSearch}
          className="venue-search-icon ms-n4"
          onClick={onSearchClicked}
        />

        <button
          type="button"
          className="reset-search-btn btn btn-warning ms-5"
          onClick={onResetClicked}
        >
          Reset
        </button>
      </div>
    </React.Fragment>
  );
}

VenueSearch.propTypes = {
  pagination: PropTypes.shape({
    current: PropTypes.number.isRequired,
    pageIndex: PropTypes.number.isRequired,
    pageSize: PropTypes.number.isRequired,
    totalCount: PropTypes.number.isRequired,
    totalPages: PropTypes.number.isRequired,
  }),
  handlePaginationChange: PropTypes.func.isRequired,
  handleVenueDataSearchResults: PropTypes.func.isRequired,
  handleSearchQueryReset: PropTypes.func.isRequired,
};

export default VenueSearch;
