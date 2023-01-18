import React from "react";
import {SideMenu} from "../side-menu/side-menu";
import {
  BrowserRouter,
  Routes,
  Route,
  Link,
  Navigate,
  Outlet,
} from "react-router-dom";
import MyTasks from "../../pages/my-tasks/my-tasks";
import MyIdeas from "../../pages/my-ideas/my-ideas";
import Archive from "../../pages/archive/archive";
import ErrorPage from "../../pages/error-page/error-page";
import Login from "../../pages/login/login";
import Direction from "../../pages/direction/direction";
import { NotificationsBtn } from "../notifications/notifications-btn";

function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<Home />}>
            <Route path="tasks" element={<MyTasks />} />
            <Route path="ideas" element={<MyIdeas />} />
            <Route path="archive/:element" element={<Archive />} />
            <Route path="directions/:id/:element" element={<Direction />} />
            <Route path="" element={<Navigate to="/tasks" replace />} />
          </Route>
          <Route path="*" element={<Navigate to="/tasks" replace />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

function Home() {
  return (
    <div className="flex h-full bg-slate-50">
      <NotificationsBtn/>
      <SideMenu/>
      <div className="w-full">
      <Outlet />
      </div>
    </div>
  );
}

export default App;
