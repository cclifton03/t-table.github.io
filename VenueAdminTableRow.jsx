import React from "react";
import PropTypes from "prop-types";
import Swal from "sweetalert2";

function VenueAdminTableRow({ venueObj, onDeleteBtnClick }) {
  const onDeleteClicked = (e) => {
    e.preventDefault();

    Swal.fire({
      title: "Are you sure?",
      text: "Once deleted, you will not be able to recover this venue!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonText: "Delete",
      buttons: true,
      cancelButtonText: "Cancel",
      dangerMode: true,
    }).then((willDelete) => {
      if (willDelete) {
        onDeleteBtnClick(venueObj, e);
        Swal.fire("The venue has been deleted!", {
          icon: "success",
        });
      }
    });
  };
  return (
    <React.Fragment>
      <tr className="venue-admin-table-tr-2">
        <td className="item1 createdBy border border-light p-2 text-white">
          <span>
            {venueObj.createdBy.firstName} {venueObj.createdBy.lastName}
          </span>
        </td>
        <td className="item2 venueName border border-light p-2 text-white">
          <span>{venueObj.name}</span>
        </td>
        <td className="item3 venueType border border-light p-2 text-white">
          <span>{venueObj.venueType.name}</span>
        </td>
        <td className="item4 venueLocation border border-light p-2 text-white">
          <span>
            {venueObj.locationInfo.lineOne} {venueObj.locationInfo.lineTwo}
          </span>
          <span>
            {venueObj.locationInfo.city}, {venueObj.locationInfo.state}{" "}
            {venueObj.locationInfo.zip}
          </span>
          <span></span>
        </td>
        <td className="item-last border border-light ps-5 py-2">
          <button
            type="button"
            value={venueObj.id}
            className="venue-delete-btn btn btn-danger "
            onClick={onDeleteClicked}
          >
            Delete
          </button>
        </td>
      </tr>
    </React.Fragment>
  );
}

VenueAdminTableRow.propTypes = {
  venueObj: PropTypes.shape({
    createdBy: PropTypes.shape({
      id: PropTypes.number.isRequired,
      firstName: PropTypes.string.isRequired,
      lastName: PropTypes.string.isRequired,
      mi: PropTypes.string,
      avatarUrl: PropTypes.string.isRequired,
    }),
    dateCreated: PropTypes.string.isRequired,
    dateModified: PropTypes.string,
    description: PropTypes.string.isRequired,
    fileImageUrl: PropTypes.string.isRequired,
    id: PropTypes.number.isRequired,
    locationInfo: PropTypes.shape({
      city: PropTypes.string.isRequired,
      id: PropTypes.number.isRequired,
      lineOne: PropTypes.string.isRequired,
      lineTwo: PropTypes.string.isRequired,
      state: PropTypes.string.isRequired,
      stateId: PropTypes.number.isRequired,
      zip: PropTypes.string.isRequired,
    }),
    modifiedBy: PropTypes.shape({
      id: PropTypes.number.isRequired,
      firstName: PropTypes.string.isRequired,
      lastName: PropTypes.string.isRequired,
      mi: PropTypes.string,
      avatarUrl: PropTypes.string.isRequired,
    }),
    name: PropTypes.string.isRequired,
    venueType: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
    }),
  }),
  onDeleteBtnClick: PropTypes.func.isRequired,
};
export default VenueAdminTableRow;
