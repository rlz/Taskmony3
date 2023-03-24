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
  CHANGE_TASK_DIRECTION,
  RESET_TASK,
} from "../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { FilterByCreator } from "../../components/filter/by-creator";
import {
  FilterByArchivedTaskType,
  FilterByFuture,
  FilterByTaskType,
} from "../../components/filter/by-task-type";
import { useSearchParams } from "react-router-dom";

type TasksProps = {
  directionId: string; directionName: string
}
  
function Tasks({ directionId, directionName } : TasksProps) {
  let [searchParams, setSearchParams] = useSearchParams();
  const future = searchParams.get("future");
  const [newTask, setNewTask] = useState(false);
  const task = useAppSelector((store) => store.editedTask);
  useEffect(()=>{
    if(task.id !== "") setNewTask(false);
    },[task.id])
  let tasksToShow = useAppSelector((store) => store.tasks.items).filter(
    (t) => t.deletedAt == null && t.direction?.id == directionId
  ).sort((a, b) => {
    if(!a.completedAt && b.completedAt) return -1
    else if(!b.completedAt && a.completedAt) return 1
    else if(!b.completedAt && !a.completedAt) return 0
    else return 0
  });
  const createdBy = searchParams.getAll("createdBy");
  if (createdBy.length > 0) {
    tasksToShow = tasksToShow.filter((i) => createdBy.includes(i.createdBy.id));
  }
  if (!future || future == "0"){
    //console.log("show no future");
    tasksToShow = tasksToShow.filter(
      (i) => i.startAt < new Date().toISOString()
    );
  }
  const direction = useAppSelector((store) => store.directions.items).filter(
    (d) => d.id == directionId
  )[0];
  useEffect(() => {
    //console.log(tasksToShow);
  }, [tasksToShow]);
  const dispatch = useAppDispatch();
  const addANewTask = (direction: string) => {
    if (task.repeatMode) dispatch(addRepeatedTasks(task, direction));
    else dispatch(addTask(task, direction));
    dispatch({ type: RESET_TASK });
  };
  const tasks = tasksToShow.map((task) => (
    <Task task={task} direction={directionName} key={task.id} />
  ));
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <AddBtn
          label={"add a new task"}
          onClick={() => {
            dispatch({ type: RESET_TASK })
            setNewTask(true);
            dispatch({
              type: CHANGE_TASK_DIRECTION,
              payload: direction,
            });
          }}
        />
        {newTask && task.id === "" && (
          <EditedTask
            label={"new task"}
            direction={directionName}
            save={() => {
              //console.log("saving a new task");
              setNewTask(false);
              addANewTask(directionId);
            }}
            close={() => {
              setNewTask(false);
            }}
          />
        )}
        {tasks}
      </div>
      <Filter directionId={directionId} />
    </div>
  );
}
type FilterProps = {
  directionId: string;
}
function Filter({ directionId } : FilterProps) {
  return (
    <div className="w-1/5 mt-4">
      <FilterByCreator id={directionId} />
      <hr />
      <FilterByFuture />
    </div>
  );
}

export default Tasks;
