import { Dispatch } from "redux";
import { checkResponse } from "../../utils/APIUtils";
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

const URL = BASE_URL + "/graphql";

export function getTasks() {
  return function (dispatch : Dispatch) {
    dispatch({ type: GET_TASKS_REQUEST });
    fetch(URL, {
  method: 'POST',

  headers: {
    "Content-Type": "application/json"
  },

  body: JSON.stringify({
    query: `{
      tasks {
      }
    }`
  })
})
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: GET_TASKS_SUCCESS,
            items: res.data,
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

export function addTask() {
  const task = useAppSelector((store) => store.editedTask);
  return function (dispatch : Dispatch) {
    dispatch({ type: ADD_TASK_REQUEST });
    fetch(URL, {
  method: 'POST',

  headers: {
    "Content-Type": "application/json"
  },

  body: JSON.stringify({
    query: `mutation {
      taskAdd(
        description: ${task.description}
        details: ${task.details}
        assigneeId: ${task.assigneeId}
        directionId: ${task.directionId}
        startAt: ${task.startAt}
        )
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
