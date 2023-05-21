import { useEffect, useState } from "react";
import { SideMenu } from "../other-components/side-menu/side-menu";
import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
  Outlet,
  useNavigate,
} from "react-router-dom";
import MyTasks from "../../pages/my-tasks";
import MyIdeas from "../../pages/my-ideas";
import Archive from "../../pages/archive/archive";
import { Login } from "../../pages/auth/login";
import Direction from "../../pages/direction/direction";
import { NotificationsBtn } from "../other-components/notifications/notifications-btn";
import { NotificationsModal } from "../other-components/notifications/notifications-modal";
import { ForgotPassword } from "../../pages/auth/forgot-password";
import { Register } from "../../pages/auth/register";
import { ResetPassword } from "../../pages/auth/reset-password";
import { getTasks } from "../../services/actions/tasksAPI";
import { useAppDispatch } from "../../utils/hooks";
import { getDirections } from "../../services/actions/directionsAPI";
import { getUserInfo } from "../../services/actions/userInfo";
import { getNotifications } from "../../services/actions/notifications";
import Cookies from 'js-cookie';
import { QueryParamProvider } from 'use-query-params';
import { ReactRouter6Adapter } from 'use-query-params/adapters/react-router-6';
import { getIdeas } from "../../services/actions/ideasAPI";
import { TaskPage } from "../../pages/task-page";
import { IdeaPage } from "../../pages/idea-page";

function App() {
  return (
    <>
      <BrowserRouter>
      <QueryParamProvider adapter={ReactRouter6Adapter}>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          <Route path="/reset-password" element={<ResetPassword />} />
          <Route path="/" element={<Home />}>
            <Route path="tasks" element={<MyTasks />} />
            <Route path="task/:itemId" element={<TaskPage />} />
            <Route path="ideas" element={<MyIdeas />} />
            <Route path="idea/:itemId" element={<IdeaPage />} />
            <Route path="archive/" element={<Navigate to="tasks" />} />
            <Route path="archive/:element" element={<Archive />} />
            <Route path="directions/:id/" element={<Navigate to="tasks" />} />
            <Route path="directions/:id/:element" element={<Direction />} />
            <Route
              path="directions/:id/:element/:type"
              element={<Direction />}
            />
            <Route
              path="directions/:id/archive/"
              element={<Navigate to="tasks" />}
            />
            <Route path="" element={<Navigate to="/tasks" replace />} />
          </Route>
          <Route path="*" element={<Navigate to="/tasks" replace />} />
        </Routes>
        </QueryParamProvider>
      </BrowserRouter>
    </>
  );
}

function Home() {
  const [openNotif, setOpenNotif] = useState(false);
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  useEffect(() => {
    if(!Cookies.get("refreshToken") || Cookies.get("refreshToken") === "undefined") navigate("/login")
    dispatch(getTasks());
    dispatch(getIdeas());
    dispatch(getDirections());
    dispatch(getUserInfo());
    dispatch(getNotifications());
    setInterval(()=>{dispatch(getNotifications())}, 60 * 1000); 
  }, []);

  return (
    <div className="flex h-full bg-slate-50">
      {!openNotif && (
        <NotificationsBtn
          onClick={() => {
            setOpenNotif(true);
          }}
        />
      )}
      <SideMenu />
      <div className="w-full">
        <Outlet />
      </div>
      {openNotif && <NotificationsModal close={() => setOpenNotif(false)} />}
    </div>
  );
}

export default App;
