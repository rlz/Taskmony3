import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterByDirection } from "../../components/filter/by-direction";
import { FilterByArchivedTaskType } from "../../components/filter/by-task-type";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import { useAppSelector } from "../../utils/hooks";

export const Archive = ({ directionId }) => {
  const location = useLocation();
  const navigate = useNavigate();
  const type = location.pathname.split("/")[3];
  const archiveType = location.pathname.split("/")[4];
  const tasks = useAppSelector((store) => store.tasks.items).filter(
    (i) => i.deletedAt != null
  );
  return (
    <div className="flex w-full">
      <div className="w-3/4 m-3 ml-0">
        {tasks.map((task) => (
          <ArchivedItem
            label={task.description}
            date={task.deletedAt}
            direction={task.direction?.name}
          />
        ))}
      </div>
      <Filter archiveType={archiveType} directionId={directionId} />
    </div>
  );
};

function Filter({ archiveType, directionId }) {
  const navigate = useNavigate();
  const changeType = (type) => {
    navigate(`/directions/${directionId}/archive/${type}`);
  };
  return (
    <div className="w-1/5 mt-4">
      <>
        <FilterItem
          label="tasks"
          checked={archiveType == "tasks"}
          onChange={() => changeType("tasks")}
          radio
        />
        <FilterItem
          label="ideas"
          checked={archiveType == "ideas"}
          onChange={() => changeType("ideas")}
          radio
        />
      </>
      <hr />
      {archiveType == "tasks" && (
        <>
          <FilterByArchivedTaskType/>
          <hr />
        </>
      )}
      <FilterByDate type="deletion" />
      <hr />
    </div>
  );
}
