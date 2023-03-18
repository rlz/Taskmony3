import { editTaskReducer, tasksReducer } from "./tasksReducers";
import { authReducer, resetPasswordReducer } from "./authReducers";
import { userInfoReducer } from "./userInfoReducer";
import { combineReducers } from "redux";
import { directionsReducer } from "./directionsReducers";
import { notificationsReducer } from "./notificationsReducer";
import { editIdeaReducer, ideasReducer } from "./ideasReducers";

export const rootReducer = combineReducers({
  tasks: tasksReducer,
  editedTask: editTaskReducer,
  ideas: ideasReducer,
  editedIdea: editIdeaReducer,
  directions: directionsReducer,
  auth: authReducer,
  resetPassword: resetPasswordReducer,
  userInfo: userInfoReducer,
  notifications : notificationsReducer
});
