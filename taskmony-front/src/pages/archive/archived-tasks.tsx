import { useState } from "react";
import { useLocation, useSearchParams } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterByDirection } from "../../components/filter/by-direction";
import { FilterByArchivedTaskType } from "../../components/filter/by-task-type";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import { useAppSelector } from "../../utils/hooks";

export const ArchivedTasks = () => {
  let [searchParams, setSearchParams] = useSearchParams();
  const chosenDirection = searchParams.get("direction");
  let tasks = useAppSelector((store) => store.tasks.items).filter(
    (i) => i.deletedAt != null
  );
  if (chosenDirection != "")
    tasks = tasks.filter(
      (i) =>
        i.direction?.name == chosenDirection ||
        (chosenDirection == "none" && !i.direction)
    );
  return (
    <div className="flex w-full">
      <div className="w-3/4 m-3 ml-0">
        {tasks.map((task, index) => (
          <ArchivedItem
            label={task.description}
            date={task.deletedAt}
            direction={task.direction?.name}
            key={index}
          />
        ))}
      </div>
      <Filter />
    </div>
  );
};

function Filter() {
  return (
    <div className="w-1/5 mt-4">
      <FilterByArchivedTaskType />
      <hr />
      <FilterByDate type="deletion" />
      <hr />
      <FilterByDirection />
    </div>
  );
}
