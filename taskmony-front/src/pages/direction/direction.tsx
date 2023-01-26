import { useState } from "react";
import { NavLink, useLoaderData, useLocation } from "react-router-dom";
import { About } from "./about";
import Ideas from "./ideas";
import Tasks from "./tasks";

function Direction() {
  const location = useLocation();
  const type = location.pathname.split("/").pop();
  const directionId = location.pathname.split("/")[2];
  const renderSwitch = (type?: string) => {
    switch(type){
      case "about": return <About/>;
      case "tasks": return <Tasks/>;
      case "ideas": return <Ideas/>;
  }}

  return (
    <div className="p-3 w-full">
        <h1 className="font-bold text-3xl">Project #1</h1>
        <Menu directionId={directionId}/>
        {
          renderSwitch(type)
          }
    </div>
  );
}

const Menu = ({directionId}) => {
  return (
    <div className="flex gap-6 mt-4">
    <MenuItem link={`/directions/${directionId}/tasks`} label={"Tasks"}/>
    <MenuItem link={`/directions/${directionId}/ideas`} label={"Ideas"}/>
    <MenuItem link={`/directions/${directionId}/about`} label={"About"}/>
    </div>
  );
};

type MenuItemT = {
  link: string,
  label: string
}

const MenuItem = ({link,label} : MenuItemT) => {
  const activeStyle = "underline underline-offset-8 text-blue-500";
  const unactiveStyle = "text-gray-300"
  return (
    <NavLink
      to={link}
      className={({ isActive }) => (isActive ? activeStyle : unactiveStyle)}
    >
      <p className={`font-semibold text-sm`}>
        {label}
      </p>
    </NavLink>
  );
};

export default Direction;
