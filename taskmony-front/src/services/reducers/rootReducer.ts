import {
  editTaskReducer,
  tasksReducer
} from "./tasksReducers";
import { authReducer, resetPasswordReducer } from "./authReducers";
import { userInfoReducer } from "./userInfoReducer";
import { combineReducers } from "redux";

export const rootReducer = combineReducers({
  tasks: tasksReducer,
  editedTask: editTaskReducer,
  auth: authReducer,
  resetPassword: resetPasswordReducer,
  userInfo: userInfoReducer,
});
