import { useSearchParams } from "react-router-dom";
import { ArchivedItem } from "../../components/task-idea/archived-item";
import { FilterByDate } from "../../components/other-components/filter/by-date";
import { FilterByDirection } from "../../components/other-components/filter/by-direction";
import { FilterByArchivedTaskType } from "../../components/other-components/filter/by-task-type";
import { useAppSelector } from "../../utils/hooks";

export const ArchivedTasks = () => {
  let [searchParams, setSearchParams] = useSearchParams();
  const startDate = searchParams.get("startDate");
  const endDate = searchParams.get("endDate");
  const archiveType = searchParams.get("archiveType");
  const chosenDirection = searchParams.getAll("direction");
  let tasks = useAppSelector((store) => store.tasks.items);
  let chosenTasks;
  if (archiveType == "deleted") {
    chosenTasks = tasks
      .filter((i) => i.deletedAt != null)
      .sort((a, b) => {
        return b.deletedAt.localeCompare(a.deletedAt);
      });
  } else {
    chosenTasks = tasks
      .filter((i) => i.completedAt != null)
      .sort((a, b) => {
        return b.completedAt.localeCompare(a.completedAt);
      });
  }
  if (chosenDirection.length > 0)
    chosenTasks = chosenTasks.filter(
      (i) =>
        chosenDirection.includes(i.direction?.name) ||
        (chosenDirection.includes("unassigned") && !i.direction)
    );
  if (startDate) {
    if (archiveType == "deleted") {
      chosenTasks = chosenTasks.filter((i) => i.deletedAt > startDate);
    } else {
      chosenTasks = chosenTasks.filter((i) => i.completedAt > startDate);
    }
  }
  if (endDate) {
    if (archiveType == "deleted") {
      chosenTasks = chosenTasks.filter((i) => i.deletedAt > endDate);
    } else {
      chosenTasks = chosenTasks.filter((i) => i.completedAt > endDate);
    }
  }
  return (
    <div className="flex w-full ">
      <div className="w-full m-3 ml-0 mainBody">
        {chosenTasks.map((task, index) => (
          <ArchivedItem
            label={task.description}
            date={archiveType == "deleted" ? task.deletedAt : task.completedAt}
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
    <div className="w-1/5 mt-4 filter">
      <FilterByArchivedTaskType />
      <hr />
      <FilterByDate type="deletion" />
      <hr />
      <FilterByDirection />
    </div>
  );
}
