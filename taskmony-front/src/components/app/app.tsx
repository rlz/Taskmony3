import React from "react";
import SideMenu from "../side-menu/side-menu";
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

function App() {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/" element={<Home />}>
            <Route path="tasks" element={<MyTasks />} />
            <Route path="ideas" element={<MyIdeas />} />
            <Route path="archive" element={<Archive />} />
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
    <>
      <SideMenu />
      <Outlet />
    </>
  );
}

export default App;
