import { Dispatch } from "redux";
import { checkResponse } from "../../utils/APIUtils";
import { getCookie } from "../../utils/cookies";
import { BASE_URL } from "../../utils/data";
import { useAppSelector } from "../../utils/hooks";
import { directionsAllQuery } from "../../utils/queries";
export const GET_DIRECTIONS_REQUEST = "GET_DIRECTIONS_REQUEST";
export const GET_DIRECTIONS_SUCCESS = "GET_DIRECTIONS_SUCCESS";
export const GET_DIRECTIONS_FAILED = "GET_DIRECTIONS_FAILED";
export const ADD_DIRECTION_REQUEST = "ADD_DIRECTION_REQUEST";
export const ADD_DIRECTION_SUCCESS = "ADD_DIRECTION_SUCCESS";
export const ADD_DIRECTION_FAILED = "ADD_DIRECTION_FAILED";

export const CHANGE_OPEN_DIRECTION = "CHANGE_OPEN_DIRECTION";
export const CHANGE_DIRECTION_DETAILS_REQUEST = "CHANGE_DIRECTION_DETAILS_REQUEST";
export const CHANGE_DIRECTION_DETAILS_SUCCESS = "CHANGE_DIRECTION_DETAILS_SUCCESS";
export const CHANGE_DIRECTION_DETAILS_FAILED = "CHANGE_DIRECTION_DETAILS_FAILED";
export const RESET_DIRECTION = "RESET_DIRECTION";
export const CHANGE_DIRECTIONS = "CHANGE_DIRECTIONS";


const URL = BASE_URL + "/graphql";

export function getDirections() {
  return function (dispatch : Dispatch) {
    console.log("getting directions");
    dispatch({ type: GET_DIRECTIONS_REQUEST });
    fetch(URL, {
  method: 'POST',
  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer "+getCookie("accessToken"),
  },

  body: JSON.stringify({
    query: directionsAllQuery   
  })
})
      .then(checkResponse)
      .then((res) => {
        console.log(res);
        if (res) {
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

export function addDirection(name) {
  return function (dispatch : Dispatch) {
    dispatch({ type: ADD_DIRECTION_REQUEST });
    console.log("adding");
    fetch(URL, {
  method: 'POST',

  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer "+getCookie("accessToken"),
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
    `
  })
})
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: ADD_DIRECTION_SUCCESS,
            direction: res.data.directionAdd
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

export function changeDetails(details,directionId) {
  return function (dispatch : Dispatch) {
    dispatch({ type: CHANGE_DIRECTION_DETAILS_REQUEST });
    console.log("adding");
    fetch(URL, {
  method: 'POST',

  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer "+getCookie("accessToken"),
  },
  body: JSON.stringify({
    query: `mutation {
      directionSetDetails(details:"${details}",directionId:"${directionId}")
    }
    `
  })
})
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_DIRECTION_DETAILS_SUCCESS,
            directionId:directionId,
            details: details
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
