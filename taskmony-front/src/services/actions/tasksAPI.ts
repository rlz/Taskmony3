import { Dispatch } from "redux";
import { checkResponse, nowDate } from "../../utils/APIUtils";
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
export const DELETE_TASK_REQUEST = "DELETE_TASK_REQUEST";
export const DELETE_TASK_SUCCESS = "DELETE_TASK_SUCCESS";
export const DELETE_TASK_FAILED = "DELETE_TASK_FAILED";

export const CHANGE_COMPLETE_TASK_DATE_REQUEST =
  "CHANGE_COMPLETE_TASK_DATE_REQUEST";
export const CHANGE_COMPLETE_TASK_DATE_SUCCESS =
  "CHANGE_COMPLETE_TASK_DATE_SUCCESS";
export const CHANGE_COMPLETE_TASK_DATE_FAILED =
  "CHANGE_COMPLETE_TASK_DATE_FAILED";
export const CHANGE_TASK_FOLLOWED_REQUEST = "CHANGE_TASK_FOLLOWED_REQUEST";
export const CHANGE_TASK_FOLLOWED_SUCCESS = "CHANGE_TASK_FOLLOWED_SUCCESS";
export const CHANGE_TASK_FOLLOWED_FAILED = "CHANGE_TASK_FOLLOWED_FAILED";

export const CHANGE_OPEN_TASK = "CHANGE_OPEN_TASK";
export const CHANGE_TASK_DESCRIPTION = "CHANGE_TASK_DESCRIPTION";
export const CHANGE_TASK_DIRECTION = "CHANGE_TASK_DIRECTION";
export const CHANGE_TASK_DETAILS = "CHANGE_TASK_DETAILS";
export const CHANGE_TASK_ASSIGNEE = "CHANGE_TASK_ASSIGNEE";
export const CHANGE_TASK_START_DATE = "CHANGE_TASK_START_DATE";
export const CHANGE_TASK_REPEAT_MODE = "CHANGE_TASK_REPEAT_MODE";
export const CHANGE_TASK_REPEAT_EVERY = "CHANGE_TASK_REPEAT_EVERY";
export const CHANGE_TASK_REPEAT_WEEK_DAYS = "CHANGE_TASK_WEEK_DAYS";
export const CHANGE_TASK_REPEAT_UNTIL = "CHANGE_TASK_REPEAT_UNTIL";

export const RESET_TASK = "RESET_TASK";
export const CHANGE_TASKS = "CHANGE_TASKS";

const URL = BASE_URL + "/graphql";

export function openTask(id) {
  const tasks = useAppSelector((store) => store.tasks.items);
  const task = tasks.filter((task) => task.id == id)[0];
  return function (dispatch: Dispatch) {
    dispatch({ type: GET_TASKS_REQUEST, task: task });
  };
}

export function getTasks() {
  return function (dispatch: Dispatch) {
    console.log("getting tasks");
    dispatch({ type: GET_TASKS_REQUEST });
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },

      body: JSON.stringify({
        query: `{tasks{${tasksAllQuery}}}`,
      }),
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

export function addTask(task, direction) {
  return function (dispatch: Dispatch) {
    dispatch({ type: ADD_TASK_REQUEST });
    console.log("adding");
    fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskAdd(description:"${
          task.description
        }", startAt:"${task.startAt}"
      ${
        direction
          ? `,directionId:"${direction}"`
          : task.direction
          ? `,directionId:"${task.direction?.id}"`
          : ""
      }
      ${task.assignee?.id ? `,assigneeId:"${task.assignee?.id}"` : ""}
      ) {
        ${tasksAllQuery}
      }
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: ADD_TASK_SUCCESS,
            task: res.data.taskAdd,
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

export function addRepeatedTasks(task, direction) {
  return function (dispatch: Dispatch) {
    dispatch({ type: ADD_TASK_REQUEST });
    console.log("adding");
    fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      tasksGenerate(description:"${
          task.description
        }", startAt:"${task.startAt}"
      ${
        direction
          ? `,directionId:"${direction}"`
          : task.direction
          ? `,directionId:"${task.direction?.id}"`
          : ""
      }
      ${task.assignee?.id ? `,assigneeId:"${task.assignee?.id}"` : ""}
      ${
        task.repeatMode
          ? `, repeatMode:${task.repeatMode}, repeatEvery:${task.repeatEvery}, repeatUntil:"${task.repeatUntil}", `
          : ""
      }
      ${task.weekDays ? `, weekDays:[${task.weekDays.join(",").split(-1)}]` : ""}
      
      )
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          console.log(res)
          // dispatch({
          //   type: ADD_TASK_SUCCESS,
          //   task: res.data.taskAdd,
          // });
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

export function changeCompleteTaskDate(taskId, date) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_COMPLETE_TASK_DATE_REQUEST });
    console.log("change complete date");
    fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetCompletedAt(taskId:"${taskId}", completedAt:${
          date ? `"${date}"` : date
        }) 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_COMPLETE_TASK_DATE_SUCCESS,
            taskId: taskId,
            date: date,
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

export function changeTaskFollowed(taskId, markFollowed) {
  return function (dispatch: Dispatch) {
    dispatch({ type: CHANGE_TASK_FOLLOWED_REQUEST });
    console.log("change followed");
    fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      ${markFollowed ? "taskSubscribe" : "taskUnsubscribe"}(taskId:"${taskId}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_FOLLOWED_SUCCESS,
            taskId: taskId,
            followed: markFollowed,
            userId: getCookie("id"),
          });
        } else {
          dispatch({
            type: CHANGE_TASK_FOLLOWED_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_FOLLOWED_FAILED,
        });
      });
  };
}
export function deleteTask(taskId) {
  return function (dispatch: Dispatch) {
    dispatch({ type: DELETE_TASK_REQUEST });
    console.log("delete task");
    fetch(URL, {
      method: "POST",

      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetDeletedAt(taskId:"${taskId}",deletedAt:"${nowDate()}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: DELETE_TASK_SUCCESS,
            taskId: taskId,
          });
        } else {
          dispatch({
            type: DELETE_TASK_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: DELETE_TASK_FAILED,
        });
      });
  };
}
