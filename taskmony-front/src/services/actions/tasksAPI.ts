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

export const CHANGE_TASK_DESCRIPTION_FAILED = "CHANGE_TASK_DESCRIPTION_FAILED";
export const CHANGE_TASK_DIRECTION_FAILED = "CHANGE_TASK_DIRECTION_FAILED";
export const CHANGE_TASK_DETAILS_FAILED = "CHANGE_TASK_DETAILS_FAILED";
export const CHANGE_TASK_ASSIGNEE_FAILED = "CHANGE_TASK_ASSIGNEE_FAILED";
export const CHANGE_TASK_START_DATE_FAILED = "CHANGE_TASK_START_DATE_FAILED";
export const CHANGE_TASK_REPEAT_MODE_FAILED = "CHANGE_TASK_REPEAT_MODE_FAILED";
export const CHANGE_TASK_REPEAT_EVERY_FAILED =
  "CHANGE_TASK_REPEAT_EVERY_FAILED";
export const CHANGE_TASK_REPEAT_WEEK_DAYS_FAILED =
  "CHANGE_TASK_WEEK_DAYS_FAILED";
export const CHANGE_TASK_REPEAT_UNTIL_FAILED =
  "CHANGE_TASK_REPEAT_UNTIL_FAILED";

export const CHANGE_TASK_DESCRIPTION_SUCCESS =
  "CHANGE_TASK_DESCRIPTION_SUCCESS";
export const CHANGE_TASK_DIRECTION_SUCCESS = "CHANGE_TASK_DIRECTION_SUCCESS";
export const CHANGE_TASK_DETAILS_SUCCESS = "CHANGE_TASK_DETAILS_SUCCESS";
export const CHANGE_TASK_ASSIGNEE_SUCCESS = "CHANGE_TASK_ASSIGNEE_SUCCESS";
export const CHANGE_TASK_START_DATE_SUCCESS = "CHANGE_TASK_START_DATE_SUCCESS";
export const CHANGE_TASK_REPEAT_MODE_SUCCESS =
  "CHANGE_TASK_REPEAT_MODE_SUCCESS";
export const CHANGE_TASK_REPEAT_EVERY_SUCCESS =
  "CHANGE_TASK_REPEAT_EVERY_SUCCESS";
export const CHANGE_TASK_REPEAT_WEEK_DAYS_SUCCESS =
  "CHANGE_TASK_WEEK_DAYS_SUCCESS";
export const CHANGE_TASK_REPEAT_UNTIL_SUCCESS =
  "CHANGE_TASK_REPEAT_UNTIL_SUCCESS";

export const CHANGE_SAVED = "CHANGE_SAVED";

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
      taskAdd(description:"${task.description}", startAt:"${task.startAt}"
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
      tasksGenerate(description:"${task.description}", startAt:"${task.startAt}"
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
      ${
        task.weekDays ? `, weekDays:[${task.weekDays.join(",").split(-1)}]` : ""
      }
      
      )
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch(getTasks());
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
export function changeTaskDescription(taskId, description) {
  return function (dispatch: Dispatch) {
    console.log("change description");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetDescription(taskId:"${taskId}",description:"${description}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_DESCRIPTION_SUCCESS,
            taskId: taskId,
            payload: description,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_DESCRIPTION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_DESCRIPTION_FAILED,
        });
      });
  };
}
export function changeTaskDetails(taskId, details) {
  return function (dispatch: Dispatch) {
    console.log("change details");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetDetails(taskId:"${taskId}",details:"${details}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_DETAILS_SUCCESS,
            taskId: taskId,
            payload: details,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_DETAILS_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_DETAILS_FAILED,
        });
      });
  };
}
export function changeTaskDirection(taskId, direction) {
  return function (dispatch: Dispatch) {
    console.log("change direction");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetDirection(taskId:"${taskId}",directionId:"${direction.id}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_DIRECTION_SUCCESS,
            taskId: taskId,
            payload: direction,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_DIRECTION_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_DIRECTION_FAILED,
        });
      });
  };
}
export function changeTaskAssignee(taskId, assignee) {
  return function (dispatch: Dispatch) {
    console.log("change assignee");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetAssignee(taskId:"${taskId}",assigneeId:"${assignee.id}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_ASSIGNEE_SUCCESS,
            taskId: taskId,
            payload: assignee,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_ASSIGNEE_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_ASSIGNEE_FAILED,
        });
      });
  };
}
export function changeTaskRepeatMode(
  taskId,
  repeatMode,
  repeatEvery,
  weekDays
) {
  return function (dispatch: Dispatch) {
    console.log("change repeatMode");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetRepeatMode(taskId:"${taskId}",repeatMode:"${repeatMode}",repeatEvery:"${repeatEvery}"
      ${repeatMode == "WEEK" ? `,weekDays:${weekDays}` : ""}) 
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_REPEAT_MODE_SUCCESS,
            taskId: taskId,
            payload: repeatMode,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_REPEAT_MODE_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_REPEAT_MODE_FAILED,
        });
      });
  };
}
export function changeTaskRepeatUntil(taskId, repeatUntil) {
  return function (dispatch: Dispatch) {
    console.log("change repeatUntil");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetRepeatUntil(taskId:"${taskId}",repeatUntil:"${repeatUntil}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_REPEAT_UNTIL_SUCCESS,
            taskId: taskId,
            payload: repeatUntil,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_REPEAT_UNTIL_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_REPEAT_UNTIL_FAILED,
        });
      });
  };
}
export function changeTaskStartAt(taskId, startAt) {
  return function (dispatch: Dispatch) {
    console.log("change startAt");
    fetch(URL, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + getCookie("accessToken"),
      },
      body: JSON.stringify({
        query: `mutation {
      taskSetStartAt(taskId:"${taskId}",startAt:"${startAt}") 
    }
    `,
      }),
    })
      .then(checkResponse)
      .then((res) => {
        if (res) {
          dispatch({
            type: CHANGE_TASK_START_DATE_SUCCESS,
            taskId: taskId,
            payload: startAt,
          });
        } else {
          dispatch({
            type: CHANGE_TASK_START_DATE_FAILED,
          });
        }
      })
      .catch((error) => {
        dispatch({
          type: CHANGE_TASK_START_DATE_FAILED,
        });
      });
  };
}

export function deleteTask(taskId) {
  const date = nowDate();
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
      taskSetDeletedAt(taskId:"${taskId}",deletedAt:"${date}") 
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
            date:date
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
