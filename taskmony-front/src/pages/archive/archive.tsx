import { useState } from "react";
import { NavLink, useLoaderData, useLocation } from "react-router-dom";
import { AddBtn } from "../../components/buttons/add-btn";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";

import { Idea } from "../../components/idea/idea";
import { ArchivedDirections } from "./archived-directions";
import { ArchivedIdeas } from "./archived-ideas";
import { ArchivedTasks } from "./archived-tasks";

function Archive() {
  const loc = useLocation();
  const type = loc.pathname.split("/").pop();
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
    <div className="flex flex-col p-3 w-full">
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
