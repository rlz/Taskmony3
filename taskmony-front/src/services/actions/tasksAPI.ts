import { Dispatch } from "redux";
import { checkResponse } from "../../utils/APIUtils";
import { getCookie } from "../../utils/cookies";
import { BASE_URL } from "../../utils/data";
import { useAppSelector } from "../../utils/hooks";
export const GET_TASKS_REQUEST = "GET_TASKS_REQUEST";
export const GET_TASKS_SUCCESS = "GET_TASKS_SUCCESS";
export const GET_TASKS_FAILED = "GET_TASKS_FAILED";
export const ADD_TASK_REQUEST = "ADD_TASK_REQUEST";
export const ADD_TASK_SUCCESS = "ADD_TASK_SUCCESS";
export const ADD_TASK_FAILED = "ADD_TASK_FAILED";

export const CHANGE_TASK_DESCRIPTION = "CHANGE_TASK_DESCRIPTION";
export const CHANGE_TASK_DETAILS = "CHANGE_TASK_DETAILS";
export const RESET_TASK = "RESET_TASK";

const URL = BASE_URL + "/graphql";

export function getTasks() {
  return function (dispatch : Dispatch) {
    console.log("getting tasks");
    dispatch({ type: GET_TASKS_REQUEST });
    fetch(URL, {
  method: 'POST',
  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer "+getCookie("accessToken"),
  },

  body: JSON.stringify({
    query: `{tasks{
      id
      description
      startAt
      direction {name }
      repeatMode
    }}`
  })
})
      .then(checkResponse)
      .then((res) => {
        console.log(res);
        if (res) {
          dispatch({
            type: GET_TASKS_SUCCESS,
            items: res.data.tasks,
          });
        } else {
          dispatch({
            type: GET_TASKS_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: GET_TASKS_FAILED,
        });
      });
  };
}

export function addTask(task) {
  return function (dispatch : Dispatch) {
    dispatch({ type: ADD_TASK_REQUEST });
    console.log("adding");
    fetch(URL, {
  method: 'POST',

  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer "+getCookie("accessToken"),
  },
  // mutation {
  //   taskAdd(description:"123", startAt:"1.12.12") {
  //     description
  //   }
  // }
  body: JSON.stringify({
    query: `mutation {
      taskAdd(description:"${task.description}", startAt:"1.12.12") {
        description
      }
    }
    `
  })
})
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: ADD_TASK_SUCCESS
          });
        } else {
          dispatch({
            type: ADD_TASK_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: ADD_TASK_FAILED,
        });
      });
  };
}