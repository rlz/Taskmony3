import { useEffect, useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterItem } from "../../components/filter/filter-item";
import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import { Task } from "../../components/task";
import { FilterDivider } from "../../components/filter/filter-divider";
import { EditedTask } from "../../components/edited/edited-task";
import { addTask } from "../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../utils/hooks";

function Tasks() {
  const [newTask, setNewTask] = useState(false);
  const task = useAppSelector((store) => store.editedTask);
  const tasksToShow = useAppSelector((store) => store.tasks.items);
  useEffect(() => {
    console.log(tasksToShow);
  }, [tasksToShow]);
  const dispatch = useAppDispatch();
  const addANewTask = () => {
    dispatch(addTask(task));
  };
  const tasks = tasksToShow.map((task) => 
    <Task task={task} />
  );
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <AddBtn label={"add a new task"} onClick={() => setNewTask(true)} />
        {newTask && (
          <EditedTask
            label={"new task"}
            save={() => {
              console.log("saving a new task");
              setNewTask(false);
              addANewTask();
            }}
          />
        )}
        {tasks}
        {/* // <Task
        //   label={"task #1"}
        //   comments={1}
        //   recurrent="every Thursday"
        //   createdBy="Ann Smith"
        //   direction="Taskmony"
        // />
        // <Task label={"task #1"} direction="Taskmony" />
        // <Task label={"task #1"} followed direction="Taskmony" />
        // <Task label={"task #1"} followed direction="Taskmony" /> */}
      </div>
      <Filter />
    </div>
  );
}

function Filter() {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  return (
    <div className="w-1/5 mt-4">
      <FilterItem label="show future tasks" checked />
      <FilterItem label="show assigned to me" checked />
      <FilterItem label="show assigned by me" checked />
      <FilterItem label="show followed" checked />
      <hr/>
      <FilterDivider isOpen={isOpen} setIsOpen={setIsOpen} title="filter by creator" />
      {isOpen && (
        <>
          <FilterItem label="all" checked />
          <FilterItem label="me" checked/>
          <FilterItem label="user #1" checked />
        </>
      )}
    </div>
  );
}

export default Tasks;
