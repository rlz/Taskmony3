import { useState } from "react";
import { NavLink, useLoaderData, useLocation } from "react-router-dom";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";

import { Idea } from "../../components/idea";
import { ArchivedDirections } from "./archived-directions";
import { ArchivedIdeas } from "./archived-ideas";
import { ArchivedTasks } from "./archived-tasks";

function Archive() {
  const location = useLocation();
  const type = location.pathname.split("/").pop();
  const renderSwitch = (type?: string) => {
    switch (type) {
      case "tasks":
        return <ArchivedTasks />;
      case "ideas":
        return <ArchivedIdeas />;
      case "directions":
        return <ArchivedDirections />;
    }
  };

  return (
    <div className="p-3 w-full">
      <h1 className="font-bold text-3xl">Archive</h1>
      <Menu />
      {renderSwitch(type)}
    </div>
  );
}

const Menu = () => {
  return (
    <div className="flex gap-6 mt-4">
      <MenuItem link={"/archive/tasks"} label={"Tasks"} />
      <MenuItem link={"/archive/ideas"} label={"Ideas"} />
      <MenuItem link={"/archive/directions"} label={"Directions"} />
    </div>
  );
};

type MenuItemT = {
  link: string;
  label: string;
};

const MenuItem = ({ link, label }: MenuItemT) => {
  const activeStyle = "underline underline-offset-8 text-blue-500";
  const unactiveStyle = "text-gray-300";
  return (
    <NavLink
      to={link}
      className={({ isActive }) => (isActive ? activeStyle : unactiveStyle)}
    >
      <p className={`font-semibold text-sm`}>{label}</p>
    </NavLink>
  );
};

export default Archive;
