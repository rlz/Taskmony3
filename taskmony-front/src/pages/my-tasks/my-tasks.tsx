import { useEffect, useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterItem } from "../../components/filter/filter-item";
import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import { Task } from "../../components/task";
import { FilterDivider } from "../../components/filter/filter-divider";
import { EditedTask } from "../../components/edited/edited-task";
import {
  addRepeatedTasks,
  addTask,
  RESET_TASK,
} from "../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { FilterByDirection } from "../../components/filter/by-direction";
import { useSearchParams } from "react-router-dom";
import {
  FilterByArchivedTaskType,
  FilterByTaskType,
} from "../../components/filter/by-task-type";
import Cookies from 'js-cookie';

function MyTasks() {
  const [newTask, setNewTask] = useState(false);
  let [searchParams, setSearchParams] = useSearchParams();
  const myId = Cookies.get("id");
  const chosenDirection = searchParams.getAll("direction");
  const future = searchParams.get("future");
  const followed = searchParams.get("followed");
  const assignedByMe = searchParams.get("assignedByMe");
  const task = useAppSelector((store) => store.editedTask);
  let tasksToShow = useAppSelector((store) => store.tasks.items).filter(
    (i) => i.deletedAt == null && (i.createdBy.id == myId || i.assignee?.id == myId || i.subscribers.some((s) => s.id == myId))
  ).sort((a, b) => {
    if(!a.completedAt && b.completedAt) return -1
    else if(!b.completedAt && a.completedAt) return 1
    else if(!b.completedAt && !a.completedAt) return 0
    else return 0
  });
  if (chosenDirection.length > 0)
    tasksToShow = tasksToShow.filter(
      (i) =>
        chosenDirection.includes(i.direction?.name) ||
        (chosenDirection.includes("unassigned") && !i.direction)
    );
    if (!future || future == "0"){
      console.log("show no future");
      tasksToShow = tasksToShow.filter(
        (i) => i.startAt < new Date().toISOString()
      );
    }
    if (!followed || followed == "0") {
      tasksToShow = tasksToShow.filter((i) => (!i.subscribers.some((s) => s.id == myId)));
    }
    if (!assignedByMe || assignedByMe == "0") {
      tasksToShow = tasksToShow.filter((i) => (!(i.createdBy.id == myId && i.assignee && i.assignee.id != myId)));
    }

  const dispatch = useAppDispatch();
  const addANewTask = () => {
    if (task.repeatMode) dispatch(addRepeatedTasks(task, null));
    else dispatch(addTask(task, null));
    dispatch({ type: RESET_TASK });
  };
  const tasks = tasksToShow.map((task) => <Task task={task} key={task.id} />);
  return (
    <div className="flex w-full">
      <div className="w-3/4  p-3 flex flex-col overflow-hidden h-screen">
        <h1 className="font-bold text-3xl">My Tasks</h1>
        <AddBtn label={"add a new task"} onClick={() => setNewTask(true)} />
        {newTask && (
          <EditedTask
            label={"new task"}
            save={() => {
              console.log("saving a new task");
              setNewTask(false);
              addANewTask();
            }}
            close={() => {
              setNewTask(false);
            }}
          />
        )}
        <div className="overflow-x-hidden">{tasks}</div>
      </div>
      <Filter />
    </div>
  );
}

function Filter() {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  return (
    <div className="w-1/5 mt-12">
      <FilterByTaskType />
      <hr />
      <FilterByDirection />
    </div>
  );
}

export default MyTasks;
