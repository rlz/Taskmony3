import { nowDate } from "../../utils/api-utils";
import { TTask } from "../../utils/types";
import { SEND_COMMENT_SUCCESS } from "../actions/comments";
import {
  ADD_TASKS,
  ADD_TASK_FAILED,
  ADD_TASK_REQUEST,
  ADD_TASK_SUCCESS,
  CHANGE_COMPLETE_TASK_DATE_SUCCESS,
  CHANGE_OPEN_TASK,
  CHANGE_TASK,
  CHANGE_TASKS_ASSIGNEE_SUCCESS,
  CHANGE_TASKS_DESCRIPTION_SUCCESS,
  CHANGE_TASKS_DETAILS_SUCCESS,
  CHANGE_TASKS_DIRECTION_SUCCESS,
  CHANGE_TASK_ASSIGNEE,
  CHANGE_TASK_DESCRIPTION,
  CHANGE_TASK_DETAILS,
  CHANGE_TASK_DIRECTION,
  CHANGE_TASK_FOLLOWED_SUCCESS,
  CHANGE_TASK_GROUP_ID,
  CHANGE_TASK_REPEAT_EVERY,
  CHANGE_TASK_REPEAT_MODE,
  CHANGE_TASK_REPEAT_MODE_FROM_NONE_SUCCESS,
  CHANGE_TASK_REPEAT_UNTIL,
  CHANGE_TASK_REPEAT_WEEK_DAYS,
  CHANGE_TASK_START_DATE,
  DELETE_TASKS_SUCCESS,
  DELETE_TASK_SUCCESS,
  GET_TASKS_FAILED,
  GET_TASKS_REQUEST,
  GET_TASKS_SUCCESS,
  REMOVE_TASK,
  REMOVE_TASKS,
  RESET_TASK,
} from "../actions/tasksAPI";

type TTasksState = {
  items: Array<TTask>;
  get_tasks_loading: boolean;
  get_tasks_error: boolean;
  add_task_loading: boolean;
  add_task_error: boolean;
};

export const tasksInitialState = {
  items: [],
  get_tasks_loading: true,
  get_tasks_error: false,
  add_task_loading: true,
  add_task_error: false,
};
export const tasksReducer = (
  state: TTasksState = tasksInitialState,
  action:
    | { type: typeof GET_TASKS_SUCCESS | typeof ADD_TASKS; items: Array<TTask> }
    | { type: typeof DELETE_TASK_SUCCESS; taskId: string; date: string }
    | { type: typeof DELETE_TASKS_SUCCESS; groupId: string; date: string }
    | {
        type: typeof CHANGE_TASKS_DESCRIPTION_SUCCESS;
        groupId: string;
        payload: string;
      }
    | {
        type: typeof REMOVE_TASK;
        id: string;
      }
    | {
        type: typeof CHANGE_TASK_REPEAT_MODE_FROM_NONE_SUCCESS;
        groupId: string;
        taskId: string;
        payload: string;
      }
    | {
        type: typeof REMOVE_TASKS;
        groupId: string;
        except: string;
      }
    | {
        type: typeof CHANGE_TASKS_DETAILS_SUCCESS;
        groupId: string;
        payload: string;
      }
    | {
        type: typeof CHANGE_TASKS_DIRECTION_SUCCESS;
        groupId: string;
        payload: { id: string; name: string };
      }
    | {
        type: typeof CHANGE_TASKS_ASSIGNEE_SUCCESS;
        groupId: string;
        payload: { id: string; displayName: string };
      }
    | { type: typeof ADD_TASK_SUCCESS; task: TTask }
    | { type: typeof CHANGE_TASK; task: TTask; id: string }
    | {
        type: typeof CHANGE_COMPLETE_TASK_DATE_SUCCESS;
        taskId: string;
        date: string;
      }
    | {
        type: typeof CHANGE_TASK_FOLLOWED_SUCCESS;
        taskId: string;
        followed: boolean;
        userId: string;
      }
    | {
        type:
          | typeof GET_TASKS_REQUEST
          | typeof GET_TASKS_FAILED
          | typeof ADD_TASK_REQUEST
          | typeof ADD_TASK_FAILED;
      }
) => {
  switch (action.type) {
    case GET_TASKS_REQUEST: {
      return {
        ...state,
        get_tasks_loading: true,
      };
    }
    case GET_TASKS_SUCCESS: {
      return {
        ...state,
        get_tasks_loading: false,
        get_tasks_error: false,
        items: action.items.reverse(),
      };
    }
    case ADD_TASKS: {
      return {
        ...state,
        items: [...action.items.reverse(), ...state.items],
      };
    }
    case GET_TASKS_FAILED: {
      return {
        ...state,
        items: [],
        get_tasks_loading: false,
        error: true,
      };
    }
    case CHANGE_TASK: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id === action.id ? { ...action.task } : item
        ),
      };
    }
    case REMOVE_TASK: {
      return {
        ...state,
        items: state.items.filter((item) => item.id !== action.id),
      };
    }
    case REMOVE_TASKS: {
      return {
        ...state,
        items: state.items.filter(
          (item) => item.groupId !== action.groupId || item.id === action.except
        ),
      };
    }
    case CHANGE_COMPLETE_TASK_DATE_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id === action.taskId
            ? { ...item, completedAt: action.date }
            : item
        ),
      };
    }
    case CHANGE_TASK_FOLLOWED_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id === action.taskId
            ? {
                ...item,
                subscribers: action.followed
                  ? [...item.subscribers, { id: action.userId }]
                  : item.subscribers.filter((s) => s.id !== action.userId),
              }
            : item
        ),
      };
    }

    case ADD_TASK_REQUEST: {
      return {
        ...state,
        add_task_loading: true,
      };
    }
    case ADD_TASK_SUCCESS: {
      return {
        ...state,
        items: [action.task, ...state.items],
        add_task_loading: false,
        add_task_error: false,
      };
    }
    case ADD_TASK_FAILED: {
      return {
        ...state,
        add_task_loading: false,
        error: true,
      };
    }
    case DELETE_TASK_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id === action.taskId ? { ...item, deletedAt: action.date } : item
        ),
      };
    }
    case DELETE_TASKS_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.groupId === action.groupId
            ? { ...item, deletedAt: action.date }
            : item
        ),
      };
    }
    case CHANGE_TASK_REPEAT_MODE_FROM_NONE_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id === action.taskId
            ? { ...item, groupId: action.groupId, repeatMode: action.payload }
            : item
        ),
      };
    }
    case CHANGE_TASKS_DESCRIPTION_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.groupId === action.groupId
            ? { ...item, description: action.payload }
            : item
        ),
      };
    }
    case CHANGE_TASKS_DETAILS_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.groupId === action.groupId
            ? { ...item, details: action.payload }
            : item
        ),
      };
    }
    case CHANGE_TASKS_DIRECTION_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.groupId === action.groupId
            ? { ...item, direction: action.payload }
            : item
        ),
      };
    }
    case CHANGE_TASKS_ASSIGNEE_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.groupId === action.groupId
            ? { ...item, assignee: action.payload }
            : item
        ),
      };
    }
    default: {
      return state;
    }
  }
};

