import { useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterItem } from "../../components/filter-item";
import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import { Task } from "../../components/task";

function MyTasks() {
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <h1 className="font-bold text-3xl">MyTasks</h1>
        <AddBtn label={"add a new task"} onClick={() => {}} />
        <Task
          label={"task #1"}
          comments={1}
          recurrent="every Thursday"
          createdBy="Ann Smith"
        />
        <Task label={"task #1"} checked />
        <Task label={"task #1"} followed />
      </div>
      <Filter />
    </div>
  );
}

function Filter() {
  const [isOpen, setIsOpen] = useState<boolean>(true);
  return (
    <div className="w-1/5 mt-28">
      <FilterItem label="show future tasks" checked />
      <FilterItem label="show assigned to me" checked />
      <FilterItem label="show assigned by me" checked />
      <FilterItem label="show followed" checked />
      <div className={"gap-4 flex m-4 justify-between"}>
        <p className={"font-bold text-gray-300 text-sm"}>filter by direction</p>
        <img
          src={isOpen ? arrowUp : arrowDown}
          onClick={() => setIsOpen(!isOpen)}
        ></img>
      </div>
      {isOpen && (
        <>
          <FilterItem label="Project #1" checked />
          <FilterItem label="Project #1" checked />
          <FilterItem label="Project #1" checked />
        </>
      )}
    </div>
  );
}

export default MyTasks;
