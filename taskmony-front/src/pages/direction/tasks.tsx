import { useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterItem } from "../../components/filter/filter-item";
import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";
import { Task } from "../../components/task";
import { FilterDivider } from "../../components/filter/filter-divider";

function Tasks() {
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <AddBtn label={"add a new task"} onClick={() => {}} />
        <Task
          label={"task #1"}
          comments={1}
          recurrent="every Thursday"
          createdBy="Ann Smith"
        />
        <Task label={"task #1"} checked />
        <Task label={"task #1"} followed/>
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