export const taskInitialState: TTask = {
  id: "",
  groupId: "",
  description: "",
  completedAt: null,
  deletedAt: null,
  assignee: {
    displayName: "",
    id: "",
  },
  subscribers: [],
  details: null,
  startAt: nowDate(),
  direction: { name: "", id: "" },
  repeatMode: null,
  createdBy: {
    id: "",
    displayName: "",
  },
  comments: [],
  repeatUntil: "",
  weekDays: [],
  repeatEvery: 1,
};

export const editTaskReducer = (
  state: TTask = taskInitialState,
  action:
    | { type: typeof RESET_TASK }
    | {
        type:
          | typeof CHANGE_TASK_DESCRIPTION
          | typeof CHANGE_TASK_DETAILS
          | typeof CHANGE_TASK_START_DATE
          | typeof CHANGE_TASK_ASSIGNEE
          | typeof CHANGE_TASK_DIRECTION
          | typeof CHANGE_TASK_REPEAT_MODE
          | typeof CHANGE_TASK_REPEAT_EVERY
          | typeof CHANGE_TASK_REPEAT_UNTIL
          | typeof CHANGE_TASK_REPEAT_WEEK_DAYS
          | typeof CHANGE_TASK_GROUP_ID;
        payload: any;
      }
    | {
        type: typeof CHANGE_OPEN_TASK;
        task: TTask;
      }
    | {
        type: typeof SEND_COMMENT_SUCCESS;
        comment: any;
      }
    | {
        type: typeof CHANGE_COMPLETE_TASK_DATE_SUCCESS;
        taskId: string;
        date: string;
      }
) => {
  switch (action.type) {
    case RESET_TASK: {
      return taskInitialState;
    }
    case CHANGE_OPEN_TASK: {
      return action.task;
    }
    case CHANGE_TASK_DESCRIPTION: {
      return {
        ...state,
        description: action.payload,
      };
    }
    case CHANGE_TASK_DETAILS: {
      return {
        ...state,
        details: action.payload,
      };
    }
    case CHANGE_TASK_START_DATE: {
      return {
        ...state,
        startAt: action.payload,
      };
    }
    case CHANGE_TASK_ASSIGNEE: {
      return {
        ...state,
        assignee: action.payload,
      };
    }
    case CHANGE_TASK_DIRECTION: {
      return {
        ...state,
        direction: action.payload,
      };
    }
    case CHANGE_TASK_REPEAT_MODE: {
      return { ...state, repeatMode: action.payload };
    }
    case CHANGE_TASK_REPEAT_EVERY: {
      return { ...state, repeatEvery: action.payload };
    }
    case CHANGE_TASK_REPEAT_UNTIL: {
      return { ...state, repeatUntil: action.payload };
    }
    case CHANGE_TASK_REPEAT_WEEK_DAYS: {
      return { ...state, weekDays: action.payload };
    }
    case CHANGE_TASK_GROUP_ID: {
      return { ...state, groupId: action.payload };
    }
    case CHANGE_COMPLETE_TASK_DATE_SUCCESS: {
      if (action.taskId === state.id)
        return { ...state, completedAt: action.date };
      return state;
    }

    case SEND_COMMENT_SUCCESS: {
      return {
        ...state,
        comments: [...state.comments, action.comment],
      };
    }
    default: {
      return state;
    }
  }
};
