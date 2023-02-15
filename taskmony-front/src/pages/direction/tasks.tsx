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

function Tasks({ directionId, directionName }) {
  const [newTask, setNewTask] = useState(false);
  const task = useAppSelector((store) => store.editedTask);
  const tasksToShow = useAppSelector((store) => store.tasks.items).filter(
    (t) => t.direction?.id == directionId
  );
  const direction = useAppSelector((store) => store.directions.items).filter(d => d.id == directionId)[0];
  useEffect(() => {
    console.log(tasksToShow);
  }, [tasksToShow]);
  const dispatch = useAppDispatch();
  const addANewTask = (direction) => {
    if(task.repeatMode) dispatch(addRepeatedTasks(task, direction));
    else dispatch(addTask(task, direction));
    dispatch({type:RESET_TASK})
  };
  const tasks = tasksToShow.map((task) => (
    <Task task={task} direction={directionName} />
  ));
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <AddBtn
          label={"add a new task"}
          onClick={() => {
            setNewTask(true);
            dispatch({
              type: CHANGE_TASK_DIRECTION,
              payload: direction,
            });
          }}
        />
        {newTask && (
          <EditedTask
            label={"new task"}
            direction={directionName}
            save={() => {
              console.log("saving a new task");
              setNewTask(false);
              addANewTask(directionId);
            }}
          />
        )}
        {tasks}
      </div>
      <Filter directionId={directionId} />
    </div>
  );
}

function Filter({ directionId }) {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  return (
    <div className="w-1/5 mt-4">
      <FilterItem label="show future tasks" checked />
      <FilterItem label="show assigned to me" checked />
      <FilterItem label="show assigned by me" checked />
      <FilterItem label="show followed" checked />
      <hr />
      <FilterByCreator id={directionId} />
    </div>
  );
}

export default Tasks;
