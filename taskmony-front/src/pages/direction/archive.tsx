import { useState } from "react";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterByDirection } from "../../components/filter/by-direction";
import { FilterByArchivedTaskType } from "../../components/filter/by-task-type";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import { useAppSelector } from "../../utils/hooks";

type ArchiveProps = {
  directionId: string;
}

export const Archive = ({ directionId } : ArchiveProps) => {
  const location = useLocation();
  const navigate = useNavigate();
  let [searchParams, setSearchParams] = useSearchParams();
  const tasksType = searchParams.get("archiveType");
  const type = location.pathname.split("/")[3];
  const archiveType = location.pathname.split("/")[4];
  const tasks = useAppSelector((store) => store.tasks.items).filter(
    (i) => (tasksType == "deleted" ? i.deletedAt != null : i.completedAt != null) && i.direction?.id == directionId
  );
  const ideas = useAppSelector((store) => store.ideas.items).filter(
    (i) => i.deletedAt != null && i.direction?.id == directionId
  );
  let items = archiveType === "tasks"? tasks : ideas;
  const startDate = searchParams.get("startDate");
  const endDate = searchParams.get("endDate");
  if(startDate){
    items = items.filter(
      (i) => i.deletedAt > startDate)
  }
  if(endDate){
    items = items.filter(
      (i) => i.deletedAt < endDate)
  }
  return (
    <div className="flex w-full">
      <div className="w-3/4 m-3 ml-0">
        {items.map((task) => (
          <ArchivedItem
            label={task.description}
            date={task.deletedAt}
            direction={task.direction?.name}
            key={task.id}
          />
        ))}
      </div>
      <Filter archiveType={archiveType} directionId={directionId} />
    </div>
  );
};

type FilterProps = {
  archiveType: string; directionId: string;
}
  
function Filter({ archiveType, directionId } : FilterProps) {
  const navigate = useNavigate();
  const changeType = (type : string) => {
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
          <FilterByArchivedTaskType />
          <hr />
        </>
      )}
      <FilterByDate type="deletion" />
      <hr />
    </div>
  );
}