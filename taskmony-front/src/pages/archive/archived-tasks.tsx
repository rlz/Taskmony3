import { useState } from "react";
import { useLocation } from "react-router-dom";
import { ArchivedItem } from "../../components/archived-item";
import { FilterByDate } from "../../components/filter/by-date";
import { FilterByDirection } from "../../components/filter/by-direction";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";
import { Idea } from "../../components/idea";
import { useAppSelector } from "../../utils/hooks";


export const ArchivedTasks = () => {
  const tasks = useAppSelector((store) => store.tasks.items).filter(i=>i.deletedAt != null);
    return (
      <div className="flex w-full">
        <div className="w-3/4 m-3 ml-0">
        {tasks.map(task=>
                    <ArchivedItem label={task.description} date={task.deletedAt} direction={task.direction?.name}/>
            )}
        </div>
        <Filter />
      </div>
    );
  }

  function Filter() {
    return (
      <div className="w-1/5 mt-4">
          <>
            <FilterItem label="deleted" checked radio />
            <FilterItem label="completed" checked={false} radio />
          </>
          <hr />
          <FilterByDate type="deletion"/>
        <hr />
        <FilterByDirection/>
      </div>
    );
  }