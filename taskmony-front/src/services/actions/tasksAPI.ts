import { Dispatch } from "redux";
import { checkResponse } from "../../utils/APIUtils";
import { getCookie } from "../../utils/cookies";
import { BASE_URL } from "../../utils/data";
import { useAppSelector } from "../../utils/hooks";
import { tasksAllQuery } from "../../utils/queries";
export const GET_TASKS_REQUEST = "GET_TASKS_REQUEST";
export const GET_TASKS_SUCCESS = "GET_TASKS_SUCCESS";
export const GET_TASKS_FAILED = "GET_TASKS_FAILED";
export const ADD_TASK_REQUEST = "ADD_TASK_REQUEST";
export const ADD_TASK_SUCCESS = "ADD_TASK_SUCCESS";
export const ADD_TASK_FAILED = "ADD_TASK_FAILED";

export const CHANGE_COMPLETE_TASK_DATE_REQUEST = "CHANGE_COMPLETE_TASK_DATE_REQUEST";
export const CHANGE_COMPLETE_TASK_DATE_SUCCESS = "CHANGE_COMPLETE_TASK_DATE_SUCCESS";
export const CHANGE_COMPLETE_TASK_DATE_FAILED = "CHANGE_COMPLETE_TASK_DATE_FAILED";

export const CHANGE_OPEN_TASK = "CHANGE_OPEN_TASK";
export const CHANGE_TASK_DESCRIPTION = "CHANGE_TASK_DESCRIPTION";
export const CHANGE_TASK_DETAILS = "CHANGE_TASK_DETAILS";
export const RESET_TASK = "RESET_TASK";
export const CHANGE_TASKS = "CHANGE_TASKS";


const URL = BASE_URL + "/graphql";

export function openTask(id) {
  const tasks = useAppSelector(
    (store) => store.tasks.items
  );
  const task = tasks.filter(task=>task.id==id)[0];
  return function (dispatch : Dispatch) {
    dispatch({ type: GET_TASKS_REQUEST, task: task });
  }
}

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
    query: tasksAllQuery   
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

export function addTask(task,direction) {
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
      taskAdd(description:"${task.description}", startAt:"1.12.12"${direction?`,directionId:"${direction}"`:task.direction?`,directionId:"${task.direction}"`:""}) {
        id
        description
        completedAt
        subscribers 
        {
            id
        }
        details
        startAt
        direction 
        { name 
          id
         }
        repeatMode
        createdBy { displayName }
      }
    }
    `
  })
})
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: ADD_TASK_SUCCESS,
            task: res.data.taskAdd
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

export function changeCompleteTaskDate(taskId,date) {
  return function (dispatch : Dispatch) {
    dispatch({ type: CHANGE_COMPLETE_TASK_DATE_REQUEST });
    console.log("change complete date");
    fetch(URL, {
  method: 'POST',

  headers: {
    "Content-Type": "application/json",
    "Authorization": "Bearer "+getCookie("accessToken"),
  },
  body: JSON.stringify({
    query: `mutation {
      taskSetCompletedAt(taskId:"${taskId}", completedAt:${date?`"${date}"`:date}) 
    }
    `
  })
})
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_COMPLETE_TASK_DATE_SUCCESS,
            taskId:taskId,
            date: date
          });
        } else {
          dispatch({
            type: CHANGE_COMPLETE_TASK_DATE_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_COMPLETE_TASK_DATE_FAILED,
        });
      });
  };
}
