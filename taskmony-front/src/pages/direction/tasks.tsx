import { useEffect, useState } from "react";
import { AddBtn } from "../../components/other-components/buttons/add-btn";
import { Task } from "../../components/task-idea/task/task";
import { EditedTask, OpenTask } from "../../components/task-idea/task/open-task/open-task";
import {
  addRepeatedTasks,
  addTask,
  CHANGE_TASK_DIRECTION,
  RESET_TASK,
} from "../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";
import { FilterByCreator } from "../../components/other-components/filter/by-creator";
import {
  FilterByFuture,
} from "../../components/other-components/filter/by-task-type";
import { useSearchParams } from "react-router-dom";
import { FilterByAssignee } from "../../components/other-components/filter/by-assignee";

type TasksProps = {
  directionId: string; directionName: string
}
  
function Tasks({ directionId, directionName } : TasksProps) {
  let [searchParams] = useSearchParams();
  const future = searchParams.get("future");
  const [newTask, setNewTask] = useState(false);
  const task = useAppSelector((store) => store.editedTask);
  const today = new Date().toISOString().slice(0,10);
  useEffect(()=>{
    if(task.id !== "") setNewTask(false);
    },[task.id])
  let tasksToShow = useAppSelector((store) => store.tasks.items).filter(
    (t) => t.deletedAt == null && t.direction?.id === directionId
  )
  .sort((a,b)=>{
    let aDate = a.startAt.slice(0,10);
    let bDate = b.startAt.slice(0,10);
    if (aDate === today && bDate !== today) return -1
    if (bDate === today && aDate !== today) return 1
    return a.startAt.slice(0,10).localeCompare(b.startAt.slice(0,10))})
  .sort((a, b) => {
    if(!a.completedAt && b.completedAt) return -1
    else if(!b.completedAt && a.completedAt) return 1
    else if(!b.completedAt && !a.completedAt) return 0
    else return 0
  });
  const createdBy = searchParams.getAll("createdBy");
  const assignedTo = searchParams.getAll("assignedTo");
  if (createdBy.length > 0) {
    tasksToShow = tasksToShow.filter((i) => createdBy.includes(i.createdBy.id));
  }
  if (assignedTo.length > 0) {
    tasksToShow = tasksToShow.filter((i) => assignedTo.includes(i.assignee?.id));
  }
  if (!future || future === "0"){
    //console.log("show no future");
    tasksToShow = tasksToShow.filter(
      (i) => i.startAt < new Date().toISOString()
    );
  }
  const direction = useAppSelector((store) => store.directions.items).filter(
    (d) => d.id === directionId
  )[0];
  useEffect(() => {
    //console.log(tasksToShow);
  }, [tasksToShow]);
  const dispatch = useAppDispatch();
  const addANewTask = (direction: string) => {
    if (task.repeatMode) dispatch(addRepeatedTasks(task, direction));
    else dispatch(addTask(task, direction));
    // dispatch({ type: RESET_TASK });
  };
  const tasks = tasksToShow.map((task,index) => (
    <Task task={task} direction={directionName} key={index} />
  ));
  return (
    <div className="flex w-full">
      <div className="w-full  m-3 mainBody">
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
          <OpenTask
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
    <div className="w-1/5 mt-4 filter">
      <FilterByCreator id={directionId} />
      <hr />
      <FilterByAssignee id={directionId} />
      <hr />
      <FilterByFuture />
    </div>
  );
}

export default Tasks;
