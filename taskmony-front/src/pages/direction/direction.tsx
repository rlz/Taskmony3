import { useEffect, useState } from "react";
import {
  NavLink,
  useLoaderData,
  useLocation,
  useNavigate,
} from "react-router-dom";
import { useAppSelector } from "../../utils/hooks";
import { Archive } from "./archive";
import { About } from "./about";
import Ideas from "./ideas";
import Tasks from "./tasks";
import { ArchivedTasks } from "../archive/archived-tasks";
import { ArchivedIdeas } from "../archive/archived-ideas";

function Direction() {
  const location = useLocation();
  const navigate = useNavigate();
  const type = location.pathname.split("/")[3];
  const directionId = location.pathname.split("/")[2];
  const archiveType = location.pathname.split("/")[4];
  const directions = useAppSelector((store) => store.directions.items);
  const direction = directions.filter((dir) => dir.id == directionId)[0];
  const renderSwitch = (type?: string) => {
    switch (type) {
      case "about":
        return <About directionId={directionId} />;
      case "tasks":
        return (
          <Tasks directionId={directionId} directionName={direction?.name} />
        );
      case "ideas":
        return <Ideas directionId={directionId} />;
      case "archive":
        return <Archive directionId={directionId} />;
    }
  };
  useEffect(() => {
    if (directions.length > 0 && direction == null) navigate("/");
  }, [direction]);
  return (
    <div className="p-3 w-full">
      <h1 className="font-bold text-3xl">{direction?.name}</h1>
      <Menu directionId={directionId} archiveType={archiveType} />
      {renderSwitch(type)}
    </div>
  );
}

const Menu = ({ directionId, archiveType }) => {
  return (
    <div className="flex gap-6 mt-4">
      <MenuItem link={`/directions/${directionId}/tasks`} label={"Tasks"} />
      <MenuItem link={`/directions/${directionId}/ideas`} label={"Ideas"} />
      <MenuItem link={`/directions/${directionId}/about`} label={"About"} />
      <MenuItem link={`/directions/${directionId}/archive`} label={"Archive"} />
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

const BigMenuItem = ({ link, label, items }) => {
  const activeStyle = "underline underline-offset-8 text-blue-500";
  const unactiveStyle = "text-gray-300";
  return (
    <div>
      <NavLink
        to={link}
        className={({ isActive }) => (isActive ? activeStyle : unactiveStyle)}
      >
        <p className={`font-semibold text-sm`}>{label}</p>
      </NavLink>
      <div className="flex gap-6 mt-2">
        {items.map((item) => (
          <MenuItem link={item.link} label={item.label} />
        ))}
      </div>
    </div>
  );
};

export default Direction;
