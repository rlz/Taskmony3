import { iteratorSymbol } from "immer/dist/internal";
import { isTemplateSpan } from "typescript";
import { nowDate } from "../../utils/APIUtils";
import { TIdea } from "../../utils/types";
import { SEND_COMMENT_SUCCESS } from "../actions/comments";
import {
  ADD_IDEA_FAILED,
  ADD_IDEA_REQUEST,
  ADD_IDEA_SUCCESS,
  CHANGE_COMPLETE_IDEA_DATE_SUCCESS,
  CHANGE_OPEN_IDEA,
  CHANGE_IDEAS,
  CHANGE_IDEA_ASSIGNEE,
  CHANGE_IDEA_ASSIGNEE_SUCCESS,
  CHANGE_IDEA_DESCRIPTION,
  CHANGE_IDEA_DESCRIPTION_SUCCESS,
  CHANGE_IDEA_DETAILS,
  CHANGE_IDEA_DETAILS_SUCCESS,
  CHANGE_IDEA_DIRECTION,
  CHANGE_IDEA_DIRECTION_SUCCESS,
  CHANGE_IDEA_FOLLOWED_SUCCESS,
  CHANGE_IDEA_REPEAT_EVERY,
  CHANGE_IDEA_REPEAT_EVERY_SUCCESS,
  CHANGE_IDEA_REPEAT_MODE,
  CHANGE_IDEA_REPEAT_MODE_SUCCESS,
  CHANGE_IDEA_REPEAT_UNTIL,
  CHANGE_IDEA_REPEAT_UNTIL_SUCCESS,
  CHANGE_IDEA_REPEAT_WEEK_DAYS,
  CHANGE_IDEA_REPEAT_WEEK_DAYS_SUCCESS,
  CHANGE_IDEA_START_DATE,
  CHANGE_IDEA_START_DATE_SUCCESS,
  DELETE_IDEA_SUCCESS,
  GET_IDEAS_FAILED,
  GET_IDEAS_REQUEST,
  GET_IDEAS_SUCCESS,
  RESET_IDEA,
  CHANGE_IDEA_CATEGORY,
  CHANGE_IDEA_GENERATION,
} from "../actions/ideasAPI";

type TIdeasState = {
  items: Array<TIdea>;
  get_ideas_loading: boolean;
  get_ideas_error: boolean;
  add_idea_loading: boolean;
  add_idea_error: boolean;
};

export const ideasInitialState = {
  items: [],
  get_ideas_loading: true,
  get_ideas_error: false,
  add_idea_loading: true,
  add_idea_error: false,
};
export const ideasReducer = (
  state: TIdeasState = ideasInitialState,
  action:
    | { type: typeof GET_IDEAS_SUCCESS; items: Array<any> }
    | { type: typeof DELETE_IDEA_SUCCESS; ideaId: string,date: any }
    | { type: typeof ADD_IDEA_SUCCESS; idea: any }
    | { type: typeof CHANGE_IDEAS; idea: any }
    | {
        type: typeof CHANGE_COMPLETE_IDEA_DATE_SUCCESS;
        ideaId: string;
        date: string;
      }
    | {
        type: typeof CHANGE_IDEA_FOLLOWED_SUCCESS;
        ideaId: string;
        followed: boolean;
        userId: string;
      }
    | {
        type:
          | typeof GET_IDEAS_REQUEST
          | typeof GET_IDEAS_FAILED
          | typeof ADD_IDEA_REQUEST
          | typeof ADD_IDEA_FAILED
          | typeof ADD_IDEA_SUCCESS;
      }
) => {
  switch (action.type) {
    case GET_IDEAS_REQUEST: {
      return {
        ...state,
        get_ideas_loading: true,
      };
    }
    case GET_IDEAS_SUCCESS: {
      return {
        ...state,
        get_ideas_loading: false,
        get_ideas_error: false,
        items: action.items.reverse(),
      };
    }
    case GET_IDEAS_FAILED: {
      return {
        ...state,
        items: [],
        get_ideas_loading: false,
        error: true,
      };
    }
    case CHANGE_IDEAS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id == action.idea.id ? action.idea : item
        ),
      };
    }
    case CHANGE_COMPLETE_IDEA_DATE_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id == action.ideaId
            ? { ...item, completedAt: action.date }
            : item
        ),
      };
    }
    case CHANGE_IDEA_FOLLOWED_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id == action.ideaId
            ? {
                ...item,
                subscribers: action.followed
                  ? [...item.subscribers, { id: action.userId }]
                  : item.subscribers.filter((s) => s.id != action.userId),
              }
            : item
        ),
      };
    }

    case ADD_IDEA_REQUEST: {
      return {
        ...state,
        add_idea_loading: true,
      };
    }
    case ADD_IDEA_SUCCESS: {
      return {
        ...state,
        items: [action.idea, ...state.items],
        add_idea_loading: false,
        add_idea_error: false,
      };
    }
    case ADD_IDEA_FAILED: {
      return {
        ...state,
        add_idea_loading: false,
        error: true,
      };
    }
    case DELETE_IDEA_SUCCESS: {
      return {
        ...state,
        items: state.items.map((item) =>
          item.id == action.ideaId
            ? { ...item, deletedAt: action.date }
            : item
        ),
      };
    }
    default: {
      return state;
    }
  }
};

export const ideaInitialState = {
  description: "",
  details: null,
  directionId: "",
  generation: "HOT",
  id: "",
};

export const editIdeaReducer = (
  state: TIdea = ideaInitialState,
  action:
    | {
        type:
          | typeof CHANGE_IDEA_DESCRIPTION
          | typeof CHANGE_IDEA_DETAILS
          | typeof RESET_IDEA
          | typeof CHANGE_IDEA_DIRECTION
          | typeof CHANGE_IDEA_CATEGORY
        payload: any;
      }
    | {
        type: typeof CHANGE_OPEN_IDEA;
        idea: any;
      }
    | {
        type: typeof SEND_COMMENT_SUCCESS;
        comment: any;
      }
    | {
        type: typeof CHANGE_COMPLETE_IDEA_DATE_SUCCESS;
        ideaId: string;
        date: string;
      }
) => {
  switch (action.type) {
    case RESET_IDEA: {
      return ideaInitialState;
    }
    case CHANGE_OPEN_IDEA: {
      return action.idea;
    }
    case CHANGE_IDEA_DESCRIPTION: {
      return {
        ...state,
        description: action.payload,
      };
    }
    case CHANGE_IDEA_DETAILS: {
      return {
        ...state,
        details: action.payload,
      };
    }
    case CHANGE_IDEA_ASSIGNEE: {
      return {
        ...state,
        assignee: action.payload,
      };
    }
    case CHANGE_IDEA_DIRECTION: {
      return {
        ...state,
        direction: action.payload,
      };
    }
    case CHANGE_IDEA_GENERATION: {
      return {
        ...state,
        generation: action.payload,
      };
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
