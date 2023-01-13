import { useState } from "react";
import { NavLink, useLoaderData, useLocation } from "react-router-dom";
import { About } from "./about";
import Ideas from "./ideas";
import Tasks from "./tasks";

function Direction() {
  const location = useLocation();
  const type = location.pathname.split("/").pop();
  const renderSwitch = (type?: string) => {
    switch(type){
      case "about": return <About/>;
      case "tasks": return <Tasks/>;
      case "ideas": return <Ideas/>;
  }}

  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <h1 className="font-bold text-3xl">Project #1</h1>
        <Menu />
        {
          renderSwitch(type)
          }
    </div>
    </div>
  );
}

const Menu = () => {
  return (
    <div className="flex gap-6 mt-4">
    <MenuItem link={"/directions/1/tasks"} label={"Tasks"}/>
    <MenuItem link={"/directions/1/ideas"} label={"Ideas"}/>
    <MenuItem link={"/directions/1/about"} label={"About"}/>
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
