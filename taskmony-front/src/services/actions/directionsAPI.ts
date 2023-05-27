import { Dispatch } from "redux";
import { checkResponse, getAccessToken } from "../../utils/api-utils";
import Cookies from "js-cookie";
import { BASE_URL } from "../../utils/base-api-url";
import { directionsAllQuery } from "../../utils/queries";
export const GET_DIRECTIONS_REQUEST = "GET_DIRECTIONS_REQUEST";
export const GET_DIRECTIONS_SUCCESS = "GET_DIRECTIONS_SUCCESS";
export const GET_DIRECTIONS_FAILED = "GET_DIRECTIONS_FAILED";
export const ADD_DIRECTION_REQUEST = "ADD_DIRECTION_REQUEST";
export const ADD_DIRECTION_SUCCESS = "ADD_DIRECTION_SUCCESS";
export const ADD_DIRECTION_FAILED = "ADD_DIRECTION_FAILED";

export const ADD_USER_REQUEST = "ADD_USER_REQUEST";
export const ADD_USER_SUCCESS = "ADD_USER_SUCCESS";
export const ADD_USER_FAILED = "ADD_USER_FAILED";

export const REMOVE_USER_REQUEST = "REMOVE_USER_REQUEST";
export const REMOVE_USER_SUCCESS = "REMOVE_USER_SUCCESS";
export const REMOVE_USER_FAILED = "REMOVE_USER_FAILED";

export const REMOVE_DIRECTION = "REMOVE_DIRECTION";
export const DELETE_DIRECTION_REQUEST = "DELETE_DIRECTION_REQUEST";
export const DELETE_DIRECTION_SUCCESS = "DELETE_DIRECTION_SUCCESS";
export const DELETE_DIRECTION_FAILED = "DELETE_DIRECTION_FAILED";

export const CHANGE_OPEN_DIRECTION = "CHANGE_OPEN_DIRECTION";
export const CHANGE_DIRECTION_DETAILS_REQUEST =
  "CHANGE_DIRECTION_DETAILS_REQUEST";
export const CHANGE_DIRECTION_DETAILS_SUCCESS =
  "CHANGE_DIRECTION_DETAILS_SUCCESS";
export const CHANGE_DIRECTION_DETAILS_FAILED =
  "CHANGE_DIRECTION_DETAILS_FAILED";
export const RESET_DIRECTION = "RESET_DIRECTION";
export const CHANGE_DIRECTIONS = "CHANGE_DIRECTIONS";

const URL = BASE_URL + "/graphql";

export function getDirections() {
  return function (dispatch: Dispatch) {
    //console.log("getting directions");
    dispatch({ type: GET_DIRECTIONS_REQUEST });
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + cookie,
          },

          body: JSON.stringify({
            query: directionsAllQuery,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (!res.errors) {
          dispatch({
            type: GET_DIRECTIONS_SUCCESS,
            items: res.data.directions,
          });
        } else {
          dispatch({
            type: GET_DIRECTIONS_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: GET_DIRECTIONS_FAILED,
        });
      });
  };
}

export function addDirection(name: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: ADD_DIRECTION_REQUEST });
    //console.log("adding");
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + Cookies.get("accessToken"),
          },
          // mutation {
          //   directionAdd(description:"123", startAt:"1.12.12") {
          //     description
          //   }
          // }
          body: JSON.stringify({
            query: `mutation {
      directionAdd(name:"${name}") {
        id
        name
        members
        {
        displayName
        id
        }
      }
    }
    `,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: ADD_DIRECTION_SUCCESS,
            direction: res.data.directionAdd,
          });
        } else {
          dispatch({
            type: ADD_DIRECTION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: ADD_DIRECTION_FAILED,
        });
      });
  };
}
export function deleteDirection(directionId: string) {
  const deletedDate = new Date().toISOString();
  return function (dispatch: Dispatch) {
    dispatch({ type: DELETE_DIRECTION_REQUEST });
    //console.log("deleting direction");
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + Cookies.get("accessToken"),
          },
          body: JSON.stringify({
            query: `mutation {
      directionSetDeletedAt(directionId:"${directionId}",deletedAt:"${deletedDate}")
    }
    `,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: DELETE_DIRECTION_SUCCESS,
            directionId: directionId,
            deletedAt: deletedDate,
          });
        } else {
          dispatch({
            type: DELETE_DIRECTION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: DELETE_DIRECTION_FAILED,
        });
      });
  };
}
export function addUser(
  directionId: string,
  user: { id: string; displayName: string }
) {
  return function (dispatch: Dispatch) {
    dispatch({ type: ADD_USER_REQUEST });
    //console.log("adding user");
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + Cookies.get("accessToken"),
          },
          body: JSON.stringify({
            query: `mutation {
      directionAddMember(directionId:"${directionId}",userId:"${user.id}")
    }
    `,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res && !res.errors) {
          dispatch({
            type: ADD_USER_SUCCESS,
            directionId: directionId,
            user: { displayName: user.displayName, id: user.id },
          });
        } else {
          dispatch({
            type: ADD_USER_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: ADD_USER_FAILED,
        });
      });
  };
}
export function removeUser(directionId: string, user: { id: string }) {
  return function (dispatch: Dispatch) {
    dispatch({ type: ADD_USER_REQUEST });
    //console.log("removing user");
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + Cookies.get("accessToken"),
          },
          body: JSON.stringify({
            query: `mutation {
      directionRemoveMember(directionId:"${directionId}",userId:"${user.id}")
    }
    `,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res && !res.errors) {
          dispatch({
            type: REMOVE_USER_SUCCESS,
            directionId: directionId,
            user: { id: user.id },
          });
        } else {
          dispatch({
            type: REMOVE_USER_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: REMOVE_USER_FAILED,
        });
      });
  };
}

export function changeDetails(details: string, directionId: string) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_DIRECTION_DETAILS_REQUEST });
    //console.log("adding");
    getAccessToken()
      .then((cookie) =>
        fetch(URL, {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + Cookies.get("accessToken"),
          },
          body: JSON.stringify({
            query: `mutation {
      directionSetDetails(details:"${details}",directionId:"${directionId}")
    }
    `,
          }),
        })
      )
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_DIRECTION_DETAILS_SUCCESS,
            directionId: directionId,
            details: details,
          });
        } else {
          dispatch({
            type: CHANGE_DIRECTION_DETAILS_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_DIRECTION_DETAILS_FAILED,
        });
      });
  };
}
