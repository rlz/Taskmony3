import { editTaskReducer, tasksReducer } from "./tasksReducers";
import { authReducer, resetPasswordReducer } from "./authReducers";
import { userInfoReducer } from "./userInfoReducer";
import { combineReducers } from "redux";
import { directionsReducer } from "./directionsReducers";

export const rootReducer = combineReducers({
  tasks: tasksReducer,
  directions: directionsReducer,
  editedTask: editTaskReducer,
  auth: authReducer,
  resetPassword: resetPasswordReducer,
  userInfo: userInfoReducer,
});
