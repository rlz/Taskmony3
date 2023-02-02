import { TTask } from "../../utils/types";
import {
  ADD_TASK_FAILED,
  ADD_TASK_REQUEST,
  ADD_TASK_SUCCESS,
  CHANGE_TASK_DESCRIPTION,
  CHANGE_TASK_DETAILS,
  GET_TASKS_FAILED,
  GET_TASKS_REQUEST,
  GET_TASKS_SUCCESS,
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
    | { type: typeof GET_TASKS_SUCCESS; items: Array<any> }
    | {
        type:
          | typeof GET_TASKS_REQUEST
          | typeof GET_TASKS_FAILED
          | typeof ADD_TASK_REQUEST
          | typeof ADD_TASK_FAILED
          | typeof ADD_TASK_SUCCESS;
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
        items: action.items,
      };
    }
    case GET_TASKS_FAILED: {
      return {
        ...state,
        get_tasks_items: [],
        get_tasks_loading: false,
        error: true,
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
    default: {
      return state;
    }
  }
};

export const taskInitialState = {
  description: "",
  details: "",
  assigneeId: "",
  directionId: "",
  startAt: "",
};

export const editTaskReducer = (
  state: TTask = taskInitialState,
  action:
    { type: typeof CHANGE_TASK_DESCRIPTION | typeof CHANGE_TASK_DETAILS | typeof RESET_TASK; payload: any }
) => {
  switch (action.type) {
    case CHANGE_TASK_DESCRIPTION: {
      return {
        ...state,
        description: action.payload,
      };
    }
    case RESET_TASK: {
      return taskInitialState;
  }
      case CHANGE_TASK_DETAILS: {
        return {
          ...state,
          details: action.payload,
        };
    }
    default: {
      return state;
    }
}};
