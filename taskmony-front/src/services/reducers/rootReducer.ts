import { editTaskReducer, tasksReducer } from "./tasksReducers";
import { authReducer, resetPasswordReducer } from "./authReducers";
import { userInfoReducer } from "./userInfoReducer";
import { combineReducers } from "redux";
import { directionsReducer } from "./directionsReducers";
import { notificationsReducer } from "./notificationsReducer";

export const rootReducer = combineReducers({
  tasks: tasksReducer,
  directions: directionsReducer,
  editedTask: editTaskReducer,
  auth: authReducer,
  resetPassword: resetPasswordReducer,
  userInfo: userInfoReducer,
  notifications : notificationsReducer,
});
